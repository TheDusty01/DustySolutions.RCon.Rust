using DustySolutions.RCon.Rust.Entities;
using DustySolutions.RCon.Rust.Exceptions;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using Websocket.Client;

namespace DustySolutions.RCon.Rust
{
    public interface IRustRconClient : IDisposable
    {
        bool IsConnected { get; }
        string RconClientName { get; }

        ICommands Commands { get; }

        IObservable<RconResponseMessage> RconMessageReceived { get; }
        IObservable<RconChatMessage> ChatMessageReceived { get; }
        IObservable<RconFeedbackReport> FeedbackReceived { get; }
        IObservable<RconPlayerReport> PlayerReportReceived { get; }

        Task StartAsync();
        Task StopAsync(WebSocketCloseStatus status, string statusDescription);

        void SendCommand(string command);
        Task<RconResponseMessage> SendCommandWithResponseAsync(string command, CancellationToken cancellationToken = default);
    }

    public class RustRconClient : IRustRconClient, IDisposable
    {
        private const string PlayerReportTargetIdPropertyName = "TargetId";

        private readonly ConcurrentDictionary<int, TaskCompletionSource<RconResponseMessage>> activeRequests = new();

        private readonly Subject<RconResponseMessage> rconMessageReceivedSubject = new Subject<RconResponseMessage>();
        private readonly Subject<RconChatMessage> chatMessageReceivedSubject = new Subject<RconChatMessage>();
        private readonly Subject<RconFeedbackReport> feedbackReceivedSubject = new Subject<RconFeedbackReport>();
        private readonly Subject<RconPlayerReport> playerReportReceivedSubject = new Subject<RconPlayerReport>();

        public bool IsConnected => WebsocketClient.IsRunning;
        public string RconClientName { get; }

        public ICommands Commands { get; }

        public IObservable<RconResponseMessage> RconMessageReceived => rconMessageReceivedSubject.AsObservable();
        public IObservable<RconChatMessage> ChatMessageReceived => chatMessageReceivedSubject.AsObservable();
        public IObservable<RconFeedbackReport> FeedbackReceived => feedbackReceivedSubject.AsObservable();
        public IObservable<RconPlayerReport> PlayerReportReceived => playerReportReceivedSubject.AsObservable();

        public IWebsocketClient WebsocketClient { get; init; }

        public RustRconClient(string url, string rconClientName) : this(new Uri(url), rconClientName, null)
        {
        }

        public RustRconClient(Uri url, string rconClientName, Func<ClientWebSocket>? clientFactory = null)
        {
            RconClientName = rconClientName;

            WebsocketClient = new WebsocketClient(url, clientFactory);
            WebsocketClient.ReconnectTimeout = null;

            WebsocketClient.MessageReceived
                .Where(x => x.Text is not null)
                .ObserveOn(TaskPoolScheduler.Default)
                .Subscribe(OnMessageReceived);

            Commands = new Commands(this);
        }

        public Task StartAsync()
        {
            return WebsocketClient.StartOrFail();
        }

        public Task StopAsync(WebSocketCloseStatus status, string statusDescription)
        {
            return WebsocketClient.StopOrFail(status, statusDescription);
        }

        private static int GetNextIdentifier()
        {
            return Random.Shared.Next(int.MaxValue);
        }

        private static T? InvokeSubjectFromJson<T>(string json, Subject<T> subject)
        {
            var data = JsonSerializer.Deserialize<T>(json);
            if (data is null)
                return default;

            subject.OnNext(data);

            return data;
        }

        private void OnMessageReceived(ResponseMessage websocketResponse)
        {
            //var response = JsonSerializer.Deserialize<RconResponseMessage>(websocketResponse.Text);
            var response = InvokeSubjectFromJson(websocketResponse.Text, rconMessageReceivedSubject);
            if (response is null)
                return;

            switch (response.Type)
            {
                case RconMessageType.Chat:
                    {
                        InvokeSubjectFromJson(response.Message, chatMessageReceivedSubject);
                        break;
                    }
                case RconMessageType.Report when response.Message.Contains(PlayerReportTargetIdPropertyName):
                    {
                        InvokeSubjectFromJson(response.Message, feedbackReceivedSubject);
                        break;
                    }
                case RconMessageType.Report:
                    {
                        InvokeSubjectFromJson(response.Message, playerReportReceivedSubject);
                        break;
                    }
                case RconMessageType.Generic:
                case RconMessageType.Warning:
                case RconMessageType.Error:
                    {
                        if (activeRequests.TryRemove(response.Identifier, out var tcs))
                            tcs.TrySetResult(response);
                        break;
                    }
            }
        }

        private void SendRequest(string command, int identifier)
        {
            if (!IsConnected)
                throw new NotConnectedException();

            RconRequestMessage request = new RconRequestMessage(identifier, command, RconClientName);
            WebsocketClient.Send(JsonSerializer.Serialize(request));
        }

        public void SendCommand(string command)
        {
            if (!IsConnected)
                throw new NotConnectedException();

            SendRequest(command, GetNextIdentifier());
        }

        public Task<RconResponseMessage> SendCommandWithResponseAsync(string command, CancellationToken cancellationToken = default)
        {
            if (!IsConnected)
                throw new NotConnectedException();

            TaskCompletionSource<RconResponseMessage> tcs = new TaskCompletionSource<RconResponseMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
            using (cancellationToken.Register(() =>
            {
                tcs.TrySetCanceled();
            }))
            {
                int identifier = GetNextIdentifier();
                if (!activeRequests.TryAdd(identifier, tcs))
                    tcs.TrySetException(new SameIdentifierException());

                SendRequest(command, identifier);
                return tcs.Task;
            }
        }

        public void Dispose()
        {
            WebsocketClient.Dispose();
        }
    }
}
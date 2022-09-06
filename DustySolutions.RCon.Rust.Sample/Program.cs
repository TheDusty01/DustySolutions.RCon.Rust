using DustySolutions.RCon.Rust;
using System.Collections.Concurrent;
using System.Text.Json;

using RustRconClient rcon = new RustRconClient("ws://your.server.com/RconPassword", "TestClient");

rcon.RconMessageReceived.Subscribe(msg =>
{
    Console.WriteLine("================================");
    Console.WriteLine($"RCon Msg received:\n{JsonSerializer.Serialize(msg, new JsonSerializerOptions { WriteIndented = true })}");
    Console.WriteLine($"{msg.Message}");
});

rcon.ChatMessageReceived.Subscribe(msg =>
{
    Console.WriteLine($"Chat Msg from {msg.Username} ({msg.UserId}) received:\n{msg.Message}");
});

rcon.WebsocketClient.ReconnectionHappened.Subscribe(info =>
{
    Console.WriteLine($"ReconnectionHappened:\n{info.Type}");
});

rcon.WebsocketClient.DisconnectionHappened.Subscribe(info =>
{
    Console.WriteLine($"DisconnectionHappened:\n{info.CloseStatus}");
});


await rcon.StartAsync();

rcon.SendCommand("status");
var resp = await rcon.Commands.EchoWithResponseAsync("Hello guys");
//var resp = await rcon.SendCommandWithResponseAsync("say test123");


Console.ReadKey(true);
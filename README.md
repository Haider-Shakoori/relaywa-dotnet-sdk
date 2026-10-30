# RelayWA .NET SDK

Official server-side C# client for the [RelayWA](https://relaywa.com) WhatsApp API.

> **Release status:** Source package prepared for NuGet. `dotnet add package RelayWA.Sdk` is available only **after** the package is published to nuget.org.

## Requirements

- .NET 8 SDK or later
- A RelayWA account and a server-side session key
- HTTPS connectivity to `https://relaywa.com/api`

## Installation (after NuGet publication)

```bash
dotnet new console -n RelayWADemo
cd RelayWADemo
dotnet add package RelayWA.Sdk --version 0.1.0
```

To test the source package before publication, clone this repository and reference the project directly:

```bash
dotnet add reference ../relaywa-dotnet-sdk/RelayWA/RelayWA.csproj
```

## Configure credentials

Set `RELAYWA_SESSION_KEY` on the **server**, not in a mobile, desktop-distributed, or browser client.

Windows CMD:

```cmd
set "RELAYWA_SESSION_KEY=your-session-key"
```

PowerShell:

```powershell
$env:RELAYWA_SESSION_KEY = "your-session-key"
```

Linux/macOS:

```bash
export RELAYWA_SESSION_KEY="your-session-key"
```

## First request: list sessions (read-only)

In `Program.cs`:

```csharp
using RelayWA;

var key = Environment.GetEnvironmentVariable("RELAYWA_SESSION_KEY");
if (string.IsNullOrWhiteSpace(key))
    throw new InvalidOperationException("Set RELAYWA_SESSION_KEY");

using var http = new HttpClient();
var relaywa = new RelayWAClient(http, key);

var sessions = await relaywa.SessionsAsync();
Console.WriteLine(sessions);
```

Run:

```bash
dotnet run
```

This reads session data without connecting, disconnecting, or sending messages.

## Send a message (live operation)

Only use a connected session and a recipient you have permission to message:

```csharp
var result = await relaywa.SendTextAsync("12025550123", "Hello from RelayWA!");
Console.WriteLine(result);
```

Sending may have succeeded even if a network timeout occurs. Do not automatically retry ambiguous sends.

## Other examples

**Session details:**

```csharp
var session = await relaywa.SessionAsync("your-session-id");
var chats = await relaywa.SessionChatsAsync("your-session-id");
```

**Image message:**

```csharp
var image = new MediaMessage(
    To: "12025550123",
    Url: "https://example.com/image.jpg",
    MimeType: "image/jpeg",
    MediaSizeBytes: 102400
);
var response = await relaywa.SendImageAsync(image);
```

**ASP.NET Core:** Register a named or typed `HttpClient` using dependency injection and instantiate `RelayWAClient` with a session key from server-side configuration. Do not expose the session key to clients.

## Features

- Text, image, video, audio, and document messaging
- Location, contact, poll, reply, and reaction messaging
- Session listing, lifecycle operations, QR code, statistics, logs, and messages
- Session contacts, chats, and groups
- Webhook management

Methods are asynchronous and return `JsonElement`. HTTP errors raise `HttpRequestException`; cancellation and timeouts can raise `TaskCanceledException`. The client configures a 30-second timeout if the supplied `HttpClient` has a longer or infinite timeout.

**Important:** Webhook signature verification is not yet part of the imported client implementation; validate incoming webhook authenticity separately before processing events.

## Troubleshooting

| Issue | Check |
| --- | --- |
| Package not found | NuGet publication may not yet be complete. |
| HTTP 401/403 | Confirm session key and account permissions. |
| Timeout | Check connectivity and do not blindly retry sends. |
| Missing class | Confirm `RelayWA.Sdk` package or project reference is installed. |

## Release process

The SDK is released independently of the RelayWA production services. Validate `dotnet build` and `dotnet pack` in GitHub Actions. Publishing to nuget.org requires explicit maintainer approval.

## Links

- [RelayWA](https://relaywa.com)
- [Source code](https://github.com/Haider-Shakoori/relaywa-dotnet-sdk)
- [Report an issue](https://github.com/Haider-Shakoori/relaywa-dotnet-sdk/issues)

## License

MIT.

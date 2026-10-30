using System.Net;
using System.Net.Http;
using System.Text;
using RelayWA;

var handler = new StubHandler();
using var http = new HttpClient(handler);
var client = new RelayWAClient(http, "test-session-key", "https://example.test/api");
var result = await client.SessionsAsync();
if (result.GetProperty("ok").GetBoolean() != true)
    throw new Exception("Unexpected response payload");
if (handler.Requests != 1 || handler.Path != "/api/whatsapp-sessions")
    throw new Exception($"Unexpected request: {handler.Requests}, {handler.Path}");
if (handler.Authorization != "Bearer test-session-key")
    throw new Exception("Authorization header missing");
try
{
    _ = new RelayWAClient(new HttpClient(), "test-session-key", "http://example.test/api");
    throw new Exception("Insecure base URL was accepted");
}
catch (ArgumentException) { }
Console.WriteLine("RelayWA SDK offline smoke test passed");

sealed class StubHandler : HttpMessageHandler
{
    public int Requests { get; private set; }
    public string? Path { get; private set; }
    public string? Authorization { get; private set; }
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Requests++;
        Path = request.RequestUri?.AbsolutePath;
        Authorization = request.Headers.Authorization?.ToString();
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"ok\":true}", Encoding.UTF8, "application/json")
        });
    }
}

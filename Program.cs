using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using WebApplication6;
using WebApplication6.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetryTracing(b =>
{
    b
    .AddOtlpExporter(exp =>
    {
        exp.Endpoint = new Uri($"{Constants.JaegerHostLocal}:{Constants.JaegerPortHttp}");
        exp.Protocol = OtlpExportProtocol.HttpProtobuf;
    })
    //.AddJaegerExporter(exp =>
    //{
    //    exp.ExportProcessorType = ExportProcessorType.Simple;
    //    exp.Endpoint = new Uri($"{Constants.JaegerHostLocal}:{Constants.JaegerPortHttp}");
    //    exp.Protocol = JaegerExportProtocol.HttpBinaryThrift;
    //    //exp.AgentHost = "localhost";
    //    //exp.AgentPort = 4318;
    //})
    //.AddConsoleExporter()
    .AddSource(Constants.ServiceName)
    .SetResourceBuilder(
        ResourceBuilder.CreateDefault()
            .AddService(serviceName: Constants.ServiceName, serviceVersion: Constants.ServiceVersion))
    .AddHttpClientInstrumentation()
    .AddAspNetCoreInstrumentation();
});

var app = builder.Build();

app.MapGet("/hello", async () =>
{
    using var activity = Telemetry.MyActivitySource.StartActivity("Hello, World!");
    var html = await new HttpClient().GetStringAsync("https://example.com/");
    if (string.IsNullOrWhiteSpace(html))
    {
        return "Hello, World!";
    }
    else
    {
        return "Hello, World!";
    }
});

app.MapGet("/rand", () =>
{
    using var activity = Telemetry.MyActivitySource.StartActivity("Random number");
    return DummyClass.Rnd.Next();
});

app.MapGet("/work/{num:int}", (int num) =>
{
    using var activity = Telemetry.MyActivitySource.StartActivity("Work");
    activity?.SetTag("operation.param", num);
    activity?.AddEvent(new("Attempt CeratePeople", DateTimeOffset.Now));
    var res = DummyClass.CreatePeople(num);
    activity?.AddEvent(new("Attempt DoImportantWork", DateTimeOffset.Now));
    return DummyClass.DoImportantWork(res);
});

app.MapGet("/generate/{num:int}", (int num) =>
{
    var activity = Telemetry.MyActivitySource.StartActivity("Generate");
    activity?.SetTag("operation.param", num);
    activity?.AddEvent(new("Attempt CeratePeople", DateTimeOffset.Now));
    var res = DummyClass.CreatePeople(num);
    activity?.AddEvent(new("Attempt DisplayPeoples", DateTimeOffset.Now));
    return DummyClass.DisplayPeoples(res);
});

app.Run();
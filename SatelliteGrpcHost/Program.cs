using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using SatelliteGrpcHost.Services;
using SatelliteService;
using SatelliteService.Bootstrapper;
using ConfigurationManager = System.Configuration.ConfigurationManager;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddSingleton<IDeploymentService, DeploymentService>();
ObjectFactoryConfigurator.Configure();

builder.WebHost.UseKestrel(options =>
{
    int port = Int32.Parse(ConfigurationManager.AppSettings["Service.Port"]);

    options.Listen(System.Net.IPAddress.Loopback, port, listenOptions =>
    {
        var connectionOptions = new HttpsConnectionAdapterOptions();

        connectionOptions.ServerCertificate = new X509Certificate2(
            ConfigurationManager.AppSettings["MachineCertificate.Path"],
            ConfigurationManager.AppSettings["MachineCertificate.Password"]);
        X509Certificate2 authority =
            new X509Certificate2(
                X509Certificate.CreateFromCertFile(ConfigurationManager.AppSettings["RootCertificate.Path"]));

        connectionOptions.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
        connectionOptions.ClientCertificateValidation = (certificate, chain, errors) =>
        {
            bool isValid = chain.ChainElements
                .Any(el => el.Certificate.Thumbprint == authority.Thumbprint);

            Console.WriteLine("Cert check: " + isValid);

            return isValid;
        };

        listenOptions.UseHttps(connectionOptions);
    });
});

var app = builder.Build();
app.UseRouting();
app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

app.UseEndpoints(endpoints => { endpoints.MapGrpcService<DeploymentController>().EnableGrpcWeb(); });

app.UseHttpsRedirection();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

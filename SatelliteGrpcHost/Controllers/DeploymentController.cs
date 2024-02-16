using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Newtonsoft.Json;
using ObjectFactory;
using GrpcSatellite;
using SatelliteService;
using SatelliteService.Contracts;
using SatelliteService.Operations;
using System.Configuration;
using ExceptionDataInfo = GrpcSatellite.ExceptionDataInfo;

namespace SatelliteGrpcHost.Services
{
    public class DeploymentController : Deployment.DeploymentBase
    {
        private readonly IDeploymentService deploymentService;

        public DeploymentController(IDeploymentService deploymentService)
        {
            this.deploymentService = deploymentService;
        }

        public override Task<IsReadyResponse> IsReady(Empty request, ServerCallContext context)
        {
            Console.WriteLine("IsReady");

            return Task.FromResult( new IsReadyResponse()
            {
                IsReady = this.deploymentService.IsReady()
            });
        }

        public override Task<GetLastExceptionResponse> GetLastException(Empty request, ServerCallContext context)
        {
            ExceptionInfo exception = this.deploymentService.GetLastException();
            RepeatedField<ExceptionDataInfo> dataInfos = new RepeatedField<ExceptionDataInfo>();

            dataInfos.AddRange(exception.ExceptionData.Select(data => new ExceptionDataInfo()
            {
                Name = data.Name,
                Value = data.Value,
                IsProperty = data.IsProperty
            }));

            return Task.FromResult(new GetLastExceptionResponse()
            {
                TypeName = exception.TypeName,
                AssemblyQualifiedTypeName = exception.AssemblyQualifiedTypeName,
                Message = exception.Message,
                Source = exception.Source,
                StackTrace = exception.StackTrace,
                ExceptionData = { dataInfos },
            });
        }

        public override Task<DeploymentServiceBasicResponse> BeginPublication(BeginPublicationRequest request, ServerCallContext context)
        {
            Console.Write("Begin publication");
            return Task.FromResult(new DeploymentServiceBasicResponse()
            {
                IsSuccess = this.deploymentService.BeginPublication(request.PublicationId)
            });
        }

        public override Task<DeploymentServiceBasicResponse> ExecuteNextOperation(Empty request, ServerCallContext context)
        {
            Console.Write("Execute next operation");
            return Task.FromResult(new DeploymentServiceBasicResponse()
            {
                IsSuccess = this.deploymentService.ExecuteNextOperation()
            });
        }

        public override Task<DeploymentServiceBasicResponse> Complete(Empty request, ServerCallContext context)
        {
            Console.Write("Completing");
            return Task.FromResult(new DeploymentServiceBasicResponse()
            {
                IsSuccess = this.deploymentService.Complete()
            });
        }

        public override Task<Empty> Rollback(Empty request, ServerCallContext context)
        {
            Console.Write("Rolling back");
            this.deploymentService.Rollback();

            return Task.FromResult(new Empty());
        }

        public override Task<Empty> ResetPackage(Empty request, ServerCallContext context)
        {
            Console.WriteLine("ResetingPackage");
            this.deploymentService.ResetPackage();
            return Task.FromResult(new Empty());
        }

        public override async Task<Empty> UploadPackageBuffer(IAsyncStreamReader<UploadPackageBufferRequest> requestStream, ServerCallContext context)
        {
            Console.WriteLine("Uploading package buffer");
            while (await requestStream.MoveNext())
            {
                UploadPackageBufferRequest request = requestStream.Current;

                byte[] bytes = new byte[request.Buffer.Length];
                request.Buffer.CopyTo(bytes, 0);

                this.deploymentService.UploadPackageBuffer(bytes);
            }

            return new Empty();
        }

        public override Task<Empty> DeployWebSite(DeployWebSiteRequest request, ServerCallContext context)
        {
            Console.WriteLine("Deploying web site");
            this.deploymentService.DeployWebSite((dynamic)request);

            return Task.FromResult(new Empty());
        }

        public override Task<Empty> ProcessConfigFile(ProccesConfigFileRequest request, ServerCallContext context)
        {
            this.deploymentService.ProcessConfigFile((dynamic)request);

            return Task.FromResult(new Empty());
        }

        public override Task<Empty> CopyFiles(CopyFilesRequest request, ServerCallContext context)
        {
            this.deploymentService.CopyFiles((dynamic)request);

            return Task.FromResult(new Empty());
        }

        public override Task<Empty> UpdateHostsFile(UpdateHostsFileRequest request, ServerCallContext context)
        {
            this.deploymentService.UpdateHostsFile((dynamic)request);

            return Task.FromResult(new Empty());
        }

        public override Task<Empty> RunSQLScripts(RunSQLScriptsRequest request, ServerCallContext context)
        {
            this.deploymentService.RunSQLScript(request);

            return Task.FromResult(new Empty());
        }

        public override Task<Empty> ApplyDacPac(ApplyDacPacRequest request, ServerCallContext context)
        {
            this.deploymentService.ApplyDacpac(request);

            return Task.FromResult(new Empty());
        }
    }
}
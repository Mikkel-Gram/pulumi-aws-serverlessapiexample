using Pulumi;
using Pulumi.Aws.ApiGateway;
using Pulumi.Aws.ApiGateway.Inputs;
using Pulumi.Aws.Lambda;
using Deployment = Pulumi.Aws.ApiGateway.Deployment;
using Resource = Pulumi.Aws.ApiGateway.Resource;
using ResourceArgs = Pulumi.Aws.ApiGateway.ResourceArgs;

public class ProxyApi
{
    public ProxyApi(Function lambda)
    {
        var api = new RestApi("api", new RestApiArgs());

        var resource = new Resource("Resource", new ResourceArgs()
        {
            ParentId = api.RootResourceId,
            PathPart = "{proxy+}",
            RestApi = api.Id
        });

        var method = new Method("Method", new MethodArgs()
        {
            HttpMethod = "ANY",
            ResourceId = resource.Id,
            RestApi = api.Id,
            Authorization = "NONE"
        });

        var integration = new Integration("Integration", new IntegrationArgs()
        {
            IntegrationHttpMethod = "POST", //This must always be POST for lambda integrations
            Type = "AWS_PROXY",
            Uri = lambda.InvokeArn,
            HttpMethod = "ANY",
            ResourceId = resource.Id,
            RestApi = api.Id
        });

        // Add the deployment and make it depend on the resource, method and integration
        var deployment = new Deployment("Deployment", new DeploymentArgs()
        {
            Description = "Deployment for our API",
            RestApi = api.Id
        }, new CustomResourceOptions() { DependsOn = { resource, method, integration } });

        var stage = new Stage("Stage", new StageArgs()
        {
            Deployment = deployment.Id,
            RestApi = api.Id,
            StageName = "api"

        });

        // Permissions for invoking the lambda function
        var lambdaBasePathPermission = new Permission("Permission-BasePath", new PermissionArgs()
        {
            Action = "lambda:InvokeFunction",
            Function = lambda.Arn,
            Principal = "apigateway.amazonaws.com",
            SourceArn = Output.Format($"{deployment.ExecutionArn}{stage.StageName}/*/")
        });
        // Same permission for the proxy path
        var lambdaProxyPermission =new Permission("Permission-ProxyPath", new PermissionArgs()
        {
            Action = "lambda:InvokeFunction",
            Function = lambda.Arn,
            Principal = "apigateway.amazonaws.com",
            SourceArn = Output.Format($"{deployment.ExecutionArn}{stage.StageName}/*/{{proxy+}}")
        });

    }
}
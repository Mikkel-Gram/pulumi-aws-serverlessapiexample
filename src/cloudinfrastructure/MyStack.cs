using Pulumi;
using Pulumi.Aws.Lambda;
using Pulumi.Aws.Lambda.Inputs;
using Pulumi.Aws.Iam;

class MyStack : Stack
{
    public MyStack()
    {
        var lambdaRole = new Role("lambdaRole", new RoleArgs
        {
                AssumeRolePolicy = 
                  @"{
                        ""Version"": ""2012-10-17"",
                        ""Statement"": [
                            {
                                ""Action"": ""sts:AssumeRole"",
                                ""Principal"": {
                                    ""Service"": ""lambda.amazonaws.com""
                                },
                                ""Effect"": ""Allow"",
                                ""Sid"": """"
                            }
                        ]
                    }"
        });

        var apiLambda = new Function("ApiLambda", new FunctionArgs()
        {
            Runtime = "dotnetcore3.1",
            Handler = "ApiLambda::ApiLambda.LambdaEntryPoint::FunctionHandlerAsync",
            Timeout = 60,
            Code = new FileArchive(@"C:\Git\Medium\pulumi-aws-serverlessapiexample\src\pulumi-aws-serverlessapiexample\bin\Release\netcoreapp3.1\pulumi-aws-serverlessapiexample.zip"),
            Role = lambdaRole.Arn
        });

        var api = new ProxyApi(apiLambda);
    }
}

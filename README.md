# pulumi-aws-serverlessapiexample
Example code for setting up a basic serverless API on AWS, using Pulumi and dotnet

To deploy the example to AWS, first package the lambda using the dotnet lambda tools, and the ´dotnet lambda package´ command.
Then update the reference to the lambda package inside "MyStack.cs", so it refers to the correct .zip file.
Lastly, run ´pulumi up´ using the pulumi CLI, and the serverless api will be deployed to AWS.

I made a post detailing this example at [Medium](https://medium.com/@mikkel_78989/serverless-cloud-api-using-aws-pulumi-and-net-core-83e2e55397b3) 

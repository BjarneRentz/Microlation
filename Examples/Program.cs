// See https://aka.ms/new-console-template for more information

using Microlation;


var ms1 = new Microservice();

var ms2 = new Microservice()
{
    Routes = new List<IRoute>
    {
        new Route<int>{Url = "Id"}
    }
};

var call = ms1
    .Call(ms2, new CallOptions<int> { Route = "Id", Policys = new RetryPolicy<int>()})
    .Validate(value => value != 0);

var result = call.Execute();



Console.WriteLine(result);
// See https://aka.ms/new-console-template for more information

using Microlation;
using Polly;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Latency;
using Polly.Contrib.Simmy.Outcomes;

var error = MonkeyPolicy.InjectException(with => with.Fault<int>(new Exception()).InjectionRate(0.1).Enabled());
var latency =
	MonkeyPolicy.InjectLatency<int>(with => with.Latency(TimeSpan.FromSeconds(2)).InjectionRate(0.1).Enabled());

var faults = latency.Wrap(error);

var ms1 = new Microservice("MS1");
var ms2 = new Microservice("MS2")
{
	Routes = new List<IRoute>
	{
		new Route<int> { Url = "Id", Value = () => 1, Faults = faults},
	}
};

var callWithoutPolicies = ms1.Call(ms2,
	new CallOptions<int>
	{
		Route = "Id",
		Interval = (_) => TimeSpan.FromMilliseconds(Random.Shared.Next(100, 700)),
	});

var timeout = Policy.Timeout<int>(TimeSpan.FromSeconds(1));
var retry = Policy<int>.Handle<Exception>().Retry();

var callWithPolicies = ms1.Call(ms2,
	new CallOptions<int>
	{
		Route = "Id",
		Policies = Policy.Wrap(timeout, retry),
		Interval = (_) => TimeSpan.FromMilliseconds(Random.Shared.Next(100,700)) ,
	});


var sim = new Simulation
{
	Microservices =
	{
		ms1,
	}
};

var result = await sim.Run(TimeSpan.FromSeconds(20));
Console.WriteLine(result);


var withoutPolicy = result[callWithoutPolicies];
var withPolicy = result[callWithPolicies];
var avgCallTimeWithout = result[callWithoutPolicies].Average(r => r.CallDuration.TotalMilliseconds);
var avgCallTimeWith = result[callWithPolicies].Average(r => r.CallDuration.TotalMilliseconds);

Console.WriteLine("{0,20}| {1,20} {2,20}", "Simulation", "Without Policies", "With Policies");
Console.WriteLine("{0,20}| {1,20} {2,20}", "Avg Time", avgCallTimeWithout, avgCallTimeWith);
Console.WriteLine("{0,20}| {1,20} {2,20}", "Exceptions", withoutPolicy.Count(r => r.Exception != null), withPolicy.Count(r => r.Exception != null));

Console.ReadKey();
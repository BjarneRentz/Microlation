// See https://aka.ms/new-console-template for more information

using Microlation;
using Polly;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Latency;
using Polly.Contrib.Simmy.Outcomes;

var error = MonkeyPolicy.InjectException(with => with.Fault<int>(new Exception()).InjectionRate(0.1).Enabled());
var latency =
	MonkeyPolicy.InjectLatency<int>(with => with.Latency(TimeSpan.FromSeconds(1)).InjectionRate(0.1).Enabled());

var faults = latency.Wrap(error);

var ms1 = new Microservice();
var ms2 = new Microservice
{
	Routes = new List<IRoute>
	{
		new Route<int> { Url = "Id", Value = () => 1, Faults = faults }
	}
};

var call = ms1
	.Call(ms2,
		new CallOptions<int>
		{
			Route = "Id",
			Policies = Policy<int>.Handle<Exception>().Retry(),
			Interval = (_) => TimeSpan.FromMilliseconds(Random.Shared.Next(100,700)) ,
		})
	.Validate(value => value != 0);

var result = await ms1.Simulate(TimeSpan.FromSeconds(100));

result.First().ForEach(Console.WriteLine);

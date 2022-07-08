using System.Collections.Concurrent;
using System.Data;
using System.Text;
using Microlation;
using Polly;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Latency;
using Polly.Contrib.Simmy.Outcomes;
using Polly.Timeout;

namespace Examples;

public class SampleEvaluation
{
	private static object lockObject = new ();
	
	public static async Task Run()
	{
		int[] timeouts = { 1000, 1500, 2000, 2500, 3000 };
		int[] retries = { 0, 1, 2, 3, 4 };
		double[] availabilities = { 0.5, 0.6, 0.7, 0.8, 0.9, 1 };



		var table = BuildTable(timeouts, retries);
		
		var tasks = (from timeout in timeouts
			from retry in retries
			from availability in availabilities
			select Task.Run(async () =>
			{
				var result = await CreateSimulation(timeout, retry, availability).Run(TimeSpan.FromSeconds(600), false);
				AddRow(table, availability, timeout, retry, result.Values.First().Sum(r => r.CallDuration.TotalMilliseconds), result.Values.First().Count(r => r.Exception != null));
			})).ToList();

		await Task.WhenAll(tasks);

		
		PrintTable(table);


		Console.ReadKey();
	}

	private static void AddRow(DataTable table
	, double availability, int timeout, int retry, double connectionTime, int errorCount)
	{
		lock (lockObject)
		{
			var row = table.Rows.Find(new object[]
			{
				availability
			});
			if (row == null)
			{
				row = table.NewRow();
				row["Availability"] = availability;
				row[$"{timeout}|{retry}_T"] = connectionTime;
				row[$"{timeout}|{retry}_E"] = errorCount;
				table.Rows.Add(row);
			}
			else
			{
				row[$"{timeout}|{retry}_T"] = connectionTime;
				row[$"{timeout}|{retry}_E"] = errorCount;
			}
			
			
		}
	}

	private static DataTable BuildTable(int[] timeouts, int[] retrys)
	{
		var table = new DataTable("Evaluation");

		DataColumn column;

		column = new DataColumn();
		column.Caption = "Availability";
		column.ColumnName = "Availability";
		column.DataType = typeof(double);


		table.Columns.Add(column);
		table.PrimaryKey = new []{column};

		foreach (var timeout in timeouts)
		{
			foreach (var retry in retrys)
			{
				var col = new DataColumn();
				col.Caption = $"{timeout}|{retry}_T";
				col.ColumnName = $"{timeout}|{retry}_T";
				col.DataType = typeof(double);
				table.Columns.Add(col);

				col = new DataColumn();
				col.Caption = $"{timeout}|{retry}_E";
				col.ColumnName = $"{timeout}|{retry}_E";
				col.DataType = typeof(int);
				table.Columns.Add(col);
			}
		}

		return table;
	}

	private static void PrintTable(DataTable table)
	{
		var sb = new StringBuilder(); 

		IEnumerable<string> columnNames = table.Columns.Cast<DataColumn>().
			Select(column => column.ColumnName);
		sb.AppendLine(string.Join(";", columnNames));

		foreach (DataRow row in table.Rows)
		{
			IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
			sb.AppendLine(string.Join(";", fields));
		}

		Console.WriteLine(sb.ToString());
		File.WriteAllText("Results.csv", sb.ToString());
	}

	private static void EvaluateResults(IDictionary<(int Timeout, int Retry), List<CallResult>> results)
	{
		Console.WriteLine("{0,20} | {1,20} | {2,20} | {3,20} | {4,20} | {5,20}", "Timeout", "# Retries", "# Calls" ,"Avg Time",
			"# Errors", "# Valid");

		foreach (var kvp in results.OrderBy(k => k.Key.Timeout).ThenBy(k => k.Key.Retry))
			Console.WriteLine("{0,20} | {1,20} | {2,20} | {3,20} | {4,20} | {5,20}", 
				kvp.Key.Timeout, kvp.Key.Retry,
				kvp.Value.Count,
				kvp.Value.Average(r => r.CallDuration.TotalMilliseconds), 
				kvp.Value.Count(r => r.Exception != null),
				kvp.Value.Count(r => r.Valid));
	}

	private static Simulation CreateSimulation(int timeout, int retries, double availability)
	{
		var injectionRate = Math.Round(1 - availability, 1);
		var faultPolicy = MonkeyPolicy
			.InjectLatency(with => with.Latency(TimeSpan.FromSeconds(4)).InjectionRate(injectionRate).Enabled()).AsPolicy<int>();

		var caller = new Microservice("Caller");
		var target = new Microservice("Target")
		{
			Routes = new List<IRoute>
			{
				new Route<int>
				{
					Url = "Target",
					Faults = faultPolicy,
					Value = () => 3
				}
			}
		};

		caller.Call(target, new CallOptions<int>
		{
			Route = "Target",
			Interval = _ => TimeSpan.FromMilliseconds(500),
			Policies = Policy<int>.Handle<TimeoutRejectedException>().Retry(retries)
				.Wrap(Policy.Timeout<int>(TimeSpan.FromMilliseconds(timeout)))
		});

		return new Simulation
		{
			Microservices = { caller, target }
		};
	}
}
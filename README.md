Overview
========
Dotnet.Strom.Adapter is a .NET Standard 2.0 implementation of [Apache Storm(TM)](http://storm.apache.org/) multi-lang protocol. You can use it to implement C# / .net components for your Storm topology. 

Prerequisites
========
 
* .NET Core framework 2.0 and above
* Git

Install form NuGet
========

This component is available as a NuGet package - see [NuGet.org here](https://www.nuget.org/packages/Dotnet.Storm.Adapter/)

		PM> Install-Package Dotnet.Storm.Adapter

Build locally
========
Run next command

		cd /strom-mulilang/dotnet
		build.sh adapter

Creating NuGet package
========

		cd /strom-mulilang/dotnet
		build.sh nuget

Run example
========

		cd /strom-mulilang/dotnet
		run.sh

Command line parameters
========

* -c (class name) - component class to instantiate
* -a (arguments) - parameters will be available through Arguments property

The example of usage is 

		spouts:
		 - id: emit-sentence
		   className: org.apache.storm.flux.wrappers.spouts.FluxShellSpout
		   constructorArgs:
			 - ["dotnet", "Dotnet.Storm.Adapter.dll", "-c", "Dotnet.Storm.Example.EmitSentense", "-a", "Dotnet.Storm.Example", "-l", "debug"]
			 - [sentence]
		   parallelism: 1

API
========

## Common
- Properties

		protected readonly static ILog Logger;

		protected string[] Arguments;

		protected static IDictionary<string, object> Configuration;

		protected static StormContext Context;

		protected static bool IsGuarantee;

		protected static int MessageTimeout;
			
- Events

		protected event EventHandler<TaskIds> OnTaskIds;

		protected event EventHandler OnInitialized;

- Methods

		public void Sync()

		public void Error(string message)

		public void Metrics(string name, object value)

		public VerificationResult VerifyInput(string component, string stream, List<object> tuple)

		public VerificationResult VerifyOutput(string stream, List<object> tuple)

## Spout specific
- Methods

		public abstract void Next();

		public void Emit(List<object> tuple, string stream = "default", long task = 0, bool needTaskIds = false)

- Events

		protected event EventHandler OnActivate;

		protected event EventHandler OnDeactivate;

- Properties

		protected bool IsEnabled = false;

## Bolt specific
- Methods

		public abstract void Execute(StormTuple tuple);

		public void Ack(string id);

		public void Fail(string id);

		public void Emit(List<object> tuple, string stream = "default", long task = 0, List<string> anchors = null, bool needTaskIds = false);

- Events

		protected event EventHandler<EventArgs> OnTick;

Testing
========

 - Create the context and configuration for all components you want to test.

		StormContext context1 = new StormContext()
		{
			ComponentId = "..."
			Streams = new string[] { "...", "..." }
			.
			.
			.
		};

		Dictionary<string, object> config => new Dictionary<string, object>
		{
			["topology.workers"] = 5,
			["topology.tick.tuple.freq.secs"] = 30,
			.
			.
			.
		};
		

- If you want to test a spout than call Next method and chech the output using GetOutput extension method from Dotnet.Storm.Adapter.Test namespace.

		MySpout1 spout1 = UnitTest.CreateComponent<MySpout1>(context1, config);
		spout1.Next();

		List<TestOutput> output = spout1.GetOutput();

		Asset.AreEqual(1045, output.Count());
		.
		.

 - If you want to test a bolt than you have to merge the outputs from all the spouts this bolt connected to and pass the result as an input to bolt.

		List<TestOutput> input = UnitTest.Merge(
			UnitTest.CreateComponent<MySpout1>(context1, config),
			UnitTest.CreateComponent<MySpout2>(context1, config),
			.
			.
		};

		MyBolt1 bolt1 = UnitTest.CreateComponent<MyBolt1>(context7, config, input);

		List<TestOutput> output = bolt1.GetOutput();

		Asset.AreEqual(1045, output.Count());
		.
		.
		.
		
 - You can find an example test code in Dotnet.Storm.Test.Example project 

# Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

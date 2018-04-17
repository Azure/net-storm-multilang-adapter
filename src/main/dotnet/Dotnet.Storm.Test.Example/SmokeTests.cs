using System;
using NUnit.Framework;
using Dotnet.Storm.Example;
using Dotnet.Storm.Adapter.Test;
using Dotnet.Storm.Adapter.Components;
using Dotnet.Storm.Adapter.Messaging;
using System.Collections.Generic;

namespace Dotnet.Storm.Test.Example
{
    public class SmokeTests
    {
        [Test]
        public void TestCacheChannel()
        {
            // Initialize configuration
            Dictionary<string, object> config = new Dictionary<string, object>();
            config["azure.spout.emit.frequency"] = "";
            config["azure.service.name"] = "example";
            config["azure.bolt.cache.span"] = "";
            config["topology.acker.executors"] = "0";

            // Initialize context
            StormContext sc = new StormContext();
            sc.StreamToOputputFields = new Dictionary<string, List<string>>();
            sc.StreamToOputputFields["default"] = new List<string>(new string[] { "default" });

            // Create, and run a spout and dump its output
            BaseSpout es = (BaseSpout)TestAPI.CreateComponent(typeof(EmitSentence), sc, config);
            TestAPI.Run(es);
            List<List<Object>> es_output = TestAPI.DumpChannel(es);
            Assert.IsNotEmpty(es_output);

            // Create, and run 1st Bolt using spout's output and dump its output
            BaseBolt ss = (BaseBolt)TestAPI.CreateComponent(typeof(SplitSentence), sc, config);
            TestAPI.Run(ss, es_output);
            List<List<Object>> ss_output = TestAPI.DumpChannel(ss);
            Assert.IsNotEmpty(ss_output);

            // Create, and run 2nd Bolt using 1st bolt's ouptut and dump its output
            BaseBolt cw = (BaseBolt)TestAPI.CreateComponent(typeof(CountWords), sc, config);
            TestAPI.Run(cw, ss_output);
            List<List<Object>> cw_output = TestAPI.DumpChannel(cw);
        }
    }
}
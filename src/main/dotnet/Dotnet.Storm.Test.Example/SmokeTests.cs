using System;
using NUnit.Framework;
using Dotnet.Storm.Example;
using Dotnet.Storm.Adapter.Test;
using Dotnet.Storm.Adapter.Components;
using Dotnet.Storm.Adapter.Channels;
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

            // Create, and run a spout
            BaseSpout es = (BaseSpout)TestAPI.CreateComponent(typeof(EmitSentence), sc, config);
            TestAPI.Run(es);
            Assert.True(TestAPI.ChannelSize() > 0);

            // Create, and run 1st Bolt
            BaseBolt ss = (BaseBolt)TestAPI.CreateComponent(typeof(SplitSentence), sc, config);
            TestAPI.Run(ss);
            Assert.True(TestAPI.ChannelSize() > 0);

            // Create, and run 2nd Bolt
            BaseBolt cw = (BaseBolt)TestAPI.CreateComponent(typeof(CountWords), sc, config);
            TestAPI.Run(cw);

            List<List<Object>> cw_output = TestAPI.DumpChannel();
            Assert.True(cw_output.Count == 0);
        }
    }
}
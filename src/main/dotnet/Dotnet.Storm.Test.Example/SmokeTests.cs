// Licensed to the Apache Software Foundation (ASF) under one
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using NUnit.Framework;
using Dotnet.Storm.Example;
using Dotnet.Storm.Adapter.Test;
using Dotnet.Storm.Adapter.Components;
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
            config["configkey1"] = "configvalue1";
            config["configkey2"] = "configvalue2";

            // Initialize context
            StormContext sc = new StormContext();
            sc.ComponentId = "componentid1";
            sc.StreamToOputputFields = new Dictionary<string, List<string>>();
            sc.StreamToOputputFields["default"] = new List<string>(new string[] { "default" });

            // Create, and run a spout
            EmitSentence es = (EmitSentence)TestApi.CreateComponent(typeof(EmitSentence), sc, config);
            es.Next();
            List<TestOutput> res = TestApi.DumpChannel(es);
            
            // Verify results and metadata
            Assert.True(res.Count > 0);
            Assert.True(res[0].Stream == "default");
            Assert.True(res[0].ComponentId == "componentid1");

            // Create, and run 1st Bolt
            sc.ComponentId = "componentid2";
            SplitSentence ss = (SplitSentence)TestApi.CreateComponent(typeof(SplitSentence), sc, config);
            foreach (var output in res)
            {
                StormTuple st = new StormTuple(((SpoutOutput)output).Id, "EmitSentence", "TaskId", output.Stream, output.Tuple);
                ss.Execute(st);
            }
            res = TestApi.DumpChannel(ss);

            // Verify results and metadata
            Assert.True(res.Count > 0);
            Assert.True(res[0].Stream == "default");

            // Create, and run 2nd Bolt
            sc.ComponentId = "componentid3";
            BaseBolt cw = (BaseBolt)TestApi.CreateComponent(typeof(CountWords), sc, config);
            foreach (var output in res)
            {
                StormTuple st = new StormTuple("id", "SplitSentence", "TaskId", output.Stream, output.Tuple);
                cw.Execute(st);
            }
        }
    }
}
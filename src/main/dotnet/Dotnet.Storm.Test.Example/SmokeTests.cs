/**
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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

            // Create, and run a spout
            BaseSpout es = (BaseSpout)TestApi.CreateComponent(typeof(EmitSentence), sc, config);
            es.Next();
            List<TestOutput> res = TestApi.DumpChannel();
            // Verify results and metadata
            Assert.True(res.Count > 0);
            Assert.True(res[0].Stream == "default");

            // Create, and run 1st Bolt
            BaseBolt ss = (BaseBolt)TestApi.CreateComponent(typeof(SplitSentence), sc, config);
            for (int i = 0; i < res.Count; i++)
            {
                StormTuple st = new StormTuple(((SpoutOutput)res[0]).Id, "EmitSentence", "TaskId", res[0].Stream, res[0].Tuple);
                ss.Execute(st);
            }
            res = TestApi.DumpChannel();
            // Verify results and metadata
            Assert.True(res.Count > 0);
            Assert.True(res[0].Stream == "default");

            // Create, and run 2nd Bolt
            BaseBolt cw = (BaseBolt)TestApi.CreateComponent(typeof(CountWords), sc, config);
            for (int i = 0; i < res.Count; i++)
            {
                StormTuple st = new StormTuple("id", "SplitSentence", "TaskId", res[0].Stream, res[0].Tuple);
                cw.Execute(st);
            }
        }
    }
}
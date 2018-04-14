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
            // Run spout and dump its output
            BaseSpout es = (BaseSpout)TestAPI.CreateComponent(typeof(EmitSentence));
            TestAPI.Process(es);
            List<List<Object>> es_output = TestAPI.DumpChannel(es);

            // Run 1st Bolt using spout's output and dump its output
            BaseBolt ss = (BaseBolt)TestAPI.CreateComponent(typeof(SplitSentence));
            TestAPI.Process(ss, es_output);
            List<List<Object>> ss_output = TestAPI.DumpChannel(ss);

            // Run 2nd Bolt using 1st bolt's ouptut and dump its output
            BaseBolt cw = (BaseBolt)TestAPI.CreateComponent(typeof(CountWords));
            TestAPI.Process(cw, ss_output);
            List<List<Object>> cw_output = TestAPI.DumpChannel(cw);
           
            Assert.Pass();
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CommandLine;
using Dotnet.Storm.Adapter.Channels;
using Dotnet.Storm.Adapter.Components;
using Dotnet.Storm.Adapter.Logging;
using Dotnet.Storm.Adapter.Messaging;
using Dotnet.Storm.Adapter.Serializers;
using log4net;
using log4net.Config;
using log4net.Core;

namespace Dotnet.Storm.Adapter.Test
{
    public static class TestAPI
    {
        public static Component CreateComponent(Type type)
        {
            Channel.Instance = new CacheChannel()
            {
                Serializer = new JsonSerializer()
            };

            Component comp = (Component)Activator.CreateInstance(type);

            Component.Context = new StormContext();
            Component.Context.StreamToOputputFields = new Dictionary<string, List<string>>();
            Component.Context.StreamToOputputFields["default"] = new List<string>(new string[]{"default"});

            return comp;
        }

        public static List<List<Object>> DumpChannel(Component component)
        {
            List <List<Object>> res = new List<List<Object>>();

            while (CacheChannel.IsEmpty() == false)
            {
                if (component is BaseSpout)
                {
                    SpoutTuple st = (SpoutTuple)((CacheChannel)Channel.Instance).OutputMessage();
                    res.Add(st.Tuple);
                }
                else if (component is BaseBolt)
                {
                    BoltTuple st = (BoltTuple)((CacheChannel)Channel.Instance).OutputMessage();
                    res.Add(st.Tuple);
                }
            }

            return res;
        }

        public static void Process(Component component, List<List<Object>> input = null)
        {
            if (component is BaseSpout)
                ((BaseSpout)component).Next();
            else if (component is BaseBolt)
            {
                for (int i = 0; i < input.Count; i++)
                {
                    if (component is BaseBolt)
                    {
                        ExecuteTuple et = new ExecuteTuple()
                        {
                            Tuple = input[i]
                        };
                        ((BaseBolt)component).Execute(new StormTuple(et));
                    }
                }
            }
        }

        public static bool IsChannelEmpty()
        {
            if (CacheChannel.IsEmpty() == true)
                return true;
            return false;
        }
    }
}

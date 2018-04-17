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
        public static Component CreateComponent(Type type, StormContext sc, Dictionary<string, object> config)
        {
            // Create channel singleton
            Channel.Instance = new CacheChannel();
            
            // Create component instance
            Component comp = (Component)Activator.CreateInstance(type);

            // Set context and configuration singleton
            Component.Context = sc;
            Component.Configuration = config;
        
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

        /// <summary>
        /// Run a component in test mode
        /// </summary>
        /// <param name="component">component's name</param>
        /// <param name="prev_component">Previous component's name, null if none</param>
        public static void Run(Component component, Component prev_component)
        {
            if (component is BaseSpout)
                ((BaseSpout)component).Next();
            else if (component is BaseBolt)
            {
                List<List<Object>> input = DumpChannel(prev_component);
                for (int i = 0; i < input.Count; i++)
                {
                    ExecuteTuple et = new ExecuteTuple()
                    {
                        Tuple = input[i]
                    };
                    ((BaseBolt)component).Execute(new StormTuple(et));
                }
            }
        }

        public static bool IsChannelEmpty()
        {
            if (CacheChannel.IsEmpty() == true)
                return true;
            return false;
        }

        public static int ChannelSize()
        {
            return CacheChannel.CacheSize();
        }
    }
}

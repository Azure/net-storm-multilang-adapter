using System;
using System.Collections.Generic;
using Dotnet.Storm.Adapter.Channels;
using Dotnet.Storm.Adapter.Components;
using Dotnet.Storm.Adapter.Messaging;

namespace Dotnet.Storm.Adapter.Test
{
    /// <summary>
    /// Test API class is used to test Spout and Bolt components outside Storm
    /// </summary>
    public static class TestAPI
    {
        /// <summary>
        /// Create an instance of the specified component type
        /// </summary>
        /// <param name="type">The component type</param>
        /// <param name="sc">Storm context</param>
        /// <param name="config">Storm configuration</param>
        /// <returns></returns>
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

        /// <summary>
        /// Dump all messages out of channel cache
        /// </summary>
        /// <returns></returns>
        public static List<List<Object>> DumpChannel()
        {
            List <List<Object>> res = new List<List<Object>>();

            while (CacheChannel.IsEmpty() == false)
            {
                OutMessage message = ((CacheChannel)Channel.Instance).OutputMessage();
                if (message is SpoutTuple)
                {
                    res.Add(((SpoutTuple)message).Tuple);
                }
                else if (message is BoltTuple)
                {
                    res.Add(((BoltTuple)message).Tuple);
                }
            }

            return res;
        }

        /// <summary>
        /// Run a component in test mode
        /// </summary>
        /// <param name="component">Teh component to be run</param>
        public static void Run(Component component)
        {
            if (component is BaseSpout)
                ((BaseSpout)component).Next();
            else if (component is BaseBolt)
            {
                List<List<Object>> input = DumpChannel();
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

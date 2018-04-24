// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

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
    public static class TestApi
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
            // Create component instance
            Component comp = (Component)Activator.CreateInstance(type);

            // Set context and configuration singleton
            comp.Context = sc;
            comp.Configuration = config;
            comp.Channel = new CacheChannel();

            return comp;
        }

        /// <summary>
        /// Dump all tuples out of channel cache
        /// </summary>
        /// <returns></returns>
        public static List<TestOutput> DumpChannel(Component c)
        {
            List<TestOutput> res = new List<TestOutput>();
            CacheChannel comp_channel = (CacheChannel)c.Channel;

            while (comp_channel.IsEmpty() == false)
            {
                OutMessage message = comp_channel.OutputMessage();
                if (message is SpoutTuple)
                {
                    res.Add(new SpoutOutput((SpoutTuple)message, c.Context.ComponentId));
                }
                else if (message is BoltTuple)
                {
                    res.Add(new BoltOutput((BoltTuple)message, c.Context.ComponentId));
                }
            }

            return res;
        }
    }
}

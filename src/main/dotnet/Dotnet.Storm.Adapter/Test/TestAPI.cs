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
using System.Collections.Generic;
using Dotnet.Storm.Adapter.Channels;
using Dotnet.Storm.Adapter.Components;
using Dotnet.Storm.Adapter.Messaging;

namespace Dotnet.Storm.Adapter.Test
{
    public static class TestApi
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

        /// <summary>
        /// Dump all messages out of channel
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
        /// <param name="component">component's name</param>
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

        public static int ChannelSize()
        {
            return CacheChannel.CacheSize();
        }
    }
}

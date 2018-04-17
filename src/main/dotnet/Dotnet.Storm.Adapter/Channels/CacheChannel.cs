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
using System.Reflection.Metadata.Ecma335;
using System.Text;
using log4net;
using Dotnet.Storm.Adapter.Messaging;
using Dotnet.Storm.Adapter.Serializers;

namespace Dotnet.Storm.Adapter.Channels
{
    internal class CacheChannel : Channel
    {
        private readonly static ILog Logger = LogManager.GetLogger(typeof(Channel));

        private static Queue<Message> Cache = new Queue<Message>();

        public override void Send(OutMessage message)
        {
            Cache.Enqueue(message);
        }

        public override InMessage Receive<T>()
        {
            if (Cache.Count == 0)
            {
                return null;
            }
            else
            {
                return (InMessage)Cache.Dequeue();
            }
        }

        public OutMessage OutputMessage()
        {
            return (OutMessage)Cache.Dequeue();
        }

        public void InputMessage(InMessage m)
        {
            Cache.Enqueue(m);
        }

        public static bool IsEmpty()
        {
            if (Cache.Count == 0)
                return true;
            return false;
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Dotnet.Storm.Adapter.Messaging;

namespace Dotnet.Storm.Adapter.Channels
{
    internal class CacheChannel : Channel
    {
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

        public static bool IsEmpty()
        {
            if (Cache.Count == 0)
                return true;
            return false;
        }

        public static int CacheSize()
        {
            return Cache.Count;
        }
    }
}

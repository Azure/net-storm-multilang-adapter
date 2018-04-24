// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Dotnet.Storm.Adapter.Messaging;

namespace Dotnet.Storm.Adapter.Channels
{
    internal class CacheChannel : Channel
    {
        private Queue<Message> Cache = new Queue<Message>();

        public override void Send(OutMessage message)
        {
            Cache.Enqueue(message);
        }

        public override InMessage Receive<T>()
        {
            throw new NotImplementedException();
        }

        public OutMessage OutputMessage()
        {
            return (OutMessage)Cache.Dequeue();
        }

        public bool IsEmpty()
        {
            if (Cache.Count == 0)
                return true;
            return false;
        }
    }
}
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Dotnet.Storm.Adapter.Messaging;
using Dotnet.Storm.Adapter.Serializers;

namespace Dotnet.Storm.Adapter.Channels
{
    internal abstract class Channel
    {
        internal Serializer Serializer { get; set; }

        public abstract void Send(OutMessage message);

        public abstract InMessage Receive<T>() where T : InMessage;
    }
}

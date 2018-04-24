// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Dotnet.Storm.Adapter.Messaging;

namespace Dotnet.Storm.Adapter.Serializers
{
    internal abstract class Serializer
    {
        internal abstract string Serialize<T>(T input) where T : OutMessage;

        internal abstract T Deserialize<T>(string input) where T : InMessage;
    }
}

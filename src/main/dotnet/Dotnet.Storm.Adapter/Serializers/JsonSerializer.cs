// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Newtonsoft.Json;

namespace Dotnet.Storm.Adapter.Serializers
{
    internal class JsonSerializer : Serializer
    {
        internal override T Deserialize<T>(string input)
        {
            return JsonConvert.DeserializeObject<T>(input);
        }

        internal override string Serialize<T>(T input)
        {
            return JsonConvert.SerializeObject(input);
        }
    }
}

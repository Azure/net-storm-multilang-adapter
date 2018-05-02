// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using CommandLine;

namespace Dotnet.Storm.Adapter
{
    public class Options
    {

        [Option('c', "class", Required = true, HelpText = "Component class name to be instantiated.")]
        public string Class { get; set; }

        [Option('a', "arguments", Required = false, HelpText = "Command line parameters.")]
        public string Arguments { get; set; }
    }
}

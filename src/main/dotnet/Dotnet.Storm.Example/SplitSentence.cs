// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Dotnet.Storm.Adapter.Components;
using System;
using System.Collections.Generic;

namespace Dotnet.Storm.Example
{
    public class SplitSentence : BaseBolt
    {
        public SplitSentence() : base() => OnTaskIds += new EventHandler<TaskIds>(ProcessTaskIds);

        private void ProcessTaskIds(object sender, TaskIds ids)
        {
            Logger.Info($"Received task ids: {string.Join(',', ids.Ids)}");
        }

        public override void Execute(StormTuple tuple)
        {
            char[] separator = new char[] { ' ', '[', ']', '<', '>', '(', ')', '.', ',' };
            char[] trimChars = new char[] { '.', ',' };

            string[] line = tuple.Tuple[0].ToString().Split(separator);

            if(line != null && line.Length > 0)
            {
                foreach(string word in line)
                {
                    if(!string.IsNullOrEmpty(word))
                    {
                        Emit(new List<object> { word.Trim(trimChars) }, "default", 0, new List<string>() { tuple.Id }, true);
                    }
                }
            }
        }
    }
}
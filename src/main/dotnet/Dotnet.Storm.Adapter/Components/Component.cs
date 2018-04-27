// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Dotnet.Storm.Adapter.Channels;
using Dotnet.Storm.Adapter.Messaging;
using log4net;
using Newtonsoft.Json;

namespace Dotnet.Storm.Adapter.Components
{
    public abstract class Component
    {
        #region Private part
        private bool? gueranteed = null;

        private int? timeout = null;

        internal Channel Channel { get; set; }

        internal void Connect(string arguments, Channel channel)
        {
            Logger.Debug($"Current arguments: {arguments}.");
            Arguments = !string.IsNullOrEmpty(arguments) ? arguments.Split(new char[] { ' ' }) : new string[0];

            Logger.Debug($"Current config: {channel.GetType().Name}.");
            Channel = channel;

            // waiting for storm to send connect message
            Logger.Debug("Waiting for connect message.");
            ConnectMessage message = (ConnectMessage)Channel.Receive<ConnectMessage>();

            int pid = Process.GetCurrentProcess().Id;

            // storm requires to create empty file named with PID
            if (!string.IsNullOrEmpty(message.PidDir) && Directory.Exists(message.PidDir))
            {
                Logger.Debug($"Creating pid file. PidDir: {message.PidDir}; PID: {pid}");
                string path = Path.Combine(message.PidDir, pid.ToString());
                File.WriteAllText(path, "");
            }

            Logger.Debug($"Current context: {JsonConvert.SerializeObject(message.Context)}.");
            Context = message.Context;

            Logger.Debug($"Current config: {JsonConvert.SerializeObject(message.Configuration)}.");
            Configuration = message.Configuration;

            // send PID back to storm
            Channel.Send(new PidMessage(pid));

            OnInitialized?.Invoke(this, EventArgs.Empty);
        }

        internal void RiseTaskIds(TaskIds args)
        {
            OnTaskIds?.Invoke(this, args);
        }

        internal abstract void Start();
        #endregion

        #region Component interface
        protected void Sync()
        {
            Channel.Send(new SyncMessage());
        }

        protected void Error(string message)
        {
            Channel.Send(new ErrorMessage(message));
        }

        protected void Metrics(string name, object value)
        {
            Channel.Send(new MetricMessage(name, value));
        }

        protected VerificationResult VerifyInput(string component, string stream, List<object> tuple)
        {
            if(stream == "__heartbeat" || stream == "__tick")
            {
                return new VerificationResult(false, "Input: OK");
            }
            if (string.IsNullOrEmpty(component))
            {
                return new VerificationResult(true, "Input: component is null");
            }
            if (string.IsNullOrEmpty(stream))
            {
                return new VerificationResult(true, "Input: stream is null");
            }
            if (tuple == null)
            {
                return new VerificationResult(true, "Input: tuple is null");
            }
            if (!Context.SourceToStreamToFields.ContainsKey(component))
            {
                return new VerificationResult(true, $"Input: component '{component}' is not defined as an input");
            }
            if (!Context.SourceToStreamToFields[component].ContainsKey(stream))
            { 
                return new VerificationResult(true, $"Input: component '{component}' doesn't contain '{stream}' stream");
            }
            int count = Context.SourceToStreamToFields[component][stream].Count;
            if (count != tuple.Count)
            {
                return new VerificationResult(true, $"Input: tuple contains [{tuple.Count}] fields but the {stream} stream can process only [{count}]");
            }
            return new VerificationResult(false, "Input: OK");
        }

        protected VerificationResult VerifyOutput(string stream, List<object> tuple)
        {
            if (string.IsNullOrEmpty(stream))
            {
                return new VerificationResult(true, "Output: stream is null");
            }
            if (tuple == null)
            {
                return new VerificationResult(true, "Output: tuple is null");
            }
            if (!Context.StreamToOputputFields.ContainsKey(stream))
            {
                return new VerificationResult(true, $"Output: component doesn't contain {stream} stream");
            }
            int count = Context.StreamToOputputFields[stream].Count;
            if (count != tuple.Count)
            {
                return new VerificationResult(true, $"Output: tuple contains [{tuple.Count}] fields but the {stream} stream can process only [{count}]");
            }
            return new VerificationResult(false, "Output: OK");
        }

        protected readonly static ILog Logger = LogManager.GetLogger(typeof(Component));

        protected string[] Arguments { get; private set; }

        protected bool IsGuaranteed
        {
            get
            {
                if(gueranteed == null)
                {
                    if (!Configuration.ContainsKey("topology.acker.executors"))
                        gueranteed = false;
                    else
                    {
                        object number = Configuration["topology.acker.executors"];

                        if (number == null)
                            gueranteed = true;
                        else
                        {
                            if (int.TryParse(number.ToString(), out int result))
                                gueranteed = result != 0;
                            else
                                gueranteed = false;
                        }
                    }
                }
                return gueranteed.Value;
            }
        }

        protected int Timeout
        {
            get
            {
                if(!timeout.HasValue)
                {
                    if (!Configuration.ContainsKey("topology.message.timeout.secs"))
                        timeout = 30;
                    else
                    {
                        object number = Configuration["topology.message.timeout.secs"];
                        if (number == null)
                            timeout = 30;
                        else
                        {
                            if (int.TryParse(number.ToString(), out int result))
                                timeout = result;
                            else
                                timeout = 30;
                        }
                    }
                }
                return timeout.Value;
            }
        }

        protected event EventHandler<TaskIds> OnTaskIds;

        protected event EventHandler OnInitialized;

        public IDictionary<string, object> Configuration { get; internal set; }

        public StormContext Context { get; internal set; }
        #endregion
    }
}

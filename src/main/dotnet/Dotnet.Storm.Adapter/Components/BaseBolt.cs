// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Dotnet.Storm.Adapter.Channels;
using Dotnet.Storm.Adapter.Messaging;
using System;
using System.Collections.Generic;

namespace Dotnet.Storm.Adapter.Components
{
    public abstract class BaseBolt : Component
    {
        #region Private part
        private bool running = true;

        internal override void Start()
        {
            Logger.Info($"Starting bolt: {Context.ComponentId}.");

            while (running)
            {
                InMessage message = Channel.Receive<ExecuteTuple>();
                if (message != null) // there are only two options: task_ids and tuple to execute
                {
                    if (message is TaskIdsMessage ids)
                    {
                        RiseTaskIds(new TaskIds(ids));
                    }
                    if (message is ExecuteTuple tuple)
                    {
                        VerificationResult result = VerifyInput(tuple.Component, tuple.Stream, tuple.Tuple);

                        if (result.IsError)
                        {
                            Logger.Error($"Validation failed: {result} in {tuple}.");
                        }
                        else
                        {
                            switch (tuple.Stream)
                            {
                                case "__heartbeat":
                                    DoHeartbeat();
                                    break;
                                case "__tick":
                                    DoTick();
                                    break;
                                default:
                                    DoExecute(tuple);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private void DoHeartbeat()
        {
            Sync();
        }

        private void DoTick()
        {
            OnTick?.Invoke(this, EventArgs.Empty);
        }

        private void DoExecute(ExecuteTuple tuple)
        {
            try
            {
                Execute(new StormTuple(tuple));
                if (IsGuaranteed)
                {
                    Ack(tuple.Id);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to process tuple. {ex}");
                if (IsGuaranteed)
                {
                    Fail(tuple.Id);
                }
            }
        }

        private void Ack(string id)
        {
            Channel.Send(new AckMessage(id));
        }

        private void Fail(string id)
        {
            Channel.Send(new FailMessage(id));
        }

        #endregion

        #region Bolt interface
        public abstract void Execute(StormTuple tuple);

        protected event EventHandler OnTick;

        protected void Emit(List<object> tuple, string stream = "default", long task = 0, List<string> anchors = null, bool needTaskIds = false)
        {
            VerificationResult result = VerifyOutput(stream, tuple);

            if (result.IsError)
            {
                Logger.Error($"{result} for nex tuple: {tuple}.");
            }
            else
            {
                BoltTuple message = new BoltTuple()
                {
                    Task = task,
                    Stream = stream,
                    Tuple = tuple,
                    Anchors = anchors,
                    NeedTaskIds = needTaskIds
                };

                Channel.Send(message);
            }
        }
        #endregion
    }
}

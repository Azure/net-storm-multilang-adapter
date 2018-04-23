﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Dotnet.Storm.Adapter.Channels;
using Dotnet.Storm.Adapter.Messaging;
using System;
using System.Collections.Generic;

namespace Dotnet.Storm.Adapter.Components
{
    public abstract class BaseBolt : Component
    {
        protected new class Storm : Component.Storm
        {
            public static void Ack(string id)
            {
                Channel.Instance.Send(new AckMessage(id));
            }

            public static void Fail(string id)
            {
                Channel.Instance.Send(new FailMessage(id));
            }

            public static void Emit(List<object> tuple, string stream = "default", long task = 0, List<string> anchors = null, bool needTaskIds = false)
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

                    Channel.Instance.Send(message);
                }
            }
        }

        private bool running = true;

        internal override void Start()
        {
            OnInitialized?.Invoke(this, EventArgs.Empty);

            while (running)
            {
                InMessage message = Channel.Instance.Receive<ExecuteTuple>();
                if (message != null)
                {
                    // there are only two options: task_ids and tuple to execute
                    if (message is TaskIdsMessage ids)
                    {
                        OnTaskIds?.Invoke(this, new TaskIds(ids));
                    }
                    if (message is ExecuteTuple tuple)
                    {
                        VerificationResult result = Storm.VerifyInput(tuple.Component, tuple.Stream, tuple.Tuple);

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
            Storm.Sync();
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
                    Storm.Ack(tuple.Id);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to process tuple. {ex}");
                if (IsGuaranteed)
                {
                    Storm.Fail(tuple.Id);
                }
            }
        }

        #region Bolt interface
        public abstract void Execute(StormTuple tuple);

        protected event EventHandler OnInitialized;

        protected event EventHandler OnTick;

        protected event EventHandler<TaskIds> OnTaskIds;
        #endregion
    }
}

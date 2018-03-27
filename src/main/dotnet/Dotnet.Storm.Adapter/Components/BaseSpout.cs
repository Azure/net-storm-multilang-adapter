﻿using Dotnet.Storm.Adapter.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Dotnet.Storm.Adapter.Components
{
    public abstract class BaseSpout : Component
    {
        private static Random random = new Random();

        private const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";

        internal static MemoryCache PendingQueue;

        internal static CacheItemPolicy policy;

        private bool running = true;

        protected new class LocalStorm : Component.LocalStorm
        {
            public void Emit(List<object> tuple, string stream = "default", long task = 0, bool needTaskIds = false)
            {
                string id = null;
                if (IsGuarantee)
                {
                    id = NextId();
                }

                VerificationResult result = VerifyOutput(stream, tuple);

                if (result.IsError)
                {
                    Logger.Error($"Validation failed: {result} in {tuple}.");
                }
                else
                {
                    SpoutTuple message = new SpoutTuple()
                    {
                        Id = id,
                        Task = task,
                        Stream = stream,
                        Tuple = tuple,
                        NeedTaskIds = needTaskIds
                    };

                    GlobalStorm.Send(message);

                    PendingQueue.Set(id, message, policy);
                }
            }
        }

        public BaseSpout()
        {
            Storm = new LocalStorm();
            PendingQueue = MemoryCache.Default;
        }

        internal override void Start()
        {
            policy = new CacheItemPolicy()
            {
                SlidingExpiration = new TimeSpan(0, MessageTimeout, 0)
            };

            while (running)
            {
                Message message = GlobalStorm.Receive<CommandMessage>();
                if (message != null)
                {
                    // there are only two options: task_ids and command
                    if (message is TaskIdsMessage ids)
                    {
                        OnTaskIds?.Invoke(this, new TaskIds(ids));
                    }
                    else
                    {
                        CommandMessage command = (CommandMessage)message;

                        switch (command.Command)
                        {
                            case "next":
                                DoNext();
                                break;
                            case "ack":
                                DoAck(command.Id);
                                break;
                            case "fail":
                                DoFail(command.Id);
                                break;
                            case "activate":
                                DoActivate();
                                break;
                            case "deactivate":
                                DoDeactivate();
                                break;
                        }

                    }
                    DoSync();
                }
            }
        }

        private void DoSync()
        {
            if (IsEnabled)
            {
                Storm.Sync();
            }
        }

        private void DoNext()
        {
            if (IsEnabled)
            {
                Next();
            }
        }

        private void DoAck(string id)
        {
            if (IsEnabled)
            {
                if (PendingQueue.Contains(id))
                {
                    PendingQueue.Remove(id);
                }
                else
                {
                    Logger.Warn($"Fail to ack message. Pending queue doesn't contain message: {id}.");
                }
            }
        }

        private void DoFail(string id)
        {
            if (IsEnabled)
            {
                if (PendingQueue.Contains(id))
                {
                    if (PendingQueue.Get(id) is SpoutTuple message)
                    {
                        GlobalStorm.Send(message);
                    }
                }
                else
                {
                    Logger.Warn($"Fail to resend message. Pending queue doesn't contain message: {id}.");
                }
            }
        }

        private void DoActivate()
        {
            if (!IsEnabled)
            {
                try
                {
                    OnActivate?.Invoke(this, EventArgs.Empty);

                    IsEnabled = true;
                }
                catch (Exception ex)
                {
                    Logger.Error("Failed to activate component.", ex);
                }
            }
        }

        private void DoDeactivate()
        {
            if (IsEnabled)
            {
                try
                {
                    OnDeactivate?.Invoke(this, EventArgs.Empty);

                    IsEnabled = true;
                }
                catch (Exception ex)
                {
                    Logger.Error("Failed to deactivate component.", ex);
                }
            }
        }

        private static string NextId()
        {
            return "id" + new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        #region Spout interface
        protected event EventHandler OnActivate;

        protected event EventHandler OnDeactivate;

        protected event EventHandler<TaskIds> OnTaskIds;

        protected new LocalStorm Storm { get; private set; }

        protected bool IsEnabled { get; private set; } = false;

        protected abstract void Next();
        #endregion
    }
}

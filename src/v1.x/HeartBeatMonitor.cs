﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
//using System.Threading;

#if NETFX_CORE
using Windows.System.Threading;
#endif

namespace Microsoft.AspNet.SignalR.Client
{
    public class HeartbeatMonitor : IDisposable
    {
#if !NETFX_CORE
        // Timer to determine when to notify the user and reconnect if required
        private Timer _timer;
#else
        private ThreadPoolTimer _timer;
#endif
        // Used to ensure that the Beat only executes when the connection is in the Connected state
        private readonly object _connectionStateLock;

        // Connection variable
        private readonly IConnection _connection;

        // To keep track of whether the user has been notified
        public bool HasBeenWarned { get; private set; }

        // To keep track of whether the client is already reconnecting
        public bool TimedOut { get; private set; }

        /// <summary>
        /// Initializes a new instance of the HeartBeatMonitor Class 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="connectionStateLock"></param>
        public HeartbeatMonitor(IConnection connection, object connectionStateLock)
        {
            _connection = connection;
            _connectionStateLock = connectionStateLock;
        }

        /// <summary>
        /// Updates LastKeepAlive and starts the timer
        /// </summary>
        public void Start()
        {
            _connection.UpdateLastKeepAlive();
            HasBeenWarned = false;
            TimedOut = false;
#if !NETFX_CORE
            _timer = new Timer(_ => Beat(), state: null, dueTime: _connection.KeepAliveData.CheckInterval, period: _connection.KeepAliveData.CheckInterval);
#else
            _timer = ThreadPoolTimer.CreatePeriodicTimer((timer) => Beat(), period: _connection.KeepAliveData.CheckInterval);
#endif
        }

        /// <summary>
        /// Callback function for the timer which determines if we need to notify the user or attempt to reconnect
        /// </summary>
#if NETFX_CORE
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Timer is not implemented on WinRT")]
#endif
        private void Beat()
        {
            TimeSpan timeElapsed = DateTime.UtcNow - _connection.KeepAliveData.LastKeepAlive;
            Beat(timeElapsed);
        }

        /// <summary>
        /// Logic to determine if we need to notify the user or attempt to reconnect
        /// </summary>
        /// <param name="timeElapsed"></param>
        public void Beat(TimeSpan timeElapsed)
        {
            lock (_connectionStateLock)
            {
                if (_connection.State == ConnectionState.Connected)
                {
                    if (timeElapsed >= _connection.KeepAliveData.Timeout)
                    {
                        if (!TimedOut)
                        {
                            // Connection has been lost
                            _connection.Trace(TraceLevels.Events, "Connection Timed-out : Transport Lost Connection");
                            TimedOut = true;
                            _connection.Transport.LostConnection(_connection);
                        }
                    }
                    else if (timeElapsed >= _connection.KeepAliveData.TimeoutWarning)
                    {
                        if (!HasBeenWarned)
                        {
                            // Inform user and set HasBeenWarned to true
                            _connection.Trace(TraceLevels.Events, "Connection Timeout Warning : Notifying user");
                            HasBeenWarned = true;
                            _connection.OnConnectionSlow();
                        }
                    }
                    else
                    {
                        HasBeenWarned = false;
                        TimedOut = false;
                    }
                }
            }
        }

        /// <summary>
        /// Dispose off the timer
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose off the timer
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_timer != null)
                {
#if !NETFX_CORE
                
                    _timer.Dispose();
                    _timer = null;
#else
                    _timer.Cancel();
                    _timer = null;
#endif
                }

            }
        }
    }
}


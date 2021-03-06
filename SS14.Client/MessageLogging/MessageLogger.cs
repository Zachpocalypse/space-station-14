﻿using SS14.Client.Interfaces.MessageLogging;
using SS14.Shared;
using SS14.Shared.GameObjects;
using SS14.Shared.Interfaces.Configuration;
using SS14.Shared.IoC;
using System;
using System.ServiceModel;
using System.Timers;

namespace SS14.Client.MessageLogging
{
    public class MessageLogger : IMessageLogger
    {
        [Dependency]
        private readonly IConfigurationManager _configurationManager;
        private Timer _pingTimer;
        private MessageLoggerServiceClient _loggerServiceClient;
        private bool _logging;

        public void Initialize()
        {
            _logging = _configurationManager.GetCVar<bool>("log.enabled");

            if (_logging)
            {
                _loggerServiceClient = new MessageLoggerServiceClient("NetNamedPipeBinding_IMessageLoggerService");
                Ping();
                _pingTimer = new Timer(5000);
                _pingTimer.Elapsed += CheckServer;
                _pingTimer.Enabled = true;
            }
        }

        #region IMessageLogger Members

        /// <summary>
        /// Check to see if the server is still running
        /// </summary>
        public void Ping()
        {
            bool failed = false;
            try
            {
                bool up = _loggerServiceClient.ServiceStatus();
            }
            catch (CommunicationException)
            {
                failed = true;
            }
            finally
            {
                if (failed)
                    _logging = false;
            }
        }

        public void LogOutgoingComponentNetMessage(int uid, ComponentFamily family, object[] parameters)
        {
            if (!_logging)
                return;

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] is Enum)
                    parameters[i] = (int) parameters[i];
            }
            try
            {
                _loggerServiceClient.LogClientOutgoingNetMessage(uid, (int) family, parameters);
            }
            catch (CommunicationException)
            {
            }
        }

        public void LogIncomingComponentNetMessage(int uid, EntityMessage entityMessage, ComponentFamily componentFamily,
                                                   object[] parameters)
        {
            if (!_logging)
                return;

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] is Enum)
                    parameters[i] = (int) parameters[i];
            }
            try
            {
                _loggerServiceClient.LogClientIncomingNetMessage(uid, (int) entityMessage, (int) componentFamily,
                                                                 parameters);
            }
            catch (CommunicationException)
            {
            }
        }

        public void LogComponentMessage(int uid, ComponentFamily senderfamily, string sendertype,
                                        ComponentMessageType type)
        {
            if (!_logging)
                return;

            try
            {
                _loggerServiceClient.LogClientComponentMessage(uid, (int) senderfamily, sendertype, (int) type);
            }
            catch (CommunicationException)
            {
            }
        }

        #endregion

        public static void CheckServer(object source, ElapsedEventArgs e)
        {
            IoCManager.Resolve<IMessageLogger>().Ping();
        }
    }
}

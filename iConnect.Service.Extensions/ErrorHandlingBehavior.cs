using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Collections.ObjectModel;
using System.ServiceModel;

namespace iConnect.Service.Extensions
{
    /// <summary>
    /// Installing the Error Handling Behavior on every channel dispatcher on service
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ErrorHandlingBehavior : Attribute, IServiceBehavior
    {
        private Type _exceptionToFaultConverterType;

        public bool EnforceFaultContract { get; set; }
        public Type ExceptionToFaultConverter
        {
            get
            {
                return _exceptionToFaultConverterType;
            }
            set
            {
                if (!typeof(IExceptionToFaultConverter).IsAssignableFrom(value))
                    throw new ArgumentException("Fault converter doesn't implement IExceptionToFaultConverter.", "value");
                _exceptionToFaultConverterType = value;
            }
        }

        #region IServiceBehavior Members

        public void AddBindingParameters(ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// To hook up ChannelDispatcher for any unhandled exception escapes the service code
        /// </summary>
        /// <param name="serviceDescription"></param>
        /// <param name="serviceHostBase"></param>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcherBase chanDispBase in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher channelDispatcher = chanDispBase as ChannelDispatcher;
                if (channelDispatcher == null)
                    continue;
                channelDispatcher.ErrorHandlers.Add(new ErrorHandler(this));
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        #endregion
    }
}

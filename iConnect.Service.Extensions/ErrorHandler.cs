using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Reflection;

namespace iConnect.Service.Extensions
{
    sealed class ErrorHandler : IErrorHandler
    {
        private ErrorHandlingBehavior _attribute;
        private IExceptionToFaultConverter _converter;

        public ErrorHandler(ErrorHandlingBehavior attribute)
        {
            _attribute = attribute;
            if (_attribute.ExceptionToFaultConverter != null)
                _converter = (IExceptionToFaultConverter)Activator.CreateInstance(_attribute.ExceptionToFaultConverter);
        }

        #region IErrorHandler Members

        /// <summary>
        /// To log/process error in WCF application
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool HandleError(Exception error)
        {
            return true;
        }

        /// <summary>
        /// To convert Application error to fault contract error
        /// </summary>
        /// <param name="error"></param>
        /// <param name="version"></param>
        /// <param name="fault"></param>
        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            //If it's a FaultException already, then we have nothing to do
            if (error is FaultException)
                return;
            string _exception = "Error: " + error.Message;
            if (error.InnerException != null)
                _exception += "\r\n" + "Inner Exception: " + error.InnerException.Message;
            MessageFault messageFault = MessageFault.CreateFault(new FaultCode("sender"), new FaultReason(_exception));
                fault = Message.CreateMessage(version, messageFault, null);

        }
        #endregion
    }
}

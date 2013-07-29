using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using NLog;

namespace iConnect.Common
{

    public static class LogManager
    {

        /// <summary>
        /// Loads required NLog layout renderers
        /// </summary>
        static LogManager()
        {
            //Register all the layout renderers and target for this assembly
            LayoutRendererFactory.AddLayoutRenderersFromAssembly(System.Reflection.Assembly.GetExecutingAssembly(), "");
        }


        /// <summary>
        /// Returns a logger with the specified name.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <returns>The logger.</returns>
        public static ILogger GetLogger(string name)
        {
            return new NLogLogger(NLog.LogManager.GetLogger(name));
        }

        /// <summary>
        /// Gets the logger named after the currently-being-initialized class.
        /// </summary>
        /// <value>The logger.</value>
        /// <remarks>This is a slow-running method. 
        /// Make sure you're not doing this in a loop.</remarks>
        public static ILogger CurrentClassLogger
        {
            get
            {
                var frame = new StackFrame(1, false);

                var logger = new NLogLogger(NLog.LogManager.GetLogger(frame.GetMethod().DeclaringType.FullName));

                return logger;
            }
        }

        public static void ConfigureFileLogger(ILogger logger, string fileName)
        {
            if (logger == null)
            {
                return;
            }
            if (logger.GetType() == typeof(NLogLogger))
            {
                ((NLog.Targets.FileTarget)
                 (((NLog.Targets.Wrappers.WrapperTargetBase)
                   (((NLogLogger)logger).Logger.Factory.Configuration.GetConfiguredNamedTargets()[1])).WrappedTarget)).
                    FileName = fileName;
            }

        }

        /// <summary>
        /// Gets the logger named after the specified class type.
        /// </summary>
        /// <param name="classType">The type of the class.</param>
        /// <returns>The logger.</returns>
        public static ILogger GetClassLogger(Type classType)
        {
            if (classType == null)
            {
                return null;
            }
            NLogLogger logger = new NLogLogger(NLog.LogManager.GetLogger(classType.FullName));

            return logger;
        }


        /// <summary>
        /// Returns the logger for the current context. This logger can for example be set by an
        /// outer class in order for the sub utilities / classes to use the specific logger.
        /// </summary>
        /// <value></value>
        public static ILogger CurrentContextLogger
        {
            get
            {
                ILogger contextLogger = null;
                try
                {
                    contextLogger = FindThreadLoggerContext();
                }
                catch (Exception)
                {
                    //We don't care... but may not fail
                }

                if (contextLogger == null)
                {
                    return CurrentClassLogger;
                }

                return contextLogger;
            }
        }

        /// <summary>
        /// Sets the current contextual logger.
        /// </summary>
        /// <param name="logger">The logger to set, if null then the next time the contextual logger is retrieved, GetCurrentClassLogger will be used.</param>
        public static void SetCurrentContextLogger(ILogger logger)
        {
            try
            {
                SetThreadLoggerContext(logger);
            }
            catch (Exception)
            {
                throw;
                //We don't care... but may not fail
            }
        }

        /// <summary>
        /// Uses ThreadManager to get a ThreadContext, and sets the "ThreadContextLogger" property.
        /// </summary>
        /// <param name="logger"></param>
        private static void SetThreadLoggerContext(ILogger logger)
        {
            Thread thread = Thread.CurrentThread;
            ThreadContext context = ThreadManager.GetThreadContext(thread);
            if (context != null)
            {
                object loggersList = null;
                context.Properties.TryGetValue("ThreadContextLogger", out loggersList);
                var loggers = loggersList as LinkedList<ILogger>;
                if (loggers == null)
                {
                    loggers = new LinkedList<ILogger>();
                    context.Properties["ThreadContextLogger"] = loggers;
                }
                loggers.AddLast(logger);
                context.Properties["IsContextLoggerDirty"] = true;
            }
        }

        /// <summary>
        /// Recursively iterates from the current thread up through the parents searching for a logger
        /// in the thread context.
        /// </summary>
        /// <returns>The logger.</returns>
        private static ILogger FindThreadLoggerContext()
        {
            ILogger logger = null;
            Thread currentThread = Thread.CurrentThread;
            while (currentThread != null)
            {
                ThreadContext context = ThreadManager.GetThreadContext(currentThread);
                if (context != null)
                {
                    object loggersList = null;
                    if (context.Properties.TryGetValue("ThreadContextLogger", out loggersList))
                    {
                        var loggers = loggersList as LinkedList<ILogger>;
                        if (loggers != null && loggers.Count > 0)
                        {
                            logger = loggers.Last.Value;
                        }
                    }
                }

                if (logger == null)
                {
                    currentThread = ThreadManager.GetParentThread(currentThread);
                }
                else
                {
                    break;
                }
            }

            return logger;
        }

    }
}

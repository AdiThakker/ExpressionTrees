using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ExpressionTrees.MethodDispatcher
{
    public class DynamicMethodDispatcher
    {
        #region Private Members

        IEnumerable<Type> _handlerTypes;

        #endregion

        #region Constructors

        public DynamicMethodDispatcher()
        {
            _handlerTypes = Assembly.GetExecutingAssembly()
                                    .GetTypes()
                                    .Where(type => type.Name.EndsWith("Handler"));
        }

        #endregion

        #region Using Reflection

        public void InvokeUsingReflection()
        {
            Dictionary<string, Object> requestHandlers = new Dictionary<string, Object>();

            _handlerTypes.ToList().ForEach(type =>
            {
                var handlerInstance = Activator.CreateInstance(type);
                var handledType = type.GetProperty("HandledType").GetValue(handlerInstance) as Type;
                requestHandlers.Add(handledType.Name, handlerInstance);
            });

            // Test
            var timer = Stopwatch.StartNew();

            for (int i = 0; i < 100000; i++)
            {
                requestHandlers["HttpRequest"].GetType().GetMethod("Handle").Invoke(requestHandlers["HttpRequest"], new Object[] { new HttpRequest() });
                requestHandlers["FtpRequest"].GetType().GetMethod("Handle").Invoke(requestHandlers["FtpRequest"], new Object[] { new FtpRequest() });
                requestHandlers["RemoteConnectionRequest"].GetType().GetMethod("Handle").Invoke(requestHandlers["RemoteConnectionRequest"], new Object[] { new RemoteConnectionRequest() });
            }

            timer.Stop();

            Console.WriteLine("Total Time to execute 100000 iterations using Reflection: " + timer.ElapsedMilliseconds + " ms.");

        }

        #endregion

        #region Using Expression Trees

        public void InvokeUsingDynamicMethod()
        {
            Dictionary<string, Action<Object>> methodHandlers = new Dictionary<string, Action<Object>>();

            _handlerTypes.ToList().ForEach(type =>
            {
                var handlerTuple = Utility.GenerateDynamicDispatcherMethod(type);
                methodHandlers.Add(handlerTuple.Item1, handlerTuple.Item2);
            });

            var timer = Stopwatch.StartNew();

            for (int i = 0; i < 100000; i++)
            {
                methodHandlers["HttpRequest"].Invoke(new HttpRequest());
                methodHandlers["FtpRequest"].Invoke(new FtpRequest());
                methodHandlers["RemoteConnectionRequest"].Invoke(new RemoteConnectionRequest());
            }

            timer.Stop();

            Console.WriteLine("Total Time to execute 100000 iterations using Expression: " + timer.ElapsedMilliseconds + " ms.");
        }

        #endregion

    }
}

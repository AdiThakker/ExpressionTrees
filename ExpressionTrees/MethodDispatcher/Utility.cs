using System;
using System.Linq.Expressions;

namespace ExpressionTrees.MethodDispatcher
{
    internal static class Utility
    {
        public static Tuple<string, Action<Object>> GenerateDynamicDispatcherMethod(Type handlerType)
        {
            // Get the instance
            var handlerInstance = Activator.CreateInstance(handlerType);
            var handledType = handlerType.GetProperty("HandledType").GetValue(handlerInstance) as Type;

            // build the Lambda param of Object type
            var parameterExpression = Expression.Parameter(typeof(Object), "param");          // = (Object param) =>

            // Cast the Object param to the actual parameter type which is to be passed to the method
            var convertExpression = Expression.Convert(parameterExpression, handledType);
            var variableExpression = Expression.Variable(handledType, "handledType");
            var convertedParamExpression = Expression.Assign(variableExpression, convertExpression);

            // Get the method to invoke
            var methodToInvoke = handlerType.GetMethod("Handle", new Type[] { handledType });

            // Build the Method Call
            var methodCallExpression = Expression.Call(Expression.Constant(handlerInstance),
                                                        methodToInvoke, variableExpression);

            // Build the Lambda Body
            var methodExpression = Expression.Block(new[] { parameterExpression, variableExpression }, convertedParamExpression, methodCallExpression);

            // Build the Lambda Expression
            var lambdaExpression = Expression.Lambda<Action<Object>>(methodExpression, parameterExpression);

            return new Tuple<string, Action<object>>(handledType.Name, lambdaExpression.Compile());

            #region Methods built after 3 iterations

            //(Object param) =>
            //{
            //    HttpRequest handledType = (HttpRequest)param;
            //    HttpRequestHandler.Handle(param);
            //};

            //(Object param) =>
            //{
            //    FtpRequest handledType = (FtpRequest)param;
            //    FtpRequestHandler.Handle(param);
            //};

            //(Object param) =>
            //{
            //    TerminalServiceRequest handledType = (TerminalServiceRequest)param;
            //    TerminalServiceRequestHandler.Handle(param);
            //}; 

            #endregion
        }
    }
}

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace ExpressionTrees.Serialization
{
    public class ExpressionUtility
    {
        public static void Save(MethodInfo methodInfo, Object paramValue)
        {
            // Get the method parameter
            var parameterInfo = methodInfo.GetParameters().Single();

            // build the parameter Expression
            var parameterExpression = Expression.Parameter(parameterInfo.ParameterType, "value");

            // Build the Body Expression
            var bodyExpression = Expression.Call(Expression.New(methodInfo.DeclaringType), methodInfo, parameterExpression);

            // Build the Lambda Expression
            var lambdaExpression = Expression<Func<Equipment, bool>>.Lambda(bodyExpression, parameterExpression);

            // Serialize Expression and Param value
            Serializer.Serialize(lambdaExpression, paramValue);
        }

        public static void Replay()
        {
            var tuple = Serializer.Deserialize();
            var expression = tuple.Item1 as Expression<Func<Equipment, bool>>;
            var equipment = tuple.Item2 as Equipment;

            // Visit Expression & Execute wtih Null value
            var modifiedExpression = new LambdaVisitor().Visit(expression) as Expression<Func<Equipment, bool>>;
            modifiedExpression.Compile().Invoke(null);

        }

    }
}
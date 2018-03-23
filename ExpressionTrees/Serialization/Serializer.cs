using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Linq;
using ExpressionSerialization;

namespace ExpressionTrees.Serialization
{
    public class Serializer
    {
        #region Private Static Members

        private static string expressionfile = @"c:\ExpressionTrees\Lambda.txt";
        private static string paramfile = @"c:\ExpressionTrees\Param.txt";

        private static FileStream fs = null;

        private static NetDataContractSerializer paramSerializer = new NetDataContractSerializer();

        private static Assembly[] assemblies = { Assembly.GetExecutingAssembly(), typeof(ExpressionType).Assembly }; 
        
        #endregion

        public static void Serialize(LambdaExpression expression, Object paramValue)
        {
            // Serialize Expression
            var serializedExpression = new ExpressionSerializer(new TypeResolver(assemblies)).Serialize(expression);
            File.WriteAllText(expressionfile, serializedExpression.ToString());

            // Serialize Parameter
            using (fs = new FileStream(paramfile, FileMode.Create, FileAccess.ReadWrite))
            {
                paramSerializer.WriteObject(fs, paramValue); 
            }
        }

        public static Tuple<Expression, Object> Deserialize()
        {
            // Deserialize Expression & Param
            var text = File.ReadAllText(expressionfile);
            var deserializedExpression = new ExpressionSerializer(new TypeResolver(assemblies)).Deserialize(XElement.Parse(text));

            using (fs = new FileStream(paramfile, FileMode.Open, FileAccess.Read))
            {
                 var param = paramSerializer.ReadObject(fs);
                 return new Tuple<Expression, Object>(deserializedExpression, param);
            }
        }
    }
}

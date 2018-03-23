using System;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;

namespace ExpressionTrees.Serialization
{
    public class LambdaVisitor : ExpressionVisitor
    {
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            // add try catch block around the existing mathod call
            
            // Build the If condition
            var condition = Expression.Condition(Expression.Equal(node.Parameters.Single(), Expression.Constant(null)), 
                                                 Expression.Throw(Expression.Constant(new ArgumentNullException()), typeof(bool)), 
                                                 node.Body);
            
            // wrap the if condition in { }
            var body = Expression.Block(typeof(bool), condition);
            
            // Build the catch block
            var catchBlock = Expression.Catch(typeof(ArgumentNullException), 
                                              Expression.Block(
                                                Expression.Call(typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }), 
                                                                Expression.Constant("Equipment cannot be null")), Expression.Constant(false)));
            
            // Build the try block and pass in catch
            var tryCatchBlock = Expression.TryCatch(body, catchBlock);

            // finally construct the lambda
            var modifiedLambda = Expression.Lambda<T>(tryCatchBlock, node.Parameters);

            // pass the modified lambda
            return base.VisitLambda<T>(modifiedLambda);

            #region Method built
            // Method built is similar to shown here
            //try
            //{
            //    if (equipment == null)
            //    {
            //        throw new ArgumentNullException();
            //    }
            //    else
            //    {
            //        new equipmentLogic.AddItem(equipment);
            //    }
            //}
            //catch (ArgumentNullException ex)
            //{
            //    Console.WriteLine("Equipment cannot be null");
            //} 
            #endregion
        }
    }
}
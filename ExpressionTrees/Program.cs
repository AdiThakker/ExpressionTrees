using System;
using System.Linq.Expressions;
using ExpressionTrees.DynamicQuery;
using ExpressionTrees.MethodDispatcher;
using ExpressionTrees.Serialization;

namespace ExpressionTrees
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Dynamic Query

            //Cars cars = new Cars();
            //cars.Filter("BMW", "Ford");

            //cars.FilterUsingPredicateBuilder("Toyota", "General Motors");

            #endregion

            #region Dynamic Method Dispatcher

            //DynamicMethodDispatcher dynamicMethodDispatcher = new DynamicMethodDispatcher();

            //dynamicMethodDispatcher.InvokeUsingReflection();

            //Console.WriteLine();

            //dynamicMethodDispatcher.InvokeUsingDynamicMethod();

            #endregion

            #region Deferred Execution / Serialization

            // Call Method
            //new EquipmentLogic().AddItem(new Equipment() { Name = "Test Equipment" });

            //// Replay
            //ExpressionUtility.Replay();

            #endregion

            #region Scratch Pad

            //var  param = Expression.Parameter(typeof(bool), "param");
            //var body = Expression.Condition(param, Expression.Constant(1), Expression.Constant(0));
            //var lambda = Expression.Lambda<Func<bool, int>>(body, param);
            //var result = lambda.Compile()(true);

            #endregion

            Console.ReadKey();
        }

    }
}

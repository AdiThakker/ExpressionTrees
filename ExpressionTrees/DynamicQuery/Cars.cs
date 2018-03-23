using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionTrees.DynamicQuery
{
    public class Cars
    {
        private List<Car> _cars = null;

        public Cars()
        {
            _cars = new List<Car>
            { 
                new Car() { Make = "Ford" },
                new Car() { Make = "Toyota" },
                new Car() { Make = "General Motors" },
                new Car() { Make = "Honda" },
                new Car() { Make = "BMW" },
                new Car() { Make = "Chrysler" }
            };
        }

        public void Filter(params string[] makes)
        {
            BinaryExpression composedExpression     = null;
            ParameterExpression parameterExpression = Expression.Parameter(typeof(Car), "car");
            MemberExpression propertyExpression     = Expression.Property(parameterExpression, "Make");

            foreach (var make in makes)
            {
                var binaryExpression   = Expression.Equal(propertyExpression, Expression.Constant(make));

                if (composedExpression == null)
                    composedExpression = binaryExpression;
                else
                    composedExpression = Expression.OrElse(binaryExpression, composedExpression);
            }

            // compose lambda
            var lambdaExpression = Expression.Lambda<Func<Car, bool>>(composedExpression, parameterExpression);

            // pass lambda to actual Where method of Queryable
            var whereCallExpression = Expression.Call(typeof(Queryable), "Where", new Type[] { typeof(Car) }, _cars.AsQueryable().Expression, lambdaExpression);

            // Execute
            var result = _cars.AsQueryable().Provider.CreateQuery<Car>(whereCallExpression);
            result.ToList().ForEach(car => Console.WriteLine(car.Make));
        }

        public void FilterUsingPredicateBuilder(params string[] makes)
        {
            var predicate = PredicateBuilder.False<Car>();
            foreach (var make in makes)
            {
                predicate = predicate.Or<Car>(car => car.Make.Equals(make));
            }
            var result = _cars.Where(predicate.Compile());
            result.ToList().ForEach(car => Console.WriteLine(car.Make));
        }

        #region Private Class

        private class Car
        {
            public string Make { get; set; }
        }

        #endregion

    }


}

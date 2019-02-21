using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionTrees.Mapper
{
    public static class MappingExtensions
    {
        // Static Thread safe Expression cache for storing already formed expressions
        private static ConcurrentDictionary<string, Expression> _expressionCache = new ConcurrentDictionary<string, Expression>();

        /// <summary>
        /// Maps the specified source.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>
        /// Mapped object.
        /// </returns>
        public static TResult MapTo<TSource, TResult>(this TSource source) where TSource : class
        {
            Expression cachedExpression = null;
            Expression<Func<TSource, TResult>> lambdaExpression = null;

            if (!_expressionCache.TryGetValue(string.Concat(typeof(TSource).FullName, typeof(TResult).FullName), out cachedExpression))
            {
                // Get the source and destination properties
                var sourceProperties = typeof(TSource).GetProperties();
                var destinationProperties = typeof(TResult).GetProperties();

                // Set the paramter expression
                var parameterExpression = Expression.Parameter(typeof(TSource), "src");

                // Set the Body Expression
                var bodyExpression = Expression.MemberInit(Expression.New(typeof(TResult)), destinationProperties
                                                                                                .Select(destProp => SetupBinding(parameterExpression, destProp, sourceProperties))
                                                                                                .Where(binding => binding != null)
                                                                                                .ToArray());

                // Set up the Lambda Expression
                lambdaExpression = Expression.Lambda<Func<TSource, TResult>>(bodyExpression, parameterExpression);

                // Add the expression
                _expressionCache.TryAdd(string.Concat(typeof(TSource).FullName, typeof(TResult).FullName), lambdaExpression);
            }
            else
            {
                lambdaExpression = cachedExpression as Expression<Func<TSource, TResult>>;
            }

            // Compile and invoke and return the result.
            var result = lambdaExpression.Compile().Invoke(source);

            return result;
        }

        private static MemberAssignment SetupBinding(Expression parameterExpression, MemberInfo destinationProperty, IEnumerable<PropertyInfo> sourceProperties)
        {
            // Flag used to allow binding after looking at the [Map] attribute of both source and destination  property . Since only one can contain that attribute.
            bool bind = false;

            bind = CheckIfMapPropertyExists((PropertyInfo)destinationProperty);

            // Get the matching property from source which matches the passed in destination property
            var sourceProperty = sourceProperties.FirstOrDefault(prop => prop.Name == destinationProperty.Name);
            if (sourceProperty != null)
            {
                // Check to bind if not true
                if (!bind)
                    bind = CheckIfMapPropertyExists(sourceProperty);

                if (bind)
                    return Expression.Bind(destinationProperty, Expression.Property(parameterExpression, sourceProperty));
            }

            return null;
        }

        private static bool CheckIfMapPropertyExists(PropertyInfo property)
        {
            if (property.GetCustomAttributes(true).FirstOrDefault(attr => attr.GetType().Name == "MapAttribute") != null)
                return true;

            return false;
        }
    }
}

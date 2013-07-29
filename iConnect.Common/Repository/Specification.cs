using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace iConnect.Common
{
    public sealed class Specification<TEntity> : ISpecification<TEntity>
    {
        
        public Expression<Func<TEntity, bool>> Predicate { get; set; }

        public Func<TEntity, bool> Function { get; set; }

        public bool IsSatisfiedBy(TEntity entity)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// declare a function for OR operation between expressions
        /// </summary>
        /// <param name="firstExpression">First Expression</param>
        /// <param name="secondExpression">Second Expression</param>
        /// <returns></returns>
        public Expression<Func<TEntity, bool>> OrExpression(Expression<Func<TEntity, bool>> firstExpression,
                                                            Expression<Func<TEntity, bool>> secondExpression)
        {
            if (firstExpression == null)
            {
                return secondExpression;
            }
            if (secondExpression == null)
            {
                return firstExpression;
            }
            return firstExpression.Compose(secondExpression, Expression.Or);
        }

        /// <summary>
        /// declare a function for AND operation between expressions
        /// </summary>
        /// <param name="firstExpression">First Expression</param>
        /// <param name="secondExpression">Second Expression</param>
        /// <returns></returns>
        public Expression<Func<TEntity, bool>> AndExpression(Expression<Func<TEntity, bool>> firstExpression,
                                                             Expression<Func<TEntity, bool>> secondExpression)
        {
            if (firstExpression == null)
            {
                return secondExpression;
            }
            if (secondExpression == null)
            {
                return firstExpression;
            }
            return firstExpression.Compose(secondExpression, Expression.And);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public static class PredicateUtility
    {
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second,
                                               Func<Expression, Expression, Expression> merge)
        {
            if(first == null)
            {
                return second;
            }
            if(second == null)
            {
                return first;
            }
            if (merge == null)
            {
                return null;
            }
            // build parameter map (from parameters of second to parameters of first)
            Dictionary<ParameterExpression, ParameterExpression> map =
                first.Parameters.Select((f, i) => new {f, s = second.Parameters[i]}).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            Expression secondBody = ParameterREBinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ParameterREBinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;

        public ParameterREBinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map,
                                                   Expression exp)
        {
            return new ParameterREBinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression expression)
        {
            ParameterExpression replacement;
            if (map.TryGetValue(expression, out replacement))
            {
                expression = replacement;
            }
            return base.VisitParameter(expression);
        }
    }
    public static class PredicateBuilder
    {
        public static Expression<Func<TElement, bool>> BuildOrExpression<TElement, TValue>(
        Expression<Func<TElement, TValue>> valueSelector,
        IEnumerable<TValue> values
        )
        {
            if (null == valueSelector)
            {
                throw new ArgumentNullException("valueSelector");
            }

            if (null == values)
            {
                throw new ArgumentNullException("values");
            }
            ParameterExpression p = valueSelector.Parameters.Single();

            if (!values.Any())
            {
                return e => false;
            }

            var equals = values.Select(value =>
                (Expression)Expression.Equal(
                     valueSelector.Body,
                     Expression.Constant(
                         value,
                         typeof(TValue)
                     )
                )
            );

            var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));

            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }
    }
}
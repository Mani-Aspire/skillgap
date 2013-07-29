using System;
using System.Linq.Expressions;

namespace iConnect.Common
{
    public interface ISpecification<TEntity>
    {
        /// <summary>
        /// Select/Where Expression
        /// </summary>
        Expression<Func<TEntity, bool>> Predicate { get; set; }


        /// <summary>
        /// Declare a function for OR operation between expressions
        /// </summary>
        /// <param name="firstExpression">Expression on the left hand side to perform the OR operation</param>
        /// <param name="secondExpression">Expression on the right hand side to perform the OR operation</param>
        /// <returns>Resulting expression</returns>
        Expression<Func<TEntity, bool>> OrExpression(Expression<Func<TEntity, bool>> firstExpression,
                                                     Expression<Func<TEntity, bool>> secondExpression);

        /// <summary>
        /// Declare a function for AND operation between expressions
        /// </summary>
        /// <param name="firstExpression">Expression on the left hand side to perform the AND operation</param>
        /// <param name="secondExpression">Expression on the right hand side to perform the AND operation</param>
        /// <returns>Resulting expression</returns>
        Expression<Func<TEntity, bool>> AndExpression(Expression<Func<TEntity, bool>> firstExpression,
                                                      Expression<Func<TEntity, bool>> secondExpression);

        /// <summary>
        /// Check whether the entity satisfies certain specification
        /// </summary>
        /// <param name="entity">The entity which needs to be checked</param>
        /// <returns><code>true</code> if successful. <code>false</code> otherwise.</returns>
        bool IsSatisfiedBy(TEntity entity);
    }
}
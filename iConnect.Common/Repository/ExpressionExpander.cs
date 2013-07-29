/*
 * The code contained in this file is taken from the free utility called LINQKit, 
 * which is a free set of extensions for LINQ to SQL and Entity Framework power users.
 * The source, demo and license information can be found at: http://www.albahari.com/nutshell/linqkit.aspx
 * */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace iConnect.Common
{
    /// <summary>
    /// Custom expresssion visitor for ExpandableQuery. This expands calls to Expression.Compile() and
    /// collapses captured lambda references in subqueries which LINQ to SQL can't otherwise handle.
    /// </summary>
    internal class ExpressionExpander : ExpressionVisitor
    {
        // Replacement parameters - for when invoking a lambda expression.
        private readonly Dictionary<ParameterExpression, Expression> _replaceVars;

        internal ExpressionExpander()
        {
        }

        private ExpressionExpander(Dictionary<ParameterExpression, Expression> replaceVars)
        {
            _replaceVars = replaceVars;
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            if ((_replaceVars != null) && (_replaceVars.ContainsKey(p)))
            {
                return _replaceVars[p];
            }
            return base.VisitParameter(p);
        }

        /// <summary>
        /// Flatten calls to Invoke so that Entity Framework can understand it. Calls to Invoke are generated
        /// by PredicateBuilder.
        /// </summary>
        protected override Expression VisitInvocation(InvocationExpression iv)
        {
            if(iv == null)
            {
                return null;
            }
            Expression target = iv.Expression;
            if (target is MemberExpression)
            {
                target = TransformExpr(target);
            }
            if (target is ConstantExpression)
            {
                target = ((ConstantExpression)target).Value as Expression;
            }

            var lambda = (LambdaExpression)target;

            Dictionary<ParameterExpression, Expression> replaceVars = _replaceVars == null ? new Dictionary<ParameterExpression, Expression>() : new Dictionary<ParameterExpression, Expression>(_replaceVars);

            try
            {
                for (int i = 0; i < lambda.Parameters.Count; i++)
                {
                    replaceVars.Add(lambda.Parameters[i], iv.Arguments[i]);
                }
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException(
                    "Invoke cannot be called recursively - try using a temporary variable.", ex);
            }

            return new ExpressionExpander(replaceVars).Visit(lambda.Body);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m == null)
            {
                return null;
            }
            if (m.Method.Name.Equals("Invoke") && m.Method.DeclaringType == typeof(Extensions))
            {
                Dictionary<ParameterExpression, Expression> replaceVars;
                LambdaExpression lambda = GetLambda(m, out replaceVars);

                return new ExpressionExpander(replaceVars).Visit(lambda.Body);
            }

            // Expand calls to an expression's Compile() method:
            if (m.Method.Name.Equals("Compile") && m.Object is MemberExpression)
            {
                var me = (MemberExpression)m.Object;
                Expression newExpr = TransformExpr(me);
                if (newExpr != me)
                {
                    return newExpr;
                }
            }

            // Strip out any nested calls to AsExpandable():
            if (m.Method.Name.Equals("AsExpandable") && m.Method.DeclaringType == typeof(Extensions))
            {
                return m.Arguments[0];
            }

            return base.VisitMethodCall(m);
        }

        private LambdaExpression GetLambda(MethodCallExpression m, out Dictionary<ParameterExpression, Expression> replaceVars)
        {
            Expression target = m.Arguments[0];
            if (target is MemberExpression)
            {
                target = TransformExpr(target);
            }
            if (target is ConstantExpression)
            {
                target = ((ConstantExpression)target).Value as Expression;
            }

            var lambda = (LambdaExpression)target;

            replaceVars = _replaceVars == null ? new Dictionary<ParameterExpression, Expression>() : new Dictionary<ParameterExpression, Expression>(_replaceVars);

            try
            {
                for (int i = 0; i < lambda.Parameters.Count; i++)
                {
                    replaceVars.Add(lambda.Parameters[i], m.Arguments[i + 1]);
                }
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException(
                    "Invoke cannot be called recursively - try using a temporary variable.", ex);
            }
            return lambda;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m == null)
            {
                return null;
            }
            // Strip out any references to expressions captured by outer variables - LINQ to SQL can't handle these:
            if (m.Member.DeclaringType.Name.StartsWith("<>", StringComparison.Ordinal))
            {
                return TransformExpr(m);
            }

            return base.VisitMemberAccess(m);
        }

        private Expression TransformExpr(Expression input)
        {
            var inputExpression = input as MemberExpression;
            if (inputExpression == null)
            {
                return input;
            }
            // Collapse captured outer variables
            if (input == null
                || !(inputExpression.Member is FieldInfo)
                || !inputExpression.Member.ReflectedType.IsNestedPrivate
                || !inputExpression.Member.ReflectedType.Name.StartsWith("<>", StringComparison.Ordinal)) // captured outer variable
            {
                return input;
            }

            if (inputExpression.Expression is ConstantExpression)
            {
                object obj = ((ConstantExpression)inputExpression.Expression).Value;
                if (obj == null)
                {
                    return input;
                }
                Type t = obj.GetType();
                if (!t.IsNestedPrivate || !t.Name.StartsWith("<>",StringComparison.CurrentCulture))
                {
                    return input;
                }
                var fi = (FieldInfo)inputExpression.Member;
                var result = fi.GetValue(obj) as Expression;
                if (result != null)
                {
                    return Visit(result);
                }
            }
            return input;
        }
    }
}
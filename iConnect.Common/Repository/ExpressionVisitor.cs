using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq.Expressions;

namespace iConnect.Common
{

    public abstract class ExpressionVisitor
    {
        public virtual Expression Visit(Expression expression)
        {
            if (expression == null)
            {
                return expression;
            }

            switch (expression.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return VisitUnary((UnaryExpression)expression);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    return VisitBinary((BinaryExpression)expression);
                case ExpressionType.TypeIs:
                    return VisitTypeIs((TypeBinaryExpression)expression);
                case ExpressionType.Conditional:
                    return VisitConditional((ConditionalExpression)expression);
                case ExpressionType.Constant:
                    return VisitConstant((ConstantExpression)expression);
                case ExpressionType.Parameter:
                    return VisitParameter((ParameterExpression)expression);
                case ExpressionType.MemberAccess:
                    return VisitMemberAccess((MemberExpression)expression);
                case ExpressionType.Call:
                    return VisitMethodCall((MethodCallExpression)expression);
                case ExpressionType.Lambda:
                    return VisitLambda((LambdaExpression)expression);
                case ExpressionType.New:
                    return VisitNew((NewExpression)expression);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return VisitNewArray((NewArrayExpression)expression);
                case ExpressionType.Invoke:
                    return VisitInvocation((InvocationExpression)expression);
                case ExpressionType.MemberInit:
                    return VisitMemberInit((MemberInitExpression)expression);
                case ExpressionType.ListInit:
                    return VisitListInit((ListInitExpression)expression);
                default:
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Unhandled expression type: '{0}'", expression.NodeType));
            }
        }

        protected virtual MemberBinding VisitBinding(MemberBinding binding)
        {
            if (binding == null)
            {
                return binding;
            }
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return VisitMemberAssignment((MemberAssignment)binding);
                case MemberBindingType.MemberBinding:
                    return VisitMemberMemberBinding((MemberMemberBinding)binding);
                case MemberBindingType.ListBinding:
                    return VisitMemberListBinding((MemberListBinding)binding);
                default:
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Unhandled binding type '{0}'", binding.BindingType));
            }
        }

        protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            if (initializer == null)
            {
                return initializer;
            }
            ReadOnlyCollection<Expression> arguments = VisitExpressionList(initializer.Arguments);
            if (arguments != initializer.Arguments)
            {
                return Expression.ElementInit(initializer.AddMethod, arguments);
            }
            return initializer;
        }

        protected virtual Expression VisitUnary(UnaryExpression expression)
        {
            if (expression == null)
            {
                return expression;
            }
            Expression operand = Visit(expression.Operand);
            if (operand != expression.Operand)
            {
                return Expression.MakeUnary(expression.NodeType, operand, expression.Type, expression.Method);
            }
            return expression;
        }

        protected virtual Expression VisitBinary(BinaryExpression expression)
        {
            if (expression == null)
            {
                return null;
            }
            Expression left = Visit(expression.Left);
            Expression right = Visit(expression.Right);
            Expression conversion = Visit(expression.Conversion);
            if (left != expression.Left || right != expression.Right || conversion != expression.Conversion)
            {
                if (expression.NodeType == ExpressionType.Coalesce && expression.Conversion != null)
                {
                    return Expression.Coalesce(left, right, conversion as LambdaExpression);
                }
                return Expression.MakeBinary(expression.NodeType, left, right, expression.IsLiftedToNull, expression.Method);
            }
            return expression;
        }

        protected virtual Expression VisitTypeIs(TypeBinaryExpression expression)
        {
            if (expression == null)
            {
                return null;
            }
            Expression expr = Visit(expression.Expression);
            if (expr != expression.Expression)
            {
                return Expression.TypeIs(expr, expression.TypeOperand);
            }
            return expression;
        }

        protected virtual Expression VisitConstant(ConstantExpression expression)
        {
            return expression;
        }

        protected virtual Expression VisitConditional(ConditionalExpression expression)
        {
            if (expression == null)
            {
                return null;
            }
            Expression test = Visit(expression.Test);
            Expression ifTrue = Visit(expression.IfTrue);
            Expression ifFalse = Visit(expression.IfFalse);
            if (test != expression.Test || ifTrue != expression.IfTrue || ifFalse != expression.IfFalse)
            {
                return Expression.Condition(test, ifTrue, ifFalse);
            }
            return expression;
        }

        protected virtual Expression VisitParameter(ParameterExpression expression)
        {
            return expression;
        }

        protected virtual Expression VisitMemberAccess(MemberExpression expression)
        {
            if (expression == null)
            {
                return null;
            }
            Expression exp = Visit(expression.Expression);
            if (exp != expression.Expression)
            {
                return Expression.MakeMemberAccess(exp, expression.Member);
            }
            return expression;
        }

        protected virtual Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (expression == null)
            {
                return null;
            }
            Expression obj = Visit(expression.Object);
            IEnumerable<Expression> args = VisitExpressionList(expression.Arguments);
            if (obj != expression.Object || args != expression.Arguments)
            {
                return Expression.Call(obj, expression.Method, args);
            }
            return expression;
        }

        protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            if (original == null)
            {
                return null;
            }
            List<Expression> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                Expression p = Visit(original[i]);
                if (list != null)
                {
                    list.Add(p);
                }
                else if (p != original[i])
                {
                    list = new List<Expression>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(p);
                }
            }
            if (list != null)
            {
                return list.AsReadOnly();
            }
            return original;
        }

        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            if (assignment == null)
            {
                return null;
            }
            Expression e = Visit(assignment.Expression);
            if (e != assignment.Expression)
            {
                return Expression.Bind(assignment.Member, e);
            }
            return assignment;
        }

        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            if (binding == null)
            {
                return null;
            }
            IEnumerable<MemberBinding> bindings = VisitBindingList(binding.Bindings);
            if (bindings != binding.Bindings)
            {
                return Expression.MemberBind(binding.Member, bindings);
            }
            return binding;
        }

        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            if (binding == null)
            {
                return null;
            }
            IEnumerable<ElementInit> initializers = VisitElementInitializerList(binding.Initializers);
            if (initializers != binding.Initializers)
            {
                return Expression.ListBind(binding.Member, initializers);
            }
            return binding;
        }

        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            if (original == null)
            {
                return null;
            }
            List<MemberBinding> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                MemberBinding b = VisitBinding(original[i]);
                if (list != null)
                {
                    list.Add(b);
                }
                else if (b != original[i])
                {
                    list = new List<MemberBinding>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(b);
                }
            }
            if (list != null)
            {
                return list;
            }
            return original;
        }

        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            if (original == null)
            {
                return null;
            }
            List<ElementInit> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                ElementInit init = VisitElementInitializer(original[i]);
                if (list != null)
                {
                    list.Add(init);
                }
                else if (init != original[i])
                {
                    list = new List<ElementInit>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(init);
                }
            }
            if (list != null)
            {
                return list;
            }
            return original;
        }

        protected virtual Expression VisitLambda(LambdaExpression lambda)
        {
            if (lambda == null)
            {
                return null;
            }
            Expression body = Visit(lambda.Body);
            if (body != lambda.Body)
            {
                return Expression.Lambda(lambda.Type, body, lambda.Parameters);
            }
            return lambda;
        }

        protected virtual NewExpression VisitNew(NewExpression newExpression)
        {
            if (newExpression == null)
            {
                return null;
            }
            IEnumerable<Expression> args = VisitExpressionList(newExpression.Arguments);
            if (args != newExpression.Arguments)
            {
                if (newExpression.Members != null)
                {
                    return Expression.New(newExpression.Constructor, args, newExpression.Members);
                }
                return Expression.New(newExpression.Constructor, args);
            }
            return newExpression;
        }

        protected virtual Expression VisitMemberInit(MemberInitExpression expression)
        {
            if (expression == null)
            {
                return null;
            }
            NewExpression n = VisitNew(expression.NewExpression);
            IEnumerable<MemberBinding> bindings = VisitBindingList(expression.Bindings);
            if (n != expression.NewExpression || bindings != expression.Bindings)
            {
                return Expression.MemberInit(n, bindings);
            }
            return expression;
        }

        protected virtual Expression VisitListInit(ListInitExpression expression)
        {
            if (expression == null)
            {
                return null;
            }
            NewExpression n = VisitNew(expression.NewExpression);
            IEnumerable<ElementInit> initializers = VisitElementInitializerList(expression.Initializers);
            if (n != expression.NewExpression || initializers != expression.Initializers)
            {
                return Expression.ListInit(n, initializers);
            }
            return expression;
        }

        protected virtual Expression VisitNewArray(NewArrayExpression expression)
        {
            if (expression == null)
            {
                return null;
            }
            IEnumerable<Expression> exprs = VisitExpressionList(expression.Expressions);
            if (exprs != expression.Expressions)
            {
                if (expression.NodeType == ExpressionType.NewArrayInit)
                {
                    return Expression.NewArrayInit(expression.Type.GetElementType(), exprs);
                }
                return Expression.NewArrayBounds(expression.Type.GetElementType(), exprs);
            }
            return expression;
        }

        protected virtual Expression VisitInvocation(InvocationExpression invocation)
        {
            if (invocation == null)
            {
                return null;
            }
            IEnumerable<Expression> args = VisitExpressionList(invocation.Arguments);
            Expression expr = Visit(invocation.Expression);
            if (args != invocation.Arguments || expr != invocation.Expression)
            {
                return Expression.Invoke(expr, args);
            }
            return invocation;
        }
    }
}
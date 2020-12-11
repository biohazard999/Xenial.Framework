using System;
using System.Linq.Expressions;
using System.Text;

using Xenial.Data;

namespace Xenial.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExpressionHelper
    {
        /// <summary>
        /// Static Factory method to create
        /// </summary>
        public static ExpressionHelper<TObj> Create<TObj>() => new();

        /// <summary>
        /// Gets the member expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static MemberExpression? GetMemberExpression(Expression? expression)
        {
            if (expression is MemberExpression memberExpression)
            {
                return memberExpression;
            }

            if (expression is LambdaExpression lambdaExpression)
            {
                if (lambdaExpression.Body is MemberExpression memberExpression2)
                {
                    return memberExpression2;
                }

                if (lambdaExpression.Body is UnaryExpression unaryExpression
                    && unaryExpression.Operand is MemberExpression memberExpression3)
                {
                    return memberExpression3;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the property path.
        /// </summary>
        /// <param name="expr">The expr.</param>
        /// <returns></returns>
        public static string GetPropertyPath(Expression? expr)
        {
            var path = new StringBuilder();
            var memberExpression = GetMemberExpression(expr);
            do
            {
                if (memberExpression is null)
                {
                    return string.Empty;
                }

                path.Insert(0, $".{memberExpression.Member.Name}");

                if (memberExpression.Expression is UnaryExpression ue)
                {
                    memberExpression = GetMemberExpression(ue.Operand);
                }
                else
                {
                    memberExpression = GetMemberExpression(memberExpression.Expression);
                }
            }
            while (memberExpression != null);

            path.Remove(0, 1);
            return path.ToString();
        }
    }
}

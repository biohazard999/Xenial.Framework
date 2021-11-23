using System;
using System.Linq;
using System.Linq.Expressions;

using DevExpress.Data.Filtering;

using Xenial.Utils;

namespace Xenial.Data;

/// <summary>
/// Helper-Class to generate strongly typed
/// [Operands](https://documentation.devexpress.com/CoreLibraries/DevExpress.Data.Filtering.OperandProperty.members)
/// and PropertyPaths.
/// </summary>
///
/// <typeparam name="TObj"> . </typeparam>

public class ExpressionHelper<TObj>
{
    internal ExpressionHelper() { }

    /// <summary>
    /// Returns a PropertyPath. Multiple nestings are allowed. `Exp.Property(p => p.A.B.C)` returns
    /// `A.B.C`.
    /// </summary>
    ///
    /// <typeparam name="TRet"> Type of the ret. </typeparam>
    /// <param name="expr"> The expression. </param>
    ///
    /// <returns>   A string. </returns>

    public string Property<TRet>(Expression<Func<TObj, TRet>> expr)
        => GetPropertyPath(expr);

    /// <summary>
    /// Returns an
    /// [OperandProperty](https://documentation.devexpress.com/CoreLibraries/DevExpress.Data.Filtering.OperandProperty.members).
    /// Multiple nestings are allowed. `Exp.Property(p => p.A.B.C)` returns `new
    /// OperandProperty(A.B.C)`.
    /// </summary>
    ///
    /// <typeparam name="TRet"> Type of the ret. </typeparam>
    /// <param name="expr"> The expression. </param>
    ///
    /// <returns>   An OperandProperty. </returns>

    public OperandProperty Operand<TRet>(Expression<Func<TObj, TRet>> expr)
        => GetOperand(expr);

    private static string GetPropertyPath<TRet>(Expression<Func<TObj, TRet>> expr)
        => ExpressionHelper.GetPropertyPath(expr);

    private static OperandProperty GetOperand<TRet>(Expression<Func<TObj, TRet>> expr)
        => new(ExpressionHelper.GetPropertyPath(expr));
}

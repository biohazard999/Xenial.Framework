﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

using Xenial.Data;
using Xenial.Framework.Layouts.ColumnItems;

#pragma warning disable CA1033 //Seal Type -> By Design
#pragma warning disable CA1710 //Rename Type to end in Collection -> By Design
#pragma warning disable CA2227 //Collection fields should not have a setter -> By Design
#pragma warning disable CA1822 //By design for fluent interface

namespace Xenial.Framework.Layouts;

/// <summary>   Class Columns. </summary>
///
/// <seealso cref="IEnumerable{Column}"/>

public record Columns : IEnumerable<Column>
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly Columns Default = new();

    /// <summary>
    /// 
    /// </summary>
    public static Columns Automatic(ListViewOptions options)
    {
        _ = options ?? throw new ArgumentNullException(nameof(options));
        options = options with
        {
            AutomaticColumns = true
        };
        return new(options);
    }

    /// <summary>
    /// 
    /// </summary>
    public Columns() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public Columns(ListViewOptions options)
        => Options = options ?? new();

    /// <summary>
    /// Specifies the <see cref="Xenial.Framework.Layouts.ListViewOptions"></see> to set the options for the ListView
    /// This only will work on top level View nodes, not included ones.
    /// </summary>
    public ListViewOptions Options { get; set; } = new();

    IEnumerator<Column> IEnumerable<Column>.GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();

    /// <summary>   Gets or sets the columns. </summary>
    ///
    /// <value> The columns. </value>

    public ColumnCollection<Column> Items { get; set; } = new();

    /// <summary>   Adds the specified column. </summary>
    ///
    /// <param name="column">   The column. </param>

    public void Add(Column column)
        => Items.Add(column);

}

/// <summary>
/// Class ColumnsBuilder.
/// Implements the <see cref="Xenial.Framework.Layouts.ColumnsBuilder" />
/// </summary>
/// <typeparam name="TModelClass">The type of the t model class.</typeparam>
/// <seealso cref="Xenial.Framework.Layouts.ColumnsBuilder" />
/// <autogeneratedoc />
[XenialCheckLicense]
public partial class ColumnsBuilder<TModelClass> : ColumnsBuilder
    where TModelClass : class
{
    /// <summary>   Gets the expression helper. </summary>
    ///
    /// <value> The expression helper. </value>

    protected static ExpressionHelper<TModelClass> ExpressionHelper { get; } = Xenial.Utils.ExpressionHelper.Create<TModelClass>();

    /// <summary>   Columns the specified expression. </summary>
    ///
    /// <typeparam name="TProperty">    The type of the t property. </typeparam>
    /// <param name="expression">   The expression. </param>
    ///
    /// <returns>   Column&lt;TModelClass&gt;. </returns>

    public Column Column<TProperty>(Expression<Func<TModelClass, TProperty>> expression)
        => ColumnItems.Column.Create(ExpressionHelper.Property(expression));

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="expression"></param>
    /// <param name="caption"></param>
    /// <returns></returns>
    public Column Column<TProperty>(Expression<Func<TModelClass, TProperty>> expression, string caption)
        => ColumnItems.Column.Create(ExpressionHelper.Property(expression)) with
        {
            Caption = caption
        };

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="expression"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    public Column Column<TProperty>(Expression<Func<TModelClass, TProperty>> expression, int width)
        => ColumnItems.Column.Create(ExpressionHelper.Property(expression)) with
        {
            Width = width
        };

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="expression"></param>
    /// <param name="caption"></param>
    /// <returns></returns>
    public Column Column<TProperty>(Expression<Func<TModelClass, TProperty>> expression, string caption, int width)
        => ColumnItems.Column.Create(ExpressionHelper.Property(expression)) with
        {
            Caption = caption,
            Width = width
        };

    /// <summary>   Columns the specified expression. </summary>
    ///
    /// <typeparam name="TProperty">    The type of the t property. </typeparam>
    /// <param name="expression">   The expression. </param>
    /// <param name="configureAction"> An action to configure the column </param>
    ///
    /// <returns>   Column&lt;TModelClass&gt;. </returns>
    public Column Column<TProperty>(Expression<Func<TModelClass, TProperty>> expression, string caption, Action<Column> configureAction)
    {
        _ = configureAction ?? throw new ArgumentNullException(nameof(configureAction));
        var column = Column(expression, caption);
        configureAction.Invoke(column);
        return column;
    }

    /// <summary>   Columns the specified expression. </summary>
    ///
    /// <typeparam name="TProperty">    The type of the t property. </typeparam>
    /// <param name="expression">   The expression. </param>
    /// <param name="configureAction"> An action to configure the column </param>
    ///
    /// <returns>   Column&lt;TModelClass&gt;. </returns>
    public Column Column<TProperty>(Expression<Func<TModelClass, TProperty>> expression, int width, Action<Column> configureAction)
    {
        _ = configureAction ?? throw new ArgumentNullException(nameof(configureAction));
        var column = Column(expression, width);
        configureAction.Invoke(column);
        return column;
    }

    /// <summary>   Columns the specified expression. </summary>
    ///
    /// <typeparam name="TProperty">    The type of the t property. </typeparam>
    /// <param name="expression">   The expression. </param>
    /// <param name="configureAction"> An action to configure the column </param>
    ///
    /// <returns>   Column&lt;TModelClass&gt;. </returns>
    public Column Column<TProperty>(Expression<Func<TModelClass, TProperty>> expression, string caption, int width, Action<Column> configureAction)
    {
        _ = configureAction ?? throw new ArgumentNullException(nameof(configureAction));
        var column = Column(expression, caption, width);
        configureAction.Invoke(column);
        return column;
    }

    /// <summary>   Columns the specified expression. </summary>
    ///
    /// <typeparam name="TProperty">    The type of the t property. </typeparam>
    /// <param name="expression">   The expression. </param>
    /// <param name="configureAction"> An action to configure the column </param>
    ///
    /// <returns>   Column&lt;TModelClass&gt;. </returns>
    public Column Column<TProperty>(Expression<Func<TModelClass, TProperty>> expression, Action<Column> configureAction)
    {
        _ = configureAction ?? throw new ArgumentNullException(nameof(configureAction));
        var column = Column(expression);
        configureAction.Invoke(column);
        return column;
    }
}

/// <summary>
/// Class ColumnsBuilder.
/// Implements the <see cref="Xenial.Framework.Layouts.ColumnsBuilder" />
/// </summary>
/// <seealso cref="Xenial.Framework.Layouts.ColumnsBuilder" />
/// <autogeneratedoc />
[XenialCheckLicense]
public partial class ColumnsBuilder
{
    /// <summary>   Columns the specified property name. </summary>
    ///
    /// <param name="propertyName"> Name of the property. </param>
    ///
    /// <returns>   Column. </returns>

    public Column Column(string propertyName)
       => ColumnItems.Column.Create(propertyName);

    /// <summary>   Columns the specified property name. </summary>
    ///
    /// <param name="propertyName"> Name of the property. </param>
    ///
    /// <returns>   Column. </returns>

    public Column Column(string propertyName, string caption)
       => ColumnItems.Column.Create(propertyName) with
       {
           Caption = caption
       };

    /// <summary>   Columns the specified property name. </summary>
    ///
    /// <param name="propertyName"> Name of the property. </param>
    ///
    /// <returns>   Column. </returns>

    public Column Column(string propertyName, int width)
       => ColumnItems.Column.Create(propertyName) with
       {
           Width = width
       };

    /// <summary>   Columns the specified property name. </summary>
    ///
    /// <param name="propertyName"> Name of the property. </param>
    ///
    /// <returns>   Column. </returns>

    public Column Column(string propertyName, string caption, int width)
       => ColumnItems.Column.Create(propertyName) with
       {
           Caption = caption,
           Width = width
       };

    /// <summary>   Columns the specified expression. </summary>
    ///
    /// <param name="propertyName">   The roperty name. </param>
    /// <param name="configureAction"> An action to configure the column </param>
    ///
    /// <returns>   Column&lt;TModelClass&gt;. </returns>
    public Column Column(string propertyName, string caption, Action<Column> configureAction)
    {
        _ = configureAction ?? throw new ArgumentNullException(nameof(configureAction));
        var column = Column(propertyName, caption);
        configureAction.Invoke(column);
        return column;
    }

    /// <summary>   Columns the specified expression. </summary>
    ///
    /// <param name="propertyName">   The roperty name. </param>
    /// <param name="configureAction"> An action to configure the column </param>
    ///
    /// <returns>   Column&lt;TModelClass&gt;. </returns>
    public Column Column(string propertyName, int width, Action<Column> configureAction)
    {
        _ = configureAction ?? throw new ArgumentNullException(nameof(configureAction));
        var column = Column(propertyName, width);
        configureAction.Invoke(column);
        return column;
    }

    /// <summary>   Columns the specified expression. </summary>
    ///
    /// <param name="propertyName">   The roperty name. </param>
    /// <param name="configureAction"> An action to configure the column </param>
    ///
    /// <returns>   Column&lt;TModelClass&gt;. </returns>
    public Column Column(string propertyName, string caption, int width, Action<Column> configureAction)
    {
        _ = configureAction ?? throw new ArgumentNullException(nameof(configureAction));
        var column = Column(propertyName, caption, width);
        configureAction.Invoke(column);
        return column;
    }


    /// <summary>   Columns the specified expression. </summary>
    ///
    /// <param name="propertyName">   The roperty name. </param>
    /// <param name="configureAction"> An action to configure the column </param>
    ///
    /// <returns>   Column&lt;TModelClass&gt;. </returns>
    public Column Column(string propertyName, Action<Column> configureAction)
    {
        _ = configureAction ?? throw new ArgumentNullException(nameof(configureAction));
        var column = Column(propertyName);
        configureAction.Invoke(column);
        return column;
    }
}

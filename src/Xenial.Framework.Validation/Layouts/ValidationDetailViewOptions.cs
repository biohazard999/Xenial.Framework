using System;

using DevExpress.ExpressApp.Validation;

namespace Xenial.Framework.Layouts;

/// <summary>
/// 
/// </summary>
[XenialModelOptions(
    typeof(IModelLayoutManagerOptionsValidation)
)]
public partial record ValidationDetailViewOptions : IDetailViewOptionsExtension
{
}

[XenialModelOptionsMapper(typeof(ValidationDetailViewOptions))]
internal partial class ViewOptionsMapper
{
}

/// <summary>
/// 
/// </summary>
public static class ValidationDetailViewOptionsExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="list"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static DetailViewOptionsExtensions Validation(this DetailViewOptionsExtensions list, ValidationDetailViewOptions options)
    {
        _ = list ?? throw new ArgumentNullException(nameof(list));
        _ = options ?? throw new ArgumentNullException(nameof(options));
        list.Add(options);

        return list;
    }

    public static void Foo()
    {

        DetailViewOptions x = new()
        {
            Caption = "Foo",
            Extensions = x => x
                .Validation(new()
                {

                })
                .Validation(new()
                {

                }),
            AllowDelete = true
        };

    }
}

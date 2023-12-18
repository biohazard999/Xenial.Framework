using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;

using Xenial;

namespace Xenial.Framework.Model.GeneratorUpdaters;

/// <summary>
/// Class ApplicationOptions.
/// </summary>
[XenialCheckLicense]
[XenialModelOptions(typeof(IModelApplication))]
public partial record AppOptions
{
}

[XenialModelOptionsMapper(typeof(AppOptions))]
internal partial class AppOptionsMapper
{
}

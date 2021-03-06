﻿using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Win.SystemModule;

using Xenial;
using Xenial.Framework.Win.Model;

#pragma warning disable CA1707 //By Design

namespace Xenial.Framework.Win.Model
{
    /// <summary>   Class SetUiTypeLogic. </summary>
    [DomainLogic(typeof(IModelOptionsWin))]
    public static class SetUiTypeLogic
    {
        /// <summary>   Gets the type of the UI. </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <param name="instance"> The instance. </param>
        ///
        /// <returns>   UIType. </returns>

        public static UIType Get_UIType(IModelOptionsWin instance)
        {
            _ = instance ?? throw new ArgumentNullException(nameof(instance));
            return SetUiTypeLogicExtensions.UIType ?? instance.UIType;
        }
    }
}

#pragma warning restore CA1707

namespace DevExpress.ExpressApp.DC
{
    /// <summary>
    /// Class SetUiTypeLogicExtensions.
    /// </summary>
    /// <autogeneratedoc />
    [XenialCheckLicence]
    public static partial class SetUiTypeLogicExtensions
    {
        internal static UIType? UIType { get; set; }

        /// <summary>
        /// Specifies the Show View Strategy (see DevExpress.ExpressApp.ShowViewStrategyBase) used in the
        /// WinForms application. Uses the type of the UI.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <param name="customLogics"> The custom logics. </param>
        /// <param name="uiType">       Type of the UI. </param>
        ///
        /// <returns>   CustomLogics. </returns>

        public static CustomLogics UseUiType(this CustomLogics customLogics, UIType uiType)
        {
            _ = customLogics ?? throw new ArgumentNullException(nameof(customLogics));
            UIType = uiType;
            customLogics.UnregisterLogic(typeof(IModelOptionsWin), typeof(SetUiTypeLogic));
            customLogics.RegisterLogic(typeof(IModelOptionsWin), typeof(SetUiTypeLogic));
            return customLogics;
        }
    }
}

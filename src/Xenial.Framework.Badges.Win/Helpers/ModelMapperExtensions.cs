using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using DevExpress.Utils.VisualEffects;
using DevExpress.XtraBars.Navigation;

using Xenial.Framework.Badges.Model;

namespace Xenial.Framework.Badges.Win.Helpers
{
    internal static class ModelMapperExtensions
    {
        internal static BadgePaintStyle ConvertPaintStyle(this XenialStaticBadgePaintStyle? paintStyle)
            => paintStyle switch
            {
                XenialStaticBadgePaintStyle.Critical => BadgePaintStyle.Critical,
                XenialStaticBadgePaintStyle.Information => BadgePaintStyle.Information,
                XenialStaticBadgePaintStyle.Question => BadgePaintStyle.Question,
                XenialStaticBadgePaintStyle.System => BadgePaintStyle.System,
                XenialStaticBadgePaintStyle.Warning => BadgePaintStyle.Warning,
                _ => BadgePaintStyle.Default
            };

#if !NET5_0
        internal static void Deconstruct<T1, T2>(this KeyValuePair<T1, T2> tuple, out T1 key, out T2 value)
            => (key, value) = (tuple.Key, tuple.Value);
#endif

        private static readonly object locker = new();

        private delegate AdornerElementViewInfo? GetBadgeViewInfoDelegate(Badge badge);

        private static GetBadgeViewInfoDelegate? getBadgeViewInfo;


        internal static BadgeViewInfo? GetViewInfo(this Badge badge)
        {
            if (getBadgeViewInfo is null)
            {
                var propertyInfo = typeof(Badge).GetProperty("ViewInfo", BindingFlags.Instance | BindingFlags.NonPublic);

                if (
                    propertyInfo is not null
                    && propertyInfo.GetMethod is MethodInfo getMethod
                )
                {
                    lock (locker)
                    {
                        getBadgeViewInfo = (GetBadgeViewInfoDelegate)Delegate.CreateDelegate(typeof(GetBadgeViewInfoDelegate), getMethod);
                    }
                }
            }

            if (getBadgeViewInfo is not null)
            {
                if (getBadgeViewInfo(badge) is BadgeViewInfo badgeViewInfo)
                {
                    return badgeViewInfo;
                }
            }

            return null;
        }

        private delegate AccordionControlForm? GetAccordionControlForm(AccordionControl accordionControl);

        private static GetAccordionControlForm? getAccordionControlForm;

        internal static AccordionControlForm? GetPopupForm(this AccordionControl accordionControl)
        {
            if (getAccordionControlForm is null)
            {
                var fieldInfo = typeof(AccordionControl).GetField("popupForm", BindingFlags.Instance | BindingFlags.NonPublic);

                if (
                    fieldInfo is not null
                )
                {
                    static GetAccordionControlForm CreateGetter(FieldInfo field)
                    {
                        var methodName = $"{field?.ReflectedType?.FullName}.get_{field?.Name}";
                        var setterMethod = new DynamicMethod(methodName, typeof(AccordionControlForm), new Type[1] { typeof(AccordionControl) }, true);
                        var gen = setterMethod.GetILGenerator();
                        if (field?.IsStatic == true)
                        {
                            gen.Emit(OpCodes.Ldsfld, field);
                        }
                        else
                        {
                            if (field is not null)
                            {
                                gen.Emit(OpCodes.Ldarg_0);
                                gen.Emit(OpCodes.Ldfld, field);
                            }
                        }
                        gen.Emit(OpCodes.Ret);
                        return (GetAccordionControlForm)setterMethod.CreateDelegate(typeof(GetAccordionControlForm));
                    }

                    lock (locker)
                    {
                        getAccordionControlForm = CreateGetter(fieldInfo);
                    }
                }
            }
            if (getAccordionControlForm is not null)
            {
                if (getAccordionControlForm(accordionControl) is AccordionControlForm accordionControlForm)
                {
                    return accordionControlForm;
                }
            }

            return null;
        }
    }
}

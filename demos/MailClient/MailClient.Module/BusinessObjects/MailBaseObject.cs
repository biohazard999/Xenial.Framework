using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using DevExpress.Xpo;

namespace MailClient.Module.BusinessObjects
{
    [NonPersistent]
    public abstract class MailBaseObject : XPBaseObject
    {
        protected MailBaseObject(Session session) : base(session) { }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected new object GetPropertyValue([CallerMemberName] string? propertyName = null)
            => base.GetPropertyValue(propertyName);

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected new T GetPropertyValue<T>([CallerMemberName] string? propertyName = null)
            => base.GetPropertyValue<T>(propertyName);

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyValueHolder">The property value holder.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected bool SetPropertyValue<T>(ref T propertyValueHolder, T newValue, [CallerMemberName] string? propertyName = null)
            => SetPropertyValue(propertyName, ref propertyValueHolder, newValue);

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyValueHolder">The property value holder.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="onChanged">A callback that is called if the user changes an value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected bool SetPropertyValue<T>(ref T propertyValueHolder, T newValue, Action<T> onChanged, [CallerMemberName] string? propertyName = null)
        {
            var changed = SetPropertyValue(propertyName, ref propertyValueHolder, newValue);

            if (changed && IsSaveForBusinessLogic)
            {
                onChanged?.Invoke(newValue);
            }

            return changed;
        }


        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected new XPCollection GetCollection([CallerMemberName] string? propertyName = null)
            => base.GetCollection(propertyName);

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected new XPCollection<T> GetCollection<T>([CallerMemberName] string? propertyName = null)
            where T : class => base.GetCollection<T>(propertyName);

        /// <summary>
        /// Gets the delayed property value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected new T GetDelayedPropertyValue<T>([CallerMemberName] string? propertyName = null)
            => base.GetDelayedPropertyValue<T>(propertyName);

        /// <summary>
        /// Sets the delayed property value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected bool SetDelayedPropertyValue<T>(T value, [CallerMemberName] string? propertyName = null)
            => SetDelayedPropertyValue(propertyName, value);

        /// <summary>
        /// Evaluates the alias.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected new object EvaluateAlias([CallerMemberName] string? propertyName = null)
            => base.EvaluateAlias(propertyName);

        /// <summary>
        /// Indicates if it's save to do a modification e.g it's not loading, saving or invalidated
        /// </summary>
        protected virtual bool IsSaveForBusinessLogic
            => !IsLoading && !IsSaving && !IsInvalidated;
    }
}

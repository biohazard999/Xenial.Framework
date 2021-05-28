using System;
using System.Collections.Generic;
using System.ComponentModel;

using DevExpress.ExpressApp.DC;

namespace Xenial.Framework.ModelBuilders
{
    /// <summary>   Manager for xaf builders. </summary>
    ///
    /// <seealso cref="BuilderManager"/>
    /// <seealso cref="ITypesInfoProvider"/>

    public class XafBuilderManager : BuilderManager, ITypesInfoProvider
    {
        /// <summary>   Gets the empty builders. </summary>
        ///
        /// <value> The empty builders. </value>

        public static IEnumerable<IBuilder> EmptyBuilders { get; } = Array.Empty<IBuilder>();

        /// <summary>   Gets the types information. </summary>
        ///
        /// <value> The types information. </value>

        public ITypesInfo TypesInfo { get; }

        /// <summary>   Initializes a new instance of the <see cref="XafBuilderManager"/> class. </summary>
        ///
        /// <param name="typesInfo">    The types information. </param>

        public XafBuilderManager(ITypesInfo typesInfo) : this(typesInfo, EmptyBuilders) { }

        /// <summary>   Initializes a new instance of the <see cref="XafBuilderManager"/> class. </summary>
        ///
        /// <param name="typesInfo">    The types information. </param>
        /// <param name="builders">     The builders. </param>

        public XafBuilderManager(ITypesInfo typesInfo, IEnumerable<IBuilder> builders) : base()
        {
            TypesInfo = typesInfo;
            Add(builders);
        }

        /// <summary>   Creates the specified types information. </summary>
        ///
        /// <param name="typesInfo">    The types information. </param>
        /// <param name="builders">     The builders. </param>
        ///
        /// <returns>   An XafBuilderManager. </returns>

        public static XafBuilderManager Create(ITypesInfo typesInfo, IEnumerable<IBuilder> builders)
            => new(typesInfo, builders);

        /// <summary>
        /// Creates the specified xaf builder manager with the the given TypesInfo.<br/>
        /// Make sure you have an public ctor that accepts a typesInfo.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">        Thrown when one or more required arguments
        ///                                                 are null. </exception>
        /// <exception cref="InvalidOperationException">    Thrown when the requested operation is
        ///                                                 invalid. </exception>
        ///
        /// <typeparam name="TBuilderManager">  The type of the t builder manager. </typeparam>
        /// <param name="typesInfo">    The types information. </param>
        ///
        /// <returns>   TBuilderManager. </returns>

        public static TBuilderManager Create<TBuilderManager>(ITypesInfo typesInfo)
            where TBuilderManager : XafBuilderManager

        {
            _ = typesInfo ?? throw new ArgumentNullException(nameof(typesInfo));
            var instance = Activator.CreateInstance(typeof(TBuilderManager), typesInfo);
            if (instance is TBuilderManager tInstance)
            {
                return tInstance;
            }
            throw new InvalidOperationException(nameof(instance));
        }

        /// <summary>   Gets a value indicating whether [force refresh members]. </summary>
        ///
        /// <value> <c>true</c> if [force refresh members]; otherwise, <c>false</c>. </value>

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual bool ForceRefreshMembers => false;

        /// <summary>   Builds the builder. </summary>
        ///
        /// <param name="builder">  The builder. </param>

        protected override void BuildBuilder(IBuilder builder)
        {
            base.BuildBuilder(builder);
            if (builder is ITypeInfoProvider typeInfoProvider)
            {
                if (ForceRefreshMembers && typeInfoProvider.TypeInfo is TypeInfo typeInfo)
                {
                    typeInfo.Refresh(true);
                }
                TypesInfo.RefreshInfo(typeInfoProvider.TypeInfo);
            }
        }
    }
}

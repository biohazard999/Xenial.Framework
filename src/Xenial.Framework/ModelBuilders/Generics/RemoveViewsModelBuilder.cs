﻿using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.DC;

namespace Xenial.Framework.ModelBuilders
{
    /// <summary>
    /// Class RemoveViewsModelBuilder. Implements the
    /// <see cref="Xenial.Framework.ModelBuilders.ModelBuilder{TClassType}" />
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    ///
    /// <seealso cref="ModelBuilder{TClassType}"/>
    /// <seealso cref="Xenial.Framework.ModelBuilders.ModelBuilder{TClassType}">    <autogeneratedoc /></seealso>

    public class RemoveViewsModelBuilder<TClassType> : ModelBuilder<TClassType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveViewsModelBuilder`1"/> class.
        /// </summary>
        ///
        /// <param name="typeInfo"> The type information. </param>

        public RemoveViewsModelBuilder(ITypeInfo typeInfo) : base(typeInfo) { }

        /// <summary>   Builds this instance. </summary>
        public override void Build()
        {
            base.Build();

            this.GenerateNoViews();
        }
    }
}

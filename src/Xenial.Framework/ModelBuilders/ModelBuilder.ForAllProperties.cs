﻿using System;
using System.Linq;

namespace Xenial.Framework.ModelBuilders;

public partial class ModelBuilder<TClassType>
{
    /// <summary>   Fors all properties. </summary>
    ///
    /// <returns>
    /// IAggregatedPropertyBuilder&lt;System.Nullable&lt;System.Object&gt;, TClassType&gt;.
    /// </returns>

    public IAggregatedPropertyBuilder<object?, TClassType> ForAllProperties()
    {
        var propertyBuilders = TypeInfo.Members.Select(m => PropertyBuilder.PropertyBuilderFor<object?, TClassType>(m));

        foreach (var propertyBuilder in propertyBuilders)
        {
            Add(propertyBuilder);
        }

        return new AggregatedPropertyBuilder<object?, TClassType>(this, propertyBuilders);
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.Xpo;

#nullable disable

namespace Xenial.Framework.Generators.Tests
{
    [XenialXpoBuilder]
    public class ValueTypesXPObject : XPBaseObject
    {
        public ValueTypesXPObject(Session session) : base(session) { }

        string stringProperty;
        int intProperty;
        bool boolProperty;
        DateTime dateTimeProperty;
        TimeSpan timeSpanProperty;
#if NET6_0_OR_GREATER
        TimeOnly timeOnlyProperty;
        DateOnly dateOnlyProperty;
#endif

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string StringProperty
        {
            get => stringProperty;
            set => SetPropertyValue(nameof(StringProperty), ref stringProperty, value);
        }

        public int IntProperty
        {
            get => intProperty;
            set => SetPropertyValue(nameof(IntProperty), ref intProperty, value);
        }

        public bool BoolProperty
        {
            get => boolProperty;
            set => SetPropertyValue(nameof(BoolProperty), ref boolProperty, value);
        }

        public DateTime DateTimeProperty
        {
            get => dateTimeProperty;
            set => SetPropertyValue(nameof(DateTimeProperty), ref dateTimeProperty, value);
        }

        public TimeSpan TimeSpanProperty
        {
            get => timeSpanProperty;
            set => SetPropertyValue(nameof(TimeSpanProperty), ref timeSpanProperty, value);
        }
#if NET6_0_OR_GREATER
        public DateOnly DateOnlyProperty
        {
            get => dateOnlyProperty;
            set => SetPropertyValue(nameof(DateOnlyProperty), ref dateOnlyProperty, value);
        }

        public TimeOnly TimeOnlyProperty
        {
            get => timeOnlyProperty;
            set => SetPropertyValue(nameof(TimeOnlyProperty), ref timeOnlyProperty, value);
        }
#endif
    }
}

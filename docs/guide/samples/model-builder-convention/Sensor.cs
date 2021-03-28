using System;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace MainDemo.Module.BusinessObjects
{
    public class Sensor : BaseObject
    {
        public Sensor(Session session) : base(session) { }

        private int value1;
        public int Value1 { get => value1; set => SetPropertyValue(nameof(Value1), ref value1, value); }

        private int value2;
        public int Value2 { get => value2; set => SetPropertyValue(nameof(Value2), ref value2, value); }

        private int value3;
        public int Value3 { get => value3; set => SetPropertyValue(nameof(Value3), ref value3, value); }

        private int value4;
        public int Value4 { get => value4; set => SetPropertyValue(nameof(Value4), ref value4, value); }

        private int value5;
        public int Value5 { get => value5; set => SetPropertyValue(nameof(Value5), ref value5, value); }
    }
}
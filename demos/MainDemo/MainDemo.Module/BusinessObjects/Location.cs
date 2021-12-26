using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace MainDemo.Module.BusinessObjects
{
    [System.ComponentModel.DefaultProperty(nameof(Latitude))]
    public class Location : BaseObject, IMapsMarker
    {
        private Employee employee;
        private double latitude;
        private double longitude;

        public Location(Session session) :
            base(session)
        {
        }

        public override string ToString()
        {
            var latitudePrefix = Latitude > 0 ? "N" : "S";
            var longitudePrefix = Longitude > 0 ? "E" : "W";
            return string.Format("{0}{1:0.###}, {2}{3:0.###}", latitudePrefix, Math.Abs(Latitude), longitudePrefix, Math.Abs(Longitude));
        }

        [Browsable(false)]
        public Employee Employee
        {
            get => employee;
            set => SetPropertyValue(nameof(Employee), ref employee, value);
        }

        [PersistentAlias("Employee.FullName")]
        public string Title => Convert.ToString(EvaluateAlias(nameof(Title)));

        public double Latitude
        {
            get => latitude;
            set => SetPropertyValue(nameof(Latitude), ref latitude, value);
        }

        public double Longitude
        {
            get => longitude;
            set => SetPropertyValue(nameof(Longitude), ref longitude, value);
        }
    }
}

using System;

using DevExpress.ExpressApp;

using Xenial.Framework.Layouts;

namespace Acme.Module.BusinessObjects
{
    [DetailViewLayoutBuilder(typeof(PersonLayout))]
    public partial class Person : NonPersistentBaseObject
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public DateTime? DateOfBirth { get; set; }
        public Address Address1 { get; set; }
        public Address Address2 { get; set; }
    }

    public class Address : NonPersistentBaseObject
    {
        public string Street { get; set; }
        public string City { get; set; }
        public Country Country { get; set; }
    }

    public class Country : NonPersistentBaseObject
    {
        public string CountryName { get; set; }
    }
}

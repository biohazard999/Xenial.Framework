using DevExpress.Xpo;

namespace MainDemo.Module.BusinessObjects
{
    [Persistent]
    public class Person : XPObject
    {
        private string firstName;
        private string lastName;
        private string fullName;
        private string phone;
        private byte[] image;
        private string email;
        private Address address1;
        private Address address2;

        public Person(Session session) : base(session) { }

        [Persistent]
        public string FirstName
        {
            get => firstName;
            set => SetPropertyValue(nameof(FirstName), ref firstName, value);
        }

        [Persistent]
        public string LastName
        {
            get => lastName;
            set => SetPropertyValue(nameof(LastName), ref lastName, value);
        }

        [Persistent]
        public string FullName
        {
            get => fullName;
            set => SetPropertyValue(nameof(FullName), ref fullName, value);
        }

        [Persistent]
        public string Phone
        {
            get => phone;
            set => SetPropertyValue(nameof(Phone), ref phone, value);
        }

        [Persistent]
        public string Email 
        {
            get => email;
            set => SetPropertyValue(nameof(Email), ref email, value);
        }

        [Persistent]
        public byte[] Image 
        {
            get => image;
            set => SetPropertyValue(nameof(Image), ref image, value);
        }

        [Persistent]
        public Address Address1
        {
            get => address1;
            set => SetPropertyValue(nameof(Address1), ref address1, value);
        }

        [Persistent]
        public Address Address2
        {
            get => address2;
            set => SetPropertyValue(nameof(Address2), ref address2, value);
        }

        [Association("Person-Addresses")]
        [Aggregated]
        public XPCollection<Address> Addresses
        {
            get
            {
                return GetCollection<Address>(nameof(Addresses));
            }
        }
    }

    [Persistent]
    public class Address : XPObject
    {
        private Person person;
        private string street;
        private string city;
        private string stateProvince;
        private string zipPostal;
        private string country;

        public Address(Session session) : base(session) { }

        [Persistent]
        [Association("Person-Addresses")]
        public Person Person
        {
            get => person;
            set => SetPropertyValue(nameof(Person), ref person, value);
        }

        [Persistent]
        public string Street
        {
            get => street;
            set => SetPropertyValue(nameof(Street), ref street, value);
        }

        [Persistent]
        public string City
        {
            get => city;
            set => SetPropertyValue(nameof(City), ref city, value);
        }

        [Persistent]
        public string StateProvince
        {
            get => stateProvince;
            set => SetPropertyValue(nameof(StateProvince), ref stateProvince, value);
        }

        [Persistent]
        public string ZipPostal
        {
            get => zipPostal;
            set => SetPropertyValue(nameof(ZipPostal), ref zipPostal, value);
        }

        [Persistent]
        public string Country
        {
            get => country;
            set => SetPropertyValue(nameof(Country), ref country, value);
        }
    }
}

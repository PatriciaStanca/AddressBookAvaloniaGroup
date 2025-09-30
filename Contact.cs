using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AddressBookAvalonia.Models
{
    public class Contact : INotifyPropertyChanged
    {
        private string _name = "";
        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        private string _street = "";
        public string Street
        {
            get => _street;
            set => SetField(ref _street, value);
        }

        private string _zipCode = "";
        public string ZipCode
        {
            get => _zipCode;
            set => SetField(ref _zipCode, value);
        }

        private string _city = "";
        public string City
        {
            get => _city;
            set => SetField(ref _city, value);
        }

        private string _phone = "";
        public string Phone
        {
            get => _phone;
            set => SetField(ref _phone, value);
        }

        private string _email = "";
        public string Email
        {
            get => _email;
            set => SetField(ref _email, value);
        }

        //public override string ToString()
        //  => $"{Name}, {Street}, {ZipCode} {City}, {Phone}, {Email}";

        public string ToFileFormat()
            => $"{Name}|{Street}|{ZipCode}|{City}|{Phone}|{Email}";

        public static Contact FromFileFormat(string line)
        {
            var parts = line.Split('|');
            return new Contact
            {
                Name = parts[0],
                Street = parts[1],
                ZipCode = parts[2],
                City = parts[3],
                Phone = parts[4],
                Email = parts[5]
            };
        }

        // --- INotifyPropertyChanged ---
        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}

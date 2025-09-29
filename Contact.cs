namespace AddressBookAvalonia.Models
{
    public class Contact
    {
        public string Name { get; set; } = "";
        public string Street { get; set; } = "";
        public string ZipCode { get; set; } = "";
        public string City { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";

        public override string ToString()
        {
            return $"{Name}, {Street}, {ZipCode} {City}, {Phone}, {Email}";
        }

        public string ToFileFormat()
        {
            return $"{Name}|{Street}|{ZipCode}|{City}|{Phone}|{Email}";
        }

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
    }
}

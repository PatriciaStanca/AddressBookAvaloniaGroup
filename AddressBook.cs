using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using AddressBookAvalonia.Models;


namespace AddressBookAvalonia.Services
{
    public class AddressBook
    {
        private readonly string filePath = "addressbook.txt";
        private List<Contact> contacts = new();

        public AddressBook()
        {
            LoadContacts();
        }

        public List<Contact> GetAllContacts() => contacts;

        public void AddContact(Contact contact)
        {
            contacts.Add(contact);
            SaveContacts();
        }

        public void DeleteContact(Contact contact)
        {
            contacts.Remove(contact);
            SaveContacts();
        }

        public List<Contact> Search(string term)
        {
            return contacts.Where(c =>
                c.Name.Contains(term, System.StringComparison.OrdinalIgnoreCase) ||
                c.City.Contains(term, System.StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public void SaveContacts()
        {
            File.WriteAllLines(filePath, contacts.Select(c => c.ToFileFormat()));
        }

        private void LoadContacts()
        {
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                contacts = lines.Select(Contact.FromFileFormat).ToList();
            }
        }
    }
}

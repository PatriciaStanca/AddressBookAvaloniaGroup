using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AddressBookAvalonia.Models;

namespace AddressBookAvalonia.Services
{
    public class AddressBook
    {
        private readonly string filePath = "addressbook.txt";

        // Changed from List<Contact> to ObservableCollection<Contact>
        public ObservableCollection<Contact> Contacts { get; } = new();

        public AddressBook()
        {
            LoadContacts();
        }

        public IReadOnlyList<Contact> GetAllContacts() => Contacts;

        public void AddContact(Contact contact)
        {
            Contacts.Add(contact);
            SaveContacts();
        }

        public void DeleteContact(Contact contact)
        {
            if (contact is null) return;
            Contacts.Remove(contact);
            SaveContacts();
        }

        public List<Contact> Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return Contacts.ToList();

            return Contacts.Where(c =>
                   c.Name?.Contains(term, StringComparison.OrdinalIgnoreCase) == true
                || c.City?.Contains(term, StringComparison.OrdinalIgnoreCase) == true)
                .ToList();
        }

        public void SaveContacts()
        {
            File.WriteAllLines(filePath, Contacts.Select(c => c.ToFileFormat()));
        }

        private void LoadContacts()
        {
            Contacts.Clear();

            if (!File.Exists(filePath)) return;

            foreach (var line in File.ReadAllLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                try
                {
                    var c = Contact.FromFileFormat(line);
                    if (c != null) Contacts.Add(c);
                }
                catch
                {
                    // Ignore malformed lines
                }
            }
        }
    }
}

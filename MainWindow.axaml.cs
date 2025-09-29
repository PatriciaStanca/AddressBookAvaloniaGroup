using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using AddressBookAvalonia.Models;
using AddressBookAvalonia.Services;

namespace AddressBookAvalonia
{
    public partial class MainWindow : Window
    {
        private AddressBook addressBook = new();

        public MainWindow()
        {
            InitializeComponent();
            RefreshList(addressBook.GetAllContacts());
        }

        // ---------------- ADD ----------------
        private async void OnAddClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (!IsInputValid())
            {
                await ShowValidationError();
                return;
            }

            var contact = new Contact
            {
                Name = NameBox.Text ?? "",
                Street = StreetBox.Text ?? "",
                ZipCode = ZipBox.Text ?? "",
                City = CityBox.Text ?? "",
                Phone = PhoneBox.Text ?? "",
                Email = EmailBox.Text ?? ""
            };

            addressBook.AddContact(contact);
            RefreshList(addressBook.GetAllContacts());
            ClearInputs();
        }

        // ---------------- UPDATE ----------------
        private async void OnUpdateClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (ContactsList.SelectedItem is Contact contact)
            {
                if (!IsInputValid())
                {
                    await ShowValidationError();
                    return;
                }

                contact.Name = NameBox.Text ?? "";
                contact.Street = StreetBox.Text ?? "";
                contact.ZipCode = ZipBox.Text ?? "";
                contact.City = CityBox.Text ?? "";
                contact.Phone = PhoneBox.Text ?? "";
                contact.Email = EmailBox.Text ?? "";

                RefreshList(addressBook.GetAllContacts());
            }
        }

        // ---------------- DELETE ----------------
        private void OnDeleteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (ContactsList.SelectedItem is Contact contact)
            {
                addressBook.DeleteContact(contact);
                RefreshList(addressBook.GetAllContacts());
                ClearInputs();
            }
        }

        // ---------------- SEARCH ----------------
        private void OnSearchKeyUp(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            string term = SearchBox.Text ?? "";
            if (string.IsNullOrWhiteSpace(term))
                RefreshList(addressBook.GetAllContacts());
            else
                RefreshList(addressBook.Search(term));
        }

        // ---------------- AUTOFILL ----------------
        private void OnContactSelected(object? sender, SelectionChangedEventArgs e)
        {
            if (ContactsList.SelectedItem is Contact contact)
            {
                NameBox.Text = contact.Name;
                StreetBox.Text = contact.Street;
                ZipBox.Text = contact.ZipCode;
                CityBox.Text = contact.City;
                PhoneBox.Text = contact.Phone;
                EmailBox.Text = contact.Email;
            }
        }

        // ---------------- POPUP EDIT ----------------
        private async void OnEditClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is Contact contact)
            {
                var editWindow = new Window
                {
                    Title = "Edit Contact",
                    Width = 400,
                    Height = 350
                };

                var stack = new StackPanel { Margin = new Thickness(10) };

                var nameBox = new TextBox { Text = contact.Name, Watermark = "Name" };
                var streetBox = new TextBox { Text = contact.Street, Watermark = "Street" };
                var zipBox = new TextBox { Text = contact.ZipCode, Watermark = "Zip" };
                var cityBox = new TextBox { Text = contact.City, Watermark = "City" };
                var phoneBox = new TextBox { Text = contact.Phone, Watermark = "Phone" };
                var emailBox = new TextBox { Text = contact.Email, Watermark = "Email" };

                var saveButton = new Button { Content = "Save", Margin = new Thickness(0, 10, 0, 0) };

                saveButton.Click += (_, __) =>
                {
                    contact.Name = nameBox.Text ?? "";
                    contact.Street = streetBox.Text ?? "";
                    contact.ZipCode = zipBox.Text ?? "";
                    contact.City = cityBox.Text ?? "";
                    contact.Phone = phoneBox.Text ?? "";
                    contact.Email = emailBox.Text ?? "";

                    RefreshList(addressBook.GetAllContacts());
                    editWindow.Close();
                };

                stack.Children.Add(nameBox);
                stack.Children.Add(streetBox);
                stack.Children.Add(zipBox);
                stack.Children.Add(cityBox);
                stack.Children.Add(phoneBox);
                stack.Children.Add(emailBox);
                stack.Children.Add(saveButton);

                editWindow.Content = stack;

                await editWindow.ShowDialog(this);
            }
        }

        // ---------------- HELPERS ----------------
        private void RefreshList(List<Contact> contacts)
        {
            ContactsList.ItemsSource = null;
            ContactsList.ItemsSource = contacts;
        }

        private void ClearInputs()
        {
            NameBox.Text = "";
            StreetBox.Text = "";
            ZipBox.Text = "";
            CityBox.Text = "";
            PhoneBox.Text = "";
            EmailBox.Text = "";
        }

        private bool IsInputValid()
        {
            return !(string.IsNullOrWhiteSpace(NameBox.Text) ||
                     string.IsNullOrWhiteSpace(StreetBox.Text) ||
                     string.IsNullOrWhiteSpace(ZipBox.Text) ||
                     string.IsNullOrWhiteSpace(CityBox.Text) ||
                     string.IsNullOrWhiteSpace(PhoneBox.Text) ||
                     string.IsNullOrWhiteSpace(EmailBox.Text));
        }

        private async System.Threading.Tasks.Task ShowValidationError()
        {
            var dialog = new Window
            {
                Title = "Validation Error",
                Width = 300,
                Height = 150,
                Content = new TextBlock
                {
                    Text = "All fields are required. Please fill in all values.",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            };

            await dialog.ShowDialog(this);
        }
    }
}

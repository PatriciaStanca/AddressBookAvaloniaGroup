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
            ContactsList.ItemsSource = addressBook.Contacts;
            this.Closing += OnClosing;
        }

        // ---------------- ADD ----------------
        private async void OnAddClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (!IsInputValid(out string errorMessage))
            {
                await ShowValidationError(errorMessage);
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
            ClearInputs();
        }

        // ---------------- UPDATE ----------------
        private async void OnUpdateClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (ContactsList.SelectedItem is Contact contact)
            {
                if (!IsInputValid(out string errorMessage))
                {
                    await ShowValidationError(errorMessage);
                    return;
                }

                contact.Name = NameBox.Text ?? "";
                contact.Street = StreetBox.Text ?? "";
                contact.ZipCode = ZipBox.Text ?? "";
                contact.City = CityBox.Text ?? "";
                contact.Phone = PhoneBox.Text ?? "";
                contact.Email = EmailBox.Text ?? "";

                addressBook.SaveContacts();
                ClearInputs();
            }
        }

        // ---------------- DELETE ----------------
        private void OnDeleteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (ContactsList.SelectedItem is Contact contact)
            {
                addressBook.DeleteContact(contact);
                ClearInputs();
            }
        }

        // ---------------- SEARCH ----------------
        private void OnSearchKeyUp(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            string term = SearchBox.Text ?? "";
            if (string.IsNullOrWhiteSpace(term))
                ContactsList.ItemsSource = addressBook.Contacts;
            else
                ContactsList.ItemsSource = addressBook.Search(term);
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

        private void OnClearClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ClearInputs();
            ContactsList.SelectedItem = null;
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

                saveButton.Click += async (_, __) =>
                {
                    if (string.IsNullOrWhiteSpace(nameBox.Text) ||
                        string.IsNullOrWhiteSpace(streetBox.Text) ||
                        string.IsNullOrWhiteSpace(zipBox.Text) ||
                        string.IsNullOrWhiteSpace(cityBox.Text) ||
                        string.IsNullOrWhiteSpace(phoneBox.Text) ||
                        string.IsNullOrWhiteSpace(emailBox.Text))
                    {
                        await ShowValidationError("All fields must be filled.");
                        return;
                    }

                    if (!int.TryParse(zipBox.Text, out int _))
                    {
                        await ShowValidationError("Zip code must be numeric.");
                        return;
                    }

                    if (!long.TryParse(phoneBox.Text, out long _))
                    {
                        await ShowValidationError("Phone must be numeric.");
                        return;
                    }

                    if (!emailBox.Text.Contains("@"))
                    {
                        await ShowValidationError("Email must contain @.");
                        return;
                    }

                    contact.Name = nameBox.Text ?? "";
                    contact.Street = streetBox.Text ?? "";
                    contact.ZipCode = zipBox.Text ?? "";
                    contact.City = cityBox.Text ?? "";
                    contact.Phone = phoneBox.Text ?? "";
                    contact.Email = emailBox.Text ?? "";

                    addressBook.SaveContacts();
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

        private void ClearInputs()
        {
            NameBox.Text = "";
            StreetBox.Text = "";
            ZipBox.Text = "";
            CityBox.Text = "";
            PhoneBox.Text = "";
            EmailBox.Text = "";
        }

        private bool IsInputValid(out string errorMessage)
        {
            errorMessage = "";

            if (string.IsNullOrWhiteSpace(NameBox.Text) ||
                string.IsNullOrWhiteSpace(StreetBox.Text) ||
                string.IsNullOrWhiteSpace(ZipBox.Text) ||
                string.IsNullOrWhiteSpace(CityBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneBox.Text) ||
                string.IsNullOrWhiteSpace(EmailBox.Text))
            {
                errorMessage = "All fields must be filled.";
                return false;
            }

            if (!int.TryParse(ZipBox.Text, out int _))
            {
                errorMessage = "Zip code must be numeric.";
                return false;
            }

            if (!long.TryParse(PhoneBox.Text, out long _))
            {
                errorMessage = "Phone must be numeric.";
                return false;
            }

            if (!EmailBox.Text.Contains("@"))
            {
                errorMessage = "Email must contain @.";
                return false;
            }

            return true;
        }

        private async System.Threading.Tasks.Task ShowValidationError(string message)
        {
            var dialog = new Window
            {
                Title = "Validation Error",
                Width = 300,
                Height = 150,
                Content = new TextBlock
                {
                    Text = message,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                }
            };

            await dialog.ShowDialog(this);
        }

        private void OnClosing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            addressBook.SaveContacts();
        }
    }
}
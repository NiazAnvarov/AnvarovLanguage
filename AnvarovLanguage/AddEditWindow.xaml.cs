using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AnvarovLanguage
{
    /// <summary>
    /// Логика взаимодействия для AddEditWindow.xaml
    /// </summary>
    public partial class AddEditWindow : Window
    {
        private Client currentClient = new Client();

        public bool check = false;

        public AddEditWindow(Client client)
        {
            InitializeComponent();

            if(client != null)
            {
                check = true;
                currentClient = client;
                BirthdayDate.Text = currentClient.Birthday.ToString();
                if(currentClient.GenderCode == "м")
                {
                    GenderCodeBox.SelectedIndex = 0;
                }
                else
                {
                    GenderCodeBox.SelectedIndex = 1;
                }
                 
            }
            else
            {
                IDText.Visibility = Visibility.Hidden;
                IDBox.Visibility = Visibility.Hidden;
                GenderCodeBox.SelectedIndex = 0;
            }
            DataContext = currentClient;
            
        }

        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            if (myOpenFileDialog.ShowDialog() == true)
            {
                currentClient.PhotoPath = myOpenFileDialog.FileName;
                PhotoImage.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName));
            }
        }

        private void ClientSave_Click(object sender, RoutedEventArgs e)
        {

            if(currentClient.ID == 0)
            {
                AnvarovLanguageEntities.GetContext().Client.Add(currentClient);
            }
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(currentClient.LastName))
                errors.AppendLine("Укажите фамилию!");
            else
            {
                if (currentClient.LastName.Length > 50)
                    errors.AppendLine("Слишком длинная фамилия!");
            }
            if (string.IsNullOrWhiteSpace(currentClient.FirstName))
                errors.AppendLine("Укажите имя!");
            else
            {
                if (currentClient.FirstName.Length > 50)
                    errors.AppendLine("Слишком длинное имя!");
            }
            if (string.IsNullOrWhiteSpace(currentClient.Patronymic))
                currentClient.Patronymic = null;
            else
            {
                if (currentClient.Patronymic.Length > 50)
                    errors.AppendLine("Слишком длинное отчество!");
            }
            if (string.IsNullOrWhiteSpace(currentClient.Email))
                errors.AppendLine("Укажите Email!");
            else {
                string emailPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9]+$";
                if(!Regex.IsMatch(currentClient.Email, emailPattern))
                    errors.AppendLine("Не корректный Email!");
            }
            if (string.IsNullOrWhiteSpace(currentClient.Phone))
                errors.AppendLine("Укажите номер телефона!");
            else
            {
                if (currentClient.Phone.Count(c => c == '+') == 1) {
                    if (!(currentClient.Phone[0] == '+'))
                        errors.AppendLine("Не корректный номер телефона!");
                }
                if (currentClient.Phone.Count(c => c == '+') > 1 || currentClient.Phone.Count(c => c == '(') > 1 || currentClient.Phone.Count(c => c == ')') > 1 || currentClient.Phone.Count(c => c == '-') > 2)
                {
                    errors.AppendLine("Не корректный номер телефона! ");
                }
            }
            if(BirthdayDate.Text == ""){
                errors.AppendLine("Укажите дату рождения");
            }
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            currentClient.Birthday = Convert.ToDateTime(BirthdayDate.Text);
            if (GenderCodeBox.SelectedIndex == 0)
            {
                currentClient.GenderCode = "м";
            }
            else
            {
                currentClient.GenderCode = "ж";
            }

            List<Client> allClient = AnvarovLanguageEntities.GetContext().Client.ToList();
            allClient = allClient.Where(p => p.FirstName == currentClient.FirstName && p.LastName == currentClient.LastName && p.Patronymic == currentClient.Patronymic && p.Phone == currentClient.Phone).ToList();
            if (allClient.Count == 0 || check == true)
            {
                if(check == false)
                {
                    currentClient.RegistrationDate = DateTime.Now;
                }
                if (currentClient.ID == 0)
                {
                    AnvarovLanguageEntities.GetContext().Client.Add(currentClient);
                }
                try
                {
                    AnvarovLanguageEntities.GetContext().SaveChanges();
                    MessageBox.Show("Информация сохранена");
                    this.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            else
                MessageBox.Show("Такой клиент уже существует");
        }

        private void PhoneBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string filteredText = new string(textBox.Text.Where(c => char.IsDigit(c) || c == '+' || c == '-' || c == '(' || c == ')' || c == ' ').ToArray());

            if (filteredText != textBox.Text)
            {
                textBox.Text = filteredText;
                textBox.SelectionStart = filteredText.Length; 
            }
        }

        private void EmailBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string filteredText = new string(textBox.Text.Where(c => char.IsLetterOrDigit(c) || c == '_' || c == '.' || c == '@').ToArray());

            if (filteredText != textBox.Text)
            {
                textBox.Text = filteredText;
                textBox.SelectionStart = filteredText.Length;
            }
        }

        private void FirstNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string filteredText = new string(textBox.Text.Where(c => char.IsLetter(c) || c == ' ' || c == '-').ToArray());

            if (filteredText != textBox.Text)
            {
                textBox.Text = filteredText;
                textBox.SelectionStart = filteredText.Length;
            }
        }

        private void LastNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string filteredText = new string(textBox.Text.Where(c => char.IsLetter(c) || c == ' ' || c == '-').ToArray());

            if (filteredText != textBox.Text)
            {
                textBox.Text = filteredText;
                textBox.SelectionStart = filteredText.Length;
            }
        }

        private void PatronymicBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string filteredText = new string(textBox.Text.Where(c => char.IsLetter(c) || c == ' ' || c == '-').ToArray());

            if (filteredText != textBox.Text)
            {
                textBox.Text = filteredText;
                textBox.SelectionStart = filteredText.Length;
            }
        }
    }
}

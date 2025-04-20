using System.Windows;
using EkzamenADO.DataAccess;
using EkzamenADO.Models;

namespace EkzamenADO
{
    public partial class LoginWindow : Window
    {
        private DbManager db;

        public LoginWindow()
        {
            InitializeComponent();
            db = new DbManager();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailBox.Text;
            string password = PasswordBox.Password;

            var user = db.Login(email, password);

            if (user != null)
            {
                MessageBox.Show("Успешный вход!", "Добро пожаловать");

                AdsWindow adsWindow = new AdsWindow(user);
                adsWindow.Show();

                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль");
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailBox.Text;
            string password = PasswordBox.Password;

            var user = new User
            {
                Name = "Пользователь", // позже добавим форму с вводом имени
                Email = email,
                Phone = "0000000000" // можно тоже позже расширить
            };

            bool success = db.RegisterUser(user, password);

            if (success)
                MessageBox.Show("Регистрация прошла успешно!");
            else
                MessageBox.Show("Ошибка при регистрации. Возможно, такой email уже существует.");
        }
    }
}

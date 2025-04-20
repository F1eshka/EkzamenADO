using System.Windows;
using EkzamenADO.DataAccess;
using EkzamenADO.Models;

namespace EkzamenADO
{
    public partial class ProfileWindow : Window
    {
        private readonly User currentUser;
        private readonly DbManager db;

        public ProfileWindow(User user)
        {
            InitializeComponent();
            db = new DbManager();
            currentUser = user;

            NameBox.Text = user.Name;
            EmailLabel.Text = user.Email;
            PhoneBox.Text = user.Phone;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            currentUser.Name = NameBox.Text;
            currentUser.Phone = PhoneBox.Text;

            db.UpdateUser(currentUser);
            MessageBox.Show("Дані оновлено");
            this.Close();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Ви впевнені, що хочете видалити акаунт?", "Підтвердження", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                db.DeleteUser(currentUser.Id);
                MessageBox.Show("Ваш акаунт було видалено згідно статті 17 GDPR");
                Application.Current.Shutdown(); // закриваємо програму
            }
        }
    }
}

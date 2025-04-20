using System.Collections.Generic;
using System.Windows;
using EkzamenADO.DataAccess;
using EkzamenADO.Models;
using static EkzamenADO.DataAccess.DbManager;

namespace EkzamenADO
{
    public partial class AdsWindow : Window
    {
        private readonly DbManager db;
        private readonly User currentUser;

        public AdsWindow(User user)
        {
            InitializeComponent();
            db = new DbManager();
            currentUser = user;

            LoadAds();
        }

        private void LoadAds()
        {
            var ads = db.GetAdsByUser(currentUser.Id);
            AdList.ItemsSource = ads;
        }

        private void AddAd_Click(object sender, RoutedEventArgs e)
        {
            AddAdWindow addWindow = new AddAdWindow(currentUser);
            addWindow.ShowDialog();
            LoadAds(); // перезагрузим объявления после добавления
        }
        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            ProfileWindow profile = new ProfileWindow(currentUser);
            profile.ShowDialog();
        }
        private void EditAd_Click(object sender, RoutedEventArgs e)
        {
            if (AdList.SelectedItem is AdWithCategory selected)
            {
                EditAdWindow edit = new EditAdWindow(currentUser, selected.Id);
                edit.ShowDialog();
                LoadAds();
            }
            else
            {
                MessageBox.Show("Оберіть оголошення для редагування.");
            }
        }

        private void DeleteAd_Click(object sender, RoutedEventArgs e)
        {
            if (AdList.SelectedItem is AdWithCategory selected)
            {
                var result = MessageBox.Show("Ви впевнені, що хочете видалити це оголошення?", "Підтвердження", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    db.DeleteAd(selected.Id);
                    LoadAds();
                }
            }
            else
            {
                MessageBox.Show("Оберіть оголошення для видалення.");
            }
        }

    }
}

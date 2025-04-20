using System;
using System.Windows;
using EkzamenADO.DataAccess;
using EkzamenADO.Models;

namespace EkzamenADO
{
    public partial class EditAdWindow : Window
    {
        private readonly DbManager db;
        private readonly User currentUser;
        private readonly int adId;
        private Ad currentAd;

        public EditAdWindow(User user, int adId)
        {
            InitializeComponent();
            db = new DbManager();
            currentUser = user;
            this.adId = adId;

            CategoryBox.ItemsSource = db.GetAllCategories();
            CategoryBox.DisplayMemberPath = "Name";

            LoadAd();
        }

        private void LoadAd()
        {
            currentAd = db.GetAdById(adId);

            TitleBox.Text = currentAd.Title;
            DescriptionBox.Text = currentAd.Description;
            PriceBox.Text = currentAd.Price.ToString();

            // Выбор категории в ComboBox
            foreach (Category cat in CategoryBox.Items)
            {
                if (cat.Id == currentAd.CategoryId)
                {
                    CategoryBox.SelectedItem = cat;
                    break;
                }
            }
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleBox.Text) || !decimal.TryParse(PriceBox.Text, out decimal price) || CategoryBox.SelectedItem == null)
            {
                MessageBox.Show("Перевірте правильність заповнення полів.");
                return;
            }

            currentAd.Title = TitleBox.Text;
            currentAd.Description = DescriptionBox.Text;
            currentAd.Price = price;
            currentAd.CategoryId = ((Category)CategoryBox.SelectedItem).Id;

            db.UpdateAd(currentAd);
            MessageBox.Show("Оголошення оновлено!");
            Close();
        }
    }
}

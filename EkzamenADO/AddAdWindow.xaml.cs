using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using EkzamenADO.DataAccess;
using EkzamenADO.Models;

namespace EkzamenADO
{
    public partial class AddAdWindow : Window
    {
        private readonly DbManager db;
        private readonly User currentUser;
        private string selectedImageFile = null;

        public AddAdWindow(User user)
        {
            InitializeComponent();
            db = new DbManager();
            currentUser = user;

            CategoryBox.ItemsSource = db.GetAllCategories();
            CategoryBox.DisplayMemberPath = "Name";
            CategoryBox.SelectedIndex = 0;
        }

        private void ChooseImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image Files (*.jpg;*.png)|*.jpg;*.png";

            if (dialog.ShowDialog() == true)
            {
                selectedImageFile = dialog.FileName;
                ImageLabel.Text = System.IO.Path.GetFileName(selectedImageFile);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleBox.Text) || CategoryBox.SelectedItem == null || !decimal.TryParse(PriceBox.Text, out decimal price))
            {
                MessageBox.Show("Перевірте правильність введення!");
                return;
            }

            string fileName = null;
            if (selectedImageFile != null)
            {
                string imagesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                Directory.CreateDirectory(imagesPath);
                fileName = Guid.NewGuid().ToString() + Path.GetExtension(selectedImageFile);
                File.Copy(selectedImageFile, Path.Combine(imagesPath, fileName), true);
            }

            db.AddAd(new Ad
            {
                Title = TitleBox.Text,
                Description = DescriptionBox.Text,
                Price = price,
                ImageFileName = fileName,
                UserId = currentUser.Id,
                CategoryId = ((Category)CategoryBox.SelectedItem).Id,
                CreatedAt = DateTime.Now
            });

            MessageBox.Show("Оголошення додано!");
            Close();
        }
    }
}

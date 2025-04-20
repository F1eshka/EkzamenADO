using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using EkzamenADO.Models;

namespace EkzamenADO.DataAccess
{
    public class DbManager
    {
        private readonly string _connectionString;

        public DbManager()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        // =======================
        // ---- Категории --------
        // =======================

        public List<Category> GetAllCategories()
        {
            List<Category> categories = new List<Category>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Categories", conn);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new Category
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Name"].ToString()
                        });
                    }
                }
            }

            return categories;
        }

        public void AddCategory(string name)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Categories (Name) VALUES (@name)", conn);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.ExecuteNonQuery();
            }
        }

        // =======================
        // ---- Авторизация ------
        // =======================

        public static byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public static byte[] HashPassword(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000))
            {
                return pbkdf2.GetBytes(32); // 256-bit хеш
            }
        }

        public bool RegisterUser(User user, string password)
        {
            byte[] salt = GenerateSalt();
            byte[] hashedPassword = HashPassword(password, salt);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Users (Name, Email, HashedPassword, Salt, Phone) VALUES (@name, @mail, @pass, @salt, @phone)", conn);

                cmd.Parameters.AddWithValue("@name", user.Name);
                cmd.Parameters.AddWithValue("@mail", user.Email);
                cmd.Parameters.AddWithValue("@pass", hashedPassword);
                cmd.Parameters.AddWithValue("@salt", salt);
                cmd.Parameters.AddWithValue("@phone", user.Phone);

                try
                {
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (SqlException)
                {
                    return false; // например, дубликат email
                }
            }
        }

        public User Login(string email, string password)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Users WHERE Email = @email", conn);
                cmd.Parameters.AddWithValue("@email", email);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        byte[] salt = (byte[])reader["Salt"];
                        byte[] storedHash = (byte[])reader["HashedPassword"];
                        byte[] hash = HashPassword(password, salt);

                        if (storedHash.SequenceEqual(hash))
                        {
                            return new User
                            {
                                Id = (int)reader["Id"],
                                Name = reader["Name"].ToString(),
                                Email = email,
                                Phone = reader["Phone"].ToString(),
                                CreatedAt = (DateTime)reader["CreatedAt"]
                            };
                        }
                    }
                }
            }

            return null; // Неверный логин или пароль
        }
        public class AdWithCategory
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public string CategoryName { get; set; }
        }

        public List<AdWithCategory> GetAdsByUser(int userId)
        {
            List<AdWithCategory> ads = new List<AdWithCategory>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"
            SELECT a.Id, a.Title, a.Description, a.Price, c.Name AS CategoryName
            FROM Ads a
            JOIN Categories c ON a.CategoryId = c.Id
            WHERE a.UserId = @userId", conn);

                cmd.Parameters.AddWithValue("@userId", userId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ads.Add(new AdWithCategory
                        {
                            Id = (int)reader["Id"],
                            Title = reader["Title"].ToString(),
                            Description = reader["Description"].ToString(),
                            Price = (decimal)reader["Price"],
                            CategoryName = reader["CategoryName"].ToString()
                        });
                    }
                }
            }

            return ads;
        }
        public void AddAd(Ad ad)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"INSERT INTO Ads 
            (Title, Description, Price, ImageFileName, CreatedAt, UserId, CategoryId)
            VALUES (@title, @desc, @price, @img, @created, @userId, @catId)", conn);

                cmd.Parameters.AddWithValue("@title", ad.Title);
                cmd.Parameters.AddWithValue("@desc", ad.Description);
                cmd.Parameters.AddWithValue("@price", ad.Price);
                cmd.Parameters.AddWithValue("@img", (object?)ad.ImageFileName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@created", ad.CreatedAt);
                cmd.Parameters.AddWithValue("@userId", ad.UserId);
                cmd.Parameters.AddWithValue("@catId", ad.CategoryId);

                cmd.ExecuteNonQuery();
            }
        }
        public void UpdateUser(User user)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("UPDATE Users SET Name = @name, Phone = @phone WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@name", user.Name);
                cmd.Parameters.AddWithValue("@phone", user.Phone);
                cmd.Parameters.AddWithValue("@id", user.Id);

                cmd.ExecuteNonQuery();
            }
        }
        public void DeleteUser(int userId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand deleteAds = new SqlCommand("DELETE FROM Ads WHERE UserId = @id", conn);
                deleteAds.Parameters.AddWithValue("@id", userId);
                deleteAds.ExecuteNonQuery();

                SqlCommand deleteUser = new SqlCommand("DELETE FROM Users WHERE Id = @id", conn);
                deleteUser.Parameters.AddWithValue("@id", userId);
                deleteUser.ExecuteNonQuery();
            }
        }
        public void DeleteAd(int adId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Ads WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", adId);
                cmd.ExecuteNonQuery();
            }
        }

        public Ad GetAdById(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Ads WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Ad
                        {
                            Id = (int)reader["Id"],
                            Title = reader["Title"].ToString(),
                            Description = reader["Description"].ToString(),
                            Price = (decimal)reader["Price"],
                            ImageFileName = reader["ImageFileName"].ToString(),
                            CreatedAt = (DateTime)reader["CreatedAt"],
                            UserId = (int)reader["UserId"],
                            CategoryId = (int)reader["CategoryId"]
                        };
                    }
                }
            }

            return null!;
        }

        public void UpdateAd(Ad ad)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"
            UPDATE Ads SET Title = @title, Description = @desc, Price = @price, CategoryId = @cat 
            WHERE Id = @id", conn);

                cmd.Parameters.AddWithValue("@title", ad.Title);
                cmd.Parameters.AddWithValue("@desc", ad.Description);
                cmd.Parameters.AddWithValue("@price", ad.Price);
                cmd.Parameters.AddWithValue("@cat", ad.CategoryId);
                cmd.Parameters.AddWithValue("@id", ad.Id);

                cmd.ExecuteNonQuery();
            }
        }

    }
}

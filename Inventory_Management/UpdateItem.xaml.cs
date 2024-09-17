using Microsoft.Win32;
using System;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.IO;

namespace Inventory_Management
{
    /// <summary>
    /// Interaction logic for UpdateItem.xaml
    /// </summary>
    public partial class UpdateItem : Window
    {
        private int _itemId;

        public UpdateItem(int itemId)
        {
            InitializeComponent();

            _itemId = itemId; // Store the itemId for use within the class

            LoadItemDetails(); // Load item details based on the itemId
        }
        // Method to load the item details based on the itemId
        private void LoadItemDetails()
        {
            // Connection string from App.config
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["InventoryDBConnectionString"].ConnectionString;

            // SQL query to fetch the specific item details based on itemId
            string query = "SELECT ItemName, Description, Quantity, Picture FROM InventoryItems WHERE Item_ID = @ItemId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ItemId", _itemId);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        // Fill the form fields with the item details
                        txtItemName.Text = reader["ItemName"].ToString();
                        txtDescription.Text = reader["Description"].ToString();
                        txtQuantity.Text = reader["Quantity"].ToString();

                        // Load the image as a byte
                        byte[] imageBytes = reader["Picture"] as byte[];
                        if (imageBytes != null && imageBytes.Length > 0)
                        {
                            BitmapImage bitmap = new BitmapImage();
                            using (MemoryStream stream = new MemoryStream(imageBytes))
                            {
                                bitmap.BeginInit();
                                bitmap.StreamSource = stream;
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.EndInit();
                            }
                            imgPreview.Source = bitmap;
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading item details: " + ex.Message);
                }
            }
        }
        //Logic to browse for Image in local machine
        private void BrowseImage_Click(object sender, RoutedEventArgs e)
        {
            // Implement the logic to browse and select an image file
            // Example:
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";

            if (openFileDialog.ShowDialog() == true)
            {
                txtImagePath.Text = openFileDialog.FileName;
                imgPreview.Source = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.Absolute));
            }
        }
        //Logic to Update the database
        private void UpdateItem_Click(object sender, RoutedEventArgs e)
        {
            // Connection string from App.config
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["InventoryDBConnectionString"].ConnectionString;

            // Determine if an image is provided or use the default image
            byte[] imageBytes = string.IsNullOrEmpty(txtImagePath.Text.Trim()) ? GetDefaultImageBytes() : ConvertImageToByteArray(txtImagePath.Text.Trim());

            // Get the current username
            int updatedByUserId = CurrentUser.UserID;

            // SQL query to update the item details
            string query = @"UPDATE InventoryItems 
                             SET ItemName = @ItemName, 
                                 Description = @Description, 
                                 Quantity = @Quantity, 
                                 Picture = @Picture, 
                                 UpdatedAt = GETDATE(), 
                                 UpdatedBy = @UpdatedBy 
                             WHERE Item_ID = @ItemId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ItemName", txtItemName.Text.Trim());
                command.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                command.Parameters.AddWithValue("@Quantity", int.Parse(txtQuantity.Text.Trim()));
                command.Parameters.AddWithValue("@Picture", (object)imageBytes ?? DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedBy", updatedByUserId);
                command.Parameters.AddWithValue("@ItemId", _itemId);

                try
                {
                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Item updated successfully.");
                        this.Close(); // Close the window after a successful update
                    }
                    else
                    {
                        MessageBox.Show("Error updating item. No rows affected.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating item: " + ex.Message);
                }
            }
        }

        //Code to read file path of image
        private byte[] ConvertImageToByteArray(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
            {
                return null;
            }

            try
            {
                return File.ReadAllBytes(imagePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error converting image to byte array: " + ex.Message);
                return null;
            }
        }
        // Method to retrieve the default image bytes from the database
        private byte[] GetDefaultImageBytes()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["InventoryDBConnectionString"].ConnectionString;
            string query = "SELECT Picture FROM InventoryItems WHERE Item_ID = (SELECT MIN(Item_ID) FROM InventoryItems)"; // Adjust query to get the default image

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();

                    if (result != null && result is byte[])
                    {
                        return (byte[])result;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error retrieving default image: " + ex.Message);
                }
            }

            return null; // Return null or an empty byte array if default image is not found
        }


        //Navigation Bar
        private void UpdateInfo_Click(object sender, RoutedEventArgs e)
        {
            // Redirect to UserPage
            UserPage userPage = new UserPage();
            userPage.Show();
            this.Close();
        }
        private void AddNewItem_Click(object sender, RoutedEventArgs e)
        {
            // Redirect to AddNewItem page
            AddNewItem addItemPage = new AddNewItem();
            addItemPage.Show();
            this.Close();
        }
        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            // Redirect to AddNewItem page
            InventoryPage inventoryPage = new InventoryPage();
            inventoryPage.Show();
            this.Close();
        }
        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            // Redirect to DeleteItem page
            MessageBox.Show("Delete Item clicked!");
            this.Close();
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            // Handle logout click
            // Implement logout logic here (e.g., redirect to a login page)
            MessageBox.Show("Logout clicked!");
            this.Close();
        }

    }
}

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
using System.IO;
using Microsoft.Win32;
using System.Data.SqlClient;
using System.Configuration;


namespace Inventory_Management
{
    /// <summary>
    /// Interaction logic for AddNewItem.xaml
    /// </summary>
    public partial class AddNewItem : Window
    {
        // Declare _imagePath as a private field
        private string _imagePath;

        public AddNewItem()
        {
            InitializeComponent();
        }
        // Connection string to database
        private string connectionString = ConfigurationManager.ConnectionStrings["InventoryDBConnectionString"].ConnectionString;

        //Save image
        private void BrowseImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";

            if (openFileDialog.ShowDialog() == true)
            {
                _imagePath = openFileDialog.FileName;
                txtImagePath.Text = _imagePath;
                imgPreview.Source = new BitmapImage(new Uri(_imagePath));
            }
        }

        private void SaveItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the data from the form
                string itemName = txtItemName.Text.Trim();
                string description = txtDescription.Text.Trim();
                int quantity;

                ///check text box
                if (!int.TryParse(txtQuantity.Text.Trim(), out quantity))
                {
                    MessageBox.Show("Please enter a valid quantity.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Convert the image to a byte array
                byte[] imageBytes = null;
                if (!string.IsNullOrEmpty(_imagePath))
                {
                    imageBytes = File.ReadAllBytes(_imagePath);
                }

                // Fetch the current user's User_ID from the CurrentUser class
                int createdByUserId = CurrentUser.UserID;
                DateTime createdAt = DateTime.Now;

                // Save to database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                INSERT INTO InventoryItems (ItemName, Description, Quantity, Picture, CreatedAt, CreatedBy) 
                VALUES (@ItemName, @Description, @Quantity, @Picture, @CreatedAt, @CreatedBy)";

                    //Save items to database
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ItemName", itemName);
                        command.Parameters.AddWithValue("@Description", description);
                        command.Parameters.AddWithValue("@Quantity", quantity);
                        command.Parameters.AddWithValue("@Picture", imageBytes);
                        command.Parameters.AddWithValue("@CreatedAt", createdAt);
                        command.Parameters.AddWithValue("@CreatedBy", createdByUserId);

                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            MessageBox.Show("Item added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to add item.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Redirect to UserPage
        private void UpdateInfo_Click(object sender, RoutedEventArgs e)
        {
            UserPage userPage = new UserPage();
            userPage.Show();
            this.Close();
        }

        // Redirect to InventoryPage (Dashboard)
        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            InventoryPage inventoryPage = new InventoryPage();
            inventoryPage.Show();
            this.Close();
        }

        // Implement functionality to delete an item
        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            // Add your delete item logic here
            MessageBox.Show("Delete Item clicked!");
        }

        // Logout functionality
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            // Implement logout logic here (e.g., redirect to a login page)
            MessageBox.Show("Logout clicked!");
            this.Close();
        }
    }
}

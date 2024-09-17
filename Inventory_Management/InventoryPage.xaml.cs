using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Collections.Generic;


namespace Inventory_Management
{
    /// <summary>
    /// Interaction logic for InventoryPage.xaml
    /// </summary>
    public partial class InventoryPage : Window
    {
        public InventoryPage()
        {
            InitializeComponent();
            LoadInventoryItems();
        }

        // Redirect to UserPage
        private void UpdateInfo_Click(object sender, RoutedEventArgs e)
        {
            UserPage userPage = new UserPage();
            userPage.Show();
            this.Close();
        }

        private void AddNewItem_Click(object sender, RoutedEventArgs e)
        {
            AddNewItem addItem = new AddNewItem();
            addItem.Show();
            this.Close(); // Corrected method name here
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            // Handle logout click
            // Implement logout logic here (e.g., redirect to a login page)
            MessageBox.Show("Logout clicked!");
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close(); // Corrected method name here
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            // Redirect to DeleteItemPage
            MessageBox.Show("Delete Item clicked!");
            this.Close(); // Corrected method name here
        }

        private void UpdateItem_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            if (clickedButton != null)
            {
                // Retrieve the Item_ID from the Tag property
                int itemId = (int)clickedButton.Tag;

                // Open the UpdateItem window with the selected Item_ID
                UpdateItem updateItemWindow = new UpdateItem(itemId);
                updateItemWindow.Show();
                this.Close(); // Close the current window
            }
            else
            {
                MessageBox.Show("Failed to retrieve item ID.");
            }
        }
        //Load Inventory Items
        private void LoadInventoryItems()
        {
            // Connection string from App.config
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["InventoryDBConnectionString"].ConnectionString;

            // Query to fetch inventory items
            string query = "SELECT Item_ID, ItemName, Description, Quantity, Picture FROM InventoryItems";

            List<InventoryItem> items = new List<InventoryItem>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        InventoryItem item = new InventoryItem
                        {
                            ItemID = Convert.ToInt32(reader["Item_ID"]),
                            ItemName = reader["ItemName"].ToString(),
                            Description = reader["Description"].ToString(),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            Picture = reader["Picture"] as byte[] //retrieve picture as byte
                        };
                        items.Add(item);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching inventory items: " + ex.Message);
                }
            }

            DisplayItems(items);
        }
        //Display Items
        private void DisplayItems(List<InventoryItem> items)
        {
            foreach (var item in items)
            {
                // Create a new Border for each inventory item
                Border itemBorder = new Border
                {
                    Background = new SolidColorBrush(Colors.White),
                    CornerRadius = new CornerRadius(5),
                    BorderBrush = new SolidColorBrush(Colors.DarkGreen),
                    BorderThickness = new Thickness(1),
                    Width = 250,
                    Height = 300,
                    Margin = new Thickness(10)
                };

                // Create a StackPanel to hold item details
                StackPanel stackPanel = new StackPanel();

                // Image for the item
                Image image = new Image
                {
                    Height = 150,
                    Width = 250,
                    Stretch = Stretch.UniformToFill
                };

                if (item.Picture != null && item.Picture.Length > 0)
                {
                    try
                    {
                        using (var stream = new System.IO.MemoryStream(item.Picture))
                        {
                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.StreamSource = stream;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();
                            image.Source = bitmap;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading image from database: {ex.Message}");
                        image.Source = new BitmapImage(new Uri("Images/default_image.png", UriKind.Relative));
                    }
                }
                else
                {
                    image.Source = new BitmapImage(new Uri("Images/default_image.png", UriKind.Relative));
                }

                // Add the image to the StackPanel
                stackPanel.Children.Add(image);

                // TextBlocks for item name, description, and quantity
                stackPanel.Children.Add(new TextBlock
                {
                    Text = item.ItemName,
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(5)
                });

                stackPanel.Children.Add(new TextBlock
                {
                    Text = item.Description,
                    FontSize = 14,
                    Margin = new Thickness(5)
                });

                stackPanel.Children.Add(new TextBlock
                {
                    Text = $"Quantity: {item.Quantity}",
                    FontSize = 14,
                    Margin = new Thickness(5)
                });

                // Update button for each item
                Button updateButton = new Button
                {
                    Content = "Update",
                    Style = (Style)FindResource("ModernButtonStyle"),
                    Margin = new Thickness(10)
                };
                // Pass the itemId to the UpdateItem method when the button is clicked
                int itemId = item.ItemID; // Capture the current item's ID
                updateButton.Click += (sender, e) => HandleItemUpdate(itemId);

                stackPanel.Children.Add(updateButton);

                // Set the content of the border to the stack panel
                itemBorder.Child = stackPanel;

                // Add the border to the WrapPanel
                ItemWrapPanel.Children.Add(itemBorder);
            }
        }

        private void HandleItemUpdate(int itemId)
        {
            // Logic to handle item update with the specific itemId
            UpdateItem updateItem = new UpdateItem(itemId);
            updateItem.Show();
            this.Close();
        }
    }
}
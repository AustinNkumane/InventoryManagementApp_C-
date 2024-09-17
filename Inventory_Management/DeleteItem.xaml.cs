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
using System.Data;

namespace Inventory_Management
{
    /// <summary>
    /// Interaction logic for DeleteItem.xaml
    /// </summary>
    public partial class DeleteItem : Window
    {
            public DeleteItem()
            {
                InitializeComponent();
                LoadItems();
            }

            // Method to load items from the database
            private void LoadItems()
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["InventoryDBConnectionString"].ConnectionString;

                string query = "SELECT Item_ID, ItemName, Description, Quantity, Picture FROM InventoryItems";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable itemsTable = new DataTable();
                    adapter.Fill(itemsTable);

                    ItemDataGrid.ItemsSource = itemsTable.DefaultView; // Bind to DataGrid
                }
            }

            // Method to delete an item from the database
            private void DeleteItem_Click(object sender, RoutedEventArgs e)
            {
                Button deleteButton = (Button)sender;
                int itemId = Convert.ToInt32(deleteButton.Tag); // Retrieve the item ID from the button's Tag

                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["InventoryDBConnectionString"].ConnectionString;

                string query = "DELETE FROM InventoryItems WHERE Item_ID = @ItemId";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ItemId", itemId);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Item deleted successfully.");
                            LoadItems(); // Reload items after deletion
                        }
                        else
                        {
                            MessageBox.Show("Error deleting item.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }
    }
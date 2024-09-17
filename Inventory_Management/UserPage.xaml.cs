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

namespace Inventory_Management
{
    /// <summary>
    /// Interaction logic for UserPage.xaml
    /// </summary>
    public partial class UserPage : Window
    {
        public UserPage()
        {
            InitializeComponent();
        }

        private void AddNewItem_Click(object sender, RoutedEventArgs e)
        {
            // Redirect to AddNewItem page
            AddNewItem addItemPage = new AddNewItem();
            addItemPage.Show();
            this.Close();
        }
        private void Inventory_Click(object sender, RoutedEventArgs e)
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
        //update user
        private void UpdateUser_Click(object sender, RoutedEventArgs e)
        {
            // Handle logout click
            // Implement logout logic here (e.g., redirect to a login page)
            MessageBox.Show("User Updated");
            this.Close();
        }
    }
}

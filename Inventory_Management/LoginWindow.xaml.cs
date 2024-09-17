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
using System.Security.Cryptography;

namespace Inventory_Management
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }


        //custom program
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            if (ValidateUser(username, password))
            {
                // Store the logged-in user's details in the CurrentUser static class
                CurrentUser.Username = username;

                // Open the InventoryPage window
                InventoryPage inventoryPage = new InventoryPage();
                inventoryPage.Show();
                this.Close(); // Close the login window
            }
            else
            {
                MessageBox.Show("Invalid username or password.");
            }
        }
        //validate user against database
        private bool ValidateUser(string username, string password)
        {
            // Connection string from App.config
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["InventoryDBConnectionString"].ConnectionString;

            // Query to select the User_ID, Password, and Role for the given username
            string query = "SELECT User_ID, Password, Role FROM Users WHERE Username = @Username";

            // Variable to store the stored password hash from the database
            string storedPasswordHash = null;
            int userId = 0;
            string role = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Username", username);

                try
                {
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        // Get the stored password hash, User_ID, and Role from the database
                        userId = Convert.ToInt32(reader["User_ID"]);
                        storedPasswordHash = reader["Password"].ToString();
                        role = reader["Role"].ToString();

                        // Store user details in the CurrentUser static class
                        CurrentUser.UserID = userId;
                        CurrentUser.Username = username;
                        CurrentUser.Role = role;
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during database access
                    Console.WriteLine("Error accessing the database: " + ex.Message);
                    return false;
                }
            }

            // If no user was found with the given username, return false
            if (storedPasswordHash == null)
            {
                return false;
            }

            // Compare the provided password with the stored password hash
            return VerifyPasswordHash(password, storedPasswordHash);
        }

        //verify password Hash
        private bool VerifyPasswordHash(string password, string storedHash)
        {
            // Hash the input password
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                string passwordHash = Convert.ToBase64String(hashBytes);

                // Compare the hashed password with the stored hash
                return passwordHash == storedHash;
            }
        }

        private void RegisterLink_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the registration window
            SignupWIndow signupWindow = new SignupWIndow();
            // Show the registration window
            signupWindow.Show();
            // Optionally, close the main window
            this.Close();
        }
    }
}

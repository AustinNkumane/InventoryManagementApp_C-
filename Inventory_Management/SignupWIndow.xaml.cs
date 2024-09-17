using System;
using System.Collections.Generic;
using System.Configuration;
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
    /// Interaction logic for SignupWIndow.xaml
    /// </summary>
    public partial class SignupWIndow : Window
    {
        public SignupWIndow()
        {
            InitializeComponent( );
        }

        ///custom code <summary>
        
        void Register_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmailAddress.Text;
            string username = txtUsername.Text;
            string password = txtPassword.Password;
            string role = cmbRole.Text;

            // Validate input
            if (string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Please fill in all fields.", "Registration Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // H
            // ash the password using SHA-256
            string hashedPassword = HashPassword(password);

            // Connection string from App.config
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["InventoryDBConnectionString"].ConnectionString;

            // SQL Insert command
            string insertQuery = "INSERT INTO Users (Username, Password, Email, Role, created_at) " +
                         "VALUES (@Username, @Password, @Email, @Role, @CreatedAt)";

            // SQL to check if user already exists
            string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username OR Email = @Email";

            //try Catch 
            try
            {
                // Insert data into the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    // Check if the user already exists
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@Username", username);
                    checkCommand.Parameters.AddWithValue("@Email", email);

                    //open database connection
                    connection.Open();

                    int userCount = (int)checkCommand.ExecuteScalar();

                    if (userCount > 0)
                    {
                        MessageBox.Show("Username or Email already exists. ", "Registration Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        // Redirect to Login WIndow 
                        LoginWindow loginWindow = new LoginWindow();
                        loginWindow.Show();
                        this.Close(); // Close the Registration window                      
                    }

                    // Insert new user into the database
                    SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                    insertCommand.Parameters.AddWithValue("@Username", username);
                    insertCommand.Parameters.AddWithValue("@Password", hashedPassword);
                    insertCommand.Parameters.AddWithValue("@Email", email);
                    insertCommand.Parameters.AddWithValue("@Role", role);
                    insertCommand.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                    //result to check if data saved succesfully
                    int result = insertCommand.ExecuteNonQuery();

                    // Check if the insert was successful
                    if (result > 0)
                    {
                        MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        // Redirect to login window                      
                        LoginWindow loginWindow = new LoginWindow();
                        loginWindow.Show();
                        this.Close(); // Close the Registration window
                    }
                    else
                    {
                        MessageBox.Show("Registration failed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        // Method to hash the password
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}

using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Inventory_Management
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //Redirect to Registration Window
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the SignupWindow
            SignupWIndow signupWindow = new SignupWIndow();
            // Show the SignupWindow
            signupWindow.Show();
            // Optionally, you can close the main window if it's no longer needed
            this.Close();
        }

        //Redirect to Login Page
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            // Handle Login button click
            // For example, you might want to show a login window or perform another action
            LoginWindow loginWindow = new LoginWindow();
            // Show the SignupWindow
            loginWindow.Show();
            // Optionally, you can close the main window if it's no longer needed
            this.Close();
        }
    }
}
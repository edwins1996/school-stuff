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
using System.Windows.Navigation;
using System.Windows.Shapes;
using conceptProject.Login;

namespace conceptProject
{
    /// <summary>
    /// Interaction logic for HeadMenu.xaml
    /// </summary>
    public partial class LoginScreen : UserControl 
    {
        public LoginScreen()
        {
            InitializeComponent();
        }
        private void placeHolder(object sender, RoutedEventArgs e)
        {
            if (inlogVeld.Text == "Gebruikersnaam")
                inlogVeld.Text = "";
        }

        private void placeHolder2nd(object sender, RoutedEventArgs e)
        {
            if (inlogVeld.Text == "")
                inlogVeld.Text = "Gebruikersnaam";
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Login.Login loginVari = new Login.Login();
            loginVari.inlogName = inlogVeld.Text;
            loginVari.inlogPass = wachtwoordVeld.Password;

            if (loginVari.userLogin() == true)
            {
                HeadMenu nextPage = new HeadMenu();
                TheGrid.Children.Clear();
                TheGrid.Children.Add(nextPage);
            }
            else
                MessageBox.Show("Uw gebruikersnaam/wachtwoord is onjuist.\nKlik op OK om opnieuw te proberen.", "FOUT!", MessageBoxButton.OK);
        }
    }
}

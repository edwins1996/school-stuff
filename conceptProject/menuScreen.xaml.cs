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
using System.Text.RegularExpressions;

namespace conceptProject
{
    /// <summary>
    /// Interaction logic for menuScreen.xaml
    /// </summary>
    public partial class menuScreen : UserControl
    {
        private voorraadMgmt stockData = new voorraadMgmt();
        private menuMgmt menuData = new menuMgmt();
        private Regex digitsOnly = new Regex(@"[^\d]");
        public menuScreen()
        {
            InitializeComponent();
        }

        private void searchItems(object sender, KeyEventArgs e)
        {
            searchItemsbox.Items.Clear();
            List<voorraadMgmt.stocknAantal> zoekList = stockData.readDBstock().FindAll(x => x.omschrijving.Contains(menuIngr.Text));
            foreach (voorraadMgmt.stocknAantal row in zoekList)
                searchItemsbox.Items.Add(row.omschrijving + ", (" + row.eenheid + ")");
        }

        private void searchMenus(object sender, KeyEventArgs e)
        {
            searchItemsboxDel.Items.Clear();
            List<menuMgmt.menuStruct> zoekList = menuData.readDBmenus().FindAll(x => x.menuBeschrijving.Contains(menuIngrDel.Text));
            foreach (menuMgmt.menuStruct row in zoekList)
                searchItemsboxDel.Items.Add(row.menuBeschrijving);
        }

        private void menDelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (searchItemsboxDel.SelectedItem != null)
            {
                menuMgmt.menuStruct zoekList = menuData.readDBmenus().Find(x => x.menuBeschrijving.Contains(searchItemsboxDel.SelectedItem.ToString()));
                MessageBoxResult check = MessageBox.Show("Weet u het zeker?", "Controle", MessageBoxButton.YesNo, MessageBoxImage.Hand);
                if (check == MessageBoxResult.Yes)
                {
                    if (menuData.delDBmenus(zoekList.menuID))
                        MessageBox.Show("Menu succesvol verwijderd.", "Succesvol", MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                        MessageBox.Show("Menu niet verwijderd.\nMogelijk wordt deze nog gebruikt in een bestelling.\nProbeer het opnieuw.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                    MessageBox.Show("Menu niet verwijderd.", "Succesvol", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
                MessageBox.Show("Geen menu gekozen.\nProbeer het opnieuw.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void plusBtn_Click(object sender, RoutedEventArgs e)
        {
            if (searchItemsbox.SelectedItem != null && aantBox.Text != "")
            {
                if (digitsOnly.Replace(aantBox.Text, "") != "")
                {
                    string[] input = searchItemsbox.SelectedItem.ToString().Split(',');
                    if (chosenIngbox.Text.Contains(input[0]) == false)
                    {
                        chosenIngbox.Text += input[0] + "\n";
                        chosenIngboxaant.Text += digitsOnly.Replace(aantBox.Text, "") + "\n";
                    }
                    else
                        MessageBox.Show("Ingrediënt reeds ingevoerd.\nProbeer het opnieuw.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                    MessageBox.Show("Er is aantal ingevoerd.\nProbeer het opnieuw.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                MessageBox.Show("Er is geen item geselecteerd of aantal ingevoerd.\nProbeer het opnieuw.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void resetBtn_Click(object sender, RoutedEventArgs e)
        {
            chosenIngbox.Text = "";
            chosenIngboxaant.Text = "";
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (chosenIngbox.Text != "" && chosenIngboxaant.Text != "" && menuName.Text != "" && menuPrice.Text != "")
            {
                if (menuData.addDBmenus(menuName.Text, menuPrice.Text))
                {
                    string[] array = chosenIngbox.Text.Split('\n');
                    string[] array1 = chosenIngboxaant.Text.Split('\n');
                    int i = 0;
                    foreach (string data in array)
                    {
                        if (data != "")
                        {
                            if (menuData.addDBingr(menuData.lastDBid(), menuData.prodDBid(data), int.Parse(array1[i])))
                            {
                                if (i == 0)
                                    MessageBox.Show("Menu succesvol opgeslagen.", "Succesvol", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                                MessageBox.Show("Er is iets fout gegaan.\nProbeer het opnieuw.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);

                        }
                        i++;
                    }
                }
                else
                    MessageBox.Show("Er is iets fout gegaan.\nProbeer het opnieuw.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                MessageBox.Show("Er zijn een aantal velden niet ingevuld.\nProbeer het opnieuw.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            HeadMenu nextPage = new HeadMenu();
            TheGrid.Children.Clear();
            TheGrid.Children.Add(nextPage);
        }

        private void searchMenusEdit(object sender, KeyEventArgs e)
        {
            searchItemsboxedit.Items.Clear();
            List<menuMgmt.menuStruct> zoekList = menuData.readDBmenus().FindAll(x => x.menuBeschrijving.Contains(menuIngrsrch.Text));
            foreach (menuMgmt.menuStruct row in zoekList)
                searchItemsboxedit.Items.Add(row.menuBeschrijving);
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            if (searchItemsboxedit.SelectedItem != null)
            {
                menuMgmt.menuStruct zoekList = menuData.readDBmenus().Find(x => x.menuBeschrijving.Contains(searchItemsboxedit.SelectedItem.ToString()));
                menuEditScreen dialog = new menuEditScreen(zoekList.menuID, zoekList.menuBeschrijving, zoekList.prijs);
                dialog.ShowDialog();
            }
            else
                MessageBox.Show("U heeft geen menu gekozen.\nProbeer het opnieuw.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

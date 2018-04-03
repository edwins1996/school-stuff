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
    /// Interaction logic for drankScreen.xaml
    /// </summary>
    public partial class drankScreen : UserControl
    {
        private Regex digitsOnly = new Regex(@"[^\d]");
        private voorraadMgmt stockData = new voorraadMgmt();
        private drankMgmt drankData = new drankMgmt();
        public drankScreen()
        {
            InitializeComponent();
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            HeadMenu nextPage = new HeadMenu();
            TheGrid.Children.Clear();
            TheGrid.Children.Add(nextPage);
        }

        private void searchProd(object sender, KeyEventArgs e)
        {
            prodBox.Items.Clear();
            List<voorraadMgmt.stocknAantal> zoekList = stockData.readDBstock().FindAll(x => x.omschrijving.Contains(searchProdbox.Text));
            foreach (voorraadMgmt.stocknAantal row in zoekList)
                prodBox.Items.Add(row.omschrijving);
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (prodBox.SelectedItem != null)
            {
                if (digitsOnly.Replace(prijsBox.Text, "") != "" && prijsBox.Text != "")
                {
                    if (drankData.drankDBadd(prodBox.SelectedItem.ToString(), prijsBox.Text))
                        MessageBox.Show("Drank succesvol toegevoegd.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                        MessageBox.Show("Product bestaat al,\nof u heeft een foutieve prijs ingevoerd.\nProbeer het opnieuw.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                    MessageBox.Show("Vul een juiste prijs in.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                MessageBox.Show("Selecteer een product.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void searchDrankbox_KeyUp(object sender, KeyEventArgs e)
        {
            drankBox.Items.Clear();
            List<string> zoekList = drankData.readDBdrank().FindAll(x => x.Contains(searchDrankbox.Text));
            foreach (string row in zoekList)
                drankBox.Items.Add(row);
        }

        private void fillFunc(object sender, SelectionChangedEventArgs e)
        {
            if (drankBox.SelectedItem != null)
            {
                drankNaam.Text = drankBox.SelectedItem.ToString();
                drankPrijs.Text = drankData.readDBdrankEdit(drankBox.SelectedItem.ToString()).ToString();
            }
            else
                MessageBox.Show("Selecteer een drankje.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            if(drankNaam.Text != "" && drankPrijs.Text != "")
            {
                if(drankData.editDBdrank(drankBox.SelectedItem.ToString(), drankNaam.Text, drankPrijs.Text))
                    MessageBox.Show("Drank succesvol gewijzigd.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show("De naam van uw drankje bestaat al.\nOf u heeft geen juiste prijs ingevoerd.\nVul een juiste prijs in (bijv. 15,99).", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                MessageBox.Show("Vul alle velden in.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void delBtn_Click(object sender, RoutedEventArgs e)
        {
            if (drankBox.SelectedItem != null)
            {
                MessageBoxResult res = MessageBox.Show("Weet u het zeker?", "Controle", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    if (drankData.delDBdrank(drankBox.SelectedItem.ToString()))
                        MessageBox.Show("Drank succesvol verwijderd.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                        MessageBox.Show("Drank niet verwijderd.\nWordt mogelijk nog gebruikt in een bestelling.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                    MessageBox.Show("Verwijderen geannuleerd.", "Annuleren", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}

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
    /// Interaction logic for voorraadScreen.xaml
    /// </summary>
    public partial class voorraadScreen : UserControl
    {
        private voorraadMgmt stockData = new voorraadMgmt();
        private Regex digitsOnly = new Regex(@"[^\d]");
        public voorraadScreen()
        {
            InitializeComponent();
            init();
        }
        private void init()
        {
            VoorraadGrid.Children.Clear();
            int x = 0;
            foreach (voorraadMgmt.stocknAantal data in stockData.readDBstock())
            {
                TextBlock MenuNames = new TextBlock();
                MenuNames.Text = data.omschrijving;
                MenuNames.Name = "Prod" + x;
                MenuNames.Tag = data.id;
                MenuNames.HorizontalAlignment = HorizontalAlignment.Left;
                MenuNames.VerticalAlignment = VerticalAlignment.Top;
                MenuNames.Margin = new Thickness(0, 30 * x, 0, 0);

                TextBox MenuAantal = new TextBox();
                MenuAantal.Text = data.aantal.ToString();
                MenuAantal.HorizontalAlignment = HorizontalAlignment.Left;
                MenuAantal.VerticalAlignment = VerticalAlignment.Top;
                MenuAantal.Height = 25;
                MenuAantal.Width = 50;
                MenuAantal.Name = "Prod" + x;
                MenuAantal.Tag = data.id;
                MenuAantal.Margin = new Thickness(440, 30 * x, 0, 0);
                VoorraadGrid.Children.Add(MenuNames);
                VoorraadGrid.Children.Add(MenuAantal);
                x++;
            }
        }
        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            HeadMenu nextPage = new HeadMenu();
            TheGrid.Children.Clear();
            TheGrid.Children.Add(nextPage);
        }

        private void searchProd(object sender, KeyEventArgs e)
        {
            delList.Items.Clear();
            List <voorraadMgmt.stocknAantal> zoekList = stockData.readDBstock().FindAll(x => x.omschrijving.Contains(searchField.Text));
            foreach (voorraadMgmt.stocknAantal row in zoekList)
                delList.Items.Add(row.omschrijving);
        }

        private void fillDelbox(object sender, SelectionChangedEventArgs e)
        {
            if(delList.SelectedItem != null)
            {
                voorraadMgmt.stocknAantal filler = stockData.readDBstock().Find(x => x.omschrijving.Contains(delList.SelectedItem.ToString()));
                prodNaambox1.Text = filler.omschrijving;
                prodAantbox1.Text = filler.aantal.ToString();
                minvoorBox.Text = filler.minVoorraad;
                eenBox.Text = filler.eenheid;
                prodnrbox1.Text = filler.artikelnr;
            }
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            if(prodNaambox.Text != "" && prodAantbox.Text != "" && minVoorraadBox.Text != "" && eenheidBox.Text != "" && prodartNrBox.Text != "")
            {
                switch(stockData.saveDBprod(prodNaambox.Text, prodAantbox.Text, prodartNrBox.Text, digitsOnly.Replace(minVoorraadBox.Text, ""), eenheidBox.Text))
                {
                    case true:
                        init();
                        MessageBox.Show("Product succesvol opgeslagen","Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    case false:
                        MessageBox.Show("Product niet opgeslagen\nProbeer het opnieuw!", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                }
            }else
                MessageBox.Show("Een of meerdere velden zijn niet ingevuld.\nProbeer het opnieuw!", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void voorraadSave(object sender, RoutedEventArgs e)
        {
            List<TextBox> prodAantal = VoorraadGrid.Children.OfType<TextBox>().ToList();
            List<TextBlock> prodNaam = VoorraadGrid.Children.OfType<TextBlock>().ToList();
            List<voorraadMgmt.stocknAantal> editList = new List<voorraadMgmt.stocknAantal>();
            int i = 0;
            
            foreach (TextBlock mNaam in prodNaam)
            {
                
                foreach (TextBox mAantal in prodAantal)
                {
                    if (mNaam.Name == mAantal.Name && mNaam.Text != "")
                    {
                        
                        if (digitsOnly.Replace(mAantal.Text, "") != "")
                        {
                            voorraadMgmt.stocknAantal editProd = new voorraadMgmt.stocknAantal(mNaam.Text, int.Parse(digitsOnly.Replace(mAantal.Text, "")), "", "", int.Parse(mNaam.Tag.ToString()), "");
                            editList.Add(editProd);
                        }
                        else
                            MessageBox.Show("Vul een getal in voor de voorraad.\nProbeer het opnieuw!", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                i++;
            }
            switch (stockData.editDBprod(editList))
            {
                case true:
                    MessageBox.Show("Wijziging succesvol opgeslagen", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case false:
                    MessageBox.Show("Wijziging niet opgeslagen\nProbeer het opnieuw!", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
            }
        }

        private void changeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (prodAantbox1.Text != "" && prodAantbox1.Text != "" && prodnrbox1.Text != "" && minvoorBox.Text != "" && eenBox.Text != "")
            {
                List<TextBox> prodAantal = VoorraadGrid.Children.OfType<TextBox>().ToList();
                List<TextBlock> prodNaam = VoorraadGrid.Children.OfType<TextBlock>().ToList();
                List<voorraadMgmt.stocknAantal> editList = new List<voorraadMgmt.stocknAantal>();
                voorraadMgmt.stocknAantal search = stockData.readDBstock().Find(x => x.omschrijving.Contains(delList.SelectedItem.ToString()));
                if (digitsOnly.Replace(prodAantbox1.Text, "") != "" && digitsOnly.Replace(minvoorBox.Text, "") != "")
                {
                    voorraadMgmt.stocknAantal filler = new voorraadMgmt.stocknAantal(prodNaambox1.Text, int.Parse(digitsOnly.Replace(prodAantbox1.Text, "")), digitsOnly.Replace(minvoorBox.Text, ""), eenBox.Text, search.id, prodnrbox1.Text);
                    editList.Add(filler);
                    switch (stockData.editDBprod(editList))
                    {
                        case true:
                            MessageBox.Show("Wijziging succesvol opgeslagen", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                            break;
                        case false:
                            MessageBox.Show("Wijziging niet opgeslagen\nProbeer het opnieuw!", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                            break;
                    }
                    TextBlock edIT = prodNaam.Find(x => x.Text.Contains(delList.SelectedItem.ToString()));
                    edIT.Text = filler.omschrijving;
                    foreach (TextBox test in prodAantal)
                    {
                        if (test.Tag.ToString() == edIT.Tag.ToString())
                        {
                            test.Text = filler.aantal.ToString();
                        }
                    }
                }
                else
                    MessageBox.Show("Vul een getal in voor de voorraad.\nProbeer het opnieuw!", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
                MessageBox.Show("Er zijn een of meerdere velden leeg.\nProbeer het opnieuw!", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void delBtn_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(delList.SelectedItem.ToString());
            if (delList.SelectedItem != null)
            {
                voorraadMgmt.stocknAantal filler = stockData.readDBstock().Find(x => x.omschrijving.Contains(delList.SelectedItem.ToString()));
                MessageBoxResult res = MessageBox.Show("Weet u het zeker?", "Controle", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    switch (stockData.deleteDBprod(filler.id))
                    {
                        case true:
                            MessageBox.Show("Succesvol verwijderd!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                            break;
                        case false:
                            MessageBox.Show("Verwijderen niet gelukt.\nMogelijk wordt dit product gebruikt in een menu/drankje", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                            break;
                    }
                }
                else
                    MessageBox.Show("Verwijderen geannuleerd", "Annuleren", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
                MessageBox.Show("Geen item geselecteerd.\nProbeer het opnieuw!", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}

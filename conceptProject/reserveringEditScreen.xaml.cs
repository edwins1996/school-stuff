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
using conceptProject.Reserveringsdata;
using System.Text.RegularExpressions;

namespace conceptProject
{
    /// <summary>
    /// Interaction logic for reserveringEditScreen.xaml
    /// </summary>
    public partial class reserveringEditScreen : Window
    {
        reserverenData check = new reserverenData();
        drankMgmt drankCheck = new drankMgmt();
        private Regex digitsOnly = new Regex(@"[^\d]");
        private int reservID;
        public reserveringEditScreen(int resID)
        {
            InitializeComponent();
            reservID = resID;
            reserverenData menuData = new reserverenData();
            menuData.showMenus();
            int x = 0, y = 0;
            foreach (reserverenData.menuNAantal data in menuData.puMenus)
            {
                if (menuData.showMenus2(data.menuAantal))
                {
                    y++;
                    TextBlock MenuNames = new TextBlock();
                    MenuNames.Text = data.menuBeschrijving;
                    MenuNames.Name = "Menu" + x;
                    MenuNames.HorizontalAlignment = HorizontalAlignment.Left;
                    MenuNames.VerticalAlignment = VerticalAlignment.Top;
                    MenuNames.Margin = new Thickness(0, 30 * x, 0, 0);

                    TextBox MenuAantal = new TextBox();
                    MenuAantal.Text = "";
                    MenuAantal.HorizontalAlignment = HorizontalAlignment.Left;
                    MenuAantal.VerticalAlignment = VerticalAlignment.Top;
                    MenuAantal.Height = 20;
                    MenuAantal.Width = 40;
                    MenuAantal.Name = "Menu" + x;
                    MenuAantal.Margin = new Thickness(440, 30 * x, 0, 0);

                    MenuGrid.Children.Add(MenuNames);
                    MenuGrid.Children.Add(MenuAantal);
                    x++;
                }
            }
            drankjesFill();
            List<int> aantallen = new List<int>();
            List<int> menuIDS = new List<int>();
            int i = 0;
            int totaal = 0;
            int menData = 0;
            foreach (voorraadMgmt.stocknAantal row in check.readResDB(resID))
            {
                huidigeMenus.Text += row.omschrijving + "       " + row.aantal + "\n";
                totaal = totaal + row.aantal;
            }

            saveBtn.Click += (s, e) =>
            {
                List<TextBox> MenuAantal = MenuGrid.Children.OfType<TextBox>().ToList();
                List<TextBlock> MenuNaam = MenuGrid.Children.OfType<TextBlock>().ToList();
                int newTotaal = 0;
                foreach (TextBox aantal in MenuAantal)
                {
                    foreach (TextBlock naam in MenuNaam)
                    {
                        if (aantal.Name == naam.Name && aantal.Text != "")
                        {
                            if (digitsOnly.Replace(aantal.Text, "") != "")
                            {
                                newTotaal = newTotaal + int.Parse(digitsOnly.Replace(aantal.Text, ""));
                                aantallen.Add(int.Parse(digitsOnly.Replace(aantal.Text, "")));
                                menuIDS.Add(menuData.readDBmenID(naam.Text));
                            }
                        }
                    }
                    i++;
                }
                if (totaal == newTotaal)
                    menData = 1;
                else
                    menData = 2;

                if (menData == 1)
                {
                    menuData.showMenus();
                    int z = 0;
                    foreach (reserverenData.menuNAantal row in menuData.puMenus)
                    {
                        if (menuData.showMenus2(row.menuAantal))
                            z++;
                    }
                    menuData.delBestDB(resID);
                    menuData.insertDBbest(aantallen, menuIDS);
                    menuData.updateBestDB(resID);
                    if (z < y)
                    {
                        MessageBox.Show("Door deze reservering te maken zijn er één\nof meerdere menu's onder de minimale\n voorraad van 5 sets per menu gekomen.", "Melding", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    else
                    {
                        MessageBox.Show("Gegevens succesvol opgeslagen!", "MELDING", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }

                }
                else if(menData == 2)
                    MessageBox.Show("Er zijn meer/minder menu's geselecteerd dan er aantal personen zijn.\nVul opnieuw in.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            };
            bool checker = false;
            saveBtn_2.Click += (s, e) =>
            {
                List<TextBox> DrankAantal = DrankGrid.Children.OfType<TextBox>().ToList();
                List<TextBlock> DrankNaam = DrankGrid.Children.OfType<TextBlock>().ToList();
                menuData.delDBdrankbest(resID);
                foreach (TextBox aantal in DrankAantal)
                {
                    foreach (TextBlock naam in DrankNaam)
                    {
                        if (aantal.Name == naam.Name && aantal.Text != "")
                        {
                            if (digitsOnly.Replace(aantal.Text, "") != "")
                            {
                                if (menuData.insertDBdrankbest(resID, naam.Text, digitsOnly.Replace(aantal.Text, "")))
                                    checker = true;
                                else
                                    checker = false;

                            }
                        }
                    }
                    i++;
                }
                if(checker)
                    MessageBox.Show("Gegevens succesvol opgeslagen!", "MELDING", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                else
                    MessageBox.Show("Gegevens niet opgeslagen!", "MELDING", MessageBoxButton.OK, MessageBoxImage.Error);
            };
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        public void drankjesFill()
        {
            int y = 0;
            foreach (reserverenData.menuNAantal row in check.readDrankDB(reservID))
            {
                TextBlock MenuNames = new TextBlock();
                MenuNames.Text = row.menuBeschrijving;
                MenuNames.Name = "Menu" + y;
                MenuNames.HorizontalAlignment = HorizontalAlignment.Left;
                MenuNames.VerticalAlignment = VerticalAlignment.Top;
                MenuNames.Margin = new Thickness(0, 30 * y, 0, 0);

                TextBox MenuAantal = new TextBox();
                MenuAantal.Text = row.menuAantal.ToString();
                MenuAantal.HorizontalAlignment = HorizontalAlignment.Left;
                MenuAantal.VerticalAlignment = VerticalAlignment.Top;
                MenuAantal.Height = 20;
                MenuAantal.Width = 40;
                MenuAantal.Name = "Menu" + y;
                MenuAantal.Margin = new Thickness(440, 30 * y, 0, 0);

                DrankGrid.Children.Add(MenuNames);
                DrankGrid.Children.Add(MenuAantal);
                y++;

            }
        }

        private void fillDrankbox(object sender, KeyEventArgs e)
        {
            drankBox.Items.Clear();
            List<string> zoekList = drankCheck.readDBdrank().FindAll(x => x.Contains(searchDrank.Text));
            foreach (string row in zoekList)
                drankBox.Items.Add(row);
        }

        private void addDrank_Click(object sender, RoutedEventArgs e)
        {
            if(drankBox.SelectedItem != null && digitsOnly.Replace(drankAantal.Text, "") != "")
            {
                if (check.insertDBdrankbest(reservID, drankBox.SelectedItem.ToString(), digitsOnly.Replace(drankAantal.Text, "")))
                {
                    DrankGrid.Children.Clear();
                    drankjesFill();
                }
                else
                    MessageBox.Show("Dit drankje is al ingevoerd!", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                MessageBox.Show("Selecteer een drankje!", "Fout", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
    }
}

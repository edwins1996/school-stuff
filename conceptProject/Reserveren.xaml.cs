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
using conceptProject.Klant;
using conceptProject.Reserveringsdata;
using System.Text.RegularExpressions;

namespace conceptProject
{
    /// <summary>
    /// Interaction logic for Reserveren.xaml
    /// </summary>
    public partial class Reserveren : UserControl
    {
        private Regex digitsOnly = new Regex(@"[^\d]");
        public Reserveren()
        {
            InitializeComponent();
            klantData woonplaatsen = new klantData();
            foreach (string row in woonplaatsen.customCities())
                woonplaatsBox.Items.Add(row);
            klantID.Visibility = Visibility.Hidden;
            timeBox.IsEnabled = false;
            dateBox.IsEnabled = false;
            tableBox.IsEnabled = false;
            reserverenData menuData = new reserverenData();
            menuData.showMenus();
            int x = 0, y = 0;
            foreach(reserverenData.menuNAantal data in menuData.puMenus)
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

            submitButton.Click += (s, e) =>
            {
                //Invoeren van de reservering in de database n.a.v. het klikken van de button Reservering opslaan
                if (achternaamBox.Text != "" && postcodeBox.Text != "" && huisnrBox.Text != "" && persBox.Text != "" && timeBox.Text != "" && dateBox.Text != "" && tableBox.Text != "")
                {
                    bool data = false;
                    List<TextBox> MenuNaam = MenuGrid.Children.OfType<TextBox>().ToList();
                    List<TextBlock> MenuAantal = MenuGrid.Children.OfType<TextBlock>().ToList();
                    List<string> timeID = new List<string>();
                    List<int> aantallen = new List<int>();
                    List<int> menuIDS = new List<int>();

                    string[] tafels = tableBox.Text.Split(',');
                    string[] timeSplit = timeBox.Text.Split('\n');
                    foreach (string timeslot in timeSplit)
                    {
                        switch (timeslot)
                        {
                            case "17:00 - 19:00":
                                timeID.Add("1");
                                break;
                            case "19:00 - 21:00":
                                timeID.Add("2");
                                break;
                            case "21:00 - 23:00":
                                timeID.Add("3");
                                break;
                        }
                    }
                    int i = 0;
                    int totAantal = 0;
                    foreach (TextBox mNaam in MenuNaam)
                    {
                        i++;
                        foreach (TextBlock mAantal in MenuAantal)
                        {
                            if (mNaam.Name == mAantal.Name && mNaam.Text != "")
                            {
                                if (digitsOnly.Replace(mNaam.Text, "") != "")
                                {
                                    int henk = int.Parse(mNaam.Text);
                                    totAantal = totAantal + henk;
                                    int aantalPers = 0;
                                    try
                                    {
                                        aantalPers = int.Parse(digitsOnly.Replace(persBox.Text, ""));
                                    }
                                    catch
                                    {
                                        aantalPers = -1;
                                    }
                                    if (totAantal != aantalPers)
                                        data = false;
                                    else
                                        data = true;
                                    aantallen.Add(int.Parse(digitsOnly.Replace(mNaam.Text, "")));
                                    menuIDS.Add(menuData.readDBmenID(mAantal.Text));
                                }
                            }
                        }
                    }
                    if (data == false)
                    {
                        MessageBox.Show("Er zijn meer/minder menu's geselecteerd dan er aantal personen zijn.\nVul opnieuw in.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (data != false)
                    {
                        menuData.insertDBbest(aantallen, menuIDS);
                        menuData.insertDBreser(klantID.Text, (DateTime.Parse(dateBox.Text)).ToString("yyyy-MM-dd"), persBox.Text);
                        menuData.insertDBtimetable(tafels, timeID);
                        menuData.showMenus();
                        int z = 0;
                        foreach (reserverenData.menuNAantal row in menuData.puMenus)
                        {
                            if (menuData.showMenus2(row.menuAantal))
                                z++;
                        }
                        if (z < y)
                        {
                            MessageBoxResult result = MessageBox.Show("Door deze reservering te maken zullen er één\nof meerdere menu's onder de minimale\n voorraad van 5 sets per menu komen.\nWilt u toch doorgaan?", "Melding", MessageBoxButton.YesNo, MessageBoxImage.Information);
                            if (result == MessageBoxResult.Yes)
                            {
                                MessageBox.Show("Gegevens succesvol opgeslagen!", "MELDING", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                                HeadMenu nextPage = new HeadMenu();
                                TheGrid.Children.Clear();
                                TheGrid.Children.Add(nextPage);
                            }
                            else
                            {
                                menuData.delDBreser(menuData.maxResID());
                                MessageBox.Show("Reservering gestopt.", "MELDING", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                                HeadMenu nextPage = new HeadMenu();
                                TheGrid.Children.Clear();
                                TheGrid.Children.Add(nextPage);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Gegevens succesvol opgeslagen!", "MELDING", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                            HeadMenu nextPage = new HeadMenu();
                            TheGrid.Children.Clear();
                            TheGrid.Children.Add(nextPage);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Er zijn een aantal velden niet ingevuld.\nProbeer het opnieuw.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };
        }

        private void dataSearch(object sender, KeyEventArgs e)
        {
            klantenBox.Items.Clear();
            klantenBox.IsEnabled = true;
            if (searchBox.Text.Length > 0)
            {
                klantData objSearch = new klantData();
                objSearch.dbCustomer(searchBox.Text);

                foreach (string strKlant in objSearch.dataCustom)
                    klantenBox.Items.Add(strKlant);

                if(klantenBox.Items.Count == 0)
                {
                    klantenBox.Items.Add("Geen resultaten...");
                    klantenBox.IsEnabled = false;
                }
                else
                {

                }
            }
            else
            {
                klantenBox.Items.Add("Geen resultaten...");
                klantenBox.IsEnabled = false;
            }
        }
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            klantData klantRet = new klantData();
            klantRet.klantVoornaam = voornaamBox.Text;
            klantRet.klantTussenvoegsel = tussenvoegselBox.Text;
            klantRet.klantAchternaam = achternaamBox.Text;
            klantRet.klantAdres = adresBox.Text;
            klantRet.klantHuisnummer = huisnrBox.Text;
            klantRet.klantPostcode = postcodeBox.Text;
            klantRet.klantWoonplaats = woonplaatsBox.SelectedItem.ToString();
            klantRet.klantEmail = emailBox.Text;
            klantRet.klantTelefoon = telBox.Text;

            if (klantRet.dbCSave() == true)
                MessageBox.Show("Gegevens succesvol opgeslagen!", "MELDING", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            else
                MessageBox.Show("Fout bij gegevens opslaan.\nProbeer het opnieuw!", "MELDING", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            HeadMenu nextPage = new HeadMenu();
            TheGrid.Children.Clear();
            TheGrid.Children.Add(nextPage);
        }

        private void klantInfoChange(object sender, SelectionChangedEventArgs e)
        {
            if (klantenBox.SelectedItem != null)
            {
                klantData klantRet = new klantData();
                klantRet.dbCdata(klantenBox.SelectedItem.ToString());
                klantID.Text = klantRet.klantID.ToString();
                voornaamBox.Text = klantRet.klantVoornaam;
                tussenvoegselBox.Text = klantRet.klantTussenvoegsel;
                achternaamBox.Text = klantRet.klantAchternaam;
                adresBox.Text = klantRet.klantAdres;
                huisnrBox.Text = klantRet.klantHuisnummer;
                postcodeBox.Text = klantRet.klantPostcode;
                woonplaatsBox.SelectedValue = klantRet.klantWoonplaats;
                emailBox.Text = klantRet.klantEmail;
                telBox.Text = klantRet.klantTelefoon;

            }
            else
                MessageBox.Show("U heeft niets geselecteerd!", "FOUT!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void platteGrondBtn_Click(object sender, RoutedEventArgs e)
        {
            timeBox.Text = ""; tableBox.Text = ""; dateBox.Text = "";
            reserverenData data = new reserverenData();
            reserveerOverzicht win = new reserveerOverzicht();
            win.ShowDialog();
            if (win.IsActive == false)
            {
                dateBox.Text = data.readDBtempDate(DateTime.Today).ToString("dd-MM-yyyy");
                foreach(int iTime in data.readDBtempTimeslots())
                {
                    switch (iTime)
                    {
                        case 1:
                            timeBox.Text += "17:00 - 19:00\n";
                            break;
                        case 2:
                            timeBox.Text += "19:00 - 21:00\n";
                            break;
                        case 3:
                            timeBox.Text += "21:00 - 23:00\n";
                            break;
                    }
                }
                if (data.readDBtempTables().Count != 0)
                {
                    int last = data.readDBtempTables().Last();
                    foreach (int iTable in data.readDBtempTables())
                    {
                        if (iTable.Equals(last))
                            tableBox.Text += iTable;
                        else
                            tableBox.Text += iTable + ", ";
                    }
                }
                data.deleteTempdata();
            }
        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

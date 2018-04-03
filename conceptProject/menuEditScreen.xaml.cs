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
using System.Text.RegularExpressions;

namespace conceptProject
{
    /// <summary>
    /// Interaction logic for menuEditScreen.xaml
    /// </summary>
    public partial class menuEditScreen : Window
    {
        private Regex digitsOnly = new Regex(@"[^\d]");
        voorraadMgmt check = new voorraadMgmt();
        menuMgmt save = new menuMgmt();
        public menuEditScreen(int id, string beschrijving, string prijs)
        {
            InitializeComponent();
            menuName.Text = beschrijving;
            menuPrice.Text = prijs; int x = 0;
            foreach (voorraadMgmt.stocknAantal data in check.readDBstock())
            {
                TextBlock ProdNames = new TextBlock();
                ProdNames.Text = data.omschrijving;
                ProdNames.Name = "Product" + x;
                ProdNames.Tag = data.id;
                ProdNames.HorizontalAlignment = HorizontalAlignment.Left;
                ProdNames.VerticalAlignment = VerticalAlignment.Top;
                ProdNames.Margin = new Thickness(0, 30 * x, 0, 0);

                TextBox MenuAantal = new TextBox();
                if(check.readDBprodAant(id, data.id).ToString() != "0")
                    MenuAantal.Text = check.readDBprodAant(id, data.id).ToString();
                MenuAantal.HorizontalAlignment = HorizontalAlignment.Left;
                MenuAantal.VerticalAlignment = VerticalAlignment.Top;
                MenuAantal.Height = 20;
                MenuAantal.Width = 40;
                MenuAantal.Name = "Product" + x;
                MenuAantal.Margin = new Thickness(440, 30 * x, 0, 0);

                MenuIngBox.Children.Add(ProdNames);
                MenuIngBox.Children.Add(MenuAantal);
                x++;
            }

            saveBtn.Click += (s, e) => 
            {
                List<TextBox> ingAantal = MenuIngBox.Children.OfType<TextBox>().ToList();
                List<TextBlock> ingNaam = MenuIngBox.Children.OfType<TextBlock>().ToList();
                if (save.saveDBmenuN(id, menuName.Text, menuPrice.Text))
                {
                    save.editDelDB(id);
                    int i = 0;
                    foreach (TextBox aantal in ingAantal)
                    {
                        i++;
                        foreach (TextBlock naam in ingNaam)
                        {
                            if (aantal.Name == naam.Name && aantal.Text != "")
                            {
                                if (digitsOnly.Replace(aantal.Text, "") != "")
                                {
                                    if (save.addDBingr(id, int.Parse(naam.Tag.ToString()), int.Parse(aantal.Text)))
                                    {
                                        if (i == 1)
                                            MessageBox.Show("Wijziging succesvol opgeslagen.", "Succesvol", MessageBoxButton.OK, MessageBoxImage.Information);
                                    }
                                    else
                                        MessageBox.Show("Er is iets fout gegaan.\nProbeer het opnieuw.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                                else
                                    MessageBox.Show("Vul voor aantal een getal in.\nProbeer het opnieuw.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    Close();
                }
                else
                    MessageBox.Show("Er is iets fout gegaan.\nProbeer het opnieuw.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            };
        }
    }
}

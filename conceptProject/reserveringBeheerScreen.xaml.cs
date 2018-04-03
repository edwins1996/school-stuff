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
using conceptProject.Reserveringsdata;

namespace conceptProject
{
    /// <summary>
    /// Interaction logic for reserveringBeheerScreen.xaml
    /// </summary>
    public partial class reserveringBeheerScreen : UserControl
    {
        reserverenData check = new reserverenData();
        public reserveringBeheerScreen()
        {
            InitializeComponent();
            int x = 0;
            foreach (reserverenData.reserData data in check.readDBres())
            {
                TextBlock Reserveringen = new TextBlock();
                Reserveringen.Text = data.reserOmschrijving;
                Reserveringen.Name = "Menu" + x;
                Reserveringen.Tag = data.reserID;
                Reserveringen.HorizontalAlignment = HorizontalAlignment.Left;
                Reserveringen.VerticalAlignment = VerticalAlignment.Top;
                Reserveringen.Margin = new Thickness(0, 30 * x, 0, 0);

                RadioButton checkbox = new RadioButton();
                checkbox.Content = "";
                checkbox.HorizontalAlignment = HorizontalAlignment.Left;
                checkbox.VerticalAlignment = VerticalAlignment.Top;
                checkbox.Height = 20;
                checkbox.Width = 40;
                checkbox.Name = "Menu" + x;
                checkbox.Margin = new Thickness(440, 30 * x, 0, 0);

                ReserGrid.Children.Add(Reserveringen);
                ReserGrid.Children.Add(checkbox);
                x++;
            }

            delBtn.Click += (s, e) =>
            {
                //Verwijderen van de reservering n.a.v. klikken van de Annuleren button
                List<RadioButton> reserveringCheck = ReserGrid.Children.OfType<RadioButton>().ToList();
                List<TextBlock> reserveringNaam = ReserGrid.Children.OfType<TextBlock>().ToList();
                int i = 0;
                foreach (RadioButton rCheck in reserveringCheck)
                {
                    
                    foreach (TextBlock rNaam in reserveringNaam)
                    {
                        if (rCheck.Name == rNaam.Name && rCheck.IsChecked != false)
                        {
                            MessageBoxResult res = MessageBox.Show("Weet u het zeker?", "Controle", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (res == MessageBoxResult.Yes)
                            {
                                if (check.delDBreser(int.Parse(rNaam.Tag.ToString())))
                                        MessageBox.Show("Gegevens succesvol verwijderd!", "MELDING", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                            }
                            else
                                MessageBox.Show("Gegevens niet verwijderd!", "MELDING", MessageBoxButton.OK, MessageBoxImage.Information);

                        }
                    }
                    i++;
                }
            };
            editBtn.Click += (s, e) =>
            {
                //Wijzigen van de reservering n.a.v. klikken van de Wijzigen button
                List<RadioButton> reserveringCheck = ReserGrid.Children.OfType<RadioButton>().ToList();
                List<TextBlock> reserveringNaam = ReserGrid.Children.OfType<TextBlock>().ToList();
                int i = 0;
                foreach (RadioButton rCheck in reserveringCheck)
                {

                    foreach (TextBlock rNaam in reserveringNaam)
                    {
                        if (rCheck.Name == rNaam.Name && rCheck.IsChecked != false)
                        {
                            reserveringEditScreen screen = new reserveringEditScreen(int.Parse(rNaam.Tag.ToString()));
                            screen.ShowDialog();
                        }
                    }
                    i++;
                }
            };
            facBtn.Click += (s, e) =>
            {
                List<RadioButton> reserveringCheck = ReserGrid.Children.OfType<RadioButton>().ToList();
                List<TextBlock> reserveringNaam = ReserGrid.Children.OfType<TextBlock>().ToList();
                int i = 0;
                foreach (RadioButton rCheck in reserveringCheck)
                {

                    foreach (TextBlock rNaam in reserveringNaam)
                    {
                        if (rCheck.Name == rNaam.Name && rCheck.IsChecked != false)
                        {
                            factuurScreen screen = new factuurScreen(int.Parse(rNaam.Tag.ToString()));
                            screen.ShowDialog();
                        }
                    }
                    i++;
                }
            };
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            HeadMenu nextPage = new HeadMenu();
            TheGrid.Children.Clear();
            TheGrid.Children.Add(nextPage);
        }
    }
}

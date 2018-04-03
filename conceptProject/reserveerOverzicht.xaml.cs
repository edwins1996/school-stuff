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
namespace conceptProject
{
    /// <summary>
    /// Interaction logic for reserveerOverzicht.xaml
    /// </summary>
    public partial class reserveerOverzicht : Window
    { 
        private List<Button> tables()
        {
            List<Button> tafels = TheGrid.Children.OfType<Button>().ToList();
            return tafels;
        }
        private List<CheckBox> timeBlocks()
        {
            List<CheckBox> tijdblokken = TheGrid.Children.OfType<CheckBox>().ToList();
            return tijdblokken;
        }
        private reserverenData check = new reserverenData();
        public reserveerOverzicht()
        {
            InitializeComponent();
            DatumKiezer.Text = DateTime.Today.ToString();            
            foreach (Button table in tables())
            {
                    table.Click += (s, e) => {
                        if (((Image)table.Content).Source.ToString().Contains("tafel.png") == true)
                        {
                            table.Content = new Image { Source = new BitmapImage(new Uri("tafel_gekozen.png", UriKind.RelativeOrAbsolute)), VerticalAlignment = VerticalAlignment.Center };
                            if(!tafelnummersBox.Text.Contains("Tafel " + table.TabIndex.ToString() + "\n"))
                                tafelnummersBox.Text += "Tafel " + table.TabIndex + "\n";
                        }
                        else
                        {
                            table.Content = new Image { Source = new BitmapImage(new Uri("tafel.png", UriKind.RelativeOrAbsolute)), VerticalAlignment = VerticalAlignment.Center };
                            if (tafelnummersBox.Text.Contains("Tafel " + table.TabIndex.ToString() + "\n"))
                                tafelnummersBox.Text = tafelnummersBox.Text.Replace("Tafel " + table.TabIndex.ToString() + "\n", "");
                        }
                    };
            }
        }
        private void checkBox_changed(object sender, RoutedEventArgs e)
        {
            List<int> timeSlots = new List<int>();
            foreach (CheckBox timeAmount in timeBlocks())
            {
                if (timeAmount.IsChecked == true)
                    timeSlots.Add(timeAmount.TabIndex);
                if (timeBlocks()[0].IsChecked == true && timeBlocks()[2].IsChecked == true)
                {
                    timeAmount.IsChecked = true;
                }                
            }
            List<int> tafels = check.bezetTafels(DatumKiezer.SelectedDate.ToString(), timeSlots);
            if (tafels.Count == 0)
            {
                foreach (Button table in tables())
                {
                    table.IsEnabled = true;
                    tafelnummersBox.Text = null;
                    table.Content = new Image { Source = new BitmapImage(new Uri("tafel.png", UriKind.RelativeOrAbsolute)), VerticalAlignment = VerticalAlignment.Center };
                    table.Foreground = Brushes.Transparent;
                }
            }
            else {
                foreach (Button table in tables())
                {
                    foreach (int henk in tafels)
                    {
                        if (table.TabIndex == henk)
                        {
                            table.IsEnabled = false;
                            table.Content = new Image { Source = new BitmapImage(new Uri("tafel-bezet.png", UriKind.RelativeOrAbsolute)), VerticalAlignment = VerticalAlignment.Center };
                            table.Foreground = Brushes.Transparent;
                        }
                    }
                }
            }
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            List<int> William = new List<int>();
            List<int> Marc = new List<int>();
            foreach (Button table in tables())
            {
                if (((Image)table.Content).Source.ToString().Contains("tafel_gekozen.png") == true)
                    Marc.Add(table.TabIndex);             

            }
            foreach (CheckBox chkStr in timeBlocks())
            {
               if (chkStr.IsChecked == true)
                    William.Add(chkStr.TabIndex);
            }

            //Slaat tafels, tijdsloten en datum op in temp tabel
                if (William.Count != 0 && Marc.Count != 0)
                    check.insDBtempDate((DateTime)DatumKiezer.SelectedDate, William, Marc);
                else if (William.Count == 0 && Marc.Count != 0)
                    MessageBox.Show("U heeft geen tijd(en) gekozen!", "FOUT!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                else if (William.Count != 0 && Marc.Count == 0)
                    MessageBox.Show("U heeft geen tafel(s) gekozen!", "FOUT!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                else if (William.Count == 0 && Marc.Count == 0)
                    MessageBox.Show("U heeft geen tijd(en) en tafel(s) gekozen!", "FOUT!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                else
                    MessageBox.Show("Er is een fout opgetreden!", "FOUT!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Close();
        } 
    }
}

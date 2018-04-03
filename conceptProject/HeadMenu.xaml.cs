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
using Excel = Microsoft.Office.Interop.Excel;

namespace conceptProject
{
    /// <summary>
    /// Interaction logic for HeadMenu.xaml
    /// </summary>
    public partial class HeadMenu : UserControl
    {
        public HeadMenu()
        {
            InitializeComponent();
            beheerBtn.Content = "Reserveringen\nbeheer";
            gebruiker.Content = Login.Login.Gebruiker;
            if(Login.Login.Functie == "Manager")
            {
                button.Visibility = Visibility.Hidden;
                voorraadBtn.Visibility = Visibility.Hidden;
                menuBtn.Visibility = Visibility.Hidden;
                drankBtn.Visibility = Visibility.Hidden;
                levBtn.Visibility = Visibility.Hidden;
            }
            else if(Login.Login.Functie == "Kok")
            {
                button.Visibility = Visibility.Hidden;
                beheerBtn.Visibility = Visibility.Hidden;
                klantBtn.Visibility = Visibility.Hidden;
            }
            else if(Login.Login.Functie == "Serveerster")
            {
                klantBtn.Visibility = Visibility.Hidden;
                voorraadBtn.Visibility = Visibility.Hidden;
                menuBtn.Visibility = Visibility.Hidden;
                drankBtn.Visibility = Visibility.Hidden;
                levBtn.Visibility = Visibility.Hidden;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Reserveren nextPage = new Reserveren();
            TheGrid.Children.Clear();
            TheGrid.Children.Add(nextPage);
        }

        private void voorraadBtn_Click(object sender, RoutedEventArgs e)
        {
            voorraadScreen nextPage = new voorraadScreen();
            TheGrid.Children.Clear();
            TheGrid.Children.Add(nextPage);
        }

        private void menuBtn_Click(object sender, RoutedEventArgs e)
        {
            menuScreen nextPage = new menuScreen();
            TheGrid.Children.Clear();
            TheGrid.Children.Add(nextPage);
        }

        private void uitlogBtn_Click(object sender, RoutedEventArgs e)
        {
            LoginScreen nextPage = new LoginScreen();
            TheGrid.Children.Clear();
            TheGrid.Children.Add(nextPage);
        }

        private void beheerBtn_click(object sender, RoutedEventArgs e)
        {
            reserveringBeheerScreen nextPage = new reserveringBeheerScreen();
            TheGrid.Children.Clear();
            TheGrid.Children.Add(nextPage);
        }

        private void klantBtn_Click(object sender, RoutedEventArgs e)
        {
            klantData check = new klantData();
            Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return;
            }

            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            xlWorkSheet.Cells[1, 1] = "Voornaam";
            xlWorkSheet.Cells[1, 2] = "Tussenvoegsel";
            xlWorkSheet.Cells[1, 3] = "Achternaam";
            xlWorkSheet.Cells[1, 4] = "Woonplaats";
            xlWorkSheet.Cells[1, 5] = "Adres";
            xlWorkSheet.Cells[1, 6] = "Huisnummer";
            xlWorkSheet.Cells[1, 7] = "Postcode";
            xlWorkSheet.Cells[1, 8] = "Emailadres";
            xlWorkSheet.Cells[1, 9] = "Telefoonnummer";
            int i = 1;
            foreach (klantData.ExcelData row in check.excel())
            {
                i++;
                xlWorkSheet.Cells[i, 1] = row.voornaam;
                xlWorkSheet.Cells[i, 2] = row.tussenvoegsel;
                xlWorkSheet.Cells[i, 3] = row.achternaam;
                xlWorkSheet.Cells[i, 4] = row.woonplaats;
                xlWorkSheet.Cells[i, 5] = row.adres;
                xlWorkSheet.Cells[i, 6] = row.huisnummer;
                xlWorkSheet.Cells[i, 7] = row.postcode;
                xlWorkSheet.Cells[i, 8] = row.emailadres;
                xlWorkSheet.Cells[i, 9] = row.telefoonnummer;
            }

            xlWorkBook.SaveAs("C:\\Users\\Public\\Documents\\klantenfile.xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();
            MessageBox.Show("Klantenbestand gedownload. U vindt het bestand op de volgende locatie:\nC:\\Users\\Public\\Documents\\klantenfile.xls");
        }

        private void drankBtn_Click(object sender, RoutedEventArgs e)
        {
            drankScreen nextPage = new drankScreen();
            TheGrid.Children.Clear();
            TheGrid.Children.Add(nextPage);
        }

        private void levBtn_Click(object sender, RoutedEventArgs e)
        {
            leveringScreen nextPage = new leveringScreen();
            TheGrid.Children.Clear();
            TheGrid.Children.Add(nextPage);
        }
    }
}

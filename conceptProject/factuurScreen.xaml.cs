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
using Excel = Microsoft.Office.Interop.Excel;

namespace conceptProject
{
    /// <summary>
    /// Interaction logic for factuurScreen.xaml
    /// </summary>
    public partial class factuurScreen : Window
    {
        reserverenData check = new reserverenData();
        public factuurScreen(int resID)
        {
            InitializeComponent();
            decimal totaal = 0;
            foreach(reserverenData.factuur row in check.readFacDB(resID))
            {   
                Menu.Text += row.menu + "\n";
                Aantal.Text += row.aantal + "\n";
                Prijs.Text += "€ " + row.prijs.ToString() + "\n";

                totaal = totaal + (row.prijs * int.Parse(row.aantal));
                helpField.Text = row.help;
            }
            foreach(reserverenData.factuur row in check.factuurReadDrankDB(resID))
            {
                Menu.Text += row.menu + "\n";
                Aantal.Text += row.aantal + "\n";
                Prijs.Text += "€ " + row.prijs.ToString() + "\n";

                totaal = totaal + (row.prijs * int.Parse(row.aantal));
            }
            totaalField.Text = "€ " + totaal.ToString();

            pdfFactuur.Click += (s, e) =>
            {
                decimal facTotaal = 0;
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

                xlWorkSheet.Cells[3, 1] = "Menu";
                xlWorkSheet.Cells[3, 3] = "Aantal";
                xlWorkSheet.Cells[3, 5] = "Prijs";

                int i = 3;
                string help = "";
                foreach (reserverenData.factuur row in check.readFacDB(resID))
                {
                    i++;
                    xlWorkSheet.Cells[i, 1] = row.menu;
                    xlWorkSheet.Cells[i, 3] = row.aantal;
                    xlWorkSheet.Cells[i, 5] = "€" + row.prijs;
                    facTotaal = facTotaal + (row.prijs * int.Parse(row.aantal));
                    help = row.help;
                }
                int j = check.readFacDB(resID).Count;
                foreach (reserverenData.factuur row in check.factuurReadDrankDB(resID))
                {
                    i++;
                    xlWorkSheet.Cells[j, 1] = row.menu;
                    xlWorkSheet.Cells[j, 3] = row.aantal;
                    xlWorkSheet.Cells[j, 5] = "€" + row.prijs;
                    facTotaal = facTotaal + (row.prijs * int.Parse(row.aantal));
                }
                xlWorkSheet.Cells[1, 1] = "Totaal:";
                xlWorkSheet.Cells[1, 2] = "€" + facTotaal;
                xlWorkSheet.Cells[1, 4] = "Geholpen door:";
                xlWorkSheet.Cells[1, 6] = help;
                xlWorkBook.SaveAs("C:\\Users\\Public\\Documents\\factuur.xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();
                Excel.Application app = new Excel.Application();
                Excel.Workbook wkb = app.Workbooks.Open("C:\\Users\\Public\\Documents\\factuur.xls");
                wkb.ExportAsFixedFormat(Excel.XlFixedFormatType.xlTypePDF, "C:\\Users\\Public\\Documents\\factuur.PDF");
                wkb.Close();
                app.Quit();
                MessageBox.Show("Klantenbestand gedownload. U vindt het bestand op de volgende locatie:\nC:\\Users\\Public\\Documents\\factuur.pdf");
            };
        }

        private void pdfFactuur_Click(object sender, RoutedEventArgs e)
        {


        }
    }
}

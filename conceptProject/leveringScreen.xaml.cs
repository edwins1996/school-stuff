using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using System.Diagnostics;

namespace conceptProject
{
    /// <summary>
    /// Interaction logic for leveringScreen.xaml
    /// </summary>
    public partial class leveringScreen : UserControl
    {
        private Regex digitsOnly = new Regex(@"[^\d]");
        public struct suppOrder
        {
            public string artikelNummer;
            public string artikelOmschrijving;
            public string artikelEenheid;
            public string artikelOrder;
            public string artikelGeleverd;
            public string artikelBackorder;

            public suppOrder(string artikel, string omschr, string een, string ord, string gelev, string backO)
            {
                artikelNummer = artikel;
                artikelOmschrijving = omschr;
                artikelEenheid = een;
                artikelOrder = ord;
                artikelGeleverd = gelev;
                artikelBackorder = backO;
            }
        }
        private List<suppOrder> orderList = new List<suppOrder>();
        leveringData levVar = new leveringData();
        voorraadMgmt voorMgmt = new voorraadMgmt();
        public leveringScreen()
        {
            InitializeComponent();
            // Initialiseert bestellijst
            int x = 0;
            foreach (voorraadMgmt.stocknAantal data in levVar.checkMinVoorraad())
            {
                int nodig = (int.Parse(data.minVoorraad) - data.aantal) + 1;
                TextBlock prodNaam = new TextBlock();
                prodNaam.Text = data.omschrijving + "    (voorraad: " + data.aantal + ")";
                prodNaam.Name = "prod" + x;
                prodNaam.Height = 30;
                prodNaam.HorizontalAlignment = HorizontalAlignment.Left;
                prodNaam.VerticalAlignment = VerticalAlignment.Top;
                prodNaam.Margin = new Thickness(0, 30 * x, 0, 0);

                TextBox prodAantal = new TextBox();
                prodAantal.Text = nodig.ToString();
                prodAantal.HorizontalAlignment = HorizontalAlignment.Left;
                prodAantal.VerticalAlignment = VerticalAlignment.Top;
                prodAantal.Height = 25;
                prodAantal.Width = 50;
                prodAantal.Name = data.eenheid;
                prodAantal.Tag = data.artikelnr + " " + data.omschrijving;
                prodAantal.Margin = new Thickness(710, 30 * x, 0, 0);

                TextBlock prodEenheid = new TextBlock();
                prodEenheid.Text = data.eenheid;
                prodEenheid.Name = "prod" + x;
                prodEenheid.HorizontalAlignment = HorizontalAlignment.Left;
                prodEenheid.VerticalAlignment = VerticalAlignment.Top;
                prodEenheid.Margin = new Thickness(765, 30 * x, 0, 0);

                BestelGrid.Children.Add(prodNaam);
                BestelGrid.Children.Add(prodEenheid);
                BestelGrid.Children.Add(prodAantal);
                x++;
            }

            exportBtn.Click += (s, e) =>
            {
                List<TextBox> prodAantal = BestelGrid.Children.OfType<TextBox>().ToList();
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                string date = digitsOnly.Replace(DateTime.Today.ToString("yyyy-MM-dd"), "");
                string filename = "bestellijst" + date + ".pdf";
                dlg.FileName = filename; // Default file name
                dlg.DefaultExt = ".pdf"; // Default file extension
                dlg.Filter = "PDF documents (.pdf)|*.pdf"; // Filter files by extension 

                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    Document document = new Document();
                    Section section = document.AddSection();
                    Paragraph paragraph = section.AddParagraph();
                    FormattedText ft = paragraph.AddFormattedText("Bestellijst", TextFormat.Bold);
                    ft.Font.Size = 26;
                    section.AddParagraph("Gegenereerd door: " + Login.Login.Gebruiker);
                    section.AddParagraph("Datum: " + DateTime.Now.ToString("dd-MM-yyyy H:m"));
                    section.AddParagraph("");
                    foreach (TextBox row in prodAantal)
                    {
                        if(row.Text != "" && digitsOnly.Replace(row.Text, "") != "")
                            section.AddParagraph(row.Tag.ToString() + "\t\t" + row.Text + " " + row.Name);               
                    }

                    PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(false, PdfFontEmbedding.Always);

                    pdfRenderer.Document = document;
                    pdfRenderer.RenderDocument();
                    try
                    {
                        pdfRenderer.PdfDocument.Save(dlg.FileName);
                        Process.Start(dlg.FileName);
                    }
                    catch
                    {
                        MessageBox.Show("Bestand met deze naam bestaat al,\nen is op dit moment geopend.", "Succesvol", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            };
        }

        private void fileSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            //Zorgt ervoor dat de pc doorzocht kan worden voor het gewenste bestand.
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Text|*.txt";
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string fileName = dlg.FileName;
                chosenFile.Text = fileName;
            }
        }

        private void showFile_Click(object sender, RoutedEventArgs e)
        {
            //Schrijft alle benodigde inhoud van de leveringsbon in een listbox zodat er een overzicht van is. Wel wordt dit geselecteerd
            if (chosenFile.Text != "")
            {
                if (File.ReadLines(chosenFile.Text).Skip(0).Take(1).First() == "LEVERINGSBON")
                {
                    fileItems.Items.Clear();
                    orderList.Clear();
                    List<string> fileText = File.ReadLines(chosenFile.Text).Skip(8).Reverse().Skip(6).ToList();
                    foreach (string fileRead in fileText)
                    {
                        string strip = Regex.Replace(fileRead, @"\t+", ",");
                        string[] split = strip.Split(',');
                        suppOrder structVar = new suppOrder(split[0], split[1], split[2], split[3], split[4], split[5]);
                        orderList.Add(structVar);
                    }
                    foreach (suppOrder row in orderList)
                        fileItems.Items.Add("Artikelnr.: " + row.artikelNummer + " / Artikelnaam: " + row.artikelOmschrijving + " / geleverd: " + row.artikelGeleverd);
                }
                else
                {
                    MessageBox.Show("Kies een leveringsbon.", "Fout", MessageBoxButton.OK, MessageBoxImage.Information);
                    chosenFile.Text = "";
                }
            }
            else
                MessageBox.Show("Kies een bestand.", "Fout", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private void fileSave_Click(object sender, RoutedEventArgs e)
        {
            //Inlezen van de leveringsbonnen.
            if (chosenFile.Text != "")
            {
                string dateLine = File.ReadLines(chosenFile.Text).Reverse().Skip(1).Take(1).First();
                dateLine = Regex.Replace(dateLine, @"[^\d]+", "", RegexOptions.Compiled);
                if (levVar.checkDate(dateLine))
                {
                    levVar.insDBdate(dateLine);
                    int i = 0;
                    foreach (suppOrder row in orderList)
                    {
                        i++;
                        if (levVar.checkArtikel(row.artikelNummer, row.artikelOmschrijving))
                        {
                            if (levVar.editVoorraadDB(row.artikelNummer, row.artikelOmschrijving, row.artikelGeleverd))
                            {
                                if (i < 2)
                                    MessageBox.Show("Voorraad succesvol bijgewerkt.", "Succesvol", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                                MessageBox.Show("Voorraad succesvol bijgewerkt.", "Succesvol", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBoxResult result = MessageBox.Show("Er is een fout opgetreden.\n" + row.artikelOmschrijving + " bestaat niet.\nWilt u deze nu aanmaken?", "Annuleren", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                            if (result == MessageBoxResult.Yes)
                            {
                                if (voorMgmt.saveDBprod(row.artikelOmschrijving, row.artikelGeleverd, row.artikelNummer, "5", row.artikelEenheid))
                                {
                                    MessageBox.Show(row.artikelOmschrijving + " succesvol aangemaakt.", "Succesvol", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                    MessageBox.Show("Aanmaken fout gegaan.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else
                                MessageBox.Show("Aanmaken geannuleerd.", "Annuleren", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                else
                    MessageBox.Show("Invoeren geannuleerd.\nOp datum al bestand ingevoerd.", "Annuleren", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
                MessageBox.Show("Kies een bestand.", "Fout", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            //Terug naar de vorige pagina -> het menu
            HeadMenu nextPage = new HeadMenu();
            TheGrid.Children.Clear();
            TheGrid.Children.Add(nextPage);
        }
    }
}

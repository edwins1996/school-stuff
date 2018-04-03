using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace conceptProject
{
    /// <summary>
    /// Log -> Applicatie versie 1.1:    ///
    /// 11/12/2017: Toegevoegd leveringScreen. Toegevoegd txt-files inlezen en juiste gegevens eruit halen.
    /// 12/12/2017: Toegevoegd in leveringsScreen -> bestellijst op basis van minVoorraad -> PDF genereren op basis van ingevulde bestellijst
    /// 13/12/2017: Error ontdekt, als bestand al bestaat en is geopend bij PDF genereren op basis van bestellijst, wordt er een fout gegeven. Dit is verholpen.
    /// 13/12/2017: Reservering annuleren ingebouwd. Aparte CS file gemaakt voor database details. Alleen tonen menu's met meer dan 5 sets op voorraad gemaakt.
    /// 13/12/2017: Melding gemaakt voor bestelling die ervoor zorgt dat de minimumvoorraad van 5 sets per menu wordt bereikt.
    /// 14/12/2017: Beveiliging ingebouwd dat er geen andere textbestanden ingelezen kunnen worden behalve een leveringsbon.
    /// 14/12/2017: Error ontdekt in de reserveringen wijzigen. Drankjes werden niet van voorraad afgehaald en ook niet hersteld. Probleem opgelost.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoginScreen newScherm = new LoginScreen();
            TheGrid.Children.Add(newScherm);
        }
    }
}

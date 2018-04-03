using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace conceptProject.Reserveringsdata
{
    class reserverenData
    {
        private static string connectionString = "Server=" + dbConn.server + ";Database=" + dbConn.database + ";User ID=" + dbConn.UID + ";Password=" + dbConn.pass + ";";

        private List<menuNAantal> prMenus = new List<menuNAantal>();
        private List<int> usedTables = new List<int>();
        public List<menuNAantal> puMenus {
            get { return prMenus; }
            set { prMenus = value; }
        }
        public struct menuNAantal
        {
            public string menuBeschrijving;
            public int menuAantal;
            
            public menuNAantal(string beschrijving, int aantal)
            {
                menuBeschrijving = beschrijving;
                menuAantal = aantal;
            }
        }
        public struct factuur
        {
            public string menu;
            public string aantal;
            public decimal prijs;
            public string help;

            public factuur(string sMenu, string sAantal, decimal sPrijs, string sHelp)
            {
                menu = sMenu;
                aantal = sAantal;
                prijs = sPrijs;
                help = sHelp;
            }
        }
        public void showMenus()
        {
            //haalt alle menu's op uit database
            prMenus.Clear();
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT menuOmschrijving, menuID FROM menubeschrijvingen";
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
                prMenus.Add(new menuNAantal((string)reader["menuOmschrijving"], (int)reader["menuID"]));
            reader.Close();
            mydbCon.Close();
        }
        public bool showMenus2(int id)
        {
            //toont alle menus met juiste hoeveelheid op voorraad
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM menus, voorraad WHERE menus.productID = voorraad.productID AND menus.menuID = " + id + " AND menus.aantal * 5 <= voorraad";
            int query1 = Convert.ToInt32(command.ExecuteScalar());
            command.CommandText = "SELECT COUNT(*) FROM menus, voorraad WHERE menus.productID = voorraad.productID AND menus.menuID = " + id + "";
            int query2 = Convert.ToInt32(command.ExecuteScalar());
            if(query1 < query2)
            {
                mydbCon.Close();
                return false;
            }
            else
            {
                mydbCon.Close();
                return true;
            }
        }
        public int maxResID()
        {
            // Deze functie geeft het maximale reserveringID terug dat als laatste is opgeslagen
            int id = 0;
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT MAX(reserveringID) as id FROM reserveringen";
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
                id = (int)reader["id"];
            reader.Close();
            mydbCon.Close();
            return id;
        }
        
        public List<int> bezetTafels(string datum, List<int> timeSlots)
        {
            // Deze functie geeft een lijst van integers terug van tafels die bezet zijn.
            usedTables.Clear();
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            foreach (int query in timeSlots)
            {
                command.CommandText = "SELECT tafelnummer FROM ((tafelreserveringen INNER JOIN reserveringstijdblokken ON tafelreserveringen.reserveringID = reserveringstijdblokken.reserveringID) INNER JOIN reserveringen ON reserveringen.reserveringID = tafelreserveringen.reserveringID) WHERE datum = '" + DateTime.Parse(datum).ToString("yyyy-MM-dd") + "' AND tijdblok = " + query + "";
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    usedTables.Add((int)reader["tafelnummer"]);
                }
                reader.Close();
            }            
            mydbCon.Close();
            return usedTables;
        }
        public bool insDBtempDate(DateTime datum, List<int> timeslots, List<int> tafelnummers)
        {
            // Deze functie slaat in de tijdelijke tabellen de gegevens op die daarin horen.
            if (tafelnummers.Count != 0)
            {
                MySqlConnection mydbCon = new MySqlConnection(connectionString);
                mydbCon.Open();
                MySqlCommand command = mydbCon.CreateCommand();
                foreach (int timez in timeslots)
                {
                    command.CommandText = "INSERT INTO temp_datum_tijd(datum, tijdslot) VALUES('" + datum.ToString("yyyy-MM-dd") + "', " + timez + ")";
                    try
                    {
                        command.ExecuteNonQuery();                        
                    }
                    catch
                    {
                        mydbCon.Close();
                        return false;
                    }
                }
                foreach (int tablez in tafelnummers)
                {
                    command.CommandText = "INSERT INTO temp_tafels(tafelnummer) VALUES('" + tablez + "')";
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch
                    {
                        mydbCon.Close();
                        return false;
                    }
                }
                mydbCon.Close();
                return true;                
            }
            else
                return false;            
        }
        public DateTime readDBtempDate(DateTime datum)
        {
            // Deze functie leest de datum uit een tijdelijke tabel uit.
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT datum FROM temp_datum_tijd";
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
                datum = (DateTime)reader["datum"];
            reader.Close();
            mydbCon.Close();
            return datum;
        }
        public List<int> readDBtempTables()
        {
            // Deze functie leest de tafelnummers uit de tijdelijke tabel uit
            List<int> tableList = new List<int>();
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT tafelnummer FROM temp_tafels";
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
                tableList.Add((int)reader["tafelnummer"]);
            reader.Close();
            mydbCon.Close();
            return tableList;
        }

        public List<int> readDBtempTimeslots()
        {
            // Deze functie leest de tijdsloten uit de tijdelijke tabel uit
            List<int> timeList = new List<int>();
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT tijdslot FROM temp_datum_tijd";
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
                timeList.Add((int)reader["tijdslot"]);
            reader.Close();
            mydbCon.Close();
            return timeList;
        }
        public void deleteTempdata()
        {
            // Deze functie verwijdert alle gegevens uit de tijdelijke tabellen
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "DELETE FROM temp_datum_tijd";
            command.ExecuteNonQuery();
            command.CommandText = "DELETE FROM temp_tafels";
            command.ExecuteNonQuery();
            mydbCon.Close();
        }
        public void insertDBbest(List<int> aantallen, List<int> menuID)
        {
            //Invoeren van de bestelling van een reservering in de tabel bestellingen en het daarbij ophalen van de aantallen per product adhv de menuIDs
            int id = 0;
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT MAX(bestellingID) as id FROM bestellingen";
            string iRows = command.ExecuteScalar().ToString();
            if (iRows != "")
            {
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                    id = (int)reader["id"];
                reader.Close();

                    int i = 0;
                    id++;
                foreach (int row in aantallen)
                {
                    command.CommandText = "INSERT INTO bestellingen(bestellingID, menuID, aantal) VALUES(" + id + ", " + menuID[i] + ", " + row + ")";
                    command.ExecuteNonQuery();
                    resStockEditneg(menuID[i], row);
                    i++;
                }
                mydbCon.Close();                  
            }
            else
            {
                    int i = 0;
                foreach (int row in aantallen)
                {
                    command.CommandText = "INSERT INTO bestellingen(bestellingID, menuID, aantal) VALUES(1, " + menuID[i] + ", " + row + ")";

                    command.ExecuteNonQuery();
                    i++;
                }
                mydbCon.Close();
            }            
        }
        public void resStockEditneg(int menuID, int aantal)
        {
            //Afschrijven van de voorraad in de database aan de hand van reserveringen.
            List<int> dataAantal = new List<int>();
            List<int> dataProdid = new List<int>();
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT aantal, productID FROM menus WHERE menuID = " + menuID + "";
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                dataAantal.Add((int)reader["aantal"]);
                dataProdid.Add((int)reader["productID"]);
            }
            reader.Close();
            int i = 0;
            foreach(int row in dataAantal)
            {
                int waarde = row * aantal;
                command.CommandText = "UPDATE voorraad SET voorraad = voorraad-" + waarde + " WHERE productID = " + dataProdid[i] + "";
                command.ExecuteNonQuery();
                i++;
            }
            mydbCon.Close();
        }
        public void resStockEditpos(int menuID, int aantal)
        {
            //Herstel van de voorraad in de database aan de hand van reserveringen die geannuleert worden.
            List<int> dataAantal = new List<int>();
            List<int> dataProdid = new List<int>();
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT aantal, productID FROM menus WHERE menuID = " + menuID + "";
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                dataAantal.Add((int)reader["aantal"]);
                dataProdid.Add((int)reader["productID"]);
            }
            reader.Close();
            int i = 0;
            foreach (int row in dataAantal)
            {
                int waarde = row * aantal;
                command.CommandText = "UPDATE voorraad SET voorraad = voorraad+" + waarde + " WHERE productID = " + dataProdid[i] + "";
                command.ExecuteNonQuery();
                i++;
            }
            mydbCon.Close();
        }
        public int readDBmenID(string omschrijving)
        {
            //Haalt menuID op uit database
            int id = 0;
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT menuID FROM menubeschrijvingen WHERE menuOmschrijving = '" + omschrijving + "'";
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
                id = (int)reader["menuID"];
            reader.Close();
            mydbCon.Close();
            return id;
        }
        public bool insertDBreser(string klantID, string datum, string aantalpers)
        {
            // Deze functie slaat een reservering op in de database.
            int id = 0;
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT MAX(bestellingID) as id FROM bestellingen";
            string iRows = command.ExecuteScalar().ToString();
            if (iRows != "")
            {
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                    id = (int)reader["id"];
                reader.Close();
                command.CommandText = "INSERT INTO reserveringen(klantID, bestellingID, datum, aantalPers, gebruikerID) VALUES(" + klantID + ", " + id + ", '" + datum + "', " + aantalpers + ", " + Login.Login.gebruikerID + ")";
                try
                {
                    command.ExecuteNonQuery();
                    mydbCon.Close();
                    return true;
                }
                catch
                {
                    mydbCon.Close();
                    return false;
                }
            }
            else
            {
                mydbCon.Close();
                return false;
            }
        }
        public void insertDBtimetable(string[] tafels, List<string> times)
        {
            // Deze functie slaat gekozen tijdsloten en tafels op van de reserveringen.
            int id = 0;
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT MAX(reserveringID) as id FROM reserveringen";
            string iRows = command.ExecuteScalar().ToString();
            if (iRows != "")
            {
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                    id = (int)reader["id"];
                reader.Close();
                foreach (string tijd in times)
                {
                    command.CommandText = "INSERT INTO reserveringstijdblokken(reserveringID, tijdblok) VALUES(" + id + ", " + tijd + ")";
                    command.ExecuteNonQuery();
                }
                foreach (string tafel in tafels)
                {
                    command.CommandText = "INSERT INTO tafelreserveringen(reserveringID, tafelnummer) VALUES(" + id + ", " + tafel + ")";
                    command.ExecuteNonQuery();
                }
                mydbCon.Close();
            }
            else
            {
                mydbCon.Close();
            }
        }
        public struct reserData
        {
            // Deze struct is bedoelt data in op te slaan.
            public string reserOmschrijving;
            public int reserID;
            public reserData(string omschrijving, int id)
            {
                reserOmschrijving = omschrijving;
                reserID = id;
            }
        }
        public List<reserData> readDBres()
        {
            // Deze functie leest basisgegevens over reservering uit om kort weer te geven.
            List<reserData> data = new List<reserData>();
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT klanten.achternaam, klanten.postcode, klanten.huisnummer, reserveringen.datum, reserveringen.reserveringID FROM reserveringen, klanten WHERE reserveringen.klantID = klanten.klantID AND datum >= '" + DateTime.Today.ToString("yyyy-MM-dd") + "' ORDER BY reserveringen.datum";
            if (command.ExecuteNonQuery() != 0)
            {
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    reserData resVari = new reserData(((DateTime)reader["datum"]).ToString("yyyy-MM-dd") + " | " + (string)reader["achternaam"] + ", " + (string)reader["postcode"] + ", " + ((int)reader["huisnummer"]).ToString(), (int)reader["reserveringID"]);
                    data.Add(resVari);
                }
            return data;
            }
            else
                return data;
        }
        public bool delDBreser(int id)
        {
            //Verwijdert reservering uit database
            int delID = 0;
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT bestellingen.bestellingID FROM reserveringen, bestellingen WHERE reserveringen.bestellingID = bestellingen.bestellingID AND reserveringen.reserveringID = " + id + "";
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
                delID = (int)reader["bestellingID"];
            reader.Close();
            try
            {
                command.CommandText = "SELECT menuID, aantal FROM bestellingen WHERE bestellingID = " + delID + "";
                reader = command.ExecuteReader();
                delDBdrankbest(id);
                while (reader.Read())
                    resStockEditpos((int)reader["menuID"], (int)reader["aantal"]);
                reader.Close();
                command.CommandText = "DELETE FROM bestellingen WHERE bestellingID = " + delID + "; DELETE FROM reserveringen WHERE reserveringID = " + id + "; DELETE FROM reserveringstijdblokken WHERE reserveringID = " + id + "; DELETE FROM tafelreserveringen WHERE reserveringID = " + id + "";
                command.ExecuteNonQuery();
                mydbCon.Close();
                return true;
            }
            catch
            {
                mydbCon.Close();
                return false;
            }
        }
        public List<voorraadMgmt.stocknAantal> readResDB(int resID)
        {
            // Leest gereserveerde menu's en de aantallen daarvan uit op basis van reserveringID
            List<voorraadMgmt.stocknAantal> data = new List<voorraadMgmt.stocknAantal>();
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT aantal, menubeschrijvingen.menuID, menubeschrijvingen.menuOmschrijving FROM bestellingen, reserveringen, menubeschrijvingen WHERE menubeschrijvingen.menuID = bestellingen.menuID AND reserveringen.bestellingID = bestellingen.bestellingID AND reserveringen.reserveringID = " + resID + "";
            if(command.ExecuteNonQuery() != 0)
            {
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    voorraadMgmt.stocknAantal vari = new voorraadMgmt.stocknAantal((string)reader["menuOmschrijving"], (int)reader["aantal"], "", "", (int)reader["menuID"], "");
                    data.Add(vari);
                }
                return data;
            }
            else
                return data;
        }
        public void delBestDB(int resID)
        {
            // Verwijderd een bestelling uit de database
            int bestID = 0, i = 0;
            List<int> menID = new List<int>();
            List<int> aant = new List<int>();
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT bestellingen.bestellingID FROM bestellingen, reserveringen WHERE bestellingen.bestellingID = reserveringen.bestellingID AND reserveringen.reserveringID = " + resID + "";
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
                bestID = (int)reader["bestellingID"];
            reader.Close();
            command.CommandText = "SELECT menuID, aantal FROM bestellingen WHERE bestellingID = " + bestID + "";
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                menID.Add((int)reader["menuID"]);
                aant.Add((int)reader["aantal"]);
            }
            reader.Close();
            foreach(int row in menID)
            {
                resStockEditpos(row, aant[i]);
                i++;
            }
            command.CommandText = "DELETE FROM bestellingen WHERE bestellingID = " + bestID + "";
            command.ExecuteNonQuery();
            mydbCon.Close();
        }
        public void updateBestDB(int resID)
        {
            // Update een bestellingID in de database
            int id = 0;
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT MAX(bestellingID) as id FROM bestellingen";
            if (command.ExecuteNonQuery() != 0)
            {
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                    id = (int)reader["id"];
                reader.Close();
                command.CommandText = "UPDATE reserveringen SET bestellingID = " + id + " WHERE reserveringID = " + resID + "";
                command.ExecuteNonQuery();
            }
            mydbCon.Close();
        }
        public List<factuur> readFacDB(int resID)
        {
            // Leest gegevens uit voor een factuur
            List<factuur> data = new List<factuur>();
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT * FROM reserveringen, bestellingen, menubeschrijvingen, gebruikers WHERE gebruikers.gebruikerID = reserveringen.gebruikerID AND reserveringen.bestellingID = bestellingen.bestellingID AND menubeschrijvingen.menuID= bestellingen.menuID AND reserveringen.reserveringID = " + resID + "";
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                factuur vari = new factuur((string)reader["menuOmschrijving"], ((int)reader["aantal"]).ToString(), (decimal)reader["prijs"], (string)reader["inlognaam"]);
                data.Add(vari);
            }
            return data;
        }
        public List<menuNAantal> readDrankDB(int resID)
        {
            // Leest bestelde dranken uit de database uit voor een factuur
            int bestID = 0;
            List<menuNAantal> data = new List<menuNAantal>();
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT bestellingID FROM reserveringen WHERE reserveringID = " + resID + "";
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
                bestID = (int)reader["bestellingID"];
            reader.Close();
            command.CommandText = "SELECT productOmschrijving, aantal FROM voorraad, drankbestellingen, drankjes WHERE voorraad.productID = drankjes.productID AND drankjes.drankID = drankbestellingen.drankID AND drankbestellingen.bestellingID = " + bestID + "";
            if (command.ExecuteNonQuery() != 0)
            {
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    menuNAantal row = new menuNAantal((string)reader["productOmschrijving"], (int)reader["aantal"]);
                    data.Add(row);
                }
                reader.Close();
                mydbCon.Close();
                return data;
            }
            else
            {
                mydbCon.Close();
                return data;
            }
        }
        public bool insertDBdrankbest(int resID, string prodNaam, string prodAantal)
        {
            // Slaat bestelling van drankjes op in database
            int bestID = 0, drankID = 0, prodID = 0;
            delDBdrankbest(resID);
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT bestellingID, drankID, drankjes.productID  FROM reserveringen, voorraad, drankjes WHERE voorraad.productID = drankjes.productID AND voorraad.productOmschrijving = '" + prodNaam + "' AND reserveringen.reserveringID = " + resID + "";
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                bestID = (int)reader["bestellingID"];
                drankID = (int)reader["drankID"];
                prodID = (int)reader["productID"];
            }
            reader.Close();
            if (checkTrue(drankID, bestID).Count == 0)
            {
                command.CommandText = "INSERT INTO drankbestellingen(bestellingID, drankID, aantal) VALUES(" + bestID + ", " + drankID + ", " + prodAantal + ")";
                try
                {
                    command.ExecuteNonQuery();
                    command.CommandText = "UPDATE voorraad SET voorraad = voorraad-" + prodAantal + " WHERE productID = " + prodID + "";
                    command.ExecuteNonQuery();
                    mydbCon.Close();
                    return true;
                }
                catch
                {
                    mydbCon.Close();
                    return false;
                }
            }
            else
                return false;
        }
        public List<int> checkTrue(int prodID, int bestID)
        {
            // Geeft drankID terug 
            List<int> data = new List<int>();
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT * FROM drankbestellingen WHERE drankID = " + prodID + " AND bestellingID = " + bestID + "";
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
                data.Add((int)reader["drankID"]);
            reader.Close();
            return data;
        }
        public void delDBdrankbest(int resID)
        {
            // Verwijdert drankje uit database op basis van reserveringID
            int id = 0;
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT bestellingID FROM reserveringen WHERE reserveringID = " + resID + "";
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
                id = (int)reader["bestellingID"];
            reader.Close();
            restoreDrankBest(id);
            command.CommandText = "DELETE FROM drankbestellingen WHERE bestellingID = " + id + "";
            command.ExecuteNonQuery();
            mydbCon.Close();
        }
        public void restoreDrankBest(int bestID)
        {
            List<int> prodID = new List<int>();
            List<int> aant = new List<int>();
            int i = 0;
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT drankbestellingen.drankID, aantal, productID FROM drankbestellingen, drankjes WHERE drankjes.drankID = drankbestellingen.drankID AND bestellingID = " + bestID + "";
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                prodID.Add((int)reader["productID"]);
                aant.Add((int)reader["aantal"]);
            }
            reader.Close();
            foreach(int row in prodID)
            {
                command.CommandText = "UPDATE voorraad SET voorraad = voorraad+" + aant[i] + "";
                command.ExecuteNonQuery();
                i++;
            }
            mydbCon.Close();
        }
        public List<factuur> factuurReadDrankDB(int resID)
        {
            // Leest uit drankjes database uit voor factuur
            List<factuur> data = new List<factuur>();
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT voorraad.productOmschrijving, drankbestellingen.aantal, drankjes.prijs FROM reserveringen, drankbestellingen, voorraad, drankjes WHERE drankbestellingen.dranKID = drankjes.drankID AND drankjes.productID = voorraad.productID AND reserveringen.bestellingID = drankbestellingen.bestellingID AND reserveringen.reserveringID = " + resID + "";
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                factuur vari = new factuur((string)reader["productOmschrijving"], ((int)reader["aantal"]).ToString(), (decimal)reader["prijs"], "");
                data.Add(vari);
            }
            return data;
        }
    }
}

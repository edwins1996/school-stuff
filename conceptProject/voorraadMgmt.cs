using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace conceptProject
{
    class voorraadMgmt
    {
        private static string connectionString = "Server=" + dbConn.server + ";Database=" + dbConn.database + ";User ID=" + dbConn.UID + ";Password=" + dbConn.pass + ";";
        public struct stocknAantal
        {
            // In deze struct is het mogelijk alle gegevens van een product in één variabele op te slaan
            public string omschrijving;
            public int aantal;
            public string minVoorraad;
            public string eenheid;
            public int id;
            public string artikelnr;

            public stocknAantal(string prodOmschrijving, int prodAantal, string minvoor, string eenh,int ID, string artnr)
            {
                omschrijving = prodOmschrijving;
                aantal = prodAantal;
                minVoorraad = minvoor;
                eenheid = eenh;
                id = ID;
                artikelnr = artnr;
            }
        }
        public List<stocknAantal> readDBstock()
        {
            // Deze functie haalt alle producten uit de voorraad tabel op.
            List<stocknAantal> data = new List<stocknAantal>();
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT productOmschrijving, voorraad, productID, minVoorraad, eenheid, artikelNummer FROM voorraad";
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                
                while (reader.Read())
                {
                    stocknAantal dbData = new stocknAantal((string)reader["productOmschrijving"], (int)reader["voorraad"], ((int)reader["minVoorraad"]).ToString(), (string)reader["eenheid"], (int)reader["productID"], (string)reader["artikelNummer"]);
                    data.Add(dbData);
                }
                reader.Close();
                mydbCon.Close();
                return data;
            }
            else {
                reader.Close();
                mydbCon.Close();
                return data;
            }
        }
        public bool saveDBprod(string prodNaam, string prodAantal, string artnr, string minvoor, string eenheid)
        {
            //Opslaan van de nieuwe producten in de database
            bool data = true;
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT * FROM voorraad WHERE productOmschrijving = '" + prodNaam + "' OR artikelNummer = '" + artnr + "'";
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
                data = false;                
            else
            {
                reader.Close();
                command.CommandText = "INSERT INTO voorraad(productOmschrijving, voorraad, artikelNummer, minVoorraad, eenheid) VALUES('" + prodNaam + "', " + prodAantal + ", '" + artnr + "', " + minvoor + ", '" + eenheid + "')";
                try
                {
                    command.ExecuteNonQuery();
                    data = true;
                }
                catch
                {
                    data = false;
                }
            }
            reader.Close();
            mydbCon.Close();
            return data;     
        }
        public bool editDBprod(List<stocknAantal> prod)
        {
            // Deze functie wijzigt een product in de database
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            foreach (stocknAantal row in prod)
            {
                command.CommandText = "UPDATE voorraad SET productOmschrijving = '" + row.omschrijving + "', voorraad = " + row.aantal + " WHERE productID = " + row.id + "";
                command.ExecuteNonQuery();
            }
            mydbCon.Close();
            return true;           
        }
        public bool deleteDBprod(int id)
        {
            //Verwijderd product
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT * FROM menus WHERE productID = " + id + "; SELECT * FROM drankjes WHERE productID = " + id + "";
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Close();
                return false;
            }
            else {
                reader.Close();
                command.CommandText = "DELETE FROM voorraad WHERE productID = '" + id + "'";
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
        }
        public int readDBprodAant(int menuID, int prodID)
        {
            // Deze functie geeft een aantal terug op basis van menuID en productID 
            int aantal = 0;
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT aantal FROM menus WHERE menuID = " + menuID + " AND productID = " + prodID + "";
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
                aantal = (int)reader["aantal"];
            reader.Close();
            mydbCon.Close();
            return aantal;
        }
    }
}

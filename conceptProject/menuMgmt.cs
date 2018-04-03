using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace conceptProject
{
    class menuMgmt
    {
        private static string connectionString = "Server=" + dbConn.server + ";Database=" + dbConn.database + ";User ID=" + dbConn.UID + ";Password=" + dbConn.pass + ";";
        public struct menuStruct
        {
            // Deze struct is bedoelt om de hoofdgegevens van een menu op een gestructureerde wijze in variabele te kunnen zetten.
            public string menuBeschrijving;
            public int menuID;
            public string prijs;
            public menuStruct(string beschrijving, int id, string price)
            {
                menuBeschrijving = beschrijving;
                menuID = id;
                prijs = price;
            }
        }
        public List<menuStruct> readDBmenus()
        {
            // In deze functie worden met behulp van de menuStruct de belangrijkste gegevens van de menus uitgelezen
            List<menuStruct> data = new List<menuStruct>();
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT menuOmschrijving, menuID, prijs FROM menubeschrijvingen";
            string iRows = command.ExecuteScalar().ToString();
            if (iRows != "")
            {
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    menuStruct dbData = new menuStruct((string)reader["menuOmschrijving"], (int)reader["menuID"], ((decimal)reader["prijs"]).ToString());
                    data.Add(dbData);
                }
                reader.Close();
                mydbCon.Close();
                return data;
            }
            else {
                mydbCon.Close();
                return data;
            }
        }
        public bool delDBmenus(int id)
        {
            // In deze functie worden menus verwijderd
            bool data = true;
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT bestellingID FROM bestellingen WHERE menuID = " + id + "";
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
                data = false;
            else
            {
                reader.Close();
                command.CommandText = "DELETE FROM menubeschrijvingen WHERE menuID = " + id + ";DELETE FROM menus WHERE menuID = " + id + "";
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
        public bool addDBmenus(string naam, string prijs)
        {
            // In deze functie worden menus aangemaakt
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            string newPrijs = prijs.Replace(",", ".");
            command.CommandText = "INSERT INTO menubeschrijvingen(menuOmschrijving, prijs) VALUES('" + naam + "', '" + newPrijs + "')";
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
        public bool addDBingr(int menuID, int prodID, int aantal)
        {
            // In deze functie worden er ingrediënten aan een menu toegevoegd.
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "INSERT INTO menus(menuID, productID, aantal) VALUES(" + menuID + ", " + prodID + ", " + aantal + ")";
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
        public int lastDBid()
        {
            // Deze functie haalt het laatst aangemaakte menuID op
            int id = 0;
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT MAX(menuID) as id FROM menubeschrijvingen";
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
                id = (int)reader["id"];
            reader.Close();
            mydbCon.Close();
            return id;
        }
        public int prodDBid(string naam)
        {
            // Deze functie haalt het productID op aan de hand van een productomschrijving
            int id = 0;
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT productID FROM voorraad WHERE productOmschrijving = '" + naam + "'";
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
                id = (int)reader["productID"];
            reader.Close();
            mydbCon.Close();
            return id;
        }
        public bool saveDBmenuN(int id, string naam, string prijs)
        {
            // Deze functie wijzigt de hoofd gegevens van een menu
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            string newPrijs = prijs.Replace(",", ".");
            command.CommandText = "UPDATE menubeschrijvingen SET menuOmschrijving = '" + naam + "', prijs = '" + newPrijs + "' WHERE menuID = " + id + "";
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
        public void editDelDB(int id)
        {
            // Deze functie verwijderd een menu uit de database behoort bij het wijzigen van een menu
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "DELETE FROM menus WHERE menuID = " + id + "";
            command.ExecuteNonQuery();
            mydbCon.Close();
        }
    }
}

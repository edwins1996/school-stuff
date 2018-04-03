using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text.RegularExpressions;

namespace conceptProject
{
    class drankMgmt
    {
        private static string connectionString = "Server=" + dbConn.server + ";Database=" + dbConn.database + ";User ID=" + dbConn.UID + ";Password=" + dbConn.pass + ";";

        public bool drankDBadd(string product, string prijs)
        {
            // Met deze functie worden de drankjes die aangemaakt worden in de database opgeslagen
            int id = 0;
            bool data = true;
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT productID FROM voorraad WHERE productOmschrijving = '" + product + "'";
            if(command.ExecuteNonQuery() != 0)
            {
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                    id = (int)reader["productID"];
                reader.Close();
            }
            command.CommandText = "SELECT * FROM drankjes WHERE productID = " + id + "";
            MySqlDataReader reader1 = command.ExecuteReader();
            if (reader1.HasRows)
            {
                data = false;
            }
            else
            {
                reader1.Close();
                string newPrijs = prijs.Replace(",", ".");
                command.CommandText = "INSERT INTO drankjes(productID, prijs) VALUES(" + id + ", '" + newPrijs + "')";
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
            reader1.Close();
            mydbCon.Close();
            return data;
        }
        public List<string> readDBdrank()
        {
            // Met deze functie worden alle drankjes uit de database uitgelezen
            List<string> data = new List<string>();
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT productOmschrijving FROM voorraad, drankjes WHERE voorraad.productID = drankjes.productID";
            if (command.ExecuteNonQuery() != 0)
            {
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    data.Add((string)reader["productOmschrijving"]);
                reader.Close();
                mydbCon.Close();
                return data;
            }
            else {
                mydbCon.Close();
                return data;
            }
        }
        public decimal readDBdrankEdit(string drankNaam)
        {
            // In deze functie wordt de prijs van een drankje op basis van de omschrijving van een drankje uit de database opgehaald.
            decimal data = 0;
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT prijs FROM voorraad, drankjes WHERE voorraad.productID = drankjes.productID AND voorraad.productOmschrijving = '" + drankNaam + "'";
            string iRows = command.ExecuteScalar().ToString();
            if (iRows != "")
            {
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                    data = (decimal)reader["prijs"];
                reader.Close();
            }
            return data;
        }
        public bool editDBdrank(string drankNaam, string newDranknaam, string prijs)
        {
            // In deze functie wordt er een drankje gewijzigd.
            int id = 0;
            bool data = true;
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT * FROM voorraad WHERE productOmschrijving = '" + newDranknaam + "'";
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                data = false;
            }
            else
            {
                reader.Close();
                command.CommandText = "SELECT productID FROM voorraad WHERE productOmschrijving = '" + drankNaam + "'";
                if (command.ExecuteNonQuery() != 0)
                {
                    reader = command.ExecuteReader();
                    if (reader.Read())
                        id = (int)reader["productID"];
                    reader.Close();
                }
                string newPrijs = prijs.Replace(",", ".");
                command.CommandText = "UPDATE drankjes, voorraad SET drankjes.prijs = '" + newPrijs + "', voorraad.productOmschrijving = '" + newDranknaam + "' WHERE voorraad.productID = " + id + "";
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
        public bool delDBdrank(string drankNaam)
        {
            // In deze functie wordt er een drankje uit de tabel drankjes verwijderd. NIET uit de tabel voorraad
            int id = 0;
            bool data = true;
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT productID FROM voorraad WHERE productOmschrijving = '" + drankNaam + "'";            
            if (command.ExecuteNonQuery() != 0)
            {
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                    id = (int)reader["productID"];
                reader.Close();
            }
            command.CommandText = "SELECT bestellingID FROM drankbestellingen, drankjes WHERE drankbestellingen.drankID = drankjes.drankID AND drankjes.productID = " + id + "";
            MySqlDataReader reader1 = command.ExecuteReader();
            if (reader1.HasRows)
                data = false;
            else
            {
                reader1.Close();
                command.CommandText = "DELETE FROM drankjes WHERE productID = " + id + "";
                if (command.ExecuteNonQuery() != 0)
                    data = true;
                else
                    data = false;
            }
            reader1.Close();
            mydbCon.Close();
            return data;
        }
    }
}

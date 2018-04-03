using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace conceptProject
{
    class leveringData
    {
        private static string connectionString = "Server=" + dbConn.server + ";Database=" + dbConn.database + ";User ID=" + dbConn.UID + ";Password=" + dbConn.pass + ";";

        public bool checkDate(string datum)
        {
            //Controleert of de datum van de leveringsbon al bestaat
            bool check = true;
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT datum FROM leveringen WHERE datum = '" + datum + "'";
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
                check = false;
            else
                check = true;
            reader.Close();
            mydbCon.Close();
            return check;
        }
        public bool checkArtikel(string artnr, string prodOm)
        {
            //Controleert of de leveringsproducten al bestaan.
            bool check = true;
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT * FROM voorraad WHERE artikelNummer = '" + artnr + "' OR productOmschrijving = '" + prodOm + "'";
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
                check = true;
            else
                check = false;
            reader.Close();
            mydbCon.Close();
            return check;
        }
        public void insDBdate(string datum)
        {
            //Slaat datum op in DB
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "INSERT INTO leveringen(datum) VALUES('" + datum + "')";
            command.ExecuteNonQuery();
            mydbCon.Close();
        }
        public bool editVoorraadDB(string artnr, string artomschrijving, string geleverd)
        {
            //Telt geleverde artikelen op bij bestaande voorraad en slaat deze op in de DB
            int huidigeVoorraad = 0, prodID = 0;
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT voorraad, productID FROM voorraad WHERE artikelNummer = '" + artnr + "' OR productOmschrijving = '" + artomschrijving + "'";
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                huidigeVoorraad = (int)reader["voorraad"];
                prodID = (int)reader["productID"];
            }
            reader.Close();
            int nieuweVoorraad = int.Parse(geleverd) + huidigeVoorraad;
            command.CommandText = "UPDATE voorraad SET voorraad = " + nieuweVoorraad + " WHERE productID = " + prodID + "";
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
        public List<voorraadMgmt.stocknAantal> checkMinVoorraad()
        {
            //SELECT * FROM voorraad WHERE voorraad <= minVoorraad
            List<voorraadMgmt.stocknAantal> data = new List<voorraadMgmt.stocknAantal>();
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT * FROM voorraad WHERE voorraad <= minVoorraad";
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                voorraadMgmt.stocknAantal readData = new voorraadMgmt.stocknAantal((string)reader["productOmschrijving"], (int)reader["voorraad"], ((int)reader["minVoorraad"]).ToString(), (string)reader["eenheid"], (int)reader["productID"], (string)reader["artikelNummer"]);
                data.Add(readData);
            }
            return data;
        }
    }
}

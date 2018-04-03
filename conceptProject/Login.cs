using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace conceptProject.Login
{
    /// <summary>
    /// Deze class is uitsluitend voor het inloggen in de applicatie en werkt samen met LoginScreen.xaml.cs
    /// </summary>
    class Login
    {
        private static string connectionString = "Server=" + dbConn.server + ";Database=" + dbConn.database + ";User ID=" + dbConn.UID + ";Password=" + dbConn.pass + ";";

        public static string Gebruiker;
        public static string Functie;
        public static int gebruikerID;

        private string strName;
        private string strPass;
        private bool loginStatus = false;

        public string inlogName {
            get { return strName; }
            set { strName = value; }
        }
        public string inlogPass
        {
            get
            {
                return strPass;
            }
            set
            {
                strPass = value;
            }
        }

        public bool userLogin()
        {
            //Controleert logingegevens en slaat de belangrijkste gegevens op in variabelen
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            strName = strName.Replace('\'', '"');
            strPass = strPass.Replace('\'', '"');
            command.CommandText = "SELECT * FROM gebruikers, functies WHERE gebruikers.functieID = functies.functieID AND gebruikers.inlognaam = '" + strName + "' AND gebruikers.wachtwoord = '" + strPass + "'";

            int iRows = Convert.ToInt32(command.ExecuteScalar());

            if (iRows <= 0)
                loginStatus = false;
            else
            {
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Gebruiker = (string)reader["inlognaam"];
                    Functie = (string)reader["functieOmschrijving"];
                    gebruikerID = (int)reader["gebruikerID"];
                }
                loginStatus = true;
            }

            mydbCon.Close();

            return loginStatus;
        }
    }
}

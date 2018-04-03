using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text.RegularExpressions;

namespace conceptProject.Klant
{
    class klantData
    {
        private static string connectionString = "Server=" + dbConn.server + ";Database=" + dbConn.database + ";User ID=" + dbConn.UID + ";Password=" + dbConn.pass + ";";

        /// <summary>
        /// Klantgegevens
        /// </summary>
        private string klVoornaam, klTussenvoegsel, klAchternaam, klWoonplaats, klAdres, klPostcode, klHuisnummer, klEmail, klTelefoon;
        private int klID;

        public int klantID { get { return klID; } set { klID = value; } }
        public string klantVoornaam { get { return klVoornaam; } set { klVoornaam = value; } }
        public string klantTussenvoegsel { get { return klTussenvoegsel; } set { klTussenvoegsel = value; } }
        public string klantAchternaam { get { return klAchternaam; } set { klAchternaam = value; } }
        public string klantWoonplaats { get { return klWoonplaats; } set { klWoonplaats = value; } }
        public string klantAdres { get { return klAdres; } set { klAdres = value; } }
        public string klantPostcode { get { return klPostcode; } set { klPostcode = value; } }
        public string klantHuisnummer { get { return klHuisnummer; } set { klHuisnummer = value; } }
        public string klantEmail { get { return klEmail; } set { klEmail = value; } }
        public string klantTelefoon { get { return klTelefoon; } set { klTelefoon = value; } }
        /// <summary>
        /// Einde klantgegevens
        /// </summary>


        private List<string> customData = new List<string>();

        public List<string> dataCustom { get { return customData; } set { value = customData; } }
        public klantData()
        {

        }

        public void dbCustomer(string strInvoer)
        {
            // In deze functie wordt een array die eerder in deze file is aangemaakt gevuld met informatie over klanten
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT * FROM klanten WHERE achternaam LIKE '" + strInvoer + "%' OR postcode LIKE '" + strInvoer + "%'";

            int iRows = Convert.ToInt32(command.ExecuteScalar());

            if (iRows != 0)
            {
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if((string)reader["tussenvoegsel"] == "")
                        customData.Add((string)reader["achternaam"] + ", " + (string)reader["postcode"] + ", " + Convert.ToString((int)reader["huisnummer"]));
                    else
                        customData.Add((string)reader["tussenvoegsel"] + " " + (string)reader["achternaam"] + ", " + (string)reader["postcode"] + ", " + Convert.ToString((int)reader["huisnummer"]));
                }
                reader.Close();
            }
            mydbCon.Close();
        }
        public void dbCdata(string naamPostcode)
        {
            // In deze functie worden alle gegevens van klanten in variabelen gezet die te benaderen zijn vanuit alle projectfiles.
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            string[] nameSplit = naamPostcode.Split(',');
            command.CommandText = "SELECT * FROM klanten, woonplaatsen WHERE CONCAT(tussenvoegsel, ' ', achternaam) LIKE  '%" + nameSplit[0] + "%' AND postcode = '" + nameSplit[1].Replace(" ", string.Empty) + "' AND klanten.woonplaatsID = woonplaatsen.woonplaatsID";

            int iRows = Convert.ToInt32(command.ExecuteScalar());

            if(iRows != 0)
            {
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    klID = (int)reader["klantID"];
                    klVoornaam = (string)reader["voornaam"];
                    klTussenvoegsel = (string)reader["tussenvoegsel"];
                    klAchternaam = (string)reader["achternaam"];
                    klWoonplaats = (string)reader["plaatsnaam"];
                    klAdres = (string)reader["adres"];
                    klPostcode = (string)reader["postcode"];
                    klHuisnummer = Convert.ToString((int)reader["huisnummer"]);
                    klTelefoon = (string)reader["telefoonnummer"];
                    klEmail = (string)reader["emailadres"];
                }
                reader.Close();
            }

            mydbCon.Close();
        }
        public bool dbCSave()
        {
            // In deze functie wordt er een nieuwe klant aangemaakt en opgeslagen in de database.
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT * FROM klanten WHERE postcode = '" + rgx.Replace(klPostcode, "") + "' AND huisnummer = '" + klHuisnummer + "'";

            int iRows = Convert.ToInt32(command.ExecuteScalar());

            if (iRows != 0)
                return false;
            else
            {
                
                command = mydbCon.CreateCommand();
                if (klAchternaam != "" && klPostcode != "" && klHuisnummer != "" && klTelefoon != "")
                {
                    command.CommandText = "INSERT INTO klanten(voornaam, tussenvoegsel, achternaam, woonplaatsID, adres, huisnummer, postcode, emailadres, telefoonnummer) VALUES('" + klVoornaam + "', '" + klTussenvoegsel + "', '" + klAchternaam + "', " + dbCWPid() + ", '" + klAdres + "', " + klHuisnummer + ", '" + rgx.Replace(klPostcode, "") + "', '" + klEmail + "', '" + klTelefoon + "')";
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
                    return false;
            }
        }
        private int dbCWPid()
        {
            // Deze functie geeft een ID weer van elke woonplaats die in de database staat opgeslagen.
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT woonplaatsID FROM woonplaatsen WHERE plaatsnaam = '" + klWoonplaats + "'";

            int iRows = Convert.ToInt32(command.ExecuteScalar());

            if (iRows != 0)
            {
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                    return (int)reader["woonplaatsID"];
                reader.Close();
            }
            else
                return 0;

            mydbCon.Close();
            return 0;
        }
        public struct ExcelData
        {
            //Deze struct is gebruikt voor het invoeren van de klantgegevens voor het excel klantenbestand.
            public string voornaam;
            public string tussenvoegsel;
            public string achternaam;
            public string woonplaats;
            public string adres;
            public string huisnummer;
            public string postcode;
            public string emailadres;
            public string telefoonnummer;
            
            public ExcelData(string a, string b, string c, string d, string e, string f, string g, string h, string i)
            {
                voornaam = a;
                tussenvoegsel = b;
                achternaam = c;
                woonplaats = d;
                adres = e;
                huisnummer = f;
                postcode = g;
                emailadres = h;
                telefoonnummer = i;
            }
        }
        public List<ExcelData> excel()
        {
            // In deze functie wordt alle klantendata zo teruggegeven dat dit meteen uitgelezen kan worden in een excelfile
            List<ExcelData> data = new List<ExcelData>();
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT * FROM klanten , woonplaatsen WHERE klanten.woonplaatsID = woonplaatsen.woonplaatsID";
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                ExcelData vari = new ExcelData((string)reader["voornaam"], (string)reader["tussenvoegsel"], (string)reader["achternaam"], (string)reader["plaatsnaam"], (string)reader["adres"], Convert.ToString((int)reader["huisnummer"]), (string)reader["postcode"], (string)reader["emailadres"], (string)reader["telefoonnummer"]);
                data.Add(vari);
            }
            return data;
        }
        public List<string> customCities()
        {
            // In deze functie worden alle plaatsen opgehaald uit de database
            List<string> data = new List<string>();
            MySqlConnection mydbCon = new MySqlConnection(connectionString);
            mydbCon.Open();
            MySqlCommand command = mydbCon.CreateCommand();
            command.CommandText = "SELECT plaatsnaam FROM woonplaatsen";
            if (command.ExecuteNonQuery() != 0)
            {
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    data.Add((string)reader["plaatsnaam"]);
                reader.Close();
                mydbCon.Close();
                return data;
            }
            else {
                mydbCon.Close();
                return data;
            }
        }
    }
}

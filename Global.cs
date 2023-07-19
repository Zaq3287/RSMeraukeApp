using MySqlConnector;
using RSMerauke.Pages;

namespace RSMerauke
{
    public class Global
    {
        // -------------------------------------------------------------
        // -------------------------------------------------------------
        // constanta
        // -------------------------------------------------------------
        // -------------------------------------------------------------
        public const string strTitle = "RS Merauke";
        public const string strVersion = "1.0.0";
        public const string strWeb = "www.zaqstore.com";
        public const bool isDev = true;

        public const string conStrSep = "#~#";



        // -------------------------------------------------------------
        // -------------------------------------------------------------
        // fields
        // -------------------------------------------------------------
        // -------------------------------------------------------------
        public static string strDBName = "db_rsmerauke";

        public static pgDataDoctorNurseVM pgData;
        public static pgDataRuleVM pgRule;

        // -------------------------------------------------------------
        // -------------------------------------------------------------
        // properties
        // -------------------------------------------------------------
        // -------------------------------------------------------------



        // -------------------------------------------------------------
        // -------------------------------------------------------------
        // function
        // -------------------------------------------------------------
        // -------------------------------------------------------------
        public async static void showMessage(string strMessage)
        {
            await App.Current.MainPage.DisplayAlert(strTitle, strMessage, "OK");
        }

        public async static void errorMessage(string strMessage)
        {
            if (isDev)
            {
                await App.Current.MainPage.DisplayAlert(strTitle, strMessage, "OK");
            }
        }

        public static MySqlConnectionStringBuilder getConString(string dbName = "")
        {
            if (dbName == "") dbName = strDBName;

            MySqlConnectionStringBuilder conString = new MySqlConnectionStringBuilder();

            conString.Server = "";
            conString.UserID = "";
            conString.Password = "";
            conString.Database = "";
            conString.Port = 3306;
            conString.CharacterSet = "utf8mb4";
            conString.ConnectionTimeout = 15;
            conString.SslMode = MySqlSslMode.None;

            return conString;
        }


        public static bool recordExists(string tableName)
        {
            //get mysql connection string
            MySqlConnectionStringBuilder conString = getConString();

            try
            {
                // open a connection asynchronously
                var connection = new MySqlConnection(conString.ConnectionString);
                connection.Open();

                //count
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Count(*) AS total FROM " + tableName;
                var total = Convert.ToInt32(command.ExecuteScalar());

                connection.Close();

                return total > 0 ? true : false;
            }
            catch (Exception ex)
            {
                errorMessage(ex.Message);
                return false;
            }
        }
    }
}

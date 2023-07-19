using MySqlConnector;
using System.Windows.Input;

namespace RSMerauke.Pages
{
    public class pgEditRuleVM : BindProperty
    {
        // -------------------------------------------------------------
        // -------------------------------------------------------------
        // constanta
        // -------------------------------------------------------------
        // -------------------------------------------------------------


        // -------------------------------------------------------------
        // -------------------------------------------------------------
        // fields
        // -------------------------------------------------------------
        // -------------------------------------------------------------
        private string _strTitle;
        private int _intID;
        private string _strDetail = "";
        private string _strCaption;
        private string _strTable;
        private string _strMode;
        private string _strButton;
        private int _data = 0;
        private bool _bolEdit;


        // -------------------------------------------------------------
        // -------------------------------------------------------------
        // properties
        // -------------------------------------------------------------
        // -------------------------------------------------------------
        public ICommand comAdd { get; set; }

        public string strTitle { get { return _strTitle; } set { _strTitle = value; OnPropertyChanged("strTitle"); } }
        public string strButton { get { return _strButton; } set { _strButton = value; OnPropertyChanged("strButton"); } }
        public int intID { get { return _intID; } set { _intID = value; OnPropertyChanged("intID"); } }
        public string strDetail { get { return _strDetail; } set { _strDetail = value; OnPropertyChanged("strDetail"); } }
        public bool bolEdit { get { return _bolEdit; } set { _bolEdit = value; OnPropertyChanged("bolEdit"); } }


        // -------------------------------------------------------------
        // -------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------
        // -------------------------------------------------------------
        public pgEditRuleVM()
        {
            initCommands();
            initList();

            strTitle = "Tambah tata tertib";
            strButton = "Tambah";

            //get default number
            intID = getNumber();
        }

        public pgEditRuleVM(int data) : this()
        {
            strTitle = "Edit tata tertib";
            _data = data;

            getData(data);

            strButton = "Simpan";
        }


        // -------------------------------------------------------------
        // -------------------------------------------------------------
        // Methods
        // -------------------------------------------------------------
        // -------------------------------------------------------------
        private void initCommands()
        {
            comAdd = new Command(doAdd);
        }

        private void initList()
        {

        }

        private int getNumber()
        {
            int result = 1;

            //get mysql connection string
            MySqlConnectionStringBuilder conString = Global.getConString();

            try
            {
                using (MySqlConnection sqlConnection = new MySqlConnection(conString.ConnectionString))
                {
                    using (var cmd = sqlConnection.CreateCommand())
                    {
                        sqlConnection.Open();

                        string strSQL = "SELECT MAX(ID) AS x FROM tbl_rule";
                        cmd.CommandText = strSQL;
                        MySqlDataReader sqlReader = cmd.ExecuteReader();

                        while (sqlReader.Read())
                        {
                            result = sqlReader.GetInt32(0) + 1;
                        }

                        sqlReader.Close();
                        sqlConnection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                return 1;
            }

            return result;
        }

        private void getData(int data)
        {
            //get mysql connection string
            MySqlConnectionStringBuilder conString = Global.getConString();

            try
            {
                using (MySqlConnection sqlConnection = new MySqlConnection(conString.ConnectionString))
                {
                    using (var cmd = sqlConnection.CreateCommand())
                    {
                        sqlConnection.Open();

                        string strSQL = "SELECT ID, Detail FROM tbl_rule WHERE ID = " + data + "";
                        cmd.CommandText = strSQL;
                        MySqlDataReader sqlReader = cmd.ExecuteReader();

                        while (sqlReader.Read())
                        {
                            intID = sqlReader.GetInt32(0);
                            strDetail = sqlReader.GetString(1);
                        }

                        sqlReader.Close();
                        sqlConnection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Global.showMessage(ex.Message);
            }
        }

        private async void doAdd(object sender)
        {
            if (intID > 0)
            {
                if (strDetail.Length > 0)
                {
                    if (_data > 0)
                    {
                        string strSQL = "tbl_rule WHERE ID = " + intID + " AND ID <> " + _data;

                        if (Global.recordExists(strSQL))
                        {
                            Global.showMessage("No urut tersebut sudah digunakan, silahkan ganti dengan no lainnya!");
                            return;
                        }

                        //get mysql connection string
                        MySqlConnectionStringBuilder conString = Global.getConString();

                        try
                        {
                            // open a connection asynchronously
                            var connection = new MySqlConnection(conString.ConnectionString);
                            connection.Open();

                            //get status
                            var command = connection.CreateCommand();
                            command.CommandText = "UPDATE tbl_rule SET ID = " + intID + ", Detail = '" + strDetail + "' WHERE ID = " + _data + "";
                            command.ExecuteScalar();

                            connection.Close();

                            Global.showMessage("Data berhasil diubah!");
                            Global.pgRule.getData();
                            await App.Current.MainPage.Navigation.PopAsync();
                        }
                        catch (Exception ex)
                        {
                            Global.errorMessage(ex.Message);
                        }
                    }
                    else //add new
                    {
                        string strSQL = "tbl_rule WHERE ID = " + intID + "";

                        if (Global.recordExists(strSQL))
                        {
                            Global.showMessage("No urut tersebut sudah digunakan, silahkan ganti dengan no lainnya!");
                            return;
                        }

                        //get mysql connection string
                        MySqlConnectionStringBuilder conString = Global.getConString();

                        try
                        {
                            // open a connection asynchronously
                            var connection = new MySqlConnection(conString.ConnectionString);
                            connection.Open();

                            //get status
                            var command = connection.CreateCommand();
                            command.CommandText = "INSERT INTO tbl_rule (ID, Detail) VALUES (" + intID + ", '" + strDetail + "')";
                            command.ExecuteScalar();

                            connection.Close();

                            Global.showMessage("Data berhasil ditambahkan!");
                            Global.pgRule.getData();
                            await App.Current.MainPage.Navigation.PopAsync();
                        }
                        catch (Exception ex)
                        {
                            Global.errorMessage(ex.Message);
                        }
                    }
                }
                else
                {
                    Global.showMessage("Mohon untuk mengisi detail dahulu!");
                }
            }
            else
            {
                Global.showMessage("Mohon untuk mengisi no lebih dari 0");
            }
        }
            
    }
}

using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RSMerauke.Pages
{
    public class pgEditDoctorNurseVM : BindProperty
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
        private string _strName = "";
        private string _strPhone = "";
        private string _strCaption;
        private string _strTable;
        private string _strMode;
        private string _strButton;
        private string _data = "";
        private bool _bolEdit;


        // -------------------------------------------------------------
        // -------------------------------------------------------------
        // properties
        // -------------------------------------------------------------
        // -------------------------------------------------------------
        public ICommand comAdd { get; set; }

        public string strTitle { get { return _strTitle; } set { _strTitle = value; OnPropertyChanged("strTitle"); } }
        public string strButton { get { return _strButton; } set { _strButton = value; OnPropertyChanged("strButton"); } }
        public string strName { get { return _strName; } set { _strName = value; OnPropertyChanged("strName"); } }
        public string strPhone { get { return _strPhone; } set { _strPhone = value; OnPropertyChanged("strPhone"); } }
        public bool bolEdit { get { return _bolEdit; } set { _bolEdit = value; OnPropertyChanged("bolEdit"); } }


        // -------------------------------------------------------------
        // -------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------
        // -------------------------------------------------------------
        public pgEditDoctorNurseVM(string mode)
        {
            initCommands();
            initList();

            _strMode = mode;

            if (_strMode == "doctor")
            {
                _strCaption = "dokter";
                _strTable = "tbl_doctor";
            }
            else if (_strMode == "nurse")
            {
                _strCaption = "perawat";
                _strTable = "tbl_nurse";
            }

            strTitle = "Tambah " + _strCaption;
            strButton = "Tambah";
        }

        public pgEditDoctorNurseVM(string mode, string data) : this(mode)
        {
            strTitle = "Edit " + _strCaption;
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

        private void getData(string data)
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

                        string strSQL = "SELECT Name, Phone FROM " + _strTable + " WHERE Name = '" + data + "'";
                        cmd.CommandText = strSQL;
                        MySqlDataReader sqlReader = cmd.ExecuteReader();

                        while (sqlReader.Read())
                        {
                            strName = sqlReader.GetString(0);
                            strPhone = sqlReader.GetString(1);
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
            if (strName.Length > 0)
            {
                if (strPhone.Length > 0)
                {
                    if (strPhone.IndexOf("628") == -1)
                    {
                        Global.showMessage("Format nomor telephone salah, silahkan ganti dengan awalan 628!");
                        return;
                    }
                }
                if (_data.Length > 0)
                {
                    string strSQL = _strTable + " WHERE Name = '" + strName + "' AND Name <> '" + _data + "'";

                    if (Global.recordExists(strSQL))
                    {
                        Global.showMessage("Nama tersebut sudah dipakai, silahkan ganti dengan nama lainnya!");
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
                        command.CommandText = "UPDATE " + _strTable + " SET Name = '" + strName + "', Phone = '" + strPhone + "' WHERE Name = '" + _data + "'";
                        command.ExecuteScalar();

                        connection.Close();

                        Global.showMessage("Data berhasil diubah!");
                        Global.pgData.getData();
                        await App.Current.MainPage.Navigation.PopAsync();
                    }
                    catch (Exception ex)
                    {
                        Global.errorMessage(ex.Message);
                    }
                }
                else //add new
                {
                    string strSQL = _strTable + " WHERE Name = '" + strName + "'";

                    if (Global.recordExists(strSQL))
                    {
                        Global.showMessage("Nama tersebut sudah dipakai, silahkan ganti dengan nama lainnya!");
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
                        command.CommandText = "INSERT INTO " + _strTable + " (Name, Phone) VALUES ('" + strName + "', '" + strPhone + "')";
                        command.ExecuteScalar();

                        connection.Close();

                        Global.showMessage("Data berhasil ditambahkan!");
                        Global.pgData.getData();
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
                Global.showMessage("Mohon untuk mengisi nama dahulu!");
            }
        }
    }
}

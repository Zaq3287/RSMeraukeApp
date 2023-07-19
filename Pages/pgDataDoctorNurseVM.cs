using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RSMerauke.Pages
{
    public class pgDataDoctorNurseVM : BindProperty
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
        private string _strMode;
        private string _strCaption;
        private string _strTable;
        private bool _bolEdit = false;
        private bool _bolWA = false;
        private ObservableCollection<Data> _lstData;


        // -------------------------------------------------------------
        // -------------------------------------------------------------
        // properties
        // -------------------------------------------------------------
        // -------------------------------------------------------------
        public ICommand comAdd { get; set; }
        public ICommand comDelete { get; set; }
        public ICommand comEdit { get; set; }
        public ICommand comWA { get; set; }

        public string strTitle { get { return _strTitle; } set { _strTitle = value; OnPropertyChanged("strTitle"); } }
        public bool bolEdit { get { return _bolEdit; } set { _bolEdit = value; OnPropertyChanged("bolEdit"); } }
        public bool bolWA { get { return _bolWA; } set { _bolWA = value; OnPropertyChanged("bolWA"); } }
        public ObservableCollection<Data> lstData { get { return _lstData; } set { _lstData = value; OnPropertyChanged("lstData"); } }

        // -------------------------------------------------------------
        // -------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------
        // -------------------------------------------------------------
        public pgDataDoctorNurseVM(string mode)
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

            strTitle = "Data " + _strCaption;

            if (DeviceInfo.Current.Platform == DevicePlatform.WinUI) //windows
            {
                bolEdit = true;
                bolWA = false;
            }
            else //android
            {
                bolEdit = false;
                bolWA = true;
            }

            getData();
        }


        // -------------------------------------------------------------
        // -------------------------------------------------------------
        // Methods
        // -------------------------------------------------------------
        // -------------------------------------------------------------
        private void initCommands()
        {
            comAdd = new Command(doAdd);
            comDelete = new Command(doDelete);
            comEdit = new Command(doEdit);
            comWA = new Command(doWA);
        }

        private void initList()
        {
            lstData = new ObservableCollection<Data>();
        }

        public void getData()
        {
            //clear list
            lstData.Clear();

            //get mysql connection string
            MySqlConnectionStringBuilder conString = Global.getConString();

            try
            {
                using (MySqlConnection sqlConnection = new MySqlConnection(conString.ConnectionString))
                {
                    using (var cmd = sqlConnection.CreateCommand())
                    {
                        sqlConnection.Open();

                        string strSQL = "SELECT Name, Phone FROM " + _strTable + " ORDER BY Name";
                        cmd.CommandText = strSQL;
                        MySqlDataReader sqlReader = cmd.ExecuteReader();

                        while (sqlReader.Read())
                        {
                           lstData.Add( new Data
                           {
                               Name = sqlReader.GetString(0),
                               Phone = sqlReader.GetString(1)
                           });
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
            var page = new pgEditDoctorNurse();
            page.BindingContext = new pgEditDoctorNurseVM(_strMode);

            await App.Current.MainPage.Navigation.PushAsync(page, false);
        }

        private async void doEdit(object sender)
        {
            var page = new pgEditDoctorNurse();
            page.BindingContext = new pgEditDoctorNurseVM(_strMode, sender.ToString());

            await App.Current.MainPage.Navigation.PushAsync(page, false);
        }

        private async void doDelete(object sender)
        {
            bool answer = await App.Current.MainPage.DisplayAlert("Hapus", "Apakah data tersebut mau dihapus?", "Ya", "Tidak");

            if (!answer) return;

            //get mysql connection string
            MySqlConnectionStringBuilder conString = Global.getConString();

            try
            {
                // open a connection asynchronously
                var connection = new MySqlConnection(conString.ConnectionString);
                connection.Open();

                //get status
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM " + _strTable + " WHERE Name = '" + sender.ToString() + "'";
                command.ExecuteScalar();

                connection.Close();

                getData();
                Global.showMessage("Data berhasil dihapus!");
            }
            catch (Exception ex)
            {
                Global.errorMessage(ex.Message);
            }
        }

        private async void doWA(object sender)
        {
            await Launcher.Default.OpenAsync("https://api.whatsapp.com/send?phone=" + sender.ToString());
        }
    }

    public class Data
    {
        public string Name { get; set;}
        public string Phone { get; set; }
    }
}

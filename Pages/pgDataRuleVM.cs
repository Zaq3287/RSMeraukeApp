using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RSMerauke.Pages
{
    public class pgDataRuleVM : BindProperty
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
        private string _strImage;
        private string _strCaption;
        private string _strTable;
        private bool _bolEdit = false;
        private ObservableCollection<Rule> _lstData;


        // -------------------------------------------------------------
        // -------------------------------------------------------------
        // properties
        // -------------------------------------------------------------
        // -------------------------------------------------------------
        public ICommand comAdd { get; set; }
        public ICommand comDelete { get; set; }
        public ICommand comEdit { get; set; }

        public string strImage { get { return _strImage; } set { _strImage = value; OnPropertyChanged("strImage"); } }
        public bool bolEdit { get { return _bolEdit; } set { _bolEdit = value; OnPropertyChanged("bolEdit"); } }
        public ObservableCollection<Rule> lstData { get { return _lstData; } set { _lstData = value; OnPropertyChanged("lstData"); } }

        // -------------------------------------------------------------
        // -------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------
        // -------------------------------------------------------------
        public pgDataRuleVM()
        {
            initCommands();
            initList();

            getData();

            if (DeviceInfo.Current.Platform == DevicePlatform.WinUI) //windows
            {
                bolEdit = true;
            }
            else //android
            {
                bolEdit = false;
            }
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
        }

        private void initList()
        {
            lstData = new ObservableCollection<Rule>();
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

                        string strSQL = "SELECT ID, Detail FROM tbl_rule ORDER BY ID, Detail";
                        cmd.CommandText = strSQL;
                        MySqlDataReader sqlReader = cmd.ExecuteReader();

                        while (sqlReader.Read())
                        {
                            lstData.Add(new Rule
                            {
                                ID = sqlReader.GetInt32(0),
                                Detail = sqlReader.GetInt32(0) + ". " + sqlReader.GetString(1)
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
            var page = new pgEditRule();
            page.BindingContext = new pgEditRuleVM();

            await App.Current.MainPage.Navigation.PushAsync(page, false);
        }

        private async void doEdit(object sender)
        {
            var page = new pgEditRule();
            page.BindingContext = new pgEditRuleVM(Convert.ToInt32(sender));

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
                command.CommandText = "DELETE FROM tbl_rule WHERE ID = " + sender.ToString() + "";
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
    }

    public class Rule
    {
        public int ID { get; set; }
        public string Detail { get; set; }
    }
}

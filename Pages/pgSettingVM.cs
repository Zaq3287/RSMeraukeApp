using System.Windows.Input;

namespace RSMerauke.Pages
{
    public class pgSettingVM : BindProperty
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


        // -------------------------------------------------------------
        // -------------------------------------------------------------
        // properties
        // -------------------------------------------------------------
        // -------------------------------------------------------------
        public ICommand comDoctor { get; set; }
        public ICommand comNurse { get; set; }
        public ICommand comRule { get; set; }
        public ICommand comMap { get; set; }


        // -------------------------------------------------------------
        // -------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------
        // -------------------------------------------------------------
        public pgSettingVM()
        {
            initCommands();
            initList();
        }


        // -------------------------------------------------------------
        // -------------------------------------------------------------
        // Methods
        // -------------------------------------------------------------
        // -------------------------------------------------------------
        private void initCommands()
        {
            comDoctor = new Command(doDoctor);
            comNurse = new Command(doNurse);
            comRule = new Command(doRule);
            comMap = new Command(doMap);
        }

        private void initList()
        {

        }

        private async void doDoctor(object sender)
        {
            var page = new pgDataDoctorNurse();
            var pageVM = new pgDataDoctorNurseVM("doctor");
            page.BindingContext = pageVM;
            Global.pgData = pageVM;

            await App.Current.MainPage.Navigation.PushAsync(page, false);
        }

        private async void doNurse(object sender)
        {
            var page = new pgDataDoctorNurse();
            var pageVM = new pgDataDoctorNurseVM("nurse");
            page.BindingContext = pageVM;
            Global.pgData = pageVM;

            await App.Current.MainPage.Navigation.PushAsync(page, false);
        }

        private async void doRule(object sender)
        {
            var page = new pgDataRule();
            var pageVM = new pgDataRuleVM();
            page.BindingContext = pageVM;
            Global.pgRule = pageVM;

            await App.Current.MainPage.Navigation.PushAsync(page, false);
        }

        private async void doMap(object sender)
        {
            var page = new pgMap();

            await App.Current.MainPage.Navigation.PushAsync(page, false);
        }
    }
}

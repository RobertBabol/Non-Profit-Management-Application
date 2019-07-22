using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NonProfitManagement
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class MemberSearchPage : Page
    {
        //Get database connection information
        string server;
        string database;
        string username;
        string password;
        public MemberSearchPage()
        {
            InitializeComponent();

            //Get database connection information
            server = ConfigurationManager.AppSettings["Server"];
            database = ConfigurationManager.AppSettings["Database"];
            username = ConfigurationManager.AppSettings["UserName"];
            password = ConfigurationManager.AppSettings["Password"];

            //Get list from the database

            //Initalise database class
            DBInterface dbi = new DBInterface(server, database, username, password);

            //Populate member levels
            DataTable dtMemberLevels = dbi.getMemberLevels();

            cbMemberLevel.Items.Add("All");

            foreach (DataRow row in dtMemberLevels.Rows)
            {
                cbMemberLevel.Items.Add(row[1]);
            }


            ////Data Binding for member dropdown
            //cbMemberLevel.ItemsSource = new List<string> { "All", "President", "President Elect", "Past President",
            //                                                    "Secretary", "Treasure", "Board Member", "Member" };
        }

        private void Member_Search_BTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                

                //Initalise database class
                DBInterface dbi = new DBInterface(server, database, username, password);

                //Convert ID from string to int
                string sID = Regex.Replace(txtMemberID.Text, "[^0-9]", "");
                Int32.TryParse(sID, out int mID);

                //Populate variables from search terms
                int mLevel = cbMemberLevel.SelectedIndex;
                string fName = Regex.Replace(txtFirstName.Text, "[^a-zA-Z]", "");
                string mName = Regex.Replace(txtMiddleName.Text, "[^a-zA-Z]", "");
                string lName = Regex.Replace(txtLastName.Text, "[^a-zA-Z]", "");
                string cPhone = Regex.Replace(txtCellPhone.Text, "[^0-9]", "");
                string wPhone = Regex.Replace(txtWorkPhone.Text, "[^0-9]", "");
                string hPhone = Regex.Replace(txtHomePhone.Text, "[^0-9]", "");
                string sAddress = Regex.Replace(txtStreetAddress.Text, "[^0-9a-zA-Z. ,#-]", "");
                string sNumber = Regex.Replace(txtStreetNumber.Text, "[^0-9]", "");
                string city = Regex.Replace(txtCity.Text, "[^a-zA-Z]", "");
                string state = Regex.Replace(txtState.Text, "[^a-zA-Z]", "");
                string zip = Regex.Replace(txtZip.Text, "[^0-9]", "");


                //Create data table with search terms
                DataTable dtb = dbi.FindMembers(mID, mLevel, fName, mName, lName, cPhone, wPhone, hPhone, sAddress, sNumber, city, state, zip);

                //Remove password column
                //dtb.Columns.Remove("password");

                //Open new page with search results data table
                NavigationService.Navigate(new MembersList(dtb));
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
            }
        }
    }
}

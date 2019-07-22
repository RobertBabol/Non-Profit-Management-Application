using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
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
using System.Data;
using System.Text.RegularExpressions;

namespace NonProfitManagement
{
    /// <summary>
    /// Interaction logic for MemberAddEditPage.xaml
    /// </summary>
    public partial class MemberAddEditPage : Page
    {
        //Initialize Variables
        bool updateMember;
        int  memberID;
        string memberPassword;
        string errorMsg;
        TextBox[] formValues;
        string[] phoneNumbers = { "Cell Phone", "Work Phone", "Home Phone" };


        public MemberAddEditPage()
        {
            InitializeComponent();

            //Add member to db
            updateMember = false;

            //Populate array of form text boxes
            formValues = new TextBox[] {txtFirstName, txtMiddleName, txtLastName, txtCellPhone, txtWorkPhone, txtHomePhone,
                                                    txtStreetAddress, txtStreetNumber, txtCity, txtState, txtZip};
        }

        public MemberAddEditPage(String[] s)
        {
            try
            {
                InitializeComponent();

                //Populate array of form text boxes
                formValues = new TextBox[] {txtFirstName, txtMiddleName, txtLastName, txtCellPhone, txtWorkPhone, txtHomePhone,
                                                    txtStreetAddress, txtStreetNumber, txtCity, txtState, txtZip};

                //Populate for db entry later
                memberID = int.Parse(s[0]);
                memberPassword = s[13];

                //Populate textboxes in form
                for (int i = 0; i < 11; i++)
                {
                    formValues[i].Text = s[i + 2];
                }

                //Change button and title text
                btnAddMember.Content = "Update Member";
                tbAddMember.Text = "Update Member";

                //Update member in db
                updateMember = true;
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
            }
        }

        private void Add_Member_BTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Reset variables
                bool submit = true;
                bool pErrorMsg = false;
                string pNumber = "";
                errorMsg = "";

                //Check if first name is empty
                if (formValues[0].Text.Length == 0)
                {
                    //Change Border Color
                    formValues[0].BorderBrush = Brushes.Red;

                    //Add to error message
                    errorMsg = errorMsg + "First Name must be filled out. \n";
                    submit = false;
                }
                else
                {
                    formValues[0].BorderBrush = Brushes.White;
                }

                //Check if last name form is empty
                if (formValues[2].Text.Length == 0)
                {
                    //Change Border Color
                    formValues[2].BorderBrush = Brushes.Red;

                    //Add to error message
                    errorMsg = errorMsg + "Last Name must be filled out. \n";
                    submit = false;
                }
                else
                {
                    formValues[2].BorderBrush = Brushes.White;
                }

                //Loop through phone numbers
                for (int i = 3; i < 6; i++)
                {
                    //Remove everything but digits
                    pNumber = Regex.Replace(formValues[i].Text, "[^0-9]", "");

                    //Check if length is correct
                    if (pNumber.Length != 10 && pNumber.Length != 0)
                    {
                        //Change Border Color
                        formValues[i].BorderBrush = Brushes.Red;

                        //Add to error message
                        errorMsg = errorMsg + phoneNumbers[i - 3] + " must either be empty or in proper format \n";
                        submit = false;
                        pErrorMsg = true;

                    }
                    else
                    {
                        formValues[i].BorderBrush = Brushes.White;
                    }
                }
                if (submit)
                {
                    //Get database connection information
                    string server = ConfigurationManager.AppSettings["Server"];
                    string database = ConfigurationManager.AppSettings["Database"];
                    string username = ConfigurationManager.AppSettings["UserName"];
                    string password = ConfigurationManager.AppSettings["Password"];

                    //Initalise database class
                    DBInterface dbi = new DBInterface(server, database, username, password);

                    //Populate Variables from Form
                    int memberLevelID = 7; //Default member level;
                    String fName = txtFirstName.Text;
                    String mName = txtMiddleName.Text;
                    String lName = txtLastName.Text;
                    String cPhone = Regex.Replace(formValues[3].Text, "[^0-9]", "");
                    String wPhone = Regex.Replace(formValues[4].Text, "[^0-9]", "");
                    String hPhone = Regex.Replace(formValues[5].Text, "[^0-9]", "");
                    String street = txtStreetAddress.Text;
                    String sNumber = Regex.Replace(formValues[7].Text, "[^0-9]", "");
                    String city = txtCity.Text;
                    String state = txtState.Text;
                    String zip = Regex.Replace(formValues[10].Text, "[^0-9]", "");

                    try
                    {
                        if (updateMember)
                        {
                            dbi.UpdateMemberInformation(memberID, memberPassword, memberLevelID, fName, mName, lName, cPhone,
                                    wPhone, hPhone, street, sNumber, city, state, zip);
                        }
                        else
                        {
                            dbi.InsertMemberInformation("", memberLevelID, fName, mName, lName, cPhone,
                                    wPhone, hPhone, street, sNumber, city, state, zip);
                        }

                        //Navigate to show all members
                        //Get Datatable with all of the members
                        DataTable dtMembers = dbi.GetAllMembers();

                        //Load members list page
                        NavigationService.Navigate(new MembersList(dtMembers));

                    }
                    catch (Exception ex)
                    {
                        //TODO: Add logging for errors
                    }
                }
                else
                {
                    if (pErrorMsg)
                    {
                        errorMsg = errorMsg + "Phone numbers must be 10 digits, don't include country code \n eg (352) 555-4444 or 3525554444 \n";
                    }
                    MessageBox.Show(errorMsg);
                }
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
            }

        }

        private void Cancel_BTN_Click(object sender, RoutedEventArgs e)
        {
            //TODO Send User to Home Page

        }
    }
}

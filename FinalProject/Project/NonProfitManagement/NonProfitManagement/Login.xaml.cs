using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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

namespace NonProfitManagement
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {

        DBInterface dbi;
        public Login()
        {
            try
            {

                InitializeComponent();

                //Get database connection information
                string server = ConfigurationManager.AppSettings["Server"];
                string database = ConfigurationManager.AppSettings["Database"];
                string username = ConfigurationManager.AppSettings["UserName"];
                string password = ConfigurationManager.AppSettings["Password"];

                //Initalise database class
                dbi = new DBInterface(server, database, username, password);
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int MemberID;

                string password = txtPassword.Password.ToString();

                bool bAppAccess = false;

                bool bAdmin = false;

                if (int.TryParse(txtMemberID.Text, out MemberID))
                {
                    if (password.Length > 0)
                    {
                        DataTable dt = dbi.getLoginInformation(MemberID, password);

                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];
                            bAppAccess = Convert.ToBoolean(Convert.ToInt16(row[14].ToString()));
                            bAdmin = Convert.ToBoolean(Convert.ToInt16(row[15].ToString()));
                            ((MainWindow)Application.Current.MainWindow).ApplicationAccess(bAppAccess, bAdmin);

                            if (!bAppAccess && !bAdmin)
                            {
                                //Notify of no app access
                                lblLoginError.Content = "You do not have access to the application";
                            }
                        }
                        else
                        {
                            //Notify of no app access
                            lblLoginError.Content = "Invalid MemberID and Password";
                        }
                    }
                    else
                    {
                        //Invalid
                        lblLoginError.Content = "Please enter a password";
                    }
                }
                else
                {
                    //Invalid
                    lblLoginError.Content = "Invalid MemberID";
                }
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
            }



        }


    }
}

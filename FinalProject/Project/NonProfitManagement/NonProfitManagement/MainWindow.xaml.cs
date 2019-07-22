using System;
using System.Collections.Generic;
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
using System.Windows.Markup;
using System.Configuration;
using System.Data;
using System.Windows.Controls.Primitives;
using System.IO;
using System.Windows.Media.Imaging;

namespace NonProfitManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public object ViewStates { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "logo.png"))
                {
                    imgLogo.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "logo.png", UriKind.Absolute));
                }
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
            }

            ApplicationAccess(false, false);

            frmLoadPage.Content = new Login();



                MouseLeftButtonDown += delegate {
                    try
                    {
                        DragMove();
                    }
                    catch (Exception ex)
                    {
                        //In case clicking the wrong part of the gui causes issues
                    }
                };
     
                
            

        }



        public void ApplicationAccess(bool app, bool admin)
        {

            btnAdmin.Visibility = System.Windows.Visibility.Hidden;

            if (app || admin)
            {
                btnMembers.IsEnabled = true;
                btnAddEditMember.IsEnabled = true;
                btnMembersSearch.IsEnabled = true;
                btnAddEditEvent.IsEnabled = true;
                btnEventsSearch.IsEnabled = true;
                btnProcessPayment.IsEnabled = true;
                btbPaymentsSearch.IsEnabled = true;

                frmLoadPage.Content = null; //Clear login page
            }

            if(admin)
            {
 
                btnAdmin.Visibility = System.Windows.Visibility.Visible;

                btnAdmin.IsEnabled = true;
                btnMembers.IsEnabled = true;
                btnAddEditMember.IsEnabled = true;
                btnMembersSearch.IsEnabled = true;
                btnAddEditEvent.IsEnabled = true;
                btnEventsSearch.IsEnabled = true;
                btnProcessPayment.IsEnabled = true;
                btbPaymentsSearch.IsEnabled = true;
            }

            if(!app && !admin)
            {
                btnAdmin.IsEnabled = false;
                btnMembers.IsEnabled = false;
                btnAddEditMember.IsEnabled = false;
                btnMembersSearch.IsEnabled = false;
                btnAddEditEvent.IsEnabled = false;
                btnEventsSearch.IsEnabled = false;
                btnProcessPayment.IsEnabled = false;
                btbPaymentsSearch.IsEnabled = false;
            }
        }
        //Button Click Event for Add/Edit Member Page
        private void btnAddEditMember_Click(object sender, RoutedEventArgs e)
        {
            frmLoadPage.Content = new MemberAddEditPage();
        }
        //Button Click Event for Add/Edit Event Page
        private void btnAddEditEvent_Click(object sender, RoutedEventArgs e)
        {
            frmLoadPage.Content = new EventAddEditPage();
        }
        //Button Click Event for Member Search Page
        private void BtnMembersSearch_Click(object sender, RoutedEventArgs e)
        {
            frmLoadPage.Content = new MemberSearchPage();
        }
        //Button Click Event for Member List Page
        private void btnMembers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Get database connection information
                string server = ConfigurationManager.AppSettings["Server"];
                string database = ConfigurationManager.AppSettings["Database"];
                string username = ConfigurationManager.AppSettings["UserName"];
                string password = ConfigurationManager.AppSettings["Password"];

                //Initalise database class
                DBInterface dbi = new DBInterface(server, database, username, password);

                //Get Datatable with all of the members
                DataTable dtMembers = dbi.GetAllMembers();

                //Load members list page
                frmLoadPage.Content = new MembersList(dtMembers);
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
            }
        }
        //Button Click Event for Event Search Page
        private void BtnEventsSearch_Click(object sender, RoutedEventArgs e)
        {
            frmLoadPage.Content = new EventsSearch();
        }
        //Button Click Event for Process Payment Page
        private void btnProcessPayment_Click(object sender, RoutedEventArgs e)
        {
            frmLoadPage.Content = new ProcessPayment();
        }
        //Button Click Event for Payment Search Event
        private void btbPaymentsSearch_Click(object sender, RoutedEventArgs e)
        {
            frmLoadPage.Content = new PaymentsSearch();
        }

        private void btnAdmin_Click(object sender, RoutedEventArgs e)
        {
            frmLoadPage.Content = new Administration();
        }

        private void btnX_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btmMin_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}

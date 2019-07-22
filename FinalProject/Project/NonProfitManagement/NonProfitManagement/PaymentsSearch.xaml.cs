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
    /// Interaction logic for PaymentsSearch.xaml
    /// </summary>
    /// 
    
    public partial class PaymentsSearch : Page
    {
        DBInterface dbi;
        public PaymentsSearch()
        {
            try
            {
                InitializeComponent();

                //Populate drop downs

                //Get database connection information
                string server = ConfigurationManager.AppSettings["Server"];
                string database = ConfigurationManager.AppSettings["Database"];
                string username = ConfigurationManager.AppSettings["UserName"];
                string password = ConfigurationManager.AppSettings["Password"];

                //Initalise database class
                dbi = new DBInterface(server, database, username, password);

                //Get Datatable with all of the members
                DataTable dtMembers = dbi.GetAllMembers();

                //Default for general donation
                cboMemberID.Items.Add("");
                foreach (DataRow row in dtMembers.Rows)
                {
                    cboMemberID.Items.Add(row[5].ToString() + ", " + row[3].ToString() + " - Member ID: " + row[0].ToString());
                }

                //Populate Payment Sources TODO: May make this configurable
                cboSource.Items.Add("");
                cboSource.Items.Add("Cash");
                cboSource.Items.Add("Credit Card");
                cboSource.Items.Add("Check");
                cboSource.Items.Add("PayPal");

                //Populate types of payments, need better way to sotre the ID of the soruce

                DataTable dtSource = dbi.GetPaymentTypes();

                drpType.Items.Add("");

                foreach (DataRow row in dtSource.Rows)
                {
                    drpType.Items.Add(row[1]);
                }

                //Populate Events

                DataTable dtEvents = dbi.GetAllEvents();

                drpEventID.Items.Add("");//Default no event
                                         //TODO: find better way to store the event ID
                foreach (DataRow row in dtEvents.Rows)
                {
                    drpEventID.Items.Add(row[5] + " - Event ID:" + row[0]);
                }
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
            }
        }

        private void btnProcessPayment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dgPayments.ItemsSource = null;

                string[] strMemberID = cboMemberID.SelectedItem.ToString().Split(':');
                int memberID = -1;
                if (cboMemberID.SelectedIndex > 0)
                {
                    memberID = int.Parse(strMemberID[1]);
                }
                string startdate = "";
                string enddate = "";

                if (drpStartDate.SelectedDate != null)
                {
                    startdate = drpStartDate.SelectedDate.Value.Year.ToString() + "-" + drpStartDate.SelectedDate.Value.Month.ToString() + "-" + drpStartDate.SelectedDate.Value.Day.ToString() + " 00:00:00";
                }

                if (drpEndDate.SelectedDate != null)
                {
                    enddate = drpEndDate.SelectedDate.Value.Year.ToString() + "-" + drpEndDate.SelectedDate.Value.Month.ToString() + "-" + drpEndDate.SelectedDate.Value.Day.ToString() + " 00:00:00";
                }

                //TODO: add amount range
                float amount;

                if (!float.TryParse(txtAmount.Text, out amount))
                {
                    amount = -1;
                }
                int type = -1;

                if (drpType.SelectedIndex > 0)
                {
                    type = drpType.SelectedIndex;
                }

                int eventID = -1;
                string[] streventID = drpEventID.SelectedItem.ToString().Split(':');
                if (streventID.Count() > 1)
                {
                    eventID = int.Parse(streventID[1]);
                }

                //Payment ID not needed, may remove if edit payments is not impliemnted
                //Most organizations don't edit payemtns, they do an adjustment

                //Send the request to the database
                DataTable dtPayments = dbi.FindPayments(-1, memberID, type, amount, eventID, cboSource.Text, startdate, enddate, txtDescription.Text.Trim());

                //<DataGridTextColumn Binding="{Binding Path=Amount, StringFormat=C}" Header="Amount" />
                dgPayments.ItemsSource = dtPayments.DefaultView;
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
            }







        }
    }
}

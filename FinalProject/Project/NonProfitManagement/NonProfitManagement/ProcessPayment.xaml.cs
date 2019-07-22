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
using System.Text.RegularExpressions;

namespace NonProfitManagement
{
    /// <summary>
    /// Interaction logic for ProcessPayment.xaml
    /// </summary>
    public partial class ProcessPayment : Page
    {
        DBInterface dbi;
        public ProcessPayment()
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
                cboMemberID.Items.Add("Donation");

                foreach (DataRow row in dtMembers.Rows)
                {
                    cboMemberID.Items.Add(row[5].ToString() + ", " + row[3].ToString() + " - Memeber ID: " + row[0].ToString());
                }

                //Populate types of payments, need better way to sotre the ID of the soruce

                DataTable dtSource = dbi.GetPaymentTypes();

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

                //Populate Payment Sources TODO: May make this configurable
                drpSource.Items.Add("Cash");
                drpSource.Items.Add("Credit Card");
                drpSource.Items.Add("Check");
                drpSource.Items.Add("PayPal");

                //Default date to current date
                drpDate.SelectedDate = DateTime.Now.Date;
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
                int memberID = -1;
                bool submit = true;

                if (cboMemberID.SelectedIndex > 0)
                {
                    string[] strMemberID = cboMemberID.SelectedItem.ToString().Split(':');
                    memberID = int.Parse(strMemberID[1]);
                }

                //TODO: Add data verification

                //Format:  'YYYY-MM-DD HH:MM:SS' 
                string date = drpDate.SelectedDate.Value.Year.ToString() + "-" + drpDate.SelectedDate.Value.Month.ToString() + "-" + drpDate.SelectedDate.Value.Day.ToString() + " 00:00:00";

                string amount = Regex.Replace(txtAmount.Text, "[^0-9.$]", "");

                string source = drpSource.SelectedItem.ToString();

                int eventID = -1;

                string[] streventID = drpEventID.SelectedItem.ToString().Split(':');
                if (streventID.Count() > 1)
                {
                    eventID = int.Parse(streventID[1]);
                }

                if (txtAmount.Text.Length == 0)
                {
                    submit = false;
                    txtAmount.BorderBrush = Brushes.Red;
                }
                else
                    txtAmount.BorderBrush = Brushes.White;

                string description = txtDescription.Text;

                if (submit)
                {
                    //Send the data to the database
                    bool success = dbi.InsertPayment(memberID, drpType.SelectedIndex + 1, float.Parse(amount), eventID, source, date, description);

                    if (success)
                    {
                        MessageBox.Show("Payment Processed");
                        //Clear the form
                        cboMemberID.SelectedIndex = 0;
                        drpDate.SelectedDate = DateTime.Now.Date;
                        txtAmount.Text = "";
                        drpSource.SelectedIndex = 0;
                        drpEventID.SelectedIndex = 0;
                        drpType.SelectedIndex = 0;
                        txtDescription.Text = "";
                    }
                    else
                    {
                        //TODO: add handliojg forwhen the database is not able to process the command
                    }
                }
                else
                {
                    //Error message
                    MessageBox.Show("Donation amount must not be empty.");
                }
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
            }
        }

        
        private void ValidateCurrencyTextBox(object sender, TextCompositionEventArgs e)
        {
            try
            {
                Regex regex = new Regex("^[.][0-9]+$|^[0-9]*[.]{0,1}[0-9]*$");
                e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
            }
        }
}
}

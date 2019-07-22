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
    /// Interaction logic for EventAddEditPage.xaml
    /// </summary>
    public partial class EventAddEditPage : Page
    {
        //Initialize variables
        bool updateEvent;
        int eventID;
        bool validInput = true;
        DataTable dtEventTypes;

        public EventAddEditPage()
        {
            try
            {
                InitializeComponent();

                updateEvent = false;

                //Load items into combobox
                LoadEventTypes();
                LoadMinutes();

                //Set default time
                cbStartMinute.SelectedIndex = 59;
                cbEndMinute.SelectedIndex = 59;

                //Set default date
                dpStartDate.SelectedDate = DateTime.Today;
                dpEndDate.SelectedDate = DateTime.Today;
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
            }
        }

        public EventAddEditPage(string[] s)
        {
            try
            {
                InitializeComponent();

                //Load event types into combobox
                LoadEventTypes();
                LoadMinutes();

                //Check if eventType is in the database
                int eventType = 0;//Default
                int currentselection = 1;
                foreach (DataRow r in dtEventTypes.Rows)
                {
                    string tmp = r[1].ToString();
                    if (tmp == s[1])
                    {
                        eventType = currentselection; //set the current row
                        break;//Exit the loop
                    }
                    else
                    {
                        currentselection++;
                    }
                }




                //Populate form
                eventID = int.Parse(s[0]);
                txtCost.Text = s[2].Substring(1, s[2].Length - 1);
                dpStartDate.SelectedDate = Convert.ToDateTime(s[3]);
                dpEndDate.SelectedDate = Convert.ToDateTime(s[4]);
                txtDescription.Text = s[5];
                cbEventType.SelectedIndex = eventType - 1;

                //Split start date and time
                String[] seperateDate = s[3].Split(' ');
                string[] seperateTime = seperateDate[1].Split(':');

                cbStartHour.SelectedIndex = Int32.Parse(seperateTime[0]) - 1;
                cbStartMinute.SelectedIndex = Int32.Parse(seperateTime[1]);
                cbStartPeriod.SelectedValue = seperateDate[2];


                //Split end date and time
                seperateDate = s[4].Split(' ');
                seperateTime = seperateDate[1].Split(':');

                cbEndHour.SelectedIndex = Int32.Parse(seperateTime[0]) - 1;
                cbEndMinute.SelectedIndex = Int32.Parse(seperateTime[1]);
                cbEndPeriod.SelectedValue = seperateDate[2];

                //Update button and title text
                btnAddUpdateEvent.Content = "Update Event";
                tbAddEvent.Text = "Update Event";
                updateEvent = true;
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
            }
        }

        private void Btn_Add_Update_Event_Click(object sender, RoutedEventArgs e)
        {

            //Get database connection information
            string server = ConfigurationManager.AppSettings["Server"];
            string database = ConfigurationManager.AppSettings["Database"];
            string username = ConfigurationManager.AppSettings["UserName"];
            string password = ConfigurationManager.AppSettings["Password"];

            //Initalise database class
            DBInterface dbi = new DBInterface(server, database, username, password);

            try
            {
                //Reset variables
                validInput = true;
                string msgBox = "";
                string startTime;
                string endTime;

                //Populate variables from form
                String cost = txtCost.Text;
                DateTime startDate = dpStartDate.SelectedDate.Value;
                DateTime endDate = dpEndDate.SelectedDate.Value;
                string description = txtDescription.Text;
                int eventType = cbEventType.SelectedIndex + 1; //eventTypes in db start at 1

                //Trim to only digits and periods
                cost = Regex.Replace(cost, "[^0-9.]", "");

                //Convert 12H to 24H format
                int hrs = Int32.Parse(cbStartHour.SelectedValue.ToString());

                if (cbStartPeriod.SelectedIndex == 0)
                {
                    if (hrs == 12)
                        hrs = 0;

                    startTime = hrs + ":" + cbStartMinute.SelectedValue + ":00";
                }
                else
                {
                    startTime = hrs + ":" + cbStartMinute.SelectedValue + ":00";
                }


                //Convert 12H to 24H format
                hrs = Int32.Parse(cbEndHour.SelectedValue.ToString());

                if (cbEndPeriod.SelectedIndex == 0) // 0 == AM
                {
                    if (hrs == 12)
                        hrs = 0;

                    endTime = hrs + ":" + cbEndMinute.SelectedValue + ":00";
                }
                else
                {
                    endTime = hrs + ":" + cbEndMinute.SelectedValue + ":00";
                }

                //Combine date and time YYYY-MM-DD HH:MI:SS format
                string startDateTime = String.Format("{0:yyyy-MM-dd} {1: hh:mi:ss}", startDate, startTime);
                string endDateTime = String.Format("{0:yyyy-MM-dd} {1: hh:mi:ss}", endDate, endTime);

                //Check if start date is before end date
                if (DateTime.Parse(startDateTime) > DateTime.Parse(endDateTime))
                {
                    //start date is after end date
                    validInput = false;
                    msgBox = msgBox + "Start date cannot be after end date.";
                }

                //Check if all input is valid
                if (validInput)
                {
                    //Check if updating or inserting
                    if (updateEvent)
                    {
                        dbi.UpdateEvent(eventID, eventType, cost, startDateTime, endDateTime, description);
                    }
                    else
                    {
                        dbi.InsertEvent(eventType, cost, startDateTime, endDateTime, description);
                    }

                    //Get Datatable with all events
                    DataTable dtEvents = dbi.GetAllEvents();

                    //Load event list page
                    NavigationService.Navigate(new EventsList(dtEvents));
                }
                else
                {
                    //Display error message
                    MessageBox.Show(msgBox);
                }
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
            }
        }

        private void LoadEventTypes()
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

                //Get EventTypes from database
                dtEventTypes = dbi.GetEventTypes();

                //Get EventType column from datatable and load into combo box 
                cbEventType.ItemsSource = dtEventTypes.AsEnumerable().Select(r => r.Field<string>("EventType"));
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
            }
        }

        private void LoadMinutes()
        {
            for (int i = 0; i < 60; i++)
            {
                cbStartMinute.Items.Add(i);
                cbEndMinute.Items.Add(i);
            }
        }
    }
}

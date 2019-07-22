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
    /// Interaction logic for EventsSearch.xaml
    /// </summary>
    public partial class EventsSearch : Page
    {
        public EventsSearch()
        {
            InitializeComponent();

            //Load event types into combo box
            LoadEventTypes();
            LoadMinutes();

        }

        private void Btn_Search_Event_Click(object sender, RoutedEventArgs e)
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


                //Check if string is empty
                if (!int.TryParse(txtEventID.Text, out int eventID))
                {
                    //Set to 0 so database search doesn't include eventID
                    eventID = 0;
                }

                //Trim to only digits and periods
                string cost = Regex.Replace(txtCost.Text, "[^0-9.]", "");
                string startTime;
                string endTime;

                //Convert 12H to 24H format
                if (cbStartHour.SelectedValue != null && cbStartMinute.SelectedValue != null && cbStartPeriod.SelectedValue != null)
                {
                    if (cbStartPeriod.SelectedIndex == 0)
                    {
                        startTime = cbStartHour.SelectedValue + ":" + cbStartMinute.SelectedValue + ":00";
                    }
                    else
                    {
                        startTime = (Int32.Parse(cbStartHour.SelectedValue.ToString()) + 12) + ":" + cbStartMinute.SelectedValue + ":00";
                    }
                }
                else
                {
                    startTime = "";
                }
                //Convert 12H to 24H format
                if (cbEndHour.SelectedValue != null && cbEndMinute.SelectedValue != null && cbEndPeriod.SelectedValue != null)
                {
                    if (cbEndPeriod.SelectedIndex == 0) // 0 == AM
                    {
                        endTime = cbEndHour.SelectedValue + ":" + cbEndMinute.SelectedValue + ":00";
                    }
                    else
                    {
                        endTime = (Int32.Parse(cbEndHour.SelectedValue.ToString()) + 12) + ":" + cbEndMinute.SelectedValue + ":00";
                    }
                }
                else
                {
                    endTime = "";
                }

                string startDateTime;
                string endDateTime;

                //Check if start datepicker is empty
                if (dpStartDate.SelectedDate.HasValue)
                    startDateTime = String.Format("{0:yyyy-MM-dd} {1: hh:mi:ss}", dpStartDate.SelectedDate.Value, startTime);
                else
                    startDateTime = "";

                //Check if end datepicker is empty
                if (dpEndDate.SelectedDate.HasValue)
                    endDateTime = String.Format("{0:yyyy-MM-dd} {1: hh:mi:ss}", dpEndDate.SelectedDate.Value, endTime);
                else
                    endDateTime = "";


                string description = txtDescription.Text;
                int eventType = cbEventType.SelectedIndex;

                //Search database and pass to new page
                DataTable dt = dbi.FindEvents(eventID, eventType, cost, startDateTime, endDateTime, description);
                NavigationService.Navigate(new EventsList(dt));
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
                DataTable dt = dbi.GetEventTypes();

                //Create default event type
                DataRow row = dt.NewRow();
                row["ID"] = 0;
                row["EventType"] = "All";

                //Insert into datatable
                dt.Rows.InsertAt(row, 0);

                //Get EventType column from datatable and load into combo box 
                cbEventType.ItemsSource = dt.AsEnumerable().Select(r => r.Field<string>("EventType"));

                //Set default value
                cbEventType.SelectedIndex = 0;
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

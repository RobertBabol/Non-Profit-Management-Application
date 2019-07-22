using System;
using System.Collections.Generic;
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
    /// Interaction logic for EventsList.xaml
    /// </summary>
    public partial class EventsList : Page
    {
        public EventsList(DataTable dtMembers)
        {
            InitializeComponent();

            //Populatethe grid

            DgEvents.ItemsSource = dtMembers.DefaultView;

            //Adjust width to show all of the columns
            DgEvents.Width = 1075;
        }

        private void DgEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid dataGrid = sender as DataGrid;
                DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);

                //DataGridCell cell = dataGrid.Columns[0].GetCellContent(row).Parent as DataGridCell;
                //int EventID = int.Parse(((TextBlock)cell.Content).Text);

                string[] s = new string[6];
                for (int i = 0; i < 6; i++)
                {
                    DataGridCell tCell = dataGrid.Columns[i].GetCellContent(row).Parent as DataGridCell;
                    s[i] = (((TextBlock)tCell.Content).Text);
                }

                NavigationService.Navigate(new EventAddEditPage(s));
                //Maybe have a popup asking if they want to edit member: memberid
                //Add pop up that will allow edit, payment process or event search

            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
            }

        }
    }
}

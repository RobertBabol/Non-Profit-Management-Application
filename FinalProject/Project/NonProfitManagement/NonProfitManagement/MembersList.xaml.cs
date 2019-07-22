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
using System.Data;
using System.Collections.ObjectModel;


namespace NonProfitManagement
{
    /// <summary>
    /// Interaction logic for MembersList.xaml
    /// </summary>
    public partial class MembersList : Page
    {
        string[] passArray = { };

        public MembersList(DataTable dtMembers)
        {
            InitializeComponent();

            //Seperate password column
            string[] dArr = dtMembers.AsEnumerable().Select(r => r.Field<string>("Password")).ToArray();
            Array.Resize(ref passArray, dArr.Length);
            passArray = dArr;
            dtMembers.Columns.Remove("password");

            //Populate the grid
            dgMembers.ItemsSource = dtMembers.DefaultView;
            

            //Adjust width to show all of the columns
            dgMembers.Width = 1075;
        }

        private void DgMembers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid dataGrid = sender as DataGrid;
                DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);

                //DataGridCell cell = dataGrid.Columns[0].GetCellContent(row).Parent as DataGridCell;
                //int MemberID = int.Parse(((TextBlock)cell.Content).Text);

                string[] s = new string[14];
                for (int i = 0; i < 13; i++)
                {
                    DataGridCell tCell = dataGrid.Columns[i].GetCellContent(row).Parent as DataGridCell;
                    s[i] = (((TextBlock)tCell.Content).Text);
                }
                
                //Pass password back to member update
                s[13] = passArray[dataGrid.SelectedIndex];

                //Maybe have a popup asking if they want to edit member: memberid
                //Add pop up that will allow edit, payment process or event search
                NavigationService.Navigate(new MemberAddEditPage(s));

            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
            }

        }
    }
}

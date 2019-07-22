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
    /// Interaction logic for Administration.xaml
    /// </summary>
    /// 
    public partial class Administration : Page
    {
        DBInterface dbi;

        int SelectedMemberID;
        DataTable dtMemberLevels;
        public Administration()
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

                //Populate DropDowns
                cboAdminAccess.Items.Add("None");
                cboAdminAccess.Items.Add("Access");


                cboAppAccess.Items.Add("None");
                cboAppAccess.Items.Add("Access");

                //Populate member levels
                dtMemberLevels = dbi.getMemberLevels();

                foreach (DataRow row in dtMemberLevels.Rows)
                {
                    cboMemberLevelSearch.Items.Add(row[1]);
                    cboMemberLevel.Items.Add(row[1]);
                }

                cboMemberLevelSearch.Items.Add("Admin");

                enableControls(false, false);
            }
            catch(Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                DataTable dtMembers;
                if (cboMemberLevelSearch.Text == "Admin")
                {
                    dtMembers = dbi.AdminFindMembers(-1);
                }
                else
                {
                    dtMembers = dbi.AdminFindMembers(cboMemberLevelSearch.SelectedIndex + 1);
                }


                dgMembers.ItemsSource = dtMembers.DefaultView;
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdatePassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //SelectedMemberID
                string password = txtPassword.Password.Trim();
                string passwordconfirm = txtPasswordConfirm.Password.Trim();

                if (password.Length > 0 && password == passwordconfirm)
                {
                    bool success = dbi.updateMemberPassword(SelectedMemberID, password);
                    if (success)
                    {
                        MessageBox.Show("Password Updated");
                        txtPassword.Clear();
                        txtPasswordConfirm.Clear();
                    }
                    else
                    {
                        //TODO: need to better handle when the database does not save the update
                        MessageBox.Show("Invalid Passwords entered");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Passwords entered");
                }
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgMembers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //Reset the controls
                enableControls(false, false);

                lblMemberIDValue.Text = "";
                lblMemberName.Text = "";
                txtPassword.Password = "";
                txtPasswordConfirm.Password = "";

                DataGrid dataGrid = sender as DataGrid;
                DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);

                DataGridCell cell = dataGrid.Columns[2].GetCellContent(row).Parent as DataGridCell;

                string[] s = new string[6];
                for (int i = 0; i < 6; i++)
                {
                    DataGridCell tCell = dataGrid.Columns[i].GetCellContent(row).Parent as DataGridCell;
                    s[i] = (((TextBlock)tCell.Content).Text);
                }

                SelectedMemberID = int.Parse(s[2]);

                lblMemberIDValue.Text = s[2];

                lblMemberName.Text = s[0] + " " + s[1];

                //get member level id
                int memberlevel = 0;
                foreach(DataRow r in dtMemberLevels.Rows)
                {
                    int x = int.Parse(r[0].ToString());
                    if(r[1].ToString() == s[3])
                    {
                        memberlevel = int.Parse(r[0].ToString());
                        break;
                    }
                }

                cboMemberLevel.SelectedIndex = memberlevel - 1;

                if(s[4] == "Yes")
                {
                    cboAppAccess.SelectedIndex = 1;
                }
                else
                {
                    cboAppAccess.SelectedIndex = 0;
                }


                if (s[5] == "Yes")
                {
                    cboAdminAccess.SelectedIndex = 1;
                }
                else
                {
                    cboAdminAccess.SelectedIndex = 0;
                }


                if(SelectedMemberID == 0)
                {
                    enableControls(true, true);
                }
                else if (SelectedMemberID > 0)
                {
                    enableControls(true, false);
                }
                else
                {
                    enableControls(false, false);
                }

              

            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateMember_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool bAppAccess = false;
                bool bAdminAccess = false;

                if (cboAppAccess.SelectedIndex == 1)
                {
                    bAppAccess = true;
                }

                if (cboAdminAccess.SelectedIndex == 1)
                {
                    bAdminAccess = true;
                }

                dbi.updateMemberLevelAndAccess(SelectedMemberID, cboMemberLevel.SelectedIndex + 1, bAppAccess, bAdminAccess);

                //Refresh the grid
                DataTable dtMembers;
                if (cboMemberLevelSearch.Text == "Admin")
                {
                    dtMembers = dbi.AdminFindMembers(-1);
                }
                else
                {
                    dtMembers = dbi.AdminFindMembers(cboMemberLevelSearch.SelectedIndex + 1);
                }


                dgMembers.ItemsSource = dtMembers.DefaultView;

            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Enable"></param>
        /// <param name="adminAccount"></param>
        public void enableControls(bool Enable, bool adminAccount)
        {
            //Adjust the controls depening on the type of account that was selected
            if(adminAccount)
            {
                //Admin access levels cannot be changed
                txtPassword.IsEnabled = Enable;
                txtPasswordConfirm.IsEnabled = Enable;
                btnUpdatePassword.IsEnabled = Enable;
            }
            else
            {
                cboMemberLevel.IsEnabled = Enable;
                cboAppAccess.IsEnabled = Enable;
                cboAdminAccess.IsEnabled = Enable;
                btnUpdateMember.IsEnabled = Enable;
                txtPassword.IsEnabled = Enable;
                txtPasswordConfirm.IsEnabled = Enable;
                btnUpdatePassword.IsEnabled = Enable;
            }
            
        }
    }
}

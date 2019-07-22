using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace NonProfitManagement
{
    //TODO:Add logging for any errors that occur
    class DBInterface
    {
        //Stored connection information

        private string connectionString;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="database"></param>
        public DBInterface(string server, string database, string user, string password)
        {
            //Create connection string for the configured database
            connectionString = String.Format("server={0};database={1};uid={2};pwd={3};", server, database, user, password);
        }

        /// <summary>
        /// Use to test connection to the MYSQL server
        /// </summary>
        /// <returns></returns>
        public bool testDBConnection()
        {

            //Setup the database connection
            MySqlConnection connection;

            connection = new MySqlConnection(connectionString);
            try
            {
                DataSet mtData = new DataSet();

                connection.Open();

                connection.Close();

                return true;
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
                return false;//Default
            }

           
        }
        public DataTable getLoginInformation(int MemberID, string Password)
        {
            DataTable dt = new DataTable();

            try
            {
                //Setup the database connection
                MySqlConnection connection;

                connection = new MySqlConnection(connectionString);

                string cmdText = String.Format("Select * from members where MemberID = {0} and password = \"{1}\"", MemberID, Password);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmdText, connection);

                adapter.SelectCommand.CommandType = CommandType.Text;

                adapter.Fill(dt); //opens and closes the DB connection automatically !! (fetches from pool)


                return dt;
            }
            catch(Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
                return dt;
            }

        }
        /// <summary>
        /// Gets list of all of the members in the database
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllMembers()
        {
            try
            {

                //Setup the database connection
                MySqlConnection connection;

                connection = new MySqlConnection(connectionString);

                DataTable dt = new DataTable();

                //connection.Open();
                //string sqlCmd = "Select MemberID, (Select Level from memberlevels where ID = MemberLevelID) as 'Member Level' ,FName as 'First Name', " +
                //                                   "MName as 'Middle Name', LName as 'Last Name', CellPhone as 'Cell Phone', WorkPhone as 'Work Phone', " +
                //                                   "HomePhone as 'Home Phone', Street as 'Street Address', Number as 'Appartment Number', City, State ,Zip FROM members; ";
               

                string sqlCmd = "Select MemberID as \"ID\", Password, (Select Level from memberlevels where ID = MemberLevelID) as \"Member Level\", FName as \"First\", MName as \"Middle\", LName as \"Last\", " +
                                 "CellPhone as \"Cell Phone\", WorkPhone as \"Work Phone\", HomePhone as \"Home Phone\", Street, Number, City, State, Zip from members where memberID > 0";



                MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, connection);

                adapter.SelectCommand.CommandType = CommandType.Text;

                adapter.Fill(dt); //opens and closes the DB connection automatically !! (fetches from pool)

                return dt;
            }
            catch(Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
                return null;
            }
        }
        /// <summary>
        /// Get information for one members
        /// </summary>
        /// <param name="MemberID"></param>
        /// <returns></returns>
        public DataTable GetMemberInformation(int MemberID)
        {
            try
            {

                //Setup the database connection
                MySqlConnection connection;

                connection = new MySqlConnection(connectionString);

                DataTable dt = new DataTable();

                //connection.Open();
                string cmdText = String.Format("Select * from members " +
                                                "where MemberID = {0}", MemberID);

                //MySqlDataAdapter adapter = new MySqlDataAdapter(cmdText, connection);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmdText, connection);

                adapter.Fill(dt); //opens and closes the DB connection automatically !! (fetches from pool)

                return dt;
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
                return null;
            }
        }
        /// <summary>
        /// Note pass 0 for MemberID and MemberLevelID if you are not searhing for either of them.
        /// </summary>
        /// <param name="MemberID"></param>
        /// <param name="MemberLevelID"></param>
        /// <param name="Fname"></param>
        /// <param name="MName"></param>
        /// <param name="LName"></param>
        /// <param name="CellPhone"></param>
        /// <param name="WorkPhone"></param>
        /// <param name="HomePhone"></param>
        /// <param name="Street"></param>
        /// <param name="Number"></param>
        /// <param name="City"></param>
        /// <param name="State"></param>
        /// <param name="Zip"></param>
        /// <returns></returns>
        public DataTable FindMembers(int MemberID, int MemberLevelID, string FName, string MName, string LName, string CellPhone, string WorkPhone, string HomePhone,
                                            string Street, string Number, string City, string State, string Zip)
        {

            try
            {
                string strFilter = "where ";
                bool bUseFilter = false;

                if(MemberID != 0)
                {
                    strFilter = strFilter + "MemberID = " + MemberID + " ";
                    bUseFilter = true;
                }
                else
                {

                    //Check each parameter that was passed through and if the reimmed length is greater than 0 add it ot the query
                    if (MemberLevelID != 0)
                    {
                        strFilter = strFilter + "MemberLevelID = " + MemberLevelID + " ";
                        bUseFilter = true;
                    }
                    }
                    if (FName.Trim().Length > 0)
                    {
                        FName = FName.Trim(); //Clean spaces off the beginning and end of the search term

                        if (bUseFilter)
                        {
                            strFilter = strFilter + " and ( FName like '%" + FName + "' or FName like '%" + FName + "%' or FName like '" + FName + "%')";
                        }
                        else
                        {
                            strFilter = strFilter + "( FName like '%" + FName + "' or FName like '%" + FName + "%' or FName like '" + FName + "%')";
                            bUseFilter = true;
                        }
                    }
                    if (MName.Trim().Length > 0)
                    {
                        MName = MName.Trim(); //Clean spaces off the beginning and end of the search term

                        if (bUseFilter)
                        {
                            strFilter = strFilter + " and ( MName like '%" + MName + "' or MName like '%" + MName + "%' or MName like '" + MName + "%')";
                        }
                        else
                        {
                            strFilter = strFilter + "( MName like '%" + MName + "' or MName like '%" + MName + "%' or MName like '" + MName + "%')";
                            bUseFilter = true;
                        }
                    }
                    if (LName.Trim().Length > 0)
                    {
                        LName = LName.Trim(); //Clean spaces off the beginning and end of the search term

                        if (bUseFilter)
                        {
                            strFilter = strFilter + " and ( LName like '%" + LName + "' or LName like '%" + LName + "%' or LName like '" + LName + "%')";
                        }
                        else
                        {
                            strFilter = strFilter + "( LName like '%" + LName + "' or LName like '%" + LName + "%' or LName like '" + LName + "%')";
                            bUseFilter = true;
                        }
                    }
                    if (CellPhone.Trim().Length > 0)
                    {
                        CellPhone = CellPhone.Trim(); //Clean spaces off the beginning and end of the search term

                        if (bUseFilter)
                        {
                            strFilter = strFilter + " and ( CellPhone like '%" + CellPhone + "' or CellPhone like '%" + CellPhone + "%' or CellPhone like '" + CellPhone + "%')";
                        }
                        else
                        {
                            strFilter = strFilter + "( CellPhone like '%" + CellPhone + "' or CellPhone like '%" + CellPhone + "%' or CellPhone like '" + CellPhone + "%')";
                            bUseFilter = true;
                        }
                    }
                    if (HomePhone.Trim().Length > 0)
                    {
                        HomePhone = HomePhone.Trim(); //Clean spaces off the beginning and end of the search term

                        if (bUseFilter)
                        {
                            HomePhone = HomePhone + " and ( HomePhone like '%" + HomePhone + "' or HomePhone like '%" + HomePhone + "%' or HomePhone like '" + HomePhone + "%')";
                        }
                        else
                        {
                            strFilter = strFilter + "( HomePhone like '%" + HomePhone + "' or HomePhone like '%" + HomePhone + "%' or HomePhone like '" + HomePhone + "%')";
                            bUseFilter = true;
                        }
                    }
                    if (WorkPhone.Trim().Length > 0)
                    {
                        WorkPhone = WorkPhone.Trim(); //Clean spaces off the beginning and end of the search term

                        if (bUseFilter)
                        {
                            strFilter = strFilter + " and ( WorkPhone like '%" + WorkPhone + "' or WorkPhone like '%" + WorkPhone + "%' or WorkPhone like '" + WorkPhone + "%')";
                        }
                        else
                        {
                            strFilter = strFilter + "( WorkPhone like '%" + WorkPhone + "' or WorkPhone like '%" + WorkPhone + "%' or WorkPhone like '" + WorkPhone + "%')";
                            bUseFilter = true;
                        }
                    }
                    if (Street.Trim().Length > 0)
                    {
                        Street = Street.Trim(); //Clean spaces off the beginning and end of the search term

                        if (bUseFilter)
                        {
                            strFilter = strFilter + " and ( Street like '%" + Street + "' or Street like '%" + Street + "%' or Street like '" + Street + "%')";
                        }
                        else
                        {
                            strFilter = strFilter + "( Street like '%" + Street + "' or Street like '%" + Street + "%' or Street like '" + Street + "%')";
                            bUseFilter = true;
                        }
                    }
                    if (Number.Trim().Length > 0)
                    {
                        Number = Number.Trim(); //Clean spaces off the beginning and end of the search term

                        if (bUseFilter)
                        {
                            strFilter = strFilter + " and ( Number like '%" + Number + "' or Number like '%" + Number + "%' or Number like '" + Number + "%')";
                        }
                        else
                        {
                            strFilter = strFilter + "( Number like '%" + Number + "' or Number like '%" + Number + "%' or Number like '" + Number + "%')";
                            bUseFilter = true;
                        }
                    }
                    if (City.Trim().Length > 0)
                    {
                        City = City.Trim(); //Clean spaces off the beginning and end of the search term

                        if (bUseFilter)
                        {
                            strFilter = strFilter + " and ( City like '%" + City + "' or City like '%" + City + "%' or City like '" + City + "%')";
                        }
                        else
                        {
                            strFilter = strFilter + "( City like '%" + City + "' or City like '%" + City + "%' or City like '" + City + "%')";
                            bUseFilter = true;
                        }
                    }
                    if (State.Trim().Length > 0)
                    {
                        State = State.Trim(); //Clean spaces off the beginning and end of the search term

                        if (bUseFilter)
                        {
                            strFilter = strFilter + " and ( State like '%" + State + "' or State like '%" + State + "%' or State like '" + State + "%')";
                        }
                        else
                        {
                            strFilter = strFilter + "( State like '%" + State + "' or State like '%" + State + "%' or State like '" + State + "%')";
                            bUseFilter = true;
                        }
                    }
                    if (Zip.Trim().Length > 0)
                    {
                        Zip = Zip.Trim(); //Clean spaces off the beginning and end of the search term

                        if (bUseFilter)
                        {
                            strFilter = strFilter + " and ( Zip like '%" + Zip + "' or Zip like '%" + Zip + "%' or Zip like '" + Zip + "%')";
                        }
                        else
                        {
                            strFilter = strFilter + "( Zip like '%" + Zip + "' or Zip like '%" + Zip + "%' or Zip like '" + Zip + "%')";
                            bUseFilter = true;
                        }
                    }
                
                //Setup the database connection
                MySqlConnection connection;

                connection = new MySqlConnection(connectionString);

                DataTable dt = new DataTable();

                //connection.Open();
                
                string cmdText = "Select MemberID as \"ID\",Password, (Select Level from memberlevels where ID = MemberLevelID) as \"Member Level\", FName as \"First\", MName as \"Middle\", LName as \"Last\", " +
                                 "CellPhone as \"Cell Phone\", WorkPhone as \"Work Phone\", HomePhone as \"Home Phone\", Street, Number, City, State, Zip from members ";
                if (bUseFilter)
                {
                    cmdText = cmdText + strFilter + " and MemberID != 0";
                }
                else
                {
                    cmdText = cmdText + strFilter + " MemberID != 0";
                }

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmdText, connection);



                adapter.SelectCommand.CommandType = CommandType.Text;

                adapter.Fill(dt); //opens and closes the DB connection automatically !! (fetches from pool)

                return dt;

            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MemberLevel"></param>
        /// <returns></returns>
        public DataTable AdminFindMembers(int MemberLevel)
        {
            DataTable dt = new DataTable();
            try
            {
                //Setup the database connection
                MySqlConnection connection;

                connection = new MySqlConnection(connectionString);

                string cmdText;
                //connection.Open();
                if (MemberLevel == -1)
                {
                    cmdText = "Select FName as \"First\", LName as \"Last\", MemberID as \"Memeber ID\", (Select Level from memberlevels where ID = MemberLevelID) as \"Member Level\", "
                        +"(Select \"Yes\" where AppAccess = 1) as \"App Access\", (Select \"Yes\" where AdminAccess = 1) as \"Admin Access\" from members where AdminAccess = 1";
                }
                else
                {
                    cmdText = "Select FName as \"First\", LName as \"Last\", MemberID as \"Memeber ID\", (Select Level from memberlevels where ID = MemberLevelID) as \"Member Level\", "
                        + "(Select IF(AppAccess = 1, \"Yes\",\"No\")) as \"App Access\", (Select IF(AdminAccess = 1, \"Yes\",\"No\")) as \"Admin Access\" from members where MemberLevelID = " + MemberLevel.ToString();
                }
                
                

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmdText, connection);

                adapter.SelectCommand.CommandType = CommandType.Text;

                adapter.Fill(dt); //opens and closes the DB connection automatically !! (fetches from pool)

                return dt;
            }
            catch(Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
                return dt;
            }
        }
        /// <summary>
        /// Inserts a new member into the database
        /// </summary>
        /// <param name="Password"></param>
        /// <param name="MemberLevelID"></param>
        /// <param name="Fname"></param>
        /// <param name="MName"></param>
        /// <param name="LName"></param>
        /// <param name="CellPhone"></param>
        /// <param name="WorkPhone"></param>
        /// <param name="HomePhone"></param>
        /// <param name="Street"></param>
        /// <param name="Number"></param>
        /// <param name="City"></param>
        /// <param name="State"></param>
        /// <param name="Zip"></param>
        /// <returns></returns>
        public bool InsertMemberInformation(string Password, int MemberLevelID, string Fname, string MName, string LName, string CellPhone, string WorkPhone, string HomePhone, 
                                            string Street, string Number, string City, string State, string Zip)
        {
            MySqlConnection connection;
            connection = new MySqlConnection(connectionString);

            try
            {
                //Setup the database connection

                connection.Open();

                string cmdText = "Insert into members (Password, MemberLevelID, Fname, MName, LName, CellPhone, WorkPhone, HomePhone, Street, Number, City, State, Zip, AppAccess, AdminAccess) " +
                                 "VALUES (@Password, @MemberLevelID, @Fname, @MName, @LName, @CellPhone, @WorkPhone, @HomePhone, @Street, @Number, @City, @State, @Zip, @AppAccess, @AdminAccess)";
                MySqlCommand cmd = new MySqlCommand(cmdText, connection);

                //Set the parameters from the provided information

                cmd.Parameters.AddWithValue("@Password", Password);
                cmd.Parameters.AddWithValue("@MemberLevelID", MemberLevelID);
                cmd.Parameters.AddWithValue("@Fname", Fname);
                cmd.Parameters.AddWithValue("@MName", MName);
                cmd.Parameters.AddWithValue("@LName", LName);
                cmd.Parameters.AddWithValue("@CellPhone", CellPhone);
                cmd.Parameters.AddWithValue("@WorkPhone", WorkPhone);
                cmd.Parameters.AddWithValue("@HomePhone", HomePhone);
                cmd.Parameters.AddWithValue("@Street", Street);
                cmd.Parameters.AddWithValue("@Number", Number);
                cmd.Parameters.AddWithValue("@City", City);
                cmd.Parameters.AddWithValue("@State", State);
                cmd.Parameters.AddWithValue("@Zip", Zip);
                cmd.Parameters.AddWithValue("@AppAccess", 0);
                cmd.Parameters.AddWithValue("@AdminAccess", 0);


                cmd.ExecuteNonQuery();


                return true;

            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
                connection.Close();
                return false;
            }
        }
        /// <summary>
        /// Updates infoirmation for a single member
        /// </summary>
        /// <param name="MemberID"></param>
        /// <param name="Password"></param>
        /// <param name="MemberLevelID"></param>
        /// <param name="Fname"></param>
        /// <param name="MName"></param>
        /// <param name="LName"></param>
        /// <param name="CellPhone"></param>
        /// <param name="WorkPhone"></param>
        /// <param name="HomePhone"></param>
        /// <param name="Street"></param>
        /// <param name="Number"></param>
        /// <param name="City"></param>
        /// <param name="State"></param>
        /// <param name="Zip"></param>
        /// <returns></returns>
        public bool UpdateMemberInformation(int MemberID, string Password, int MemberLevelID, string Fname, string MName, string LName, string CellPhone, string WorkPhone, string HomePhone,
                                            string Street, string Number, string City, string State, string Zip)
        {
            MySqlConnection connection;
            connection = new MySqlConnection(connectionString);

            try
            {
                //Setup the database connection

                connection.Open();

                string cmdText = "UPDATE members SET Password = @Password,  MemberLevelID = @MemberLevelID,  Fname = @Fname,  MName = @MName,  LName = @LName,  CellPhone = @CellPhone,  " + "" +
                    "WorkPhone = @WorkPhone,  HomePhone = @HomePhone,  Street = @Street,  Number = @Number,  City = @City,  State = @State,  Zip = @Zip " +
                    "where MemberID = @MemberID";
                MySqlCommand cmd = new MySqlCommand(cmdText, connection);

                //Set the parameters from the provided information

                cmd.Parameters.AddWithValue("@MemberID", MemberID);
                cmd.Parameters.AddWithValue("@Password", Password);
                cmd.Parameters.AddWithValue("@MemberLevelID", MemberLevelID);
                cmd.Parameters.AddWithValue("@Fname", Fname);
                cmd.Parameters.AddWithValue("@MName", MName);
                cmd.Parameters.AddWithValue("@LName", LName);
                cmd.Parameters.AddWithValue("@CellPhone", CellPhone);
                cmd.Parameters.AddWithValue("@WorkPhone", WorkPhone);
                cmd.Parameters.AddWithValue("@HomePhone", HomePhone);
                cmd.Parameters.AddWithValue("@Street", Street);
                cmd.Parameters.AddWithValue("@Number", Number);
                cmd.Parameters.AddWithValue("@City", City);
                cmd.Parameters.AddWithValue("@State", State);
                cmd.Parameters.AddWithValue("@Zip", Zip);

                cmd.ExecuteNonQuery();


                return true;

            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
                connection.Close();
                return false;
            }
        }
        /// <summary>
        /// Gets a datatable filled with all of the events
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllEvents()
        {
            try
            {
                //Setup the database connection

                MySqlConnection connection;

                connection = new MySqlConnection(connectionString);

                DataTable dt = new DataTable();

                //connection.Open();
                string sqlCmd = "Select ID, (Select EventType from eventtypes where ID = EventTypeID) as \"Event Type\", concat('$', format(Cost, 2)) as Amount, StartDateTime as \"Start Date & Time\" " +
                    ", EndDateTime as \"Start Date & Time\", Description from events";

                MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, connection);

                adapter.SelectCommand.CommandType = CommandType.Text;

                adapter.Fill(dt); //opens and closes the DB connection automatically !! (fetches from pool)

                return dt;
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
                return null;
            }
        }
        /// <summary>
        /// Gets a list of the event types
        /// </summary>
        /// <returns></returns>
        public DataTable GetEventTypes()
        {
            try
            {
                //Setup the database connection

                MySqlConnection connection;

                connection = new MySqlConnection(connectionString);

                DataTable dt = new DataTable();

                //connection.Open();
                string sqlCmd = "select * from eventtypes";

                MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, connection);

                adapter.SelectCommand.CommandType = CommandType.Text;

                adapter.Fill(dt); //opens and closes the DB connection automatically !! (fetches from pool)

                return dt;
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
                return null;
            }
        }
        /// <summary>
        /// Inserts a new event
        /// </summary>
        /// <param name="Cost"></param>
        /// <param name="StartDateTime"></param>
        /// <param name="EndDateTime"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        public bool InsertEvent(int EventTypeID, string Cost, string StartDateTime, string EndDateTime, string Description)
        {
            MySqlConnection connection;
            connection = new MySqlConnection(connectionString);

            try
            {
                //Setup the database connection

                connection.Open();

                string cmdText = "Insert into events (EventTypeID, Cost, StartDateTime, EndDateTime, Description) " +
                                 "VALUES (@EventTypeID, @Cost, @StartDateTime, @EndDateTime, @Description)";
                MySqlCommand cmd = new MySqlCommand(cmdText, connection);

                //Set the parameters from the provided information
                float flCost = 0;
                float.TryParse(Cost, out flCost);

                cmd.Parameters.AddWithValue("@Cost", flCost);
                cmd.Parameters.AddWithValue("@EventTypeID", EventTypeID);
                cmd.Parameters.AddWithValue("@StartDateTime", StartDateTime);
                cmd.Parameters.AddWithValue("@EndDateTime", EndDateTime);
                cmd.Parameters.AddWithValue("@Description", Description);
      
                cmd.ExecuteNonQuery();


                return true;

            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
                connection.Close();
                return false;
            }
        }
        /// <summary>
        /// Updates a single event
        /// </summary>
        /// <param name="EventID"></param>
        /// <param name="Cost"></param>
        /// <param name="StartDateTime"></param>
        /// <param name="EndDateTime"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        public bool UpdateEvent(int EventID, int EventTypeID, string Cost, string StartDateTime, string EndDateTime, string Description)
        {
            MySqlConnection connection;
            connection = new MySqlConnection(connectionString);

            try
            {
                //Setup the database connection

                connection.Open();

                string cmdText = "UPDATE events SET EventTypeID = @EventTypeID, Cost = @Cost, StartDateTime = @StartDateTime, EndDateTime = @EndDateTime, Description = @Description " +
                                 "where ID = @EventID";
                MySqlCommand cmd = new MySqlCommand(cmdText, connection);

                //Set the parameters from the provided information

                cmd.Parameters.AddWithValue("@EventID", EventID);
                cmd.Parameters.AddWithValue("@EventTypeID", EventTypeID);
                cmd.Parameters.AddWithValue("@Cost", Cost);
                cmd.Parameters.AddWithValue("@StartDateTime", StartDateTime);
                cmd.Parameters.AddWithValue("@EndDateTime", EndDateTime);
                cmd.Parameters.AddWithValue("@Description", Description);

                cmd.ExecuteNonQuery();


                return true;

            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
                connection.Close();
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="EventID"></param>
        /// <param name="EventType"></param>
        /// <param name="Cost"></param>
        /// <param name="StartDateTime"></param>
        /// <param name="EndDateTime"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        public DataTable FindEvents(int EventID, int EventType, string Cost, string StartDateTime, string EndDateTime, string Description)
        {

            try
            {
                string strFilter = "where ";
                bool bUseFilter = false;

                //Check each parameter that was passed through and if the retrimmed length is greater than 0 add it ot the query
                if (EventID != 0)
                {
                    strFilter = strFilter + "ID = " + EventID + " ";
                    bUseFilter = true;
                }

                if (EventType != 0)
                {
                    if (bUseFilter)
                    {
                        strFilter = strFilter + " and EventTypeID = '" + EventType + "'";
                    }
                    else
                    {
                        strFilter = strFilter + " EventTypeID = '" + EventType + "'";
                        bUseFilter = true;
                    }
                }

                if (Cost.Trim().Length > 0)
                {
                    Cost = Cost.Trim(); //Clean spaces off the beginning and end of the search term

                    if (bUseFilter)
                    {
                        strFilter = strFilter + " and Cost = '" + Cost + "'";
                    }
                    else
                    {
                        strFilter = strFilter + " Cost = '" + Cost + "'";
                        bUseFilter = true;
                    }
                }
                if (StartDateTime.Trim().Length > 0)
                {
                    StartDateTime = StartDateTime.Trim(); //Clean spaces off the beginning and end of the search term

                    if (bUseFilter)
                    {
                        strFilter = strFilter + " and  StartDateTime >= '" + StartDateTime + "'";
                    }
                    else
                    {
                        strFilter = strFilter + " StartDateTime >= '" + StartDateTime + "'";
                        bUseFilter = true;
                    }
                }
                if (EndDateTime.Trim().Length > 0)
                {
                    EndDateTime = EndDateTime.Trim(); //Clean spaces off the beginning and end of the search term

                    if (bUseFilter)
                    {
                        strFilter = strFilter + " and  EndDateTime <= '" + EndDateTime + "'";
                    }
                    else
                    {
                        strFilter = strFilter + " EndDateTime <= '" + EndDateTime + "'";
                        bUseFilter = true;
                    }
                }
                if (Description.Trim().Length > 0)
                {
                    Description = Description.Trim(); //Clean spaces off the beginning and end of the search term

                    if (bUseFilter)
                    {
                        strFilter = strFilter + " and ( Description like '%" + Description + "' or Description like '%" + Description + "%' or Description like '" + Description + "%')";
                    }
                    else
                    {
                        strFilter = strFilter + "( Description like '%" + Description + "' or Description like '%" + Description + "%' or Description like '" + Description + "%')";
                        bUseFilter = true;
                    }
                }


                //Setup the database connection
                MySqlConnection connection;

                connection = new MySqlConnection(connectionString);

                DataTable dt = new DataTable();

                //connection.Open();

                string cmdText = "Select ID, (Select EventType from eventtypes where ID = EventTypeID) as \"Event Type\", concat('$', format(Cost, 2)) as Amount, StartDateTime as \"Start Date & Time\" " +
                    ", EndDateTime as \"Start Date & Time\", Description from events ";
                if (bUseFilter)
                {
                    cmdText = cmdText + strFilter;
                }

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmdText, connection);



                adapter.SelectCommand.CommandType = CommandType.Text;

                adapter.Fill(dt); //opens and closes the DB connection automatically !! (fetches from pool)

                return dt;

            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
                return null;
            }
        }
        /// <summary>
        /// Get a list of payments
        /// </summary>
        /// <returns></returns>
        public DataTable GetPayments()
        {
            try
            {
                //Setup the database connection

                MySqlConnection connection;

                connection = new MySqlConnection(connectionString);

                DataTable dt = new DataTable();

                //connection.Open();
                string sqlCmd = "select * from payments";

                MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, connection);

                adapter.SelectCommand.CommandType = CommandType.Text;

                adapter.Fill(dt); //opens and closes the DB connection automatically !! (fetches from pool)

                return dt;
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
                return null;
            }
        }
        /// <summary>
        /// Get a list of all of the payment types
        /// </summary>
        /// <returns></returns>
        public DataTable GetPaymentTypes()
        {
            try
            {
                //Setup the database connection

                MySqlConnection connection;

                connection = new MySqlConnection(connectionString);

                DataTable dt = new DataTable();

                //connection.Open();
                string sqlCmd = "select * from paymenttypes";

                MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, connection);

                adapter.SelectCommand.CommandType = CommandType.Text;

                adapter.Fill(dt); //opens and closes the DB connection automatically !! (fetches from pool)

                return dt;
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
                return null;
            }
        }
        /// <summary>
        /// Save a payment to the database
        /// </summary>
        /// <param name="MemberID"></param>
        /// <param name="PaymentType"></param>
        /// <param name="EventType"></param>
        /// <param name="PaymentSource"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        public bool InsertPayment(int MemberID, int PaymentType, float Amount, int EventID, string PaymentSource, string Date, string Description)
        {
            MySqlConnection connection;
            connection = new MySqlConnection(connectionString);

            try
            {
                //Setup the database connection

                connection.Open();

                string cmdText = "Insert into payments (MemberID, PaymentTypeID, Amount, EventID, PaymentSource, Date, Description) " +
                                 "VALUES (@MemberID, @PaymentTypeID, @Amount, @EventID, @PaymentSource, @Date, @Description)";
                MySqlCommand cmd = new MySqlCommand(cmdText, connection);

                //Set the parameters from the provided information
                if(MemberID == -1)
                {
                    cmd.Parameters.AddWithValue("@MemberID", null);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@MemberID", MemberID);
                }
                cmd.Parameters.AddWithValue("@PaymentTypeID", PaymentType);
                cmd.Parameters.AddWithValue("@Amount", Amount);
                if(EventID == -1)
                {
                    cmd.Parameters.AddWithValue("@EventID", null);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@EventID", EventID);
                }
                
                cmd.Parameters.AddWithValue("@PaymentSource", PaymentSource);
                cmd.Parameters.AddWithValue("@Date", Date);
                cmd.Parameters.AddWithValue("@Description", Description);

                cmd.ExecuteNonQuery();


                return true;

            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
                connection.Close();
                return false;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PaymentID"></param>
        /// <param name="MemberID"></param>
        /// <param name="PaymentType"></param>
        /// <param name="EventType"></param>
        /// <param name="PaymentSource"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        public DataTable FindPayments(int PaymentID, int MemberID, int PaymentType, float Amount, int EventType, string PaymentSource, string StartDateTime, string EndDateTime, string Description)
        {

            try
            {
                string strFilter = "where ";
                bool bUseFilter = false;

                if (PaymentID != -1)
                {
                    strFilter = strFilter + "PaymentID = " + PaymentID + " ";
                    bUseFilter = true;
                }
                if (MemberID != -1)
                {
                    strFilter = strFilter + "MemberID = " + MemberID + " ";
                    bUseFilter = true;
                }
                if (PaymentType != -1)
                {

                    //Check each parameter that was passed through and if the reimmed length is greater than 0 add it ot the query
                    if (bUseFilter)
                    {
                        strFilter = strFilter + " and PaymentTypeID = '" + PaymentType + "'";
                    }
                    else
                    {
                        strFilter = strFilter + " PaymentTypeID = '" + PaymentType + "'";
                        bUseFilter = true;
                    }
                }
                if (Amount != -1) // Pass negitive 1 to not search by
                {

                    //Check each parameter that was passed through and if the reimmed length is greater than 0 add it ot the query
                    if (bUseFilter)
                    {
                        strFilter = strFilter + " and Amount = '" + Amount + "'";
                    }
                    else
                    {
                        strFilter = strFilter + " Amount = '" + Amount + "'";
                        bUseFilter = true;
                    }
                }
                if (EventType != -1)
                {


                    if (bUseFilter)
                    {
                        strFilter = strFilter + " and EventID = '" + EventType + "'";
                    }
                    else
                    {
                        strFilter = strFilter + " EventID = " + EventType;
                        bUseFilter = true;
                    }
                }
                if (PaymentSource.Trim().Length > 0)
                {
                    PaymentSource = PaymentSource.Trim(); //Clean spaces off the beginning and end of the search term

                    if (bUseFilter)
                    {
                        strFilter = strFilter + " and  PaymentSource = '" + PaymentSource + "'";
                    }
                    else
                    {
                        strFilter = strFilter + " PaymentSource = '" + PaymentSource + "'";
                        bUseFilter = true;
                    }
                }
                if (Description.Trim().Length > 0)
                {
                    Description = Description.Trim(); //Clean spaces off the beginning and end of the search term

                    if (bUseFilter)
                    {
                        strFilter = strFilter + " and ( Description like '%" + Description + "' or Description like '%" + Description + "%' or Description like '" + Description + "%')";
                    }
                    else
                    {
                        strFilter = strFilter + "( Description like '%" + Description + "' or Description like '%" + Description + "%' or Description like '" + Description + "%')";
                        bUseFilter = true;
                    }
                }
                if (StartDateTime.Trim().Length > 0)
                {
                    StartDateTime = StartDateTime.Trim(); //Clean spaces off the beginning and end of the search term

                    if (bUseFilter)
                    {
                        strFilter = strFilter + " and Date >= '" + StartDateTime + "'";
                    }
                    else
                    {
                        strFilter = strFilter + " Date >= '" + StartDateTime + "'";
                        bUseFilter = true;
                    }
                }
                if (EndDateTime.Trim().Length > 0)
                {
                    EndDateTime = EndDateTime.Trim(); //Clean spaces off the beginning and end of the search term

                    if (bUseFilter)
                    {
                        strFilter = strFilter + " and  Date <= '" + EndDateTime + "'";
                    }
                    else
                    {
                        strFilter = strFilter + " Date <= '" + EndDateTime + "'";
                        bUseFilter = true;
                    }
                }


                //Setup the database connection
                MySqlConnection connection;

                connection = new MySqlConnection(connectionString);

                DataTable dt = new DataTable();

                //connection.Open();

                string cmdText = "SELECT MemberID as \"Member ID\",(Select FName from members where MemberID = payments.MemberID) as \"First Name\"," +
                    "(Select LName from members where MemberID = payments.MemberID) as \"Last Name\", (Select PaymentType from paymenttypes where ID = payments.PaymentTypeID) as \"Payment Type\", " +
                    "concat('$', format(Amount, 2)) as Amount, (Select Description from events where ID = EventID) as \"Event\", PaymentSource as \"Payment Source\", DATE_FORMAT(Date, '%M %d, %Y') as Date, Description from payments ";
                if (bUseFilter)
                {
                    cmdText = cmdText + strFilter;
                }

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmdText, connection);

                adapter.SelectCommand.CommandType = CommandType.Text;

                adapter.Fill(dt); //opens and closes the DB connection automatically !! (fetches from pool)

                return dt;

            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
                return null;
            }
        }

        public DataTable getMemberLevels()
        {
            DataTable dtMemberLevels = new DataTable();
            try
            {
                //Setup the database connection
                MySqlConnection connection;

                connection = new MySqlConnection(connectionString);

                string cmdText = "SELECT * from memberlevels";

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmdText, connection);

                adapter.SelectCommand.CommandType = CommandType.Text;

                adapter.Fill(dtMemberLevels); //opens and closes the DB connection automatically !! (fetches from pool)

                return dtMemberLevels;
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
                return dtMemberLevels;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MemberID"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public bool updateMemberPassword(int MemberID, string Password)
        {
            //Setup the database connection
            MySqlConnection connection;

            connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();

                string cmdText = "UPDATE members SET Password = @Password " +
                                 "where MemberID = @MemberID";
                MySqlCommand cmd = new MySqlCommand(cmdText, connection);

                //Set the parameters from the provided information

                cmd.Parameters.AddWithValue("@MemberID", MemberID);
                cmd.Parameters.AddWithValue("@Password", Password);

                cmd.ExecuteNonQuery();


                return true;

            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
                connection.Close();
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MemberID"></param>
        /// <param name="AppAccess"></param>
        /// <param name="AdminAccess"></param>
        /// <returns></returns>
        public bool updateMemberLevelAndAccess(int MemberID, int MemberLevelID, bool AppAccess, bool AdminAccess)
        {
            //Setup the database connection
            MySqlConnection connection;

            connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();

                string cmdText = "UPDATE members SET MemberLevelID = @MemberLevelID, AppAccess = @AppAccess, AdminAccess = @AdminAccess " +
                                 "where MemberID = @MemberID";
                MySqlCommand cmd = new MySqlCommand(cmdText, connection);

                //Set the parameters from the provided information

                cmd.Parameters.AddWithValue("@MemberID", MemberID);
                cmd.Parameters.AddWithValue("@MemberLevelID", MemberLevelID);
                cmd.Parameters.AddWithValue("@AppAccess", Convert.ToInt32(AppAccess));
                cmd.Parameters.AddWithValue("@AdminAccess", Convert.ToInt32(AdminAccess));

                cmd.ExecuteNonQuery();


                return true;

            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry can cause errors if the event type is not there, TODO: find new way to log errors
                connection.Close();
                return false;
            }
        }

  
        
    }

    

    
}
    
    /////////////////////////////////////////////////////////////
    //
    //Net Phase add functions to search the database based off of specified information
    //
    ////////////////////////////////////////////////////////////



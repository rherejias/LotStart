using LotStart.ViewModels;
using System;
using System.Data;

namespace LotStart.Models
{
    public class UserModels
    {
        public int AddUser(ADUserObject userObject)
        {
            //return Library.ConnectionString.returnCon.executeQuery("spAddUser", params_, CommandType.StoredProcedure);
            return Convert.ToInt32(Library.ConnectionString.returnCon.executeScalarQuery("exec spAddUser " +
                " @Username = '" + userObject.Username + "'," +
                " @LastName = '" + userObject.LastName + "'," +
                " @GivenName = '" + userObject.GivenName + "'," +
                " @EmployeeNbr = '" + userObject.EmployeeNbr + "'," +
                " @Email = '" + userObject.Email + "'," +
                " @IsActive = '" + userObject.IsActive + "'," +
                " @Department = '" + userObject.Department + "'," +
                " @AddedBy = '" + userObject.AddedBy + "'," +
                " @DateAdded = '" + userObject.DateAdded + "'," +
                " @Source = '" + userObject.Source + "'," +
                " @Type = '" + userObject.Type + "'," +
                " @ThumbnailPhoto = '" + userObject.ThumbnailPhoto + "'", CommandType.Text));
        }

        public int UpdateUser(ADUserObject userObject)
        {
            //return Library.ConnectionString.returnCon.executeQuery("spAddUser", params_, CommandType.StoredProcedure);
            return Convert.ToInt32(Library.ConnectionString.returnCon.executeScalarQuery("exec spUpdateUser " +
                " @Id = " + userObject.Id + "," +
                " @Username = '" + userObject.Username + "'," +
                " @LastName = '" + userObject.LastName + "'," +
                " @GivenName = '" + userObject.GivenName + "'," +
                " @EmployeeNbr = '" + userObject.EmployeeNbr + "'," +
                " @Email = '" + userObject.Email + "'," +
                " @IsActive = '" + userObject.IsActive + "'," +
                " @Department = '" + userObject.Department + "'," +
                " @Source = '" + userObject.Source + "'," +
                " @Type = '" + userObject.Type + "'," +
                " @ThumbnailPhoto = '" + userObject.ThumbnailPhoto + "'", CommandType.Text));
        }

        public int CheckIdFromLocal(string Email)
        {
            return Convert.ToInt32(Library.ConnectionString.returnCon.executeScalarQuery("exec spReadUserIdByEmail " +
                " @Email = '" + Email + "'", CommandType.Text));
        }

        public bool AssignPermissions(int userId)
        {
            //SqlParameter[] params_ = new SqlParameter[] {
            //    new SqlParameter("@FK", FK),
            //    new SqlParameter("@Name", Name),
            //    new SqlParameter("@Department", Department),
            //    new SqlParameter("@Status", Status),
            //    new SqlParameter("@AddedBy", AddedBy),
            //    new SqlParameter("@DateAdded", DateAdded),
            //    new SqlParameter("@ComputerName", System.Net.Dns.GetHostName()),
            //    new SqlParameter("@LoggedInUser", custom_helper.GetCurrentUser()),
            //};

            //return Library.ConnectionString.returnCon.executeQuery("spAddNominationMember", params_, CommandType.StoredProcedure);
            return false;
        }

        public DataTable GetRegisteredUsers()
        {
            return Library.ConnectionString.returnCon.executeSelectQuery("SELECT Id as Id_int, " +
                " Username as Username_string, " +
                " LastName as LastName_string, " +
                " GivenName as GivenName_string, " +
                " EmployeeNbr as EmployeeNbr_string, " +
                " Email as Email_string, " +
                " Department as Department_string, " +
                " Source as Source_string, " +
                " Type as Type_string " +
                " FROM tblUsers" +
                " ORDER BY LastName", CommandType.Text);
        }

        public string GetUserType(int userId)
        {
            return Library.ConnectionString.returnCon.executeScalarQuery("SELECT [Type] FROM TBLUSERS " +
                " WHERE Id = '" + userId + "'", CommandType.Text).ToString();
        }

        public int Username_count(string username)
        {
            return Convert.ToInt32(Library.ConnectionString.returnCon.executeScalarQuery
                ("Select count(*) from [tblUsers] where [Username] = '" + username + "'", CommandType.Text));
        }
    }
}
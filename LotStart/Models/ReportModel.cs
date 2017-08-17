using System.Data;

namespace LotStart.Models
{

    public class ReportModel
    {
        /// <summary>
        /// select from local table to get all records
        /// </summary>
        /// avillena 4/27/2017
        public DataTable GetRecordForReports()
        {
            return Library.ConnectionString.returnCon.executeSelectQuery(strQuery: "Select * from vwLogs ORDER BY DateRequired ASC, Package ASC, Item ASC, TargetCAS ASC, MoveOrderNbr ASC", type_: CommandType.Text);
        }

        /// <summary>
        /// get searched record from local database
        /// </summary>
        /// <returns>json array</returns>
        /// rherejias 4/25/2017
        public string getSearchRecord(string searchInput, string createdFrom, string createdTo, string requiredFrom, string requiredTo, string submittedFrom, string submittedTo)
        {
            string where = "";
            string and1 = "";
            string and2 = "";
            string created = "(CONVERT(date,DateCreated,101) >= CONVERT(date,'" + createdFrom + "',101) AND CONVERT(date,DateCreated,101) <= CONVERT(date,'" + createdTo + "',101))";
            string required = "(CONVERT(date,DateRequired,101) >= CONVERT(date,'" + requiredFrom + "',101) AND CONVERT(date,DateRequired,101) <= CONVERT(date,'" + requiredTo + "',101))";
            string submitted = "(CONVERT(date,DateSubmitted,101) >= CONVERT(date,'" + submittedFrom + "',101) AND CONVERT(date,DateSubmitted,101) <= CONVERT(date,'" + submittedTo + "',101))";
            string likeQuery = "where Id LIKE '%" + searchInput + "%' OR " +
                               "MoveOrderNbr LIKE '%" + searchInput + "%' OR " +
                               "Org LIKE '%" + searchInput + "%' OR " +
                               "Item LIKE '%" + searchInput + "%' OR " +
                               "TargetCAS LIKE '%" + searchInput + "%' OR " +
                               "Planner LIKE '%" + searchInput + "%' OR " +
                               "Quantity LIKE '%" + searchInput + "%' OR " +
                               "CONVERT(VARCHAR(10), DateCreated, 101) + ' ' + LTRIM(RIGHT(CONVERT(CHAR(20), DateCreated, 22), 11)) LIKE '%" + searchInput + "%' OR " +
                               "CONVERT(VARCHAR(10), DateRequired, 101) + ' ' + LTRIM(RIGHT(CONVERT(CHAR(20), DateRequired, 22), 11)) LIKE '%" + searchInput + "%' OR " +
                               "SubInventory LIKE '%" + searchInput + "%' OR " +
                               "Package LIKE '%" + searchInput + "%' OR " +
                               "[User] LIKE '%" + searchInput + "%' OR " +
                               "CONVERT(VARCHAR(10), DateSubmitted, 101) + ' ' + LTRIM(RIGHT(CONVERT(CHAR(20), DateSubmitted, 22), 11)) LIKE '%" + searchInput + "%' OR " +
                               "[Status] LIKE '%" + searchInput + "%' OR " +
                               "LotNumber LIKE '%" + searchInput + "%'";

            if (searchInput == "" || searchInput == null)
            {
                if ((searchInput == "" || searchInput == null) && (createdFrom == "" || createdFrom == null) && (requiredFrom == "" || requiredFrom == null) && (submittedFrom == "" || submittedFrom == null))
                    where = "";
                else
                    where = "where ";
            }

            if ((requiredFrom == "" || requiredFrom == null) && (submittedFrom == "" || submittedFrom == null))
                and1 = "";
            else
                and1 = "AND ";

            if (submittedFrom == "" || submittedFrom == null)
                and2 = "";
            else
                and2 = "AND ";

            return Library.ConnectionString.returnCon.local_DB_reader("Select * from vwLogs " + where +
                ((searchInput == "" || searchInput == null) ? "" : likeQuery) +
                ((createdFrom == "" || createdFrom == null) ? "" : created + and1) +
                ((requiredFrom == "" || requiredFrom == null) ? "" : required + and2) +
                ((submittedFrom == "" || submittedFrom == null) ? "" : submitted) +
                " ORDER BY DateRequired ASC, Package ASC, Item ASC, TargetCAS ASC, MoveOrderNbr ASC"
                , CommandType.Text);
        }
    }


}
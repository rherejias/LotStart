using LotStart.ViewModels;
using System.Data;
using System.Data.SqlClient;

namespace LotStart.Models
{
    public class AuditTrailModels
    {
        /// <summary>
        /// get all audit trail records from local table
        /// </summary>
        /// <returns>json array</returns>
        /// rherejias 4/25/2017
        public string getRecord()
        {
            return Library.ConnectionString.returnCon.local_DB_reader("spSelectAllAuditTrail", CommandType.StoredProcedure);
        }

        /// <summary>
        /// get searched record from local database
        /// </summary>
        /// <returns>json array</returns>
        /// rherejias 4/25/2017
        public string getSearchRecord(string searchInput)
        {
            SqlParameter[] params_ = new SqlParameter[] {
                new SqlParameter("@searchInput", searchInput)
            };
            return Library.ConnectionString.returnCon.local_DB_reader("spSearch", params_, CommandType.StoredProcedure);
        }

        /// <summary>
        /// add record to audit trail 
        /// </summary>
        /// rherejias 4/26/2017
        public void Audit(AuditTrailObject obj)
        {
            SqlParameter[] params_ = new SqlParameter[] {
                 new SqlParameter("@ProjectCode", "LS"),
                 new SqlParameter("@Module", obj.Module),
                 new SqlParameter("@Operation", obj.Operation),
                 new SqlParameter("@Object", obj.Object),
                 new SqlParameter("@ObjectId", obj.ObjectId),
                 new SqlParameter("@ObjectCode", obj.ObjectCode),
                 new SqlParameter("@UserCode", obj.UserCode),
                 new SqlParameter("@IP", obj.IP),
                 new SqlParameter("@MAC", obj.MAC),
                 new SqlParameter("@DateAdded", obj.DateAdded)
            };
            Library.ConnectionString.returnCon.local_DB_reader("spAddAuditTrail", params_, CommandType.StoredProcedure);
        }

        /// <summary>
        /// get user code
        /// </summary>
        /// <returns>code</returns>
        /// rherejias 2/26/2017
        public string getUserCode(string id)
        {
            return Library.ConnectionString.returnCon.executeScalarQuery("select Code from tblUsers where username='" + id + "'", CommandType.Text).ToString();
        }

        /// <summary>
        /// get user id
        /// </summary>
        /// <returns>id</returns>
        /// rherejias 2/26/2017
        public string getUserID(string id)
        {
            return Library.ConnectionString.returnCon.executeScalarQuery("select Id from tblUsers where username='" + id + "'", CommandType.Text).ToString();
        }

        /// <summary>
        /// log submitted moveorders
        /// </summary>
        /// <param name="obj">log object</param>
        /// author rherejias 5/8/2017
        public void Logs(LogsObject obj)
        {
            SqlParameter[] params_ = new SqlParameter[] {
                 new SqlParameter("@MoveOrderNbr", obj.MoveOrderNbr),
                 new SqlParameter("@Org", obj.Org),
                 new SqlParameter("@Item", obj.Item),
                 new SqlParameter("@LotNumber", obj.LotNumber),
                 new SqlParameter("@TargetCAS", obj.TargetCAS),
                 new SqlParameter("@Planner", obj.Planner),
                 new SqlParameter("@Quantity", obj.Quantity),
                 new SqlParameter("@DateCreated", obj.DateCreated),
                 new SqlParameter("@DateRequired", obj.DateRequired),
                 new SqlParameter("@SubInventory", obj.SubInventory),
                 new SqlParameter("@Package", obj.Package),
                 new SqlParameter("@User", obj.User),
                 new SqlParameter("@DateSubmitted", obj.DateSubmitted),
                 new SqlParameter("@Status", obj.Status)
            };
            Library.ConnectionString.returnCon.local_DB_reader("spAddLogs", params_, CommandType.StoredProcedure);
        }

        /// <summary>
        /// log submitted item
        /// </summary>
        /// <param name="obj">item object</param>
        /// author rherejias 5/8/2017 
        public void ItemLogs(ItemObject obj)
        {
            SqlParameter[] params_ = new SqlParameter[] {
                 new SqlParameter("@MoveOrderNbr", obj.MoveOrder),
                 new SqlParameter("@Lot", obj.Lot),
                 new SqlParameter("@Quantity", obj.Quantity),
                 new SqlParameter("@Locator", obj.Locator)
            };
            Library.ConnectionString.returnCon.local_DB_reader("spAddItemLogs", params_, CommandType.StoredProcedure);
        }

    }
}
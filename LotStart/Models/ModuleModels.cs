using System.Data;

namespace LotStart.Models
{
    public class ModuleModels
    {
        public DataTable GetAvailabeModules()
        {
            return Library.ConnectionString.returnCon.executeSelectQuery("SELECT * FROM tblModules WHERE IsActive='True' order by ParentId;", CommandType.Text);
        }
        public DataTable GetUserModules(int EntityId)
        {
            return Library.ConnectionString.returnCon.executeSelectQuery("SELECT ParentId, ObjectId FROM vwUserModules " +
                " WHERE EntityType = 'user' " +
                " AND EntityId = " + EntityId +
                " AND Category = 'module' " +
                " AND HasAccess = 1 " +
                " AND  IsActive = 1", CommandType.Text);
        }
    }
}
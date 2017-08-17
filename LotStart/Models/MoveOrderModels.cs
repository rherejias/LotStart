using System.Configuration;
using System.Data;

namespace LotStart.Models
{
    public class MoveOrderModels
    {
        /// <summary>
        /// get all record for move order list
        /// </summary>
        /// <returns>json string</returns>
        /// rherejias 5/9/2017
        public string getRecord(string moveOrder, string item, string planner, string requiredFrom, string requiredTo, string createdFrom, string createdTo, string pkg, string fromSearch)
        {
            return Library.ConnectionString.returnCon.reader(
                "select " +
                "moh.request_number MoveOrderNbr, " +
                "mp.organization_code Org, " +
                "msi.segment1 Item, " +
                "itm_det.segment1 TargetCAS, " +
                "fu.user_name Planner, " +
                "sum(mol.quantity) Quantity, " +
                "cast((from_tz(CAST(moh.creation_date AS timestamp),'US/Eastern') AT LOCAL) as date) DateCreated, " +
                "cast((from_tz(CAST(moh.date_required AS timestamp),'US/Eastern') AT LOCAL) as date) DateRequired, " +
                "mol.to_subinventory_code SubInventory, " +
                "mcb.segment1 package, " +
                "decode(mol.line_status, 1, 'Incomplete', 2, 'Pending Approval', 3, 'Approved', 4, 'Not Approved', 5, 'Closed', 6, 'Canceled', 7, 'Pre Approved', 8, 'Partially Approved') STATUS " +
                "from " +
                "inv.mtl_txn_request_headers moh, " +
                "inv.mtl_txn_request_lines mol, " +
                "inv.mtl_system_items_b msi, " +
                "inv.mtl_item_locations mil, " +
                "mtl_parameters mp, " +
                "mtl_categories_b mcb, " +
                "mtl_item_categories mic, " +
                "mtl_category_sets_tl mcst, " +
                "fnd_user fu, " +
                "(select b.segment1, to_char(b.inventory_item_id) attribute1, b.organization_id from inv.mtl_system_items_b b where " +
                "b.inventory_item_id in (select aa.attribute1 from mtl_txn_request_headers aa, mtl_parameters ab  where aa.organization_id = ab.organization_id " +
                "and ab.organization_code = '" + ConfigurationManager.AppSettings["Org"] + "' and REGEXP_LIKE(aa.attribute1, '^[[:digit:]]+$'))) itm_det " +
                "where moh.header_id = mol.header_id " +
                "and mol.organization_id = msi.organization_id " +
                "and mol.inventory_item_id = msi.inventory_item_id " +
                "and mol.to_locator_id = mil.inventory_location_id " +
                "and mol.organization_id = mp.organization_id " +
                "and mp.organization_code = '" + ConfigurationManager.AppSettings["Org"] + "' " +
                "and mol.created_by = fu.user_id " +
                "and moh.attribute1 = itm_det.attribute1 " +
                "and moh.organization_id = itm_det.organization_id " +
                 "and mic.category_id = mcb.category_id " +
                 "and mic.category_set_id = mcst.category_set_id " +
                 "and mcst.category_set_name = 'PACKAGE CODE' " +
                 "and mic.organization_id = mol.organization_id " +
                 "and mic.inventory_item_id = mol.inventory_item_id " +
                 "and mol.line_status in (3) " +
                 "and nvl(mol.quantity_detailed,0) > 0 " +
                 "and nvl(mol.quantity_delivered,0) = 0 " +
                 ((moveOrder == "" || moveOrder == null) ? "" : "and moh.request_number = '" + moveOrder + "' ") +
                 ((item == "" || item == null) ? "" : "and msi.segment1 = '" + item + "' ") +
                 ((planner == "" || planner == null) ? "" : "and fu.user_name = '" + planner + "' ") +
                 ((requiredFrom == "" || requiredFrom == null) ? "" : "and TRUNC(cast((from_tz(CAST(moh.date_required AS timestamp),'US/Eastern') AT LOCAL) as date)) BETWEEN to_date('" + requiredFrom + "', 'MM/DD/YYYY') AND to_date('" + requiredTo + "', 'MM/DD/YYYY')") +
                 ((createdFrom == "" || createdTo == null) ? "" : "and TRUNC(cast((from_tz(CAST(moh.creation_date AS timestamp),'US/Eastern') AT LOCAL) as date)) BETWEEN to_date('" + createdFrom + "', 'MM/DD/YYYY') AND to_date('" + createdTo + "', 'MM/DD/YYYY')") +
                 ((pkg == "" || pkg == null) ? "" : "and mcb.segment1 = '" + pkg + "' ") +
                 //ASC, mcb.segment1 ASC, msi.segment1 ASC, itm_det.segment1 ASC, moh.request_number ASC " +
                 " group by " +
                 "moh.request_number, " +
                 "mp.organization_code, " +
                 "msi.segment1, " +
                 "itm_det.segment1, " +
                 "fu.user_name, " +
                 "moh.creation_date, " +
                 "moh.date_required, " +
                 "mol.to_subinventory_code, " +
                 "mcb.segment1, " +
                 "decode(mol.line_status, 1, 'Incomplete', 2, 'Pending Approval', 3, 'Approved', 4, 'Not Approved', 5, 'Closed', 6, 'Canceled', 7, 'Pre Approved', 8, 'Partially Approved') order by " +
                 "moh.date_required ASC, mcb.segment1 ASC, msi.segment1 ASC, itm_det.segment1 ASC, moh.request_number ASC ", CommandType.Text);
        }


    }

}
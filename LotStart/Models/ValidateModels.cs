using System.Configuration;
using System.Data;

namespace LotStart.Models
{
    public class ValidateModels
    {
        /// <summary>
        /// oracle query 
        /// </summary>
        /// <param name="moveOrder">to specifiy which item will be displayed</param>
        /// <returns>json array</returns>
        /// rhererjias 5/9/2017
        public string GetRecord(string moveOrder)
        {
            return Library.ConnectionString.returnCon.reader(
                "select " +
                "mil.segment1||'.'||mil.segment2||'.'||mil.segment3||'.'||mil.segment4 locator, " +
                "msi.segment1 Item, " +
                "mol.lot_number lotnumber, " +
                "mol.quantity quantity " +
                "from " +
                "inv.mtl_txn_request_headers moh, " +
                "inv.mtl_txn_request_lines mol, " +
                "inv.mtl_system_items_b msi, " +
                "inv.mtl_item_locations mil, " +
                "mtl_parameters mp, " +
                "inv.mtl_material_transactions_temp alloc," +
                "fnd_user fu " +
                "where moh.header_id = mol.header_id " +
                "and mol.organization_id = msi.organization_id " +
                "and mol.inventory_item_id = msi.inventory_item_id " +
                "and mol.line_id = alloc.trx_source_line_id " +
                "and alloc.transaction_type_id = 64 " +
                "and alloc.locator_id = mil.inventory_location_id " +
                "and mol.organization_id = mp.organization_id " +
                "and mol.created_by = fu.user_id " +
                "and moh.request_number = '" + moveOrder + "'", CommandType.Text);
        }



        /// <summary>
        /// author : avillena | desc : to get the lot details of move order search into validation page (oracle query) | date : 04/25/2017 | version : 1.0
        /// </summary>
        /// <param name="moveOrder"></param>
        /// <returns>json array</returns>
        public string GetRecordofLotDetails(string moveOrder)
        {

            return Library.ConnectionString.returnCon.reader(
                "select " +
                "moh.request_number MoveOrderNbr, " +
                "mp.organization_code Org, " +
                "msi.segment1 Item, " +
                "itm_det.segment1 TargetCAS, " +
                "fu.user_name Planner, " +
                "mol.quantity Quantity, " +
                "to_char(cast((from_tz(CAST(mol.creation_dateAS timestamp), 'US/Eastern') AT LOCAL) as date), 'MM/DD/YYYY HH24:MI AM') DateCreated, " +
                "to_char(cast((from_tz(CAST(mol.date_required timestamp), 'US/Eastern') AT LOCAL) as date), 'MM/DD/YYYY HH24:MI AM') DateRequired, " +
                "mol.to_subinventory_code SubInventory, " +
                "decode(mol.line_status, 1, 'Incomplete', 2, 'Pending Approval', 3, 'Approved', 4, 'Not Approved', 5, 'Closed', 6, 'Canceled', 7, 'Pre Approved', 8, 'Partially Approved') STATUS " +
                "from " +
                "inv.mtl_txn_request_headers moh, " +
                "inv.mtl_txn_request_lines mol, " +
                "inv.mtl_system_items_b msi, " +
                "inv.mtl_item_locations mil, " +
                "mtl_parameters mp, " +
                "fnd_user fu, " +
                "(select b.segment1, to_char(b.inventory_item_id) attribute1 from inv.mtl_system_items_b b where " +
                "b.inventory_item_id in (select aa.attribute1 from mtl_txn_request_headers aa, mtl_parameters ab  where aa.organization_id = ab.organization_id " +
                "and ab.organization_code = '" + ConfigurationManager.AppSettings["Org"] + "' and REGEXP_LIKE(aa.attribute1, '^[[:digit:]]+$'))) itm_det " +
                "where moh.header_id = mol.header_id " +
                "and mol.organization_id = msi.organization_id " +
                "and mol.inventory_item_id = msi.inventory_item_id " +
                "and mol.to_locator_id = mil.inventory_location_id " +
                "and mol.organization_id = mp.organization_id " +
                "and mp.organization_code = '" + ConfigurationManager.AppSettings["Org"] + "' " +
                "and mol.created_by = fu.user_id " +
                "and mol.line_status in (1,3,5) " +
                "and moh.attribute1 = itm_det.attribute1 " +
                ((moveOrder == "" || moveOrder == null) ? "" : "and moh.request_number = '" + moveOrder + "' ")
                , CommandType.Text);
        }




        /// <summary>
        /// author : avillena | desc : get all records on test staging from oracle and to include to email notification (oracle query) | date : 04/25/2017 | version : 1.0
        /// </summary>
        /// <param name="moveOrder"></param>
        /// <returns>json array</returns>
        public string GetRecordofTestStage_ForEmail()
        {

            return Library.ConnectionString.returnCon.reader(




                "select " +
                "mrh.request_number MoveOrderNbr, " +
                "mp.organization_code Org, " +
                "msi.segment1 Item, " +
                "moh.revision rev, " +
                "fu.user_name Planner, " +
                "moh.transaction_quantity Quantity, " +
                "to_char(cast((from_tz(CAST(mrh.date_required AS timestamp), 'US/Eastern') AT LOCAL) as date), 'MM/DD/YYYY HH24:MI AM') DateRequired, " +
                "moh.lot_number, " +
                "mcb.segment1 package_code, " +
                "to_char(cast((from_tz(CAST(mmt.creation_date AS timestamp), 'US/Eastern') AT LOCAL) as date), 'MM/DD/YYYY HH24:MI AM') date_transacted, " +
                "mof.user_name last_updated_by, " +
                "cas.segment1 cas_item " +
                "from " +
                "mtl_onhand_quantities moh, " +
                "(select header_id, request_number, date_required, last_updated_by, to_number(attribute1) cas_itm from mtl_txn_request_headers " +
                "where transaction_type_id = 64 and move_order_type = 1 and REGEXP_LIKE(attribute1, '^[[:digit:]]+$')) mrh, " +
                "mtl_item_locations mil, " +
                "mtl_material_transactions mmt, " +
                "mtl_parameters mp, " +
                "fnd_user fu, " +
                "mtl_categories_b mcb, " +
                "mtl_item_categories mic, " +
                "mtl_category_sets_tl mcst, " +
                "mtl_system_items_b msi, " +
                "fnd_user mof, " +
                "mtl_system_items_b cas " +
                "where " +
                "moh.locator_id = mil.inventory_location_id " +
                "and mil.segment1 || '.' || mil.segment2 || '.' || mil.segment3 || '.' || mil.segment4 = 'TEST STAGE.R00.CA00.S00' " +
                "and moh.create_transaction_id = mmt.transaction_id " +
                "and mmt.transaction_source_id = mrh.header_id " +
                "and mmt.organization_id = mp.organization_id " +
                "and moh.created_by = fu.user_id " +
                "and mmt.inventory_item_id = msi.inventory_item_id " +
                "and mmt.organization_id = msi.organization_id " +
                "and mic.category_set_id = mcst.category_set_id " +
                "and mic.category_id = mcb.category_id " +
                "and mcst.category_set_name = 'PACKAGE CODE' " +
                "and mic.organization_id = mmt.organization_id " +
                "and mic.inventory_item_id = mmt.inventory_item_id " +
                "and mrh.last_updated_by = mof.user_id " +
                "and mrh.cas_itm = cas.inventory_item_id " +
                "and cas.organization_id = 108 ORDER BY mrh.date_required ASC, mcb.segment1 ASC, msi.segment1 ASC, cas.segment1 ASC, mrh.request_number ASC", CommandType.Text);

        }




        /// author : avillena | desc : get all records from oracle for email notification (oracle query) | date : 04/25/2017 | version : 1.0
        public string GetRecordFromOracle_ForEmailWareHouse()
        {

            return Library.ConnectionString.returnCon.reader(



                    "select " +
                    "mrh.request_number MoveOrderNbr, " +
                    "mp.organization_code Org, " +
                    "msi.segment1 SourceItem, " +
                    "mrh.revision Sourcerev, " +
                    "cas.segment1 cas_item, " +
                    "mcb.segment1 package_code, " +
                    "mrh.lot_number, " +
                    "mrh.quantity_detailed Quantity, " +
                    // "TO_CHAR(mrh.date_required, 'DD-MON-YYYY HH24:MI') DateRequiredUS,
                    "to_char(cast((from_tz(CAST(mrh.date_required AS timestamp), 'US/Eastern') AT LOCAL) as date), 'MM/DD/YYYY HH24:MI AM') DateRequired, " +
                    "fu.user_name Planner, " +
                    "mof.user_name last_updated_by, " +
                    // TO_CHAR(mrh.last_update_date, 'DD-MON-YYYY HH24:MI') LastUpdatedUS,
                    "to_char(cast((from_tz(CAST(mrh.last_update_date AS timestamp), 'US/Eastern') AT LOCAL) as date),'MM/DD/YYYY HH24:MI AM') LastUpdated " +
                    "from " +
                    "(select h.header_id, h.request_number, l.quantity_detailed, l.inventory_item_id, " +
                            "l.organization_id, l.revision, l.lot_number, " +
                            "h.date_required, l.created_by, l.last_update_date, l.last_updated_by, to_number(h.attribute1) cas_itm " +
                    "from mtl_txn_request_headers h, MTL_TXN_REQUEST_LINES l " +
                    "where h.transaction_type_id = 64 " +
                     "and h.move_order_type = 1 " +
                     "and REGEXP_LIKE(h.attribute1, '^[[:digit:]]+$') " +
                    "and l.header_id = h.header_id " +
                    "and l.organization_id = h.organization_id " +
                    "and NVL(l.quantity_detailed, 0) > 0 " + //-- allocated already " +
                    "and NVL(l.quantity_delivered, 0) = 0 " + // -- not yet delivered " +
                    ") mrh, " +
                    "mtl_parameters mp, " +
                    "fnd_user fu, " +
                    "mtl_categories_b mcb, " +
                    "mtl_item_categories mic, " +
                    "mtl_category_sets_tl mcst, " +
                    "mtl_system_items_b msi, " +
                    "fnd_user mof, " +
                    "mtl_system_items_b cas " +
                    "where 1 = 1 " +
                    "and mp.organization_id = mrh.organization_id " +
                    "and msi.inventory_item_id = mrh.inventory_item_id " +
                    "and msi.organization_id = mrh.organization_id " +
                    "and fu.user_id = mrh.created_by " +
                    "and mic.organization_id = mrh.organization_id " +
                    "and mic.inventory_item_id = mrh.inventory_item_id " +
                    "and mic.category_set_id = mcst.category_set_id " +
                    "and mic.category_id = mcb.category_id " +
                    "and mcst.category_set_name = 'PACKAGE CODE' " +
                    "and mrh.last_updated_by = mof.user_id " +
                    "and cas.inventory_item_id = mrh.cas_itm " +
                    "and cas.organization_id = mrh.organization_id " +
                    "and cas.organization_id = mrh.organization_id " +
                    "and mrh.date_required <= sysdate + (1 / 24 / 2) " + //-- 30 minutes lookahead
                    "order by DateRequired, package_code, Sourceitem, lot_number", CommandType.Text);

        }

        /// to get the email address for the recipient of email notification
        /// rherejias 5/9/2017
        public DataTable GetEmailAddressforEmailNotif()
        {
            return Library.ConnectionString.returnCon.executeSelectQuery(strQuery: "SELECT [EmailAddress] FROM [tbEmailRecipient] WHERE [IsActive] = 1 AND [Group] = '" + ConfigurationManager.AppSettings["test"].ToString() + "'", type_: CommandType.Text);
        }

        /// author : avillena | desc : get all records from oracle for email notification (oracle query) | date : 05/19/2017 | version : 1.0
        public DataTable GetEmailAddressforEmailNotifWarehouse()
        {
            return Library.ConnectionString.returnCon.executeSelectQuery(strQuery: "SELECT [EmailAddress] FROM [tbEmailRecipient] WHERE [IsActive] = 1 AND [Group] = '" + ConfigurationManager.AppSettings["warehouse"].ToString() + "'", type_: CommandType.Text);
        }
    }

}
var dateCreatedFrom, dateCreatedTo;
var dateRequiredFrom, dateRequiredTo;
var dateSubmittedFrom, dateSubmittedTo;

//rherejias android softkeyboard Submit button
$(document).delegate("form","submit", function (e) {
    e.preventDefault();
    $.ajax({
        url: '/Report/GetSearch',
        dataType: 'Text',
        data: {
            searchParam: $("#txtSearch").val(),
            createdFrom: dateCreatedFrom,
            createdTo: dateCreatedTo,
            requiredFrom: dateRequiredFrom,
            requiredTo: dateRequiredTo,
            submittedFrom: dateSubmittedFrom,
            submittedTo: dateSubmittedTo
        },
        beforeSend: function () {
            $(".loader").css('display', 'inline');
        },
        success: function (response) {
            console.log(response);
            //get total rows of data
            //var array = JSON.parse(response);
            //alert(array.length);

            if (response == "") {
                //oracle database doesn't return any record
                $("#mymodal").modal("hide");
                $("#jqxgrid_Reports").jqxGrid('clear');

                //clear calendar values
                dateCreatedFrom = "";
                dateCreatedTo = "";
                dateRequiredFrom = "";
                dateRequiredTo = "";
                dateSubmittedFrom = "";
                dateSubmittedTo = "";
            }
            else {
                //oracle returned data successfully
                $("#mymodal").modal("hide");

                var source = {
                    datatype: "json",
                    datafields: [
                            { name: 'MoveOrderNbr', type: 'string' },
                            { name: 'Org', type: 'string' },
                            { name: 'Item', type: 'string' },
                            { name: 'LotNumber', type: 'string' },
                            { name: 'TargetCAS', type: 'string' },
                            { name: 'Planner', type: 'string' },
                            { name: 'Quantity', type: 'string' },
                            { name: 'DateCreated', type: 'string' },
                            { name: 'DateRequired', type: 'string' },
                            { name: 'SubInventory', type: 'string' },
                            { name: 'Package', type: 'string' },
                            { name: 'User', type: 'string' },
                            { name: 'DateSubmitted', type: 'string' },
                            { name: 'Status', type: 'string' },
                    ],
                    localdata: response

                };
                var dataAdapter = new $.jqx.dataAdapter(source);
                $("#jqxgrid_Reports").jqxGrid({ source: dataAdapter });

                //clear calendar values
                dateCreatedFrom = "";
                dateCreatedTo = "";
                dateRequiredFrom = "";
                dateRequiredTo = "";
                dateSubmittedFrom = "";
                dateSubmittedTo = "";
            }
        }
    });
});

//export btn onclick
//rherejias 5/9/2017
$(document).delegate("#Export", "click", function () {
    $("#jqxgrid_Reports").jqxGrid('exportdata', 'xls', 'LotStartReport');
    $.ajax({
        url: '/AuditTrail/doAudit',
        type: 'get',
        dataType: 'json',
        data: {
            module: 'REPORTS',
            operation: 'EXPORT',
            object: 'tblLogs',
            objectId: '0',
            objectCode: '0'
        }
    });
});

$(document).ready(function () {
    //initialize grid
    var source =
    {
        datatype: "json",
        datafields: [
            
            { name: 'MoveOrderNbr', type: 'string' },
            { name: 'Org', type: 'string' },
            { name: 'Item', type: 'string' },
            { name: 'LotNumber', type: 'string' },
            { name: 'TargetCAS', type: 'string' },
            { name: 'Planner', type: 'string' },
            { name: 'Quantity', type: 'string' },
            { name: 'DateCreated', type: 'string' },
            { name: 'DateRequired', type: 'string' },
            { name: 'SubInventory', type: 'string' },
            { name: 'Package', type: 'string' },
            { name: 'User', type: 'string' },
            { name: 'DateSubmitted', type: 'string' },
            { name: 'Status', type: 'string' },
        ],
        url: '/Report/GetReport'
    };
    var dataAdapter = new $.jqx.dataAdapter(source, {
        downloadComplete: function (data, status, xhr) { },
        loadComplete: function (data) { },
        loadError: function (xhr, status, error) { }
    });
    $("#jqxgrid_Reports").jqxGrid(
       {
           theme: window.gridTheme,
           width: '100%',
           source: dataAdapter,
           pageable: true,
           autoheight: true,
           sortable: true,
           altrows: true,
           columnsresize: true,
           columnsautoresize: true,
           showstatusbar: true,
           //rendertoolbar: function (toolbar) {
           //    var container = $("<div style='margin: 3px;'></div>");

           //    input = $("<input class='jqx-input jqx-widget-content jqx-rc-all' id='txtSearch' type='text' style='height: 23px; float: left; width: 223px;' placeholder='Search...'/>");

           //    toolbar.append(container);
           //    container.append(input);
           //},
           renderstatusbar: function (statusbar) {
               var container = $("<div style='margin: 3px;'></div>");

               input = $("<button style='margin-left:5px;font-size:13px; font-family: arial;' class='btnx btn btn-success' id='Export'><i class='fa fa-file-excel-o' aria-hidden='true'></i> Export to Excel </button>");

               statusbar.append(container);
               container.append(input);
               $('.btnx').jqxButton({
               });
           },
           columns: [
               { text: 'Move Order Number', datafield: 'MoveOrderNbr', width: '16%' },
               { text: 'Org', datafield: 'Org', width: '5%' },
               { text: 'Item', datafield: 'Item', width: '10%' },
               { text: 'Lot Number', datafield: 'LotNumber', width: '10%' },
               { text: 'Target CAS', datafield: 'TargetCAS', width: '14%' },
               { text: 'Planner', datafield: 'Planner' , width: '8%'},
               { text: 'Quantity', datafield: 'Quantity', hidden: true },
               { text: 'Date Created', datafield: 'DateCreated', width: '14%' },
               { text: 'Date Required', datafield: 'DateRequired', width: '14%' },
               { text: 'Sub Inventory', datafield: 'SubInventory', width: '12%' },
               { text: 'Package', datafield: 'Package', width: '7%' },
               { text: 'User', datafield: 'User' , width: '6%'},
               { text: 'Date Submitted', datafield: 'DateSubmitted', width: '14%' },
               { text: 'Status', datafield: 'Status', hidden: true }
           ]
       });
});

//Search Filter Modal 
//rherejias 5/9/2017
$("#searchPanel").click(function () {

    var modal = '<div class="modal fade" id="mymodal" role="dialog" >';
    modal += '<div class="modal-dialog modal-lg">';
    modal += ' <div class="modal-content">';

    modal += '<div class="modal-header" style="background-color:#76cad4; color:#ffffff">';
    modal += '<h4 class="modal-title">Search Filters</h4>';
    modal += '<div class="loader"  style="float:right; top:-26px; position:relative; display:none"></div>';
    modal += '</div>';

    modal += '<div class="modal-body">';
    modal += '<form>';
    modal += '<div class="row">';
    modal += '       <div class="col-lg-12" style="margin-top:20px;">';
    modal += '                       <div class="row">';
    modal += '                          <div class="col-md-12">';
    modal += '                                       <div id="divItem" class="col-sm-12">';
    modal += '                                          <div><label><b>Search</b></label></div>';
    modal += '                                           <input id="txtSearch" type="text" class="form-control" placeholder="Smart Search.."/>';
    modal += '                                       </div>';
    modal += '                          </div>';
    modal += '                           <div class="col-md-12" style="margin-top:15px;">';
    modal += '                               <div class="row">';
    modal += '                                   <div class="col-md-12">';
    modal += '                                       <div id="divDateCreated" class="col-md-4" style="text-align:center;">';
    modal += '                                           <div style="text-align:center;"><label for="dateCreated"><b>Date Created</b></label></div>';
    modal += '                                           <center><div id="dateCreated"></div></center>';
    modal += '                                       </div>';
    modal += '                                       <div id="divDateRequired" class="col-md-4" style="text-align:center;">';
    modal += '                                           <div style="text-align:center;"><label for="dateRequired"><b>Date Required</b></label></div>';
    modal += '                                           <center><div id="dateRequired"></div></center>';
    modal += '                                       </div>';
    modal += '                                       <div id="divDateSubmitted" class="col-md-4" style="text-align:center;">';
    modal += '                                           <div style="text-align:center;"><label for="dateRequired"><b>Date Submitted</b></label></div>';
    modal += '                                           <center><div id="dateSubmitted"></div></center>';
    modal += '                                       </div>';
    modal += '                                   </div>';
    modal += '                               </div>';
    modal += '                           </div>';
    modal += '                       </div>';
    modal += '           </div>';
    modal += '  </div>';
    modal += '</form>';
    modal += '</div>';

    modal += '<div class="modal-footer">';
    modal += '<div class="row">';
    modal += '<button class="btn btn-default" data-dismiss="modal"';
    modal += 'style="width: 100px;">';
    modal += 'Cancel</button>';
    modal += '<button type="button" id="btnSearch" style="width: 100px; margin-right:5%;" class="btn btn-success">Search</button>';
    modal += '</div>';
    modal += '</div>';

    modal += '</div>';
    modal += '</div>';
    modal += '</div>';

    $("#form_modal").html(modal);
    $("#mymodal").modal("show");
    $("#mymodal").css('z-index', '1000000');

    //Calendar Initialization dateRequired
    $("#dateRequired").jqxCalendar({ width: 220, height: 220, selectionMode: 'range' });
    $("#dateRequired").on('change', function (event) {
        //get range string
        var selection = event.args.range;
        dateRequiredFrom = selection.from.toLocaleDateString();
        dateRequiredTo = selection.to.toLocaleDateString();
    });

    //Calendar Initialization dateCreated
    $("#dateCreated").jqxCalendar({ width: 220, height: 220, selectionMode: 'range' });
    $("#dateCreated").on('change', function (event) {
        //get range string
        var selection = event.args.range;
        dateCreatedFrom = selection.from.toLocaleDateString();
        dateCreatedTo = selection.to.toLocaleDateString();
    });

    //Calendar Initialization dateCreated
    $("#dateSubmitted").jqxCalendar({ width: 220, height: 220, selectionMode: 'range' });
    $("#dateSubmitted").on('change', function (event) {
        //get range string
        var selection = event.args.range;
        dateSubmittedFrom = selection.from.toLocaleDateString();
        dateSubmittedTo = selection.to.toLocaleDateString();
    });
});

//Search button onclick
//rherejias 5/9/2017
$(document).delegate("#btnSearch", "click", function () {
    $.ajax({
        url: '/Report/GetSearch',
        dataType: 'Text',
        data: {
            searchParam: $("#txtSearch").val(),
            createdFrom: dateCreatedFrom,
            createdTo: dateCreatedTo,
            requiredFrom: dateRequiredFrom,
            requiredTo: dateRequiredTo,
            submittedFrom: dateSubmittedFrom,
            submittedTo: dateSubmittedTo
        },
        beforeSend: function () {
            $(".loader").css('display', 'inline');
        },
        success: function (response) {
            console.log(response);
            //get total rows of data
            //var array = JSON.parse(response);
            //alert(array.length);

            if (response == "") {
                //oracle database doesn't return any record
                $("#mymodal").modal("hide");
                $("#jqxgrid_Reports").jqxGrid('clear');

                //clear calendar values
                dateCreatedFrom = "";
                dateCreatedTo = "";
                dateRequiredFrom = "";
                dateRequiredTo = "";
                dateSubmittedFrom = "";
                dateSubmittedTo = "";
            }
            else {
                //oracle returned data successfully
                $("#mymodal").modal("hide");

                var source = {
                    datatype: "json",
                    datafields: [
                            { name: 'MoveOrderNbr', type: 'string' },
                            { name: 'Org', type: 'string' },
                            { name: 'Item', type: 'string' },
                            { name: 'LotNumber', type: 'string' },
                            { name: 'TargetCAS', type: 'string' },
                            { name: 'Planner', type: 'string' },
                            { name: 'Quantity', type: 'string' },
                            { name: 'DateCreated', type: 'string' },
                            { name: 'DateRequired', type: 'string' },
                            { name: 'SubInventory', type: 'string' },
                            { name: 'Package', type: 'string' },
                            { name: 'User', type: 'string' },
                            { name: 'DateSubmitted', type: 'string' },
                            { name: 'Status', type: 'string' },
                    ],
                    localdata: response

                };
                var dataAdapter = new $.jqx.dataAdapter(source);
                $("#jqxgrid_Reports").jqxGrid({ source: dataAdapter });

                //clear calendar values
                dateCreatedFrom = "";
                dateCreatedTo = "";
                dateRequiredFrom = "";
                dateRequiredTo = "";
                dateSubmittedFrom = "";
                dateSubmittedTo = "";
            }
        }
    });
});
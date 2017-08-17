//global variables
//rherejias 5/9/2017
var dateCreatedFrom, dateCreatedTo;
var dateRequiredFrom, dateRequiredTo;
var checkBoxVal = [];
var moveOrder = "";
var item = "";
var planner = "";
var pkg = "";
var createdFrom = "";
var createdTo = "";
var requiredFrom = "";
var requiredTo = "";

$(document).ready(function () {
    //rherejias android softkeyboard Submit button
    $(document).delegate("form", "submit", function (e) {
        e.preventDefault();
        moveOrder = $("#txtMoveNumber").val();
        item = $("#txtItem").val();
        planner = $("#txtPlanner").val();
        pkg = $("#txtPKG").val();
        createdFrom = ((dateCreatedFrom == "" || dateCreatedFrom == null) ? "" : dateCreatedFrom);
        createdTo = ((dateCreatedTo == "" || dateCreatedTo == null) ? "" : dateCreatedTo);
        requiredFrom = ((dateRequiredFrom == "" || dateRequiredFrom == null) ? "" : dateRequiredFrom);
        requiredTo = ((dateRequiredTo == "" || dateRequiredTo == null) ? "" : dateRequiredTo);

        $.ajax({
            url: '/MoveOrder/GetSearchedMoveOrder',
            dataType: 'Text',
            data: {
                moveOrder: $("#txtMoveNumber").val(),
                item: $("#txtItem").val(),
                planner: $("#txtPlanner").val(),
                createdFrom: dateCreatedFrom,
                createdTo: dateCreatedTo,
                requiredFrom: dateRequiredFrom,
                requiredTo: dateRequiredTo,
                pkg: $("#txtPKG").val()

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
                    $("#jqxgrid").jqxGrid('clear');

                    //clear calendar values
                    dateCreatedFrom = "";
                    dateCreatedTo = "";
                    dateRequiredFrom = "";
                    dateRequiredTo = "";
                }
                else {
                    //oracle returned data successfully
                    $("#mymodal").modal("hide");

                    var source = {
                        datatype: "json",
                        datafields: [
                            { name: 'MOVEORDERNBR', type: 'string' },
                            { name: 'ORG', type: 'string' },
                            { name: 'ITEM', type: 'string' },
                            { name: 'TARGETCAS', type: 'string' },
                            { name: 'PLANNER', type: 'string' },
                            { name: 'QUANTITY', type: 'string' },
                            { name: 'DATECREATED', type: 'date' },
                            { name: 'DATEREQUIRED', type: 'date' },
                            { name: 'SUBINVENTORY', type: 'string' },
                            { name: 'PACKAGE', type: 'string' },
                            { name: 'STATUS', type: 'string' }
                        ],
                        localdata: response

                    };
                    var dataAdapter = new $.jqx.dataAdapter(source);
                    $("#jqxgrid").jqxGrid({ source: dataAdapter });

                    //clear calendar values
                    dateCreatedFrom = "";
                    dateCreatedTo = "";
                    dateRequiredFrom = "";
                    dateRequiredTo = "";
                }
            }
        });
    });
});

//Grid Initialization
//rherejias 5/9/2017
var source =
{
    datatype: "json",
    datafields: [
        { name: 'MOVEORDERNBR', type: 'string' },
        { name: 'ORG', type: 'string' },
        { name: 'ITEM', type: 'string' },
        { name: 'TARGETCAS', type: 'string' },
        { name: 'PLANNER', type: 'string' },
        { name: 'QUANTITY', type: 'string' },
        { name: 'DATECREATED', type: 'date' },
        { name: 'DATEREQUIRED', type: 'date' },
        { name: 'SUBINVENTORY', type: 'string' },
        { name: 'PACKAGE', type: 'string' },
        { name: 'STATUS', type: 'string' }
    ],
    url: '/MoveOrder/GetMoveOrder'
};



var cellclass = function (row, columnfield, value, data) {

    var arr1 = [];
    arr1.push(data.DATEREQUIRED);
    for (var i = 0; i < arr1.length; i++) {

        if (arr1[i] < new Date()) {

            return 'red';
        }
    }
}

var dataAdapter = new $.jqx.dataAdapter(source, {
    downloadComplete: function (data, status, xhr) { },
    loadComplete: function (data) { },
    loadError: function (xhr, status, error) { }
});
$("#jqxgrid").jqxGrid(
   {
       theme: window.gridTheme,
       width: '100%',
       source: dataAdapter,
       pageable: true,
       autoheight: true,
       sortable: true,
       //sorttype: 'date',
       altrows: true,
       columnsresize: true,
       columnsautoresize: true,
       columns: [
           { text: 'Date Required', datafield: 'DATEREQUIRED', width: '14%', cellclassname: cellclass, cellsformat: 'MM/dd/yyyy hh:mm:ss tt' },
           { text: 'Org', datafield: 'ORG', width: '5%', cellclassname: cellclass },
           { text: 'Move Order Number', datafield: 'MOVEORDERNBR', width: '10%', formatter: 'showlink', cellclassname: cellclass },
           { text: 'Item', datafield: 'ITEM', width: '10%', cellclassname: cellclass },
           { text: 'Target CAS', datafield: 'TARGETCAS', cellclassname: cellclass },
           { text: 'Quantity', datafield: 'QUANTITY', cellclassname: cellclass },
           { text: 'Sub-Inventory', datafield: 'SUBINVENTORY', width: '11%', cellclassname: cellclass },
           { text: 'Package', datafield: 'PACKAGE', cellclassname: cellclass },
           { text: 'Planner', datafield: 'PLANNER', cellclassname: cellclass },
           { text: 'Date Created', datafield: 'DATECREATED', width: '14%', cellsformat: 'MM/dd/yyyy hh:mm:ss tt', cellclassname: cellclass },
           { text: 'Status', datafield: 'STATUS', cellclassname: cellclass }
       ]
   });

//row click
//rherejias 5/9/2017
$("#jqxgrid").on('rowclick', function (event) {
    var args = event.args;
    // row's bound index.
    var boundIndex = args.rowindex;
    // row's visible index.
    var visibleIndex = args.visibleindex;
    // right click.
    var rightclick = args.rightclick;
    // original event.
    var ev = args.originalEvent;

    var rowID = $("#jqxgrid").jqxGrid('getrowid', boundIndex);
    var data = $("#jqxgrid").jqxGrid('getrowdatabyid', rowID);

    $.ajax({
        url: '/Validate/SearchFilters',
        dataType: 'Text',
        type: 'POST',
        data: {
            moveOrder: moveOrder,
            item: item,
            planner: planner,
            pkg: pkg,
            createdFrom: createdFrom,
            createdTo: createdTo,
            requiredFrom: requiredFrom,
            requiredTo: requiredTo
        }
    });

    window.location = "/Validate/Validate?moveOrder=" + data.MOVEORDERNBR + "&subInventory=" + data.SUBINVENTORY + "&planner=" + data.PLANNER + "&dateCreated=" + data.DATECREATED.toLocaleString().replace(',', '') + "&dateRequired=" + data.DATEREQUIRED.toLocaleString().replace(',', '') + "&Item=" + data.ITEM + "&Org=" + data.ORG + "&TargetCAS=" + data.TARGETCAS + "&Quantity=" + data.QUANTITY + "&Status=" + data.STATUS + "&package=" + data.PACKAGE + "&from=moveorder";

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
    modal += '<input type="submit" style="display:none;"/>';
    modal += ' <div class="row">';
    modal += '       <div class="col-lg-12" style="margin-top:20px;">';
    modal += '                       <div class="row">';
    modal += '                           <div class="col-md-12">';
    modal += '                               <div class="row">';
    modal += '                                   <div class="col-md-4" style="margin:0; padding:0;">';
    modal += '                                       <div id="divMoveOrder" class="col-sm-12" style="text-align:center; ">';
    modal += '                                          <div style="text-align:center;"><label><b>Move Order Number</b></label></div>';
    modal += '                                           <input id="txtMoveNumber" type="text" class="form-control" />';
    modal += '                                       </div>';
    modal += '                                       <div id="divItem" class="col-sm-12" style="margin-top:7px; text-align:center;">';
    modal += '                                           <div style="text-align:center;"><label><b>Item</b></label></div>';
    modal += '                                           <input id="txtItem" type="text" class="form-control" />';
    modal += '                                       </div>';
    modal += '                                       <div id="divPlanner" class="col-sm-12" style="margin-top:7px; text-align:center;">';
    modal += '                                           <div style="text-align:center;"><label><b>Planner</b></label></div>';
    modal += '                                           <input id="txtPlanner" type="text" class="form-control" />';
    modal += '                                       </div>';
    modal += '                                       <div id="divPKG" class="col-sm-12" style="margin-top:7px; text-align:center;">';
    modal += '                                           <div style="text-align:center;"><label><b>Package</b></label></div>';
    modal += '                                           <input id="txtPKG" type="text" class="form-control" />';
    modal += '                                       </div>';
    modal += '                                   </div>';
    modal += '                                   <div class="col-md-8">';
    modal += '                                       <div id="divDateCreated" class="col-sm-6" style="text-align:center;">';
    modal += '                                           <div style="text-align:center;"><label for="dateCreated"><b>Date Created</b></label></div>';
    modal += '                                           <center><div id="dateCreated"></div></center>';
    modal += '                                       </div>';
    modal += '                                       <div id="divDateRequired" class="col-sm-6" style="text-align:center;">';
    modal += '                                           <div style="text-align:center;"><label for="dateRequired"><b>Date Required</b></label></div>';
    modal += '                                           <center><div id="dateRequired"></div></center>';
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
});

//Search button onclick
//rherejias 5/9/2017
$(document).delegate("#btnSearch", "click", function () {
    //get which checkbox button is checked
    //checkBoxVal = [];
    //$("input[name=checkStatus]:checked").map(
    //  function () { checkBoxVal.push(this.value); }).get().join(",");

    moveOrder = $("#txtMoveNumber").val();
    item = $("#txtItem").val();
    planner = $("#txtPlanner").val();
    pkg = $("#txtPKG").val();
    createdFrom = ((dateCreatedFrom == "" || dateCreatedFrom == null) ? "" : dateCreatedFrom);
    createdTo = ((dateCreatedTo == "" || dateCreatedTo == null) ? "" : dateCreatedTo);
    requiredFrom = ((dateRequiredFrom == "" || dateRequiredFrom == null) ? "" : dateRequiredFrom);
    requiredTo = ((dateRequiredTo == "" || dateRequiredTo == null) ? "" : dateRequiredTo);

    $.ajax({
        url: '/MoveOrder/GetSearchedMoveOrder',
        dataType: 'Text',
        data: {
            moveOrder: $("#txtMoveNumber").val(),
            item: $("#txtItem").val(),
            planner: $("#txtPlanner").val(),
            createdFrom: dateCreatedFrom,
            createdTo: dateCreatedTo,
            requiredFrom: dateRequiredFrom,
            requiredTo: dateRequiredTo,
            pkg: $("#txtPKG").val()

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
                $("#jqxgrid").jqxGrid('clear');

                //clear calendar values
                dateCreatedFrom = "";
                dateCreatedTo = "";
                dateRequiredFrom = "";
                dateRequiredTo = "";
            }
            else {
                //oracle returned data successfully
                $("#mymodal").modal("hide");

                var source = {
                    datatype: "json",
                    datafields: [
                        { name: 'MOVEORDERNBR', type: 'string' },
                        { name: 'ORG', type: 'string' },
                        { name: 'ITEM', type: 'string' },
                        { name: 'TARGETCAS', type: 'string' },
                        { name: 'PLANNER', type: 'string' },
                        { name: 'QUANTITY', type: 'string' },
                        { name: 'DATECREATED', type: 'date' },
                        { name: 'DATEREQUIRED', type: 'date' },
                        { name: 'SUBINVENTORY', type: 'string' },
                        { name: 'PACKAGE', type: 'string' },
                        { name: 'STATUS', type: 'string' }
                    ],
                    localdata: response

                };
                var dataAdapter = new $.jqx.dataAdapter(source);
                $("#jqxgrid").jqxGrid({ source: dataAdapter });

                //clear calendar values
                dateCreatedFrom = "";
                dateCreatedTo = "";
                dateRequiredFrom = "";
                dateRequiredTo = "";
            }
        }
    });
});
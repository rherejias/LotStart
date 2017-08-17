
// author : avillena | desc: global varailbles | date : 04/25/2017
﻿var arr_search = [];
var ctr = 0;
var arr_lotnumber = [];
var arr_locator = [];
var test;
var array = 0;
var data;
// End

this.history.forward(-1);

//rherejias android softkeyboard Submit button
$("form").on("submit", function (e) {
    e.preventDefault();
    validation_function.gridrowcolor();
    $('#txtLotnumber').val('');
    if ($('#lblfinishedCtr').text() == $('#lbltotalCtr').text()) {
        $('#lblprogconfirm').text('confirmed');
    }
});


$(document).ready(function () {

  

    ctr2 = 0;
    var orderNumber = $("#moveOrder").text();

    //Grid Initialization
    var source =
    {
        datatype: "json",
        datafields: [
            { name: 'LOCATOR', type: 'string' },
            { name: 'ITEM', type: 'string' },
            { name: 'LOTNUMBER', type: 'string' },
            { name: 'QUANTITY', type: 'string' }
        ],
        url: '/Validate/GetItem' + ((orderNumber == "") ? "" : "?moveOrder=" + orderNumber + "")
       

    };

    // author : avillena | desc : to have a row color for validated lot number | date : 04/25/2017
    var cellclass = function (row, columnfield, value, data) {   
        arr_lotnumber.push(data.LOTNUMBER);
        arr_locator.push(data.LOCATOR);
        for (var i = 0; i < arr_search.length; i++) {
            if (data.LOTNUMBER == arr_search[i]) {
                return 'green'
            }           
        }
       
    }
    // End

    var dataAdapter = new $.jqx.dataAdapter(source, {
        downloadComplete: function (data, status, xhr) { },
        loadComplete: function (data) { },
        loadError: function (xhr, status, error) { }
    });

    //get total rows of data
    $.ajax({
        url: '/Validate/GetItem' + ((orderNumber == "") ? "" : "?moveOrder=" + orderNumber + ""),
        dataType: 'text',
        success: function (response) {
            console.log(response);
            //get total rows of data
            array = JSON.parse(response);
            $("#lbltotalCtr").text(array.length);
        }
    });
   

    $("#jqxgrid").jqxGrid(
       {
           theme: window.gridTheme,
           width: "100%",
           source: dataAdapter,
           pageable: true,
           autoheight: true,
           sortable: true,
           altrows: true,
           columnsresize: true,
           selectionmode: 'multiplecellsadvanced',
           altrows: true,

           columns: [

               { text: 'Locator', datafield: 'LOCATOR',cellclassname: cellclass, width: '35%' },
               { text: 'Item', datafield: 'ITEM',  cellclassname: cellclass, width: '30%'},
               { text: 'Lot Number', datafield: 'LOTNUMBER', cellclassname: cellclass, width: '20%' },
               { text: 'Quantity', datafield: 'QUANTITY', cellclassname: cellclass, width: '15%' },

           ]
       });
  

    //author : rherejias | desc : automatically display search modal for move order number | date : 04/25/2017 | version : 1.0
    if ($("#hidden").val() == '1') {
        showModal.insertModal();
    } else {
        $('#txtLotnumber').focus();
    }

   // $('#lbltotalCtr').text(Array.from(new Set(arr_lotnumber)).sort().length + 1);

    //var rows = $('#jqxgrid').jqxGrid('getrows');
    //$('#lbltotalCtr').text(rows.length);
   // alert(rows.length);
    validation_function.getrowcount();
});

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
    data = $("#jqxgrid").jqxGrid('getrowdatabyid', rowID);
});

//show modal function
//rherejias 5/9/2017
var showModal = {

    // author : rherejias | desc : modal for serching of move order number | date : 04/25/2017 | version : 1.0
    insertModal: function () {
       
        var modal = '<div class="modal fade" id="mymodal" role="dialog" data-backdrop="static">';
        modal += '<div class="modal-dialog modal-sm">';
        modal += ' <div class="modal-content">';

        modal += '<div class="modal-header" style="background-color:#76cad4; color:#ffffff">';
        modal += '<h4 class="modal-title">Enter Move Order Number</h4>';
        modal += '<div class="loader"  style="float:right; top:-26px; position:relative; display:none"></div>';
        modal += '</div>';

        modal += '<div class="modal-body" style="margin-top:20px">';
        modal += '<input type="text" class="form-control" placeholder="Move Order Number" id="txtMoveOrdNum" autofocus/>'
        modal += '<p class="text-danger" id="lblMoveOrderNumClose" hidden><i class="fa fa-exclamation-circle" aria-hidden="true"></i> Move Order Number is already closed.</p>';
        modal += '<p class="text-danger" id="lblInvalidMoveOrdNum" hidden><i class="fa fa-exclamation-circle" aria-hidden="true"></i> Invalid Move Order Number.</p>';
        modal += '</div>';

        modal += '<div class="modal-footer">';
        modal += '<div class="row">';
        modal += '<button class="btn btn-default" data-dismiss="modal"';
        modal += 'style="width: 100px;">';
        modal += 'Cancel</button>';
        modal += '<button type="button" style="width: 100px;" class="btn btn-success" id="btnSubmitMoveOrdNum">Submit</button> &nbsp ';
        modal += '</div>';
        modal += '</div>';

        modal += '</div>';
        modal += '</div>';
        modal += '</div>';

     

        $("#form_modal").html(modal);
        $("#mymodal").modal("show");
        $("#mymodal").css('z-index', '1000000');
      
    },
    //End


    // author : avillena | desc : modal for verification of submit | date : 04/27/2017 | version : 1.0
    submitVerifyModal: function () {

        var modal = '<div class="modal fade" id="verifysubmitmodal" role="dialog" data-backdrop="static">';
        modal += '<div class="modal-dialog modal-sm">';
        modal += ' <div class="modal-content">';

        modal += '<div class="modal-header" style="background-color:#76cad4; color:#ffffff">';
        modal += '<h4 class="modal-title">Enter Move Order Number</h4>';
        modal += '</div>';

        modal += '<div class="modal-body" style="margin-top:20px">';
        modal += '<p>Are you sure you want to continue?</p>'
        modal += '<p id="lblInvalidMoveOrdNum" hidden><i class="fa fa-exclamation-circle" aria-hidden="true"></i> Invalid Move Order Number</p>';
        modal += '</div>';

        modal += '<div class="modal-footer">';
        modal += '<div class="row">';
        modal += '<button class="btn btn-default" data-dismiss="modal">';
        //modal += 'style="width: 100px;">';
        modal += 'Cancel</button>';
        modal += '<button type="button" class="btn btn-success" id="btnSubmit_Save">Submit</button> &nbsp ';
        modal += '</div>';
        modal += '</div>';

        modal += '</div>';
        modal += '</div>';
        modal += '</div>';



        $("#form_modal").html(modal);
        $("#verifysubmitmodal").modal("show");
        $("#verifysubmitmodal").css('z-index', '1000000');

    },
    //End

};

var validation_function = {


    getrowcount: function () {

        var rows = $('#jqxgrid').jqxGrid('getrows');
        $('#lbltotalCtr').text(rows.length);
    },
    
    // author : avillena | desc : row color to the validated lot number and progress bar function | date : 04/25/2017 | version : 1.0
    gridrowcolor: function () {
       
        var percentage = (100 / Array.from(new Set(arr_lotnumber)).sort().length);
        arr_search.push($('#txtLotnumber').val());
        $('#jqxgrid').jqxGrid('updatebounddata');       
        var countofvalidated = Array.from(new Set(arr_lotnumber)).sort().length - $(Array.from(new Set(arr_lotnumber)).sort()).not(Array.from(new Set(arr_search)).sort()).length;

        
        var percentofprogressbar = percentage * countofvalidated;
        $("#progbarvalidation").css('width', percentofprogressbar + '%');
        if ($(Array.from(new Set(arr_lotnumber)).sort()).not(Array.from(new Set(arr_search)).sort()).length === 0) {
            $("#lblfinishedCtr").text($("#lbltotalCtr").text());
            $('#cmdValidationSubmit').prop('disabled', false);
        }
        else {
            $('#lblfinishedCtr').text(countofvalidated);
        }
    }
    // End

};


// author : avillena | desc : searchbox validation with enter function | date : 04/25/2017 | version : 1.0
$(document).delegate('#txtLotnumber', 'keypress', function (e) {

    var key = e.keycode || e.which;
    if (key == 13) {
        validation_function.gridrowcolor();
        $('#txtLotnumber').val('');
        if ($('#lblfinishedCtr').text() == $('#lbltotalCtr').text()) {
            $('#lblprogconfirm').text('confirmed');
        }
    }  
}); // End 


// author : avillena | desc : to change the class of arrow expand to details panel | date : 04/25/2017 | version : 1.0
$(document).delegate('#iconExpand_Details', 'click', function () {
    $("#iconExpand_Details").toggleClass('icon-arrow-up icon-arrow-down');
});
$(document).delegate('#iconExpand_Validlist', 'click', function () {
    $("#iconExpand_Validlist").toggleClass('icon-arrow-up icon-arrow-down');
});// End

// author : avillena | desc : serching of move order number into modal | date : 04/25/2017 | version : 1.0
$(document).delegate('#btnSubmitMoveOrdNum', 'click', function (e) {
   

    if ($("#txtMoveOrdNum").val() == null || $("#txtMoveOrdNum").val() == "") {

        $("#txtMoveOrdNum").css('border-color', 'red');
        setTimeout(function () { $('#txtMoveOrdNum').css('border-color', '#bec3c6'); }, 4000);
    } else {
            
        $.ajax({
            url: '/Validate/GetSearchedMoveOrderValidatePage',
            dataType: 'Text',
            data: {
                moveOrder: $("#txtMoveOrdNum").val(),

            },
            beforeSend: function () {
                $(".loader").css('display', 'inline');
            },
            success: function (response) {
                //checks if json is empty
                if (response == "") {
                    $(".loader").css('display', 'none');
                    $('#lblInvalidMoveOrdNum').show('slow');
                    setTimeout(function () { $('#lblInvalidMoveOrdNum').hide('slow'); }, 4000);
                }
                else {
                    //check if move order (json array) status is closed
                    var jsonparse = JSON.parse(response);
                    if (jsonparse[0].STATUS == "Closed") {
                        $(".loader").css('display', 'none');
                        $('#lblMoveOrderNumClose').show('slow');
                        setTimeout(function () { $('#lblMoveOrderNumClose').hide('slow'); }, 4000);
                    }
                    else {
                        $('#moveOrder').text(jsonparse[0].MOVEORDERNBR);
                        $('#lblsuvinventory').text(jsonparse[0].SUBINVENTORY);
                        $('#lblplanner').text(jsonparse[0].PLANNER);
                        $('#lbldatecreated').text(jsonparse[0].DATECREATED);
                        $('#lbldaterequired').text(jsonparse[0].DATEREQUIRED);

                        $('#lblItem').text(jsonparse[0].ITEM);
                        $('#lblOrg').text(jsonparse[0].ORG);
                        $('#lblTargetCAS').text(jsonparse[0].TARGETCAS);
                        $('#lblQuantity').text(jsonparse[0].QUANTITY);
                        $('#lblStatus').text(jsonparse[0].STATUS);

                        // desc : reinitialization of grid
                        var source = {
                            datatype: "json",
                            datafields: [
                                     { name: 'LOCATOR', type: 'string' },
                                     { name: 'ITEM', type: 'string' },
                                     { name: 'LOTNUMBER', type: 'string' },
                                     { name: 'QUANTITY', type: 'string', width: '5%' }
                            ],
                            url: '/Validate/GetItem?moveOrder=' + jsonparse[0].MOVEORDERNBR
                        };
                        var dataAdapter = new $.jqx.dataAdapter(source);
                        $("#jqxgrid").jqxGrid({ source: dataAdapter });

                        $('#txtLotnumber').focus();
                        $('#mymodal').modal('hide');
                    }
                }
            }
        });
    } 
}); // End 

// author : avillena | desc : searchbox validation with enter function | date : 04/25/2017 | version : 1.0
$(document).delegate('#txtLotnumber', 'keypress', function (e) {


    var key = e.keycode || e.which;
    if (key == 13) {
        validation_function.gridrowcolor();
        $('#txtLotnumber').val('');
        if ($('#lblfinishedCtr').text() == $('#lbltotalCtr').text()) {
            $('#lblprogconfirm').text('confirmed');
        }
    }
}); // End 


///submit onclick 
/// create audit trail and logs 
///rherjeias 4/28/201
$("#cmdValidationSubmit").click(function () {
    $.ajax({
        url: '/Validate/Submit',
        dataType: 'text',
        data: {
            objectId: '0',
            objectCode: $("#moveOrder").text(),
            org: $("#lblOrg").text(),
            item: $("#lblItem").text(),
            targetCAS: $("#lblTargetCAS").text(),
            planner: $("#lblplanner").text(),
            quantity: $("#lblQuantity").text(),
            dateCreated: $("#lbldatecreated").text(),
            dateRequired: $("#lbldaterequired").text(),
            subInventory: $("#lblsuvinventory").text(),
            pkg: $("#lblPackage").text(),
            status: $("#lblStatus").text(),
            lotNumber: Array.from(new Set(arr_lotnumber)).sort().toString(),
            locator: Array.from(new Set(arr_locator)).sort().toString()
        },
        success: function (response) {

            var modal = '<div class="modal fade" id="mymodal" role="dialog" data-backdrop="static" data-keyboard="false" >';
            modal += '<div class="modal-dialog modal-sm">';
            modal += ' <div class="modal-content">';

            modal += '<div class="modal-header" style="background-color:#76cad4; color:#ffffff">';
            modal += '<h4 class="modal-title">Successful</h4>';
            modal += '</div>';

            modal += '<div class="modal-body" style="margin-top:20px">';
            modal += 'Move Order validated.';
            modal += '</div>';

            modal += '<div class="modal-footer">';
            modal += '<div class="row">';
            modal += '<button type="button" id="btnBack" class="btn btn-success" >OK</button>';
            modal += '<button style="display:none;"class="btn btn-success" id="btnStay"';
            modal += 'style="width: 100px;">';
            modal += 'Validate</button> &nbsp ';
            modal += '</div>';
            modal += '</div>';

            modal += '</div>';
            modal += '</div>';
            modal += '</div>';

            $("#form_modal").html(modal);
            $("#mymodal").modal("show");
            $("#mymodal").css('z-index', '1000000');

            $("#btnBack").click(function () {
                window.location = '/MoveOrder/MoveOrder?from=1';
            });

            $("#btnStay").click(function () {
                window.location = '/Validate/Validate?from=0';
            });
        }
    });
});



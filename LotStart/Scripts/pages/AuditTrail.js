//export btn onclick
//rherejias 5/9/2017
$(document).delegate("#Export", "click", function () {
    $("#AuditTrailGrid").jqxGrid('exportdata', 'xls', 'LotStartAuditTrail');
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

//Grid Initialization
//rherejias 5/9/2017
var source =
{
    datatype: "json",
    datafields: [
        { name: 'Module', type: 'string' },
        { name: 'Operation', type: 'string' },
        { name: 'Name', type: 'string' },
        { name: 'IP', type: 'string' },
        { name: 'MAC', type: 'string' },
        { name: 'DateAdded', type: 'date' },
    ],
    url: '/AuditTrail/GetAuditTrail'
};
var dataAdapter = new $.jqx.dataAdapter(source, {
    downloadComplete: function (data, status, xhr) { },
    loadComplete: function (data) { },
    loadError: function (xhr, status, error) { }
});

var emptySource = true;
$("#AuditTrailGrid").jqxGrid(
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
       showtoolbar: true,
       showstatusbar: true,
       rendertoolbar: function (toolbar) {
           var me = this;
           var container = $("<div style='margin: 3px;'></div>");

           input = $("<input class='jqx-input jqx-widget-content jqx-rc-all' id='txtSearch' type='text' style='height: 23px; float: left; width: 223px;' placeholder='Search...'/>");

           toolbar.append(container);
           container.append(input);
       },
       renderstatusbar: function (statusbar) {
           var container = $("<div style='margin: 3px;'></div>");

           input = $("<button style='margin-left:5px;font-size:13px; font-family: arial;' class='btnx btn btn-success' id='Export'><i class='fa fa-file-excel-o' aria-hidden='true'></i> Export to Excel</button>");

           statusbar.append(container);
           container.append(input);
           $('.btnx').jqxButton({
           });
       },
       columns: [
           { text: 'DateAdded', datafield: 'DateAdded', cellsformat: 'MM/dd/yyyy hh:mm:ss tt' },
           { text: 'Name', datafield: 'Name' },
           { text: 'Module', datafield: 'Module', formatter: 'showlink' },
           { text: 'Operation', datafield: 'Operation', width: '12%' },
           { text: 'IP Address', datafield: 'IP' },
           { text: 'MAC Address', datafield: 'MAC' },
           
       ]
   });

//textbox on kry press search
//rherejias 5/9/2017
$(document).delegate("#txtSearch", "keyup", function (e) {
    $.ajax({
        url: '/AuditTrail/GetSearch',
        dataType: 'json',
        data: { searchParam: $("#txtSearch").val() },
        success: function (response) {
                var source = {
                    datatype: "json",
                    datafields: [
                        { name: 'Module', type: 'string' },
                        { name: 'Operation', type: 'string' },
                        { name: 'Name', type: 'string' },
                        { name: 'IP', type: 'string' },
                        { name: 'MAC', type: 'string' },
                        { name: 'DateAdded', type: 'date' },
                    ],
                    localdata: response

                };
                var dataAdapter = new $.jqx.dataAdapter(source);
                $("#AuditTrailGrid").jqxGrid({ source: dataAdapter });
            },
        error: function (response) {
            $("#AuditTrailGrid").jqxGrid('clear');
        }
    });
});


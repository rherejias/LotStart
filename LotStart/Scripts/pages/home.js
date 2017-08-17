$(document).ready(function() {
    $.ajax({
        url: "/home/foo",
        dataType: 'json',
        type: 'get',
        data: {},
        beforeSend: function () {

        },
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
        },
        success: function (response) {
            if (response.success) {
                $("#testChartContainer").html(response.message);
                window['init_hchart_chartid']();
            } else {
                console.log("err");
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            console.log(xhr.status);
            console.log(thrownError);
        }
    });
});
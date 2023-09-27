$(document).ready(function () {
    BindList();
});

var hCols = [];

function BindList() {
    var table = $('#tblMain').DataTable({
        "pageLength": $('#PageSize').val(),
        "scrollY": "600px",
        "scrollX": true,
        "lengthChange": true,
        "paging": true,
        "autoWidth": true,
        "processing": true,
        "serverSide": true,
        "filter": false,
        "orderMulti": false,
        "order": [0, "desc"],
        "ajax": {
            "url": "/Home/GetAddressList",
            "type": "POST",
            "datatype": "json",
            "data": function (o) {
                o.search = $("#search").val();
            }
        },
        "columns": [
            {
                mRender: function (data, type, full) {
                    return '<input type="checkbox" class="singleCheckBox" onclick="return initializeSingleCheckBox(this);" id="' + full.id + '" name="HiddenId" value="id_' + full.id + '" />';
                }
            },
            { "data": "id", "autoWidth": true },
            { "data": "firstName", "autoWidth": true },
            { "data": "email", "autoWidth": true },
            { "data": "phoneNumber", "autoWidth": true },
            { "data": "address", "autoWidth": true },
            {
                mRender: function (data, type, full) {
                    var str = '';
                    if (full.isActive) {
                        str += '<a href="/Home/ChangeStatus?Id=' + full.id + '&&Status=0" class="btn btn-danger gridbtn">Deactivate</a>';
                    }
                    else {
                        str += '<a href="/Home/ChangeStatus?Id=' + full.id + '&&Status=1" class="btn btn-success gridbtn">Activate</a>';
                    }
                    return str;
                }
            },
            {
                mRender: function (data, type, full) {
                    var str = '';
                    if (full.isBlock) {
                        str += '<a href="/Home/BlockOrUnblock?Id=' + full.id + '&&Status=0" class="btn btn-success gridbtn">Unblock</a>';
                    }
                    else {
                        str += '<a href="/Home/BlockOrUnblock?Id=' + full.id + '&&Status=1" class="btn btn-danger gridbtn">Block</a>';
                    }
                    return str;
                }
            },
            {
                mRender: function (data, type, full) {
                    var Action = '<div class="row" style="margin-left:-2px;"><a href="/Home/AddNew?Id=' + full.id + '" title="Edit" class="btn btn-primary btn-sm">Edit</a>&nbsp; ';
                    Action += '<button class="btn btn-danger btn-sm sweet-1" onclick="fnDelete(\'' + full.id + '\')">Delete</button></div>';
                    return Action;
                }
            }
        ],
        "responsive": true,
        "columnDefs": [
            { orderable: false, "targets": [0, 1, 2, 3, 4, 5, 6, 7, 8] }
        ]
    });

    $("#search").on('keyup', function (event) {
        event.preventDefault();
        table.search(this.value).draw();
    });
    
}

function fnDelete(id) {
    $.ajax({
        type: "GET",
        url: '/Home/Delete',
        async: true,
        cache: false,
        data: {
            id: id,
        },
        success: function (data) {
            $('#tblMain').DataTable().draw();
        },
        error: function (response) {
            console.log(response);
        }

    });
}

function initializeAllCheckBox() {
    var n = $("#allCheckBox").is(":checked");
    $("#tblMain .singleCheckBox").prop("checked", n ? !0 : !1);
    $("#tblMain .singleCheckBox").closest("tr").addClass(n ? "selected-row" : "not-selected-row");
    $("#tblMain .singleCheckBox").closest("tr").removeClass(n ? "not-selected-row" : "selected-row");
}

function initializeSingleCheckBox(n) {
    var t = $(n).is(":checked");
    $(n).closest("tr").addClass(t ? "selected-row" : "not-selected-row");
    $(n).closest("tr").removeClass(t ? "not-selected-row" : "selected-row");
    t && $("#tblMain .singleCheckBox").length == $("#tblMain .selected-row").length ? $("#allCheckBox").prop("checked", !0) : $("#allCheckBox").prop("checked", !1);
}


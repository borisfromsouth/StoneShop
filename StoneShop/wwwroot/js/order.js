var dataTable;

$(document).ready(function () {
    loadDataTable()
});

function loadDataTable() {
    dataTable = $("#tblData").DataTable({
        "ajax": {
            "url": "/order/GetOrderList"
        },
        "columns": [
            { "data": "id", "width": "5%" },
            { "data": "fullName", "width": "10%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "email", "width": "15%" },
            { "data": "orderDate", "width": "15%" },
            { "data": "shippingDate", "width": "15%" },
            { "data": "finalOrderTotal", "width": "10%" },
            { "data": "orderStatus", "width": "15%" },
            //{ "data": "city", "width": "10%" },
            //{ "data": "street", "width": "10%" },
            //{ "data": "postCode", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Order/Details/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                <i class="fas fa-edit"></i>
                            </a>
                        </div>
                    `;
                },
                "width": "5%"
            }
        ]
    });
}
var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#myTable').DataTable({ // Assign DataTable to dataTable variable
        "processing": true,
        "serverSide": false,  // Set true if using API pagination
        "ajax": {
            "url": "/Admin/company/Getall",
            "type": "GET",
            "dataSrc": "data"
        },
        "columns": [
            { "data": "name", "width": "25%" },
            { "data": "streetAddress", "width": "15%" },
            { "data": "city", "width": "10%" },
            { "data": "state", "width": "15%" },
            { "data": "postalCode", "width": "10%" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                    <a href="/admin/company/upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</a>
                    <a onclick="deleteProduct('/admin/company/delete/${data}')" class="btn btn-danger mx-2"> <i class="bi bi-trash"></i> Delete</a>
                    </div>`;
                },
                "width": "25%"
            }
        ]
    });
}

function deleteProduct(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    dataTable.ajax.reload(); // Ensure dataTable is correctly initialized
                    toastr.success(data.message);
                },
                error: function (xhr) {
                    toastr.error("Error deleting item. Please try again.");
                }
            });
        }
    });
}

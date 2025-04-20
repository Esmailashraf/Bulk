var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#myTable').DataTable({ // Assign DataTable to dataTable variable
        "processing": true,
        "serverSide": false,  // Set true if using API pagination
        "ajax": {
            "url": "/Admin/Product/Getall",
            "type": "GET",
            "dataSrc": "data"
        },
        "columns": [
            { "data": "title", "width": "25%" },
            { "data": "isbn", "width": "15%" },
            { "data": "author", "width": "10%" },
            { "data": "listPrice", "width": "15%" },
            { "data": "category.name", "width": "10%" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                    <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</a>
                    <a onclick="deleteProduct('/admin/product/delete/${data}')" class="btn btn-danger mx-2"> <i class="bi bi-trash"></i> Delete</a>
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

function filterOrders() {
    var typePayment = document.getElementById("category").value;
    var searchByID = document.getElementById("searchByID").value;
    var searchByName = document.getElementById("searchByName").value;
    var searchBySDT = document.getElementById("searchBySDT").value;

    let url = `Admin/Order?page=0.page&pageSize=5`;

    if (typePayment) url += `&typePayment=${typePayment}`;
    if (searchByID) url += `&searchByID=${searchByID}`;
    if (searchByName) url += `&searchByName=${encodeURIComponent(searchByName)}`;
    if (searchBySDT) url += `&searchBySDT=${searchBySDT}`;

    window.location.href = url;
}
var index = false;
$(document).ready(function () {
    $(".tabs").tabs();
})
function mostrarTab(item) {
    var itemCapitalizado = item.substring(0, 1).toUpperCase() + item.substring(1);
    $("li.collection-item.active").removeClass("active");
    if (!$("#item" + itemCapitalizado).hasClass("active")) {
        $("#item" + itemCapitalizado).addClass("active");
        window.location.href = "#" + item;
    }
}
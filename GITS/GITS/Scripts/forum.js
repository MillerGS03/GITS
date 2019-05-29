var index = false;
$(document).ready(function () {
    $(".tabs").tabs();
    $(".collapsible").collapsible();

    var instance = M.Tabs.getInstance($(".resultados .tabs").get()[0]);
    var tabs = ["usuarios", "eventos", "publicacoes"];
    mostrarTab(tabs[instance.index], true);
})

function mostrarTab(item, selecionar) {
    var tabs = ["usuarios", "eventos", "publicacoes"];

    var instance = M.Tabs.getInstance($(".resultados .tabs").get()[0]);
    if (tabs[instance.index] != item && selecionar) {
        instance.select(item);
    }

    var itemCapitalizado = item.substring(0, 1).toUpperCase() + item.substring(1);
    $("li.collection-item.active").removeClass("active");
    if (!$("#item" + itemCapitalizado).hasClass("active")) {
        $("#item" + itemCapitalizado).addClass("active");
    }
}
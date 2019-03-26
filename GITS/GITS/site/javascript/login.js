$(document).on("resize", function() {
    $("#conteinerTabela").css("top", $("#wrapperLogin").width() + $("nav").width() + "px");
})
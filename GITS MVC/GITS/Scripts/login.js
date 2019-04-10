var redimensionarLogin = function() {
    var conteinerTabela = $("#conteinerTabela");
    conteinerTabela.css("top", 20 + $("#wrapperLogin").height() + $("nav").height() + "px");
    $("#footer").css("top", conteinerTabela.offset().top + conteinerTabela.height() + "px");
    $("#loginGoogle").css("top", $("#divisaoLogin").position().top + ($("#login").height() - $("#divisaoLogin").position().top - $("#loginGoogle").height()) / 2 + "px");
    $("#loginGoogle").css("left", ($("#login").outerWidth() - $("#loginGoogle").outerWidth()) / 2 + "px")
}

$(document).ready(function() {
    redimensionarLogin();
    setTimeout(redimensionarLogin, 1000);
})
$(window).on("resize", function() {
    redimensionarLogin();
})
$("#loginGoogle").click(function() {
    $("main").html("");
    user = usuario;
    tratar(user);
})
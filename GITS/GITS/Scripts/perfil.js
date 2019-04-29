var index = false;

function realizarSolicitacao(idUsuario) {
    $.post({
        url: "/EnviarSolicitacaoPara",
        data: {
            idUsuario: idUsuario
        }
    });
}

var shiftPressionado = false;
function comecarEdicao() {
    $("#txtStatus").replaceWith("<textarea id=\"txtStatus\" class=\"materialize-textarea\">" + $("#txtStatus").text().trim().substring(1, $("#txtStatus").text().trim().length - 1) + "</textarea>")
    $("#txtStatus").on("keydown", function (e) {
        if (e.which == 13) {
            if (!shiftPressionado)
                $("#txtStatus").replaceWith("<p class=\"flow-text\" id=\"txtStatus\">\"" + $("#txtStatus").text() + "\"</p>");
        }
        else if (e.which == 16)
            shiftPressionado = true;
    })
}

$(document).on("keyup", function (e) {
    if (e.which == 16)
        shiftPressionado = false;
})

function isYourself() {
    atual = JSON.parse(getCookie("user").substring(6));
    return atual != null && atual.Id == $("#idUsuarioYYY").html();
}
var atual;


//<div class="btn" onclick="realizarSolicitacao(@ViewBag.Usuario.Id)">Adicionar como Amigo</div>

setTimeout(function () {
    if (!isYourself()) {
        $("#esquerda").html($("#esquerda").html() + `<div class="btn" onclick="realizarSolicitacao(${$("#idUsuarioYYY").html()})">Adicionar como Amigo</div>`);
        $("#eventos").html($("#eventos").html() + `<div class="btn" id="btnConvidarParaEvento" onclick="convidarParaEvento(${$("#idUsuarioYYY").html()})">Convidar para Evento</div>`)
        $("#nomeOuVc").html($("#nome").html());
    }
    else {
        $("#nomeOuVc").html("Você");
        $("#btnEditar").css("display", "inline-block");
        $("#btnEditar").click(comecarEdicao);
    }
    for (var i = 0; i < atual.Eventos.length; i++)
        console.log(atual.Eventos[i]);
}, 50)
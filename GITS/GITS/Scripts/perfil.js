var index = false;

function realizarSolicitacao(idUsuario) {
    $.post({
        url: "/EnviarSolicitacaoPara",
        data: {
            idUsuario: idUsuario
        }
    });
}
function atualizarStatus(status) {
    $.post({
        url: "/AtualizarStatus",
        data: { status: status },
        error: function () {
            console.log("Erro!!!!!");
        },
        success: function () { console.log("Sucesso!!!"); }
    })
}
function terminarEdicao() {
    $("#txtStatus").replaceWith("<p class=\"flow-text\" id=\"txtStatus\">\"" + $("#txtStatus").val().trim() + "\"</p>");
    $("#txtStatus").unbind("keydown");
    $("#txtStatus").unbind("focusout");
    atualizarStatus($("#txtStatus").text().substring(1, $("#txtStatus").text().length - 1).trim());
}

var shiftPressionado = false;
function comecarEdicao() {
    $("#txtStatus").replaceWith("<textarea id=\"txtStatus\" class=\"materialize-textarea\">" + $("#txtStatus").text().trim().substring(1, $("#txtStatus").text().trim().length - 1) + "</textarea>")
    $("#txtStatus").on("keydown", function (e) {
        if (e.which == 13) {
            if (!shiftPressionado)
                terminarEdicao();
        }
        else if (e.which == 16)
            shiftPressionado = true;
    })
    $("#txtStatus").focusout(function () {
        terminarEdicao();
    })
    $("#txtStatus").focus();
}

$(document).on("keyup", function (e) {
    if (e.which == 16)
        shiftPressionado = false;
})

function isYourself() {
    try {
        atual = JSON.parse(getCookie("user").substring(6));
        return atual != null && atual.Id == $("#idUsuarioYYY").html();
    }
    catch { return false; }
}
var atual;


//<div class="btn" onclick="realizarSolicitacao(@ViewBag.Usuario.Id)">Adicionar como Amigo</div>

setTimeout(function () {
    if (!isYourself()) {
        if (atual != null)
        {
            $("#esquerda").html($("#esquerda").html() + `<div class="btn" onclick="realizarSolicitacao(${$("#idUsuarioYYY").html()})">Adicionar como Amigo</div>`);
            $("#eventos").html($("#eventos").html() + `<div class="btn" id="btnConvidarParaEvento" onclick="convidarParaEvento(${$("#idUsuarioYYY").html()})">Convidar para Evento</div>`)
        } 
        $("#nomeOuVc").html($("#nome").html());
    }
    else {
        $("#nomeOuVc").html("Você");
        $("#btnEditar").css("display", "inline-block");
        $("#btnEditar").click(comecarEdicao);
    }
    var rets = getStatusXP($("#xpAtual").html());
    setNivel(rets[0], rets[1]);
    console.log(rets)
    if (atual != null) {
        //for (var i = 0; i < atual.Eventos.length; i++)
        //    console.log(atual.Eventos[i]);
    }
}, 50)

function setNivel(p, lvl) {
    if (p <= 50) {
        $("#containerXP").attr('style', `background-image: linear-gradient(${(p * (90 / 25)) - 90}deg, gray 50%, transparent 50%),
                linear-gradient(-90deg, #26a69a 50%, transparent 50%);`);
    }
    else {
        $("#containerXP").attr('style', `background: linear-gradient(270deg, #26a69a 50%, transparent 50%), linear-gradient(${((p - 50) / 50) * 180 - 90}deg, #26a69a 50%, gray 50%)`);
    }
    $("#levelUsuario").html(lvl);
}
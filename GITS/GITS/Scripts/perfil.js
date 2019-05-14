function realizarSolicitacao(idUsuario) {
    $.post({
        url: "/EnviarSolicitacaoPara",
        data: {
            idUsuario: idUsuario
        }
    }, function () { window.location.reload(); });
}
function excluirAmigo(idUsuario) {
    $.post({
        url: "/RemoverAmizade",
        data: {
            idUsuario: idUsuario
        }
    }, function () { window.location.reload(); });
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
        return window.usuario != null && window.usuario.Id == $("#idUsuarioYYY").html();
    }
    catch { return false; }
}
function isYourFriend() {
    try {
        var idPossivelAmigo = $("#idUsuarioYYY").html();
        for (var i = 0; i < this.usuario.Amigos.length; i++)
            if (this.usuario.Amigos[i].Id == idPossivelAmigo)
                return true;
        return false;
    }
    catch { return false; }
}
var atual;


//<div class="btn" onclick="realizarSolicitacao(@ViewBag.Usuario.Id)">Adicionar como Amigo</div>


function setNivel(p, lvl) {
    if (p <= 50) {
        $("#containerXP").attr('style', `filter: brightness(85%);background-image: linear-gradient(${(p * (90 / 25)) - 90}deg, gray 50%, transparent 50%),
                linear-gradient(-90deg, var(--tema) 50%, transparent 50%);`);
    }
    else {
        $("#containerXP").attr('style', `filter: brightness(85%);background: linear-gradient(270deg, var(--tema) 50%, transparent 50%), linear-gradient(${((p - 50) / 50) * 180 - 90}deg, var(--tema) 50%, gray 50%)`);
    }
    $("#levelUsuario").html(lvl);
}

function setGeral() {
    $.get({
        url: '/GetUsuario',
        data: {
            id: JSON.parse(getCookie("user").substring(6))
        }
    }, function (result) {
        result = JSON.parse(result)
        tituloU = parseInt(result.Titulo.split(" ")[0])
        console.log(result)
        $.get({
            url: '/GetItem',
            data: {
                id: tituloU
            }
        }, function (titu) {
            titu = JSON.parse(titu)
            $("#infoNomeTitulo h5").html(titu.Conteudo);
            var conteudos = result.Titulo.split(' ');

            if (conteudos.indexOf("R") > -1) {
                $("#infoNomeTitulo h5").attr('class', 'rainbow');
                setRainbow();
            }
            if (conteudos.indexOf("B") > -1)
                $("#infoNomeTitulo h5").css('font-weight', 'bold');
        })
        if (isYourself()) {
            configurarPostar();
        }
        var rets = getStatusXP($("#xpAtual").html());
        setNivel(rets[0], rets[1]);
        if (atual != null) {
            for (var i = 0; i < atual.Acontecimentos.length; i++)
                console.log(atual.Acontecimentos[i]);
            for (var i = 0; i < atual.Tarefas.length; i++)
                console.log(atual.Tarefas[i]);
        }
    });
}

$(document).ready(setGeral);
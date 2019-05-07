var index = false;

function publicar() {

    $.post({
        url: "/Publicar",
        data: {
            titulo: $("#tituloPost").val().trim(),
            descricao: $(".txtPostar").val().trim(),
            idsUsuariosMarcados: getIdsUsuariosMarcados()
        },
        success: function () {
            alert("Publicadlasdas");
        }
    })
}
function getIdsUsuariosMarcados() {
    var chipsData = M.Chips.getInstance($("#chips")).chipsData;
    var ids = new Array();

    for (var i = 0; i < chipsData.length; i++)
        ids.push(chipsData[i].id);

    return ids;
}
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
        $("#containerXP").attr('style', `background-image: linear-gradient(${(p * (90 / 25)) - 90}deg, gray 50%, transparent 50%),
                linear-gradient(-90deg, #26a69a 50%, transparent 50%);`);
    }
    else {
        $("#containerXP").attr('style', `background: linear-gradient(270deg, #26a69a 50%, transparent 50%), linear-gradient(${((p - 50) / 50) * 180 - 90}deg, #26a69a 50%, gray 50%)`);
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
        window.usuario = JSON.parse(result);
        if (isYourself()) {
            var data = new Object();
            var usuariosJaInclusos = new Array();

            for (var i = 0; i < window.usuario.Amigos.length; i++) {
                var amigo = window.usuario.Amigos[i];

                var nomeRepetido = false;
                for (var j = 0; j < usuariosJaInclusos.length; j++)
                    if (usuariosJaInclusos[j].nome == amigo.Nome) {
                        usuariosJaInclusos[j].numero++;
                        nomeRepetido = true;
                        data[amigo.Nome + "(" + usuariosJaInclusos[j].numero + ")"] = amigo.FotoPerfil;
                        break;
                    }
                if (!nomeRepetido) {
                    usuariosJaInclusos.push({ nome: amigo.Nome, numero: 1 })
                    data[amigo.Nome] = amigo.FotoPerfil;
                }
            }

            $(".chips-autocomplete").chips({
                autocompleteOptions: {
                    data: data,
                    limit: Infinity,
                    minLength: 1,
                },
                placeholder: "Marque um amigo!",
                onChipAdd: function (e, chipEvento) {
                    var chipsInstance = M.Chips.getInstance($("#chips"));
                    var chipsData = chipsInstance.chipsData;
                    var chip = chipsData[chipsData.length - 1];
                    var valido = false;
                    for (var i = 0; i < window.usuario.Amigos.length; i++)
                        if (window.usuario.Amigos[i].Nome == chip.tag) {
                            for (var j = 0; j < chipsData.length; j++)
                                if (chipsData[j].id == window.usuario.Amigos[i].Id)
                                    break;
                            valido = true;
                            chip.image = window.usuario.Amigos[i].FotoPerfil;
                            chip.id = window.usuario.Amigos[i].Id;
                            $(chipEvento).html(`<img src="${chip.image}">` + $(chipEvento).html());
                            break;
                        }
                    if (!valido)
                        chipsInstance.deleteChip(chipsData.length - 1);
                }
            });
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
    var verificacao = function () {
        if ($("#tituloPost").val().trim().length > 0 && $(".txtPostar").val().trim().length > 0) {
            if ($(".btnPostar").attr("disabled") == "disabled")
                $(".btnPostar").removeAttr("disabled");
        }
        else {
            if ($(".btnPostar").attr("disabled") != "disabled")
                $(".btnPostar").attr("disabled", "disabled");
        }
    }
    $("#tituloPost").on("input", verificacao);
    $(".txtPostar").on("change keyup paste", verificacao);
}

setTimeout(setGeral, 20)
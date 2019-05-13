function publicar() {

    $.post({
        url: "/Publicar",
        data: {
            titulo: $("#tituloPost").val().trim(),
            descricao: $(".txtPostar").val().trim(),
            idsUsuariosMarcados: getIdsUsuariosMarcados()
        },
        success: function () {
            window.location.reload();
        }
    })
}

var idPublicacaoADeletar = -1;
var idPublicacaoAEditar = -1;
function deletarPublicacao() {
    $.post({
        url: "/DeletarPublicacao",
        data: {
            idPublicacao: idPublicacaoADeletar
        },
        success: function () {
            if (!index)
                window.location.href = "/perfil";
            else
                window.location.reload();
        }
    })
}
function editarPublicacao() {
    $.post({
        url: "/EditarPublicacao",
        data: {
            idPublicacao: idPublicacaoAEditar,
            novoTitulo: $("#tituloEditar").val().trim(),
            novoConteudo: $(".txtEditar").val().trim()
        },
        success: function () {
            window.location.reload();
        }
    })
}
function comecarEdicaoPost(idPublicacao) {
    idPublicacaoAEditar = idPublicacao;
    $("#modalEditar #tituloEditar").val($("#titulo" + idPublicacaoAEditar).text());
    $("#modalEditar .txtEditar").val($("#descricao" + idPublicacaoAEditar).text());
}
function gostarOuDesgostarDe(idPublicacao) {
    if ($("#like" + idPublicacao).hasClass("grey")) {
        $.post({
            url: "/GostarDe",
            data: {
                idPublicacao: idPublicacao,
            },
            success: function () {
                $("#like" + idPublicacao).removeClass("grey");
                $("#likes" + idPublicacao).text(parseInt($("#likes" + idPublicacao).text()) + 1);
            }
        })
    }
    else {
        $.post({
            url: "/DesgostarDe",
            data: {
                idPublicacao: idPublicacao,
            },
            success: function () {
                $("#like" + idPublicacao).addClass("grey");
                $("#likes" + idPublicacao).text(parseInt($("#likes" + idPublicacao).text()) - 1);
            }
        })
    }
}
function responder() {
    $.post({
        url: "/Responder",
        data: {
            idPublicacao: idPublicacaoRespondendo,
            descricao: $(".txtResponder").val().trim(),
        },
        success: function () {
            window.location.reload();
        }
    })
}

var idPublicacaoRespondendo = -1;
function iniciarResposta(idPublicacao) {
    if (idPublicacaoRespondendo != idPublicacao) {
        if (idPublicacaoRespondendo != -1) 
            pararResposta(idPublicacaoRespondendo);

        idPublicacaoRespondendo = idPublicacao;

        // Cálculo da profundidade
        var profundidade = 0;
        for (var i = 1; i <= 3; i++)
            if ($("#post" + idPublicacao).hasClass("profundidade" + 1)) {
                profundidade = i;
                break;
            }

        // Inserção da textarea
        $("#post" + idPublicacao).after(
            `<div id="respondendo${idPublicacao}" class="containerPost escreverResposta right-align profundidade${profundidade + 1}">
            <textarea placeholder="Escreva aqui sua resposta..." class="materialize-textarea txtResponder" style="height: 43px;"></textarea>
            <div class="btn btnResponder" disabled onclick="responder()">Responder</div>
            <div class="btn btnCancelarResposta" onclick=\"pararResposta(${idPublicacao})\">Cancelar</div>
        </div>`);

        // Verificação da presença de texto
        $(".txtResponder").on("change keyup paste", function () {
            if ($(".txtResponder").val().trim().length > 0) {
                if ($(".btnResponder").attr("disabled") == "disabled")
                    $(".btnResponder").removeAttr("disabled");
            }
            else {
                if ($(".btnResponder").attr("disabled") != "disabled")
                    $(".btnResponder").attr("disabled", "disabled");
            }
        });
    }
}
function pararResposta(idPublicacao) {
    $("#respondendo" + idPublicacao).remove();
    idPublicacaoRespondendo = -1;
}
function mostrarComentarios(idPublicacao) {
    $('#collapsible' + idPublicacao).css("display", "initial");
    $('#collapsible' + idPublicacao).collapsible('open');
}
function getIdsUsuariosMarcados() {
    var chipsData = M.Chips.getInstance($("#chips")).chipsData;
    var ids = new Array();

    for (var i = 0; i < chipsData.length; i++)
        ids.push(chipsData[i].id);

    return ids;
}
function configurarPostar() {
    var data = new Object();
    var usuariosJaInclusos = new Array();

    var nomeComId = new Array();

    for (var i = 0; i < window.usuario.Amigos.length; i++) {
        var amigo = window.usuario.Amigos[i];

        var nomeRepetido = false;
        for (var j = 0; j < usuariosJaInclusos.length; j++)
            if (usuariosJaInclusos[j].nome == amigo.Nome) {
                usuariosJaInclusos[j].numero++;
                nomeRepetido = true;
                data[amigo.Nome + "(" + usuariosJaInclusos[j].numero + ")"] = amigo.FotoPerfil;
                nomeComId.push({ id: amigo.Id, nome: amigo.Nome + "(" + usuariosJaInclusos[j].numero + ")" })
                break;
            }
        if (!nomeRepetido) {
            usuariosJaInclusos.push({ nome: amigo.Nome, numero: 1 });
            nomeComId.push({ nome: amigo.Nome, id: amigo.Id })
            data[amigo.Nome] = amigo.FotoPerfil;
        }
    }
    $(".modal").modal();
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
            for (var i = 0; i < nomeComId.length; i++)
                if (nomeComId[i].nome == chip.tag) {
                    chip.id = nomeComId[i].id;
                    var jaExiste = false;
                    for (var j = 0; j < chipsData.length - 1; j++)
                        if (chipsData[j].id == window.usuario.Amigos[i].Id) {
                            jaExiste = true;
                            break;
                        }
                    if (jaExiste)
                        break;
                    valido = true;
                    chip.image = window.usuario.Amigos[i].FotoPerfil;
                    $(chipEvento).html(`<img src="${chip.image}">` + $(chipEvento).html());
                    break;
                }
            if (!valido)
                chipsInstance.deleteChip(chipsData.length - 1);
        }
    });
    var verificacao = function (titulo, txt, btn) {
        if ($(titulo).val().trim().length > 0 && $(txt).val().trim().length > 0) {
            if ($(btn).attr("disabled") == "disabled")
                $(btn).removeAttr("disabled");
        }
        else {
            if ($(btn).attr("disabled") != "disabled")
                $(btn).attr("disabled", "disabled");
        }
    }
    $("#tituloPost").on("input", function () { verificacao("#tituloPost", ".txtPostar", ".btnPostar"); });
    $(".txtPostar").on("change keyup paste", function () { verificacao("#tituloPost", ".txtPostar", ".btnPostar"); });

    $("#tituloEditar").on("input", function () { verificacao("#tituloEditar", ".txtEditar", ".btnEditar"); });
    $(".txtEditar").on("change keyup paste", function () { verificacao("#tituloEditar", ".txtEditar", ".btnEditar"); });
}

$(document).ready(function () {
    $(".modal").modal();
    $('.collapsible').collapsible({
        accordion: false
    });
})
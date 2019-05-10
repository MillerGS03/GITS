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
function deletarPublicacao(id) {
    $.post({
        url: "/DeletarPublicacao",
        data: {
            idPublicacao: id
        },
        success: function () {
            if (!index)
                window.location.href = "/perfil";
            else
                window.location.reload();
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
}

setTimeout(function () {
    $(".modal").modal();
}, 50);
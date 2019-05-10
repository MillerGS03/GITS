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

setTimeout(function () {
    $(".modal").modal();
}, 50);
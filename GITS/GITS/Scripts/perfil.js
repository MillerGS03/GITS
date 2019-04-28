var index = false;

function realizarSolicitacao(idUsuario) {
    $.post("enviarsolicitacaopara", idUsuario);
}


//<div class="btn" onclick="realizarSolicitacao(@ViewBag.Usuario.Id)">Adicionar como Amigo</div>

setTimeout(function () {
    if (getCookie("user") != null)
        $("#esquerda").html($("#esquerda").html() + '<div class="btn" onclick="realizarSolicitacao(@ViewBag.Usuario.Id)">Adicionar como Amigo</div>');
}, 50)
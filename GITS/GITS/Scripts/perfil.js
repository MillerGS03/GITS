var index = false;

function realizarSolicitacao(idUsuario) {
    $.post({
        url: "/EnviarSolicitacaoPara",
        data: {
            idUsuario: idUsuario
        }
    });
}


//<div class="btn" onclick="realizarSolicitacao(@ViewBag.Usuario.Id)">Adicionar como Amigo</div>

setTimeout(function () {
    atual = JSON.parse(getCookie("user").substring(6));
    if (atual != null && atual.Id != $("#idUsuarioYYY").html())
        $("#esquerda").html($("#esquerda").html() + `<div class="btn" onclick="realizarSolicitacao(${$("#idUsuarioYYY").html()})">Adicionar como Amigo</div>`);
}, 50)
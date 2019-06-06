$(document).ready(() => {
    var fazParte = window.acontecimento.IdUsuariosMarcados.includes(window.usuario.Id);
    var ehAdm = window.acontecimento.IdUsuariosAdmin.includes(window.usuario.Id);
    if (fazParte) {
        $("#operacaoPrincipal").text('Sair de acontecimento');
        $("#operacaoPrincipal").attr('class', 'waves-effect waves-light btn red lighten-3 white-text');
        $("#operacaoPrincipal").click(function () {
            sairAcontecimento();
        })
        if (ehAdm) {
            var btnExcluir = document.createElement('a')
            btnExcluir.classList.add('waves-effect')
            btnExcluir.classList.add('waves-light')
            btnExcluir.classList.add('btn')
            btnExcluir.classList.add('red')
            btnExcluir.classList.add('white-text')
            btnExcluir.innerText = 'Excluir acontecimento'
            btnExcluir.onclick = excluirAcontecimento
            btnExcluir.id = 'btnExcluirEvento'
            $("#operacaoPrincipal").parent().append(btnExcluir);
        }
    } else {
        $("#operacaoPrincipal").attr('class', 'waves-effect waves-light btn blue white-text');
        $("#operacaoPrincipal").click(function () {
            requisitarAcesso();
        })
    }
})

function requisitarAcesso() {
    $.post({
        url: '/RequisitarParticipacaoAcontecimento',
        data: {
            codAcontecimento: window.acontecimento.CodAcontecimento,
            idUsuario: window.usuario.Id
        },
        success: function () { },
        async: false
    })
}
function sairAcontecimento() {
    modalConfirmacao("Deseja realmente sair desse acontecimento?", "Este acontecimento n&atilde;o aparecer&aacute; novamente para voc&ecirc; e voc&ecirc; dever&aacute; ser convidado novamente para fazer parte dele para t&ecirc;-lo em seu calend&aacute;rio.", () => {
        $.post({
            url: '/SairDeAcontecimento',
            data: {
                codAcontecimento: window.acontecimento.CodAcontecimento,
                idUsuario: window.usuario.Id
            },
            success: function () {
                window.location.reload();
            },
            async: false
        })
    }, () => { })
}
function excluirAcontecimento() {
    modalConfirmacao("Deseja realmente excluir esse acontecimento?", "Essa a&ccedil;&atilde;o n&atilde;o poder&aacute; ser desfeita.", () => {
        $.post({
            url: '/RemoverAcontecimento',
            data: {
                id: window.acontecimento.CodAcontecimento
            },
            success: function () {
                window.location.href = "/"
            },
            async: false
        })
    }, () => { })
}
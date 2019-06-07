$(document).ready(() => {
    var fazParte = window.tarefa.IdUsuariosMarcados.includes(window.usuario.Id);
    var ehAdm = window.tarefa.IdUsuariosAdmin.includes(window.usuario.Id);
    if (fazParte) {
        if (ehAdm) {
            var btnExcluir = document.createElement('a')
            btnExcluir.classList.add('waves-effect')
            btnExcluir.classList.add('waves-light')
            btnExcluir.classList.add('btn')
            btnExcluir.classList.add('red')
            btnExcluir.classList.add('white-text')
            btnExcluir.innerText = 'Excluir tarefa'
            btnExcluir.onclick = excluirTarefa
            btnExcluir.id = 'btnExcluirEvento'
            $("#operacaoPrincipal").parent().append(btnExcluir);
        }
        if (window.tarefa.IdUsuariosMarcados.length > 1) {
            $("#operacaoPrincipal").text('Sair da tarefa');
            $("#operacaoPrincipal").attr('class', 'waves-effect waves-light btn red lighten-3 white-text');
            $("#operacaoPrincipal").click(function () {
                sairTarefa();
            })
        }
        else {
            $("#operacaoPrincipal").remove();
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
        url: '/RequisitarParticipacaoTarefa',
        data: {
            codTarefa: window.tarefa.CodTarefa,
            idUsuario: window.usuario.Id
        },
        success: function () { },
        async: false
    })
}
function sairTarefa() {
    modalConfirmacao("Deseja realmente sair dessa tarefa?", "Esta tarefa n&atilde;o aparecer&aacute; novamente para voc&ecirc; e voc&ecirc; dever&aacute; ser convidado novamente para fazer parte dela para retom&aacute;-la.", () => {
        $.post({
            url: '/SairDeTarefa',
            data: {
                codTarefa: window.tarefa.CodTarefa,
                idUsuario: window.usuario.Id
            },
            success: function () {
                window.location.reload();
            },
            async: false
        })
    }, () => { })
}
function excluirTarefa() {
    modalConfirmacao("Deseja realmente excluir essa tarefa?", "Essa a&ccedil;&atilde;o n&atilde;o poder&aacute; ser desfeita.", () => {
        $.post({
            url: '/RemoverTarefa',
            data: {
                id: window.tarefa.CodTarefa
            },
            success: function () {
                window.location.href = "/"
            },
            async: false
        })
    }, () => { })
}
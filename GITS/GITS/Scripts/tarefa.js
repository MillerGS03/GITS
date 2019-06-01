﻿$(document).ready(() => {
    var fazParte = window.tarefa.IdUsuariosMarcados.includes(window.usuario.Id);
    var ehAdm = window.tarefa.IdUsuariosAdmin.includes(window.usuario.Id);
    if (fazParte) {
        $("#operacaoPrincipal").text('Sair da tarefa');
        $("#operacaoPrincipal").attr('class', 'waves-effect waves-light btn red lighten-3 white-text');
        $("#operacaoPrincipal").click(function () {
            sairTarefa();
        })
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
    } else {
        $("#operacaoPrincipal").attr('class', 'waves-effect waves-light btn blue white-text');
        $("#operacaoPrincipal").click(function () {
            requisitarAcesso();
        })
    }
})

function requisitarAcesso() {
    //
}
function sairTarefa() {
    //
}
function excluirTarefa() {
    //
}
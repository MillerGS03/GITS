$(document).ready(() => {
    var fazParte = window.tarefa.IdUsuariosMarcados.includes(window.usuario.Id);
    if (fazParte) {
        $("#operacaoPrincipal").text('Sair da tarefa');
        $("#operacaoPrincipal").attr('class', 'waves-effect waves-light btn red lighten-3 white-text');
        $("#operacaoPrincipal").click(function () {
            sairTarefa();
        })
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
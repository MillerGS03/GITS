$(document).ready(() => {
    var fazParte = window.acontecimento.IdUsuariosMarcados.includes(window.usuario.Id);
    if (fazParte) {
        $("#operacaoPrincipal").text('Sair de acontecimento');
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
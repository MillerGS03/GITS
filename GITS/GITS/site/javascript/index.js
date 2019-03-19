var user = new Object();
user.CodUsuario = 1;
user.Email = 'vinschers@gmail.com';
user.FotoPerfil = '../imagens/a.jpg';
user.XP = 1000;
user.Status = 'Muito bom dia';
user.Insignia = 1;
user.Titulo = 'Morte';
user.Decoracao = 1;
user.TemaSite = 1;
user.Dinheiro = 500;

$(document).ready(function(){
  $('#slide-out').sidenav({
    edge: 'right'
  });
  $('#slide-out-batata').sidenav();
  $(".dropdown-trigger").dropdown({
    constrainWidth: false
  });
  $('.tooltipped').tooltip();
});

if (user)
{
  $(".imgPerfilBody").html(`<img src="${user.FotoPerfil}" data-target="slide-out-esquerda" class="sidenav-trigger">`);
  $("#imgPerfil").prop('src', user.FotoPerfil);
}
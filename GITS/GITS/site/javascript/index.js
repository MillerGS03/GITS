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

var triggerEsquerda = 0;
var estaAbrindoEsquerda = false;

$(document).ready(function(){
  $('#slide-out').sidenav({
    edge: 'right'
  });
  $('#slide-out-esquerda').sidenav();
  $(".dropdown-trigger").dropdown({
    constrainWidth: false
  });
  $('.tooltipped').tooltip();
});

if (user)
  tratar(user);

function tratar(user)
{
  $("#main").html(`
    <div id="slideEsquerda">
      <img src="" id="imgPerfil">
    </div>
  `);
  $("#slideEsquerda").height(document.getElementById('footer').getBoundingClientRect().top - $(".nav-wrapper").height());
  $("#slideEsquerda").after(`
    <div class="hexagon" id="triggerEsquerda" style="transition: 0.75s;left: ${document.getElementById('slideEsquerda').style.left + document.getElementById('slideEsquerda').style.width - 165}px;"></div>
  `);
  $("#triggerEsquerda").on('click', function(e){
    if (!estaAbrindoEsquerda)
    {
      $(".ripple").remove();
      var posX = $(this).offset().left,
          posY = $(this).offset().top,
          buttonWidth = $(this).width(),
          buttonHeight =  $(this).height();
      
      $(this).prepend("<span class='ripple'></span>");
    
      
      if(buttonWidth >= buttonHeight) {
        buttonHeight = buttonWidth;
      } else {
        buttonWidth = buttonHeight; 
      }
      buttonHeight = 50;
      var x = e.pageX - posX - buttonWidth / 2;
      var y = e.pageY - posY - buttonHeight / 2;
      
    
      $(".ripple").css({
        width: buttonWidth,
        height: buttonHeight,
        top: y + 'px',
        left: x + 'px'
      }).addClass("rippleEffect");

      estaAbrindoEsquerda = true;
      setTimeout(function(){estaAbrindoEsquerda = false;}, 750)
      var left = -triggerEsquerda*$("#slideEsquerda").width() + $("#slideEsquerda").width() - 165;
      $("#slideEsquerda").css('left', (-triggerEsquerda*$("#slideEsquerda").width()) + "px");
      console.log(left)
      $("#triggerEsquerda").css('left', left + "px")
      triggerEsquerda = Math.abs(triggerEsquerda - 1);
    }
  })
}
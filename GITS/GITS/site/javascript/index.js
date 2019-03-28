var index = true;

var usuario = new Object();
usuario.CodUsuario = 1;
usuario.Nome = "Irumyuui";
usuario.Email = 'vinschers@gmail.com';
usuario.FotoPerfil = '../imagens/ir.jpg';
usuario.XP = 800;
usuario.Status = 'Muito bom dia';
usuario.Insignia = 1;
usuario.Titulo = 'Novato';
usuario.Decoracao = 1;
usuario.TemaSite = 1;
usuario.Dinheiro = 500; 

//user = false; // user = usuario;

var lvlAtual = 1;
var xpTotal = 100;

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

  if (index)
  {
    if (usuario)
      tratar(usuario);
    else
      $('#main').load("./login.html");
  }
});

function tratar(user)
{
  $("#main").html(`
    <div id="slideEsquerda">
    </div>
  `);
  $("#slideEsquerda").html(`
    <div class="imgPerfil" style="background: url('${user.FotoPerfil}') center; background-size: cover;"></div>
    <div class="nomeUsuario"><span>${user.Nome}</span></div>
    <div class="tituloUsuario"><span>${user.Titulo}</span></div>
    <div class="lvlUsuario"><span id="lvlUsuario">40</span></div> <div class="barraLvlUsuario"><span id="enchimentoBarra"></span></div>
    `)
  $("#slideEsquerda").height(document.getElementById('footer').getBoundingClientRect().top - $(".nav-wrapper").height());
  $("#slideEsquerda").after(`
    <div class="hexagon" id="triggerEsquerda">
      <i class="material-icons" style="margin-top: 0.7em;" id="setaUmTriggerEsquerda">chevron_right</i>
      <br>
      <i class="material-icons" id="setaDoisTriggerEsquerda">chevron_right</i>
    </div>
  `);
  ganharXP(user.XP);
  $("#triggerEsquerda").on('click', function(e){
    if (!estaAbrindoEsquerda)
    {
      estaAbrindoEsquerda = true;
      if (triggerEsquerda == 0)
      {
        $("#setaUmTriggerEsquerda").rotate(-180);
        $("#setaDoisTriggerEsquerda").rotate(-180);
      }
      else
      {
        $("#setaUmTriggerEsquerda").rotate(0);
        $("#setaDoisTriggerEsquerda").rotate(0);
      }
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
      buttonWidth = 50;
      var x = e.pageX - posX - buttonWidth / 2;
      var y = e.pageY - posY - buttonHeight / 2;
      
    
      $(".ripple").css({
        width: buttonWidth,
        height: buttonHeight,
        top: y + 'px',
        left: x + 'px'
      }).addClass("rippleEffect");
      abrirEsquerda();
    }
  })
  $("#triggerEsquerda").on('dblclick', function(){
    if (!estaAbrindoEsquerda)
    {
      estaAbrindoEsquerda = true;
      if (triggerEsquerda == 0)
      {
        $("#setaUmTriggerEsquerda").rotate(-180);
        $("#setaDoisTriggerEsquerda").rotate(-180);
      }
      else
      {
        $("#setaUmTriggerEsquerda").rotate(0);
        $("#setaDoisTriggerEsquerda").rotate(0);
      }
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
      buttonWidth = 50;
      var x = e.pageX - posX - buttonWidth / 2;
      var y = e.pageY - posY - buttonHeight / 2;
      
    
      $(".ripple").css({
        width: buttonWidth,
        height: buttonHeight,
        top: y + 'px',
        left: x + 'px'
      }).addClass("rippleEffect");
      abrirEsquerda();
    }
  })
}

function abrirEsquerda()
{
  if (estaAbrindoEsquerda)
  {
    var left = -triggerEsquerda*$("#slideEsquerda").width() + $("#slideEsquerda").width() - 165;
    $("#slideEsquerda").css('left', (-triggerEsquerda*$("#slideEsquerda").width()) + "px");
    $("#triggerEsquerda").css('left', left + "px")
    triggerEsquerda = Math.abs(triggerEsquerda - 1);
    setTimeout(function(){estaAbrindoEsquerda = false;}, 1000)
  }
}

jQuery.fn.rotate = function(degrees) {
  $(this).css({'transform' : 'rotate('+ degrees +'deg)'});
};

function ganharXP(xp)
{
  var xpAtual = $("#enchimentoBarra").width()/$(".barraLvlUsuario").width();
  xpAtual += xp/xpTotal;
  xpAtual *= 100;
  xpAtual += 0.01;
  console.log(xpAtual);
  if (xpAtual >= 100) {
    xpAtual -= 100;
    lvlAtual++;
    xpTotal *= 1.2;
    ganharXP(xpAtual);
  }
  else
  {
    $("#enchimentoBarra").css('width', `${xpAtual}%`)
    $("#lvlUsuario").text(lvlAtual);
  }
}






///style="${$(window).width() > 600? `top: 6em;transition: left 1s, display 0.5s; top: ${$("#slideEsquerda").css('top')}px;left: ${document.getElementById('slideEsquerda').style.left + document.getElementById('slideEsquerda').style.width - 167}px;`:'display: none;'}"
var index = true;

var usuario = new Object();
usuario.CodUsuario = 1;
usuario.Nome = "Irumyuui";
usuario.Email = 'vinschers@gmail.com';
usuario.FotoPerfil = '../imagens/ir.jpg';
usuario.XP = 3000;
usuario.Status = 'Muito bom dia';
usuario.Insignia = 1;
usuario.Titulo = 'Novato';
usuario.Decoracao = 1;
usuario.TemaSite = 1;
usuario.Dinheiro = 500; 
usuario.Amigos = [
  {
    CodUsuario: 2,
    Nome: "Faputa",
    Email:'sla',
    FotoPerfil:'../imagens/fapu.jpg',
    XP: 50000, Status:'Matar todos',
    Insignia:3,
    Titulo:'Veterana',
    Decoracao: 3,
    TemaSite: 2,
    Dinheiro: Number.MAX_SAFE_INTEGER,
    Amigos: []
  },
  {
    CodUsuario: 3,
    Nome: "Veko",
    Email:'sla2',
    FotoPerfil:'../imagens/veko.jpg',
    XP: 50000, Status:'Uhh...',
    Insignia:2,
    Titulo:'Sábia',
    Decoracao: 3,
    TemaSite: 2,
    Dinheiro: 0,
    Amigos: []
  }
];

var user = false;  user = usuario;

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
    if (user)
      tratar(user);
    else
      $('#main').load("./login.html");
  }
  $('input.autocomplete').autocomplete({
    data: {},
    limit: 7, // The max amount of results that can be shown at once. Default: Infinity.
    onAutocomplete: function(val) {
      // Callback function when value is autcompleted.
    },
    minLength: 1, // The minimum length of the input for the autocomplete to start. Default: 1.
    });
});

function tratar(user)
{
  $("main").load("./principal.html", function() {
      $("#main").html($("#main").html() + `
        <div id="slideEsquerda">
        </div>
      `);
      var amigos = '';
      usuario.Amigos.forEach(amigo => {
        amigos += `
          <div>
            <img src="${amigo.FotoPerfil}"> <strong>${amigo.Nome}</strong> <i>"${amigo.Status}"</i>
          </div>
        `
      })
      $("#slideEsquerda").html(`
        <div class="imgPerfil" style="background: url('${user.FotoPerfil}') center; background-size: cover;"></div>
        <div class="nomeUsuario"><span>${user.Nome}</span></div>
        <div class="tituloUsuario"><span>${user.Titulo}</span></div>
        <div class="lvlUsuario"><span id="lvlUsuario">40</span></div> <div class="barraLvlUsuario"><span id="enchimentoBarra"></span></div>
        <div class="txtAmigos"><h1>Amigos</h1></div>
        <div class="amigos" id="amigos" ${user.Amigos.length>3?'style="overflow-y:scroll;"':''}>${amigos}</div>
        <div class="pesquisarAmigo">
          <div class="input-field col s6">
            <i class="prefix material-icons">search</i>
            <input id="txtPesquisa" type="text" class="validate">
            <label for="txtPesquisa">Pesquisar amigo</label>
          </div>
        </div>
        `)
      $("#slideEsquerda").height($('#footer').offset().top - $(".nav-wrapper").height());
      $("#slideEsquerda").after(`
        <div class="hexagon" id="triggerEsquerda">
          <i class="material-icons" style="margin-top: 0.7em;" id="setaUmTriggerEsquerda">chevron_right</i>
          <br>
          <i class="material-icons" id="setaDoisTriggerEsquerda">chevron_right</i>
        </div>
      `);
      $("#txtPesquisa").on('input', function(){
        var nome = $("#txtPesquisa").val();
        console.log(nome)
        for (var i = 0; i < user.Amigos.length; i++)
        {
          console.log(user.Amigos[i].Nome.toUpperCase())
          console.log(user.Amigos[i].Nome.toUpperCase().includes(nome.toUpperCase()))
          if (user.Amigos[i].Nome.toUpperCase().includes(nome.toUpperCase()))
          {
            var h = i * $('#amigos').height()/3;
            document.getElementById('amigos').scrollTop = h;
            break;
          }
        }
        // * div.height()/user.Amigos.length
        //index * (div.height()/)
      });
      ganharXP(user.XP, true);
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
          acionarEsquerda();
        }
      })
      $("#triggerEsquerda").on('dblclick', function(e){
        if (!estaAbrindoEsquerda)
        {
          estaAbrindoEsquerda = true;
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
          acionarEsquerda();
        }
      })
  });
}

function acionarEsquerda()
{
  if (estaAbrindoEsquerda)
  {
    if (triggerEsquerda == 0)
      abrirEsquerda();
    else
      fecharEsquerda();
    var left = -triggerEsquerda*$("#slideEsquerda").width() + $("#slideEsquerda").width() - 165;
    triggerEsquerda = Math.abs(triggerEsquerda - 1);
  }
}
function abrirEsquerda()
{
  $("#setaUmTriggerEsquerda").rotate(-180);
  $("#setaDoisTriggerEsquerda").rotate(-180);
  $("#triggerEsquerda").css('left', (-triggerEsquerda*$("#slideEsquerda").width() + $("#slideEsquerda").width() - 165) + "px")
  $("#slideEsquerda").css('left', (-triggerEsquerda*$("#slideEsquerda").width() - 5) + "px");
  $("#containerConteudo").css('left', $("#slideEsquerda").offset().left + $("#slideEsquerda").width() + "px");
  $("#containerConteudo").css('width', 'calc(100% - ' + $("#slideEsquerda").width() +"px)")
  estaAbrindoEsquerda = false;
}
function fecharEsquerda()
{
  $("#setaUmTriggerEsquerda").rotate(0);
  $("#setaDoisTriggerEsquerda").rotate(0);
  $("#triggerEsquerda").css('left', "-165px")
  $("#slideEsquerda").css('left', (-$("#slideEsquerda").width() - 5) + 'px');
  $("#containerConteudo").css('left', '0');
  $("#containerConteudo").css('width', '100%')
  estaAbrindoEsquerda = false;
}

jQuery.fn.rotate = function(degrees) {
  $(this).css({'transform' : 'rotate('+ degrees +'deg)'});
};

function ganharXP(xp, jaSomou)
{
  var xpAtual = ($("#enchimentoBarra").width()/$(".barraLvlUsuario").width()) * 0.91; //aqui xpAtual é a porcentagem de xp que o usuário tem
  xpAtual *= xpTotal; //aqui é o xp absoluto que o usuário tem
  xpAtual += xp; //+ o que ele vai ganhar
  if (!jaSomou)
    user.XP += xp;
  if (xpAtual >= xpTotal) {
    xpAtual -= xpTotal;
    lvlAtual++;
    xpTotal *= 2.1;
    ganharXP(xpAtual, true);
  }
  else
  {
    $("#enchimentoBarra").css('width', `${100*xpAtual/xpTotal}%`)
    $("#lvlUsuario").text(lvlAtual);
  }
}

$( window ).resize(function() {
  $("#slideEsquerda").height($('#footer').offset().top - $(".nav-wrapper").height());
  fecharEsquerda();
});



///style="${$(window).width() > 600? `top: 6em;transition: left 1s, display 0.5s; top: ${$("#slideEsquerda").css('top')}px;left: ${document.getElementById('slideEsquerda').style.left + document.getElementById('slideEsquerda').style.width - 167}px;`:'display: none;'}"
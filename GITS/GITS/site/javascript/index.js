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
    FotoPerfil:'https://vignette.wikia.nocookie.net/madeinabyss/images/8/8e/Faputa_inspects_Reg%27s_blood.jpeg/revision/latest/scale-to-width-down/185?cb=20180309113337',
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
    FotoPerfil:'https://i.redd.it/9ds2ixq39gi01.jpg',
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
    $('.tabs').tabs();
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
      $("#slideEsquerda").height($('#footer').offset().top - $(".nav-wrapper").height()-1);
      $("#slideEsquerda").after(`
        <div class="hexagon" id="triggerEsquerda">
          <i class="material-icons" style="margin-top: 0.7em;" id="setaUmTriggerEsquerda">chevron_right</i>
          <br>
          <i class="material-icons" id="setaDoisTriggerEsquerda">chevron_right</i>
        </div>
      `);
      $("#txtPesquisa").on('input', function(){
        var nome = $("#txtPesquisa").val();
        for (var i = 0; i < user.Amigos.length; i++)
        {
          if (user.Amigos[i].Nome.toUpperCase().includes(nome.toUpperCase()))
          {
            var h = i * $('#amigos').height()/3;
            document.getElementById('amigos').scrollTop = h;
            break;
          }
        }
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
    setTimeout(function(){
      estaAbrindoEsquerda = true;
      acionarEsquerda();
      estaAbrindoEsquerda = false;
    }, 100)
    $("#tabAgenda").load('./calendario.html', function(){
      var calendarEl = document.getElementById('agenda');
      calendar = new FullCalendar.Calendar(calendarEl, {
          plugins: [ 'dayGrid', 'timeGrid', 'list', 'interaction', 'moment', 'luxon' ],
          defaultView: 'dayGridMonth',
          header: {
              left: 'prevYear, prev, today, next, nextYear',
              center: 'title',
              right: 'dayGridMonth,timeGridWeek,timeGridDay'
          },
          locale: 'pt-br',
          eventClick: function(info) {
            console.log(info.event.extendedProps.opa)
          },
          eventMouseEnter: function(info) {
            info.el.style.backgroundColor = 'green';
          },
          eventMouseLeave: function(info) {
            info.el.style.backgroundColor = '';
          },
          windowResize: false
      });
      calendar.render();
      setTimeout(() => {
          calendar.setOption('height', 550);
          calendar.addEvent({
              id: 1,
              title: 'Teste',
              start: '2019-04-12',
              opa: 'lol'
          });
          calendar.setOption('header', {left: ''})
      }, 10);
  })
  });
}

function acionarEsquerda()
{
  if (estaAbrindoEsquerda)
  {
    document.getElementById('triggerEsquerda').style.WebkitTransition = 'left 1s'
    if (triggerEsquerda == 0)
      abrirEsquerda();
    else
      fecharEsquerda();
    var left = -triggerEsquerda*$("#slideEsquerda").width() + $("#slideEsquerda").width() - 165;
    triggerEsquerda = Math.abs(triggerEsquerda - 1);
    setTimeout(function() {
      estaAbrindoEsquerda = false;
      $(".apenasTelasMaiores").css('transition', '')
      $('.tabs').tabs();
      document.getElementById('triggerEsquerda').style.WebkitTransition = ''
    }, 1000);
  }
}
function abrirEsquerda()
{
  $("#slideEsquerda").css("transition","left 1s");
  setTimeout(function() {
    $("#slideEsquerda").css("transition","unset");
  }, 1000)

  $("#setaUmTriggerEsquerda").rotate(-180);
  $("#setaDoisTriggerEsquerda").rotate(-180);
  $("#triggerEsquerda").css('left', (-triggerEsquerda*$("#slideEsquerda").width() + $("#slideEsquerda").width() - 165) + "px")
  $("#slideEsquerda").css('left', "0px");
 // $("#containerConteudo").css('left', $("#slideEsquerda").offset().left + $("#slideEsquerda").width() + "px");
  $("#containerConteudo").css('width', 'calc(100% - 20em)')
  if (tarefasAtivas)
    $(".apenasTelasMaiores").attr('style', 'transition: width 1s, left 1s; width: calc(100% - (20em + 360px));') //OK
  else
  {
    $(".apenasTelasMaiores").attr('style', 'transition: left 1s, width 1.5s; width: calc(100% - 20em);')
  }

  try
  {
    lidarComAberturaSliderEsquerda();
  }
  catch{}
}
function fecharEsquerda()
{
  $("#slideEsquerda").css("transition","left 1s");
  setTimeout(function() {
    $("#slideEsquerda").css("transition","unset");
  }, 1000)

  $("#setaUmTriggerEsquerda").rotate(0);
  $("#setaDoisTriggerEsquerda").rotate(0);
  $("#triggerEsquerda").css('left', "-165px")
  $("#slideEsquerda").css('left', '-20em');
  $("#containerConteudo").css('width', '100%')
  if (tarefasAtivas)
    $(".apenasTelasMaiores").attr('style', 'transition: width 1s, left 1s; width: calc(100% - 360px);') //OK
  else
    $(".apenasTelasMaiores").attr('style', 'transition: width 1s, left 1s; width: 100%;')
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

var forcandoRedimensionamento = false;
$( window ).resize(function() {
  if (!forcandoRedimensionamento)
  {
    setTimeout(function(){
      $("#slideEsquerda").height($('#footer').offset().top - $(".nav-wrapper").height() - 1);
      if (triggerEsquerda == 1)
        $("#triggerEsquerda").css('left', ($("#slideEsquerda").width() - 165) + "px")
      else
        $("#triggerEsquerda").css('left', "-165px")
    }, 500)
    //fecharEsquerda();
    //triggerEsquerda = 0;
  }
});



///style="${$(window).width() > 600? `top: 6em;transition: left 1s, display 0.5s; top: ${$("#slideEsquerda").css('top')}px;left: ${document.getElementById('slideEsquerda').style.left + document.getElementById('slideEsquerda').style.width - 167}px;`:'display: none;'}"
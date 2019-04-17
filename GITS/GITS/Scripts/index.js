var usuario = new Object();
usuario.CodUsuario = 1;
usuario.Nome = "Irumyuui";
usuario.Email = 'vinschers@gmail.com';
usuario.FotoPerfil = '../../Images/ir.jpg';
usuario.XP = 3000;
usuario.Status = 'Muito bom dia';
usuario.Insignia = 1;
usuario.Titulo = 'Novato';
usuario.Decoracao = 1;
usuario.TemaSite = 1;
usuario.Dinheiro = 500;
usuario.lvlAtual = 1;
usuario.xpTotal = 100;
usuario.Amigos = [
    {
        CodUsuario: 2,
        Nome: "Faputa",
        Email: 'sla',
        FotoPerfil: 'https://vignette.wikia.nocookie.net/madeinabyss/images/8/8e/Faputa_inspects_Reg%27s_blood.jpeg/revision/latest/scale-to-width-down/185?cb=20180309113337',
        XP: 50000, Status: 'Matar todos',
        Insignia: 3,
        Titulo: 'Veterana',
        Decoracao: 3,
        TemaSite: 2,
        Dinheiro: Number.MAX_SAFE_INTEGER,
        Amigos: []
    },
    {
        CodUsuario: 3,
        Nome: "Veko",
        Email: 'sla2',
        FotoPerfil: 'https://i.redd.it/9ds2ixq39gi01.jpg',
        XP: 50000, Status: 'Uhh...',
        Insignia: 2,
        Titulo: 'Sábia',
        Decoracao: 3,
        TemaSite: 2,
        Dinheiro: 0,
        Amigos: []
    }
];
var user = false;

var triggerEsquerda = 0;
var estaAbrindoEsquerda = false;

$(document).ready(function () {
    $('#slide-out').sidenav({
        edge: 'right'
    });
    $('#slide-out-esquerda').sidenav();
    $(".dropdown-trigger").dropdown({
        constrainWidth: false
    });
    $('.tooltipped').tooltip();

    if (index) {
        var resultado = JSON.parse(getCookie("user"));
        if (resultado) {
            user = resultado;
            tratar(user);
        }
    }

    $('input.autocomplete').autocomplete({
        data: {},
        limit: 7, // The max amount of results that can be shown at once. Default: Infinity.
        onAutocomplete: function (val) {
            // Callback function when value is autcompleted.
        },
        minLength: 1, // The minimum length of the input for the autocomplete to start. Default: 1.
    });
    configurarFooter();
    user = usuario;
    ganharXP(user.XP, true);
});
function logar() {
    setCookie("user", JSON.stringify(usuario), 15);
    window.location.reload();
}
function tratar(user) {

    $(".apenasTelasPequenas").html(`
        <div class="imgPerfil" style="background: url('${user.FotoPerfil}') center; background-size: cover; width: 6.5em; height: 6.5em; left: 1.5em"></div>
        <div style="position: absolute; top: 1.5em; left: 4.5em; font-weight: 400; font-size: 20pt;"><span>${user.Nome}</span></div>
        <!--<div class="lvlUsuario"><span id="lvlUsuario">40</span></div> <div class="barraLvlUsuario"><span id="enchimentoBarra"></span></div>--!>
        <div class="itensTelasPequenas">
            <div style=""><img src=""><strong>Agenda</strong></div>
            <div style=""><img src=""><strong>Lista de Tarefas</strong></div>
            <div style=""><img src=""><strong>Metas e Objetivos</strong></div>
            <div style=""><img src=""><strong>Feed</strong></div>
        </div>
    `);


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
        <div class="amigos" id="amigos" ${user.Amigos.length > 5 ? 'style="overflow-y:scroll;"' : ''}>${amigos}</div>
        <div class="pesquisarAmigo">
          <div class="input-field col s6">
            <i class="prefix material-icons">search</i>
            <input id="txtPesquisa" type="text" class="validate">
            <label for="txtPesquisa">Pesquisar amigo</label>
          </div>
        </div>
        `)
    $("#slideEsquerda").height($('#footer').offset().top - $(".nav-wrapper").height() - 1);
    $("#slideEsquerda").after(`
        <div class="hexagon" id="triggerEsquerda">
          <i class="material-icons" style="margin-top: 0.7em;" id="setaUmTriggerEsquerda">chevron_right</i>
          <br>
          <i class="material-icons" id="setaDoisTriggerEsquerda">chevron_right</i>
        </div>
      `);
    $("#txtPesquisa").on('input', function () {
        var nome = $("#txtPesquisa").val();
        for (var i = 0; i < user.Amigos.length; i++) {
            if (user.Amigos[i].Nome.toUpperCase().includes(nome.toUpperCase())) {
                var h = i * $('#amigos').height() / 5;
                document.getElementById('amigos').scrollTop = h;
                break;
            }
        }
    });
    //ganharXP(user.XP, true);
    $("#triggerEsquerda").on('click', function (e) {
        if (!estaAbrindoEsquerda) {
            estaAbrindoEsquerda = true;
            if (triggerEsquerda == 0) {
                $("#setaUmTriggerEsquerda").rotate(-180);
                $("#setaDoisTriggerEsquerda").rotate(-180);
            }
            else {
                $("#setaUmTriggerEsquerda").rotate(0);
                $("#setaDoisTriggerEsquerda").rotate(0);
            }
            $(".ripple").remove();
            var posX = $(this).offset().left,
                posY = $(this).offset().top,
                buttonWidth = $(this).width(),
                buttonHeight = $(this).height();

            $(this).prepend("<span class='ripple'></span>");


            if (buttonWidth >= buttonHeight) {
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
    $("#triggerEsquerda").on('dblclick', function (e) {
        if (!estaAbrindoEsquerda) {
            estaAbrindoEsquerda = true;
            $(".ripple").remove();
            var posX = $(this).offset().left,
                posY = $(this).offset().top,
                buttonWidth = $(this).width(),
                buttonHeight = $(this).height();

            $(this).prepend("<span class='ripple'></span>");


            if (buttonWidth >= buttonHeight) {
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
    setTimeout(function () {
        estaAbrindoEsquerda = true;
        acionarEsquerda();
        estaAbrindoEsquerda = false;
    }, 100)
    $('.pesquisarAmigo').attr('style', `top: calc(1000px - 12.5em);`);
    $('#amigos').height(`calc((1000px - 33.5em)`);
    $("#tabAgenda").load('/Main/_Calendario', function () {
        var calendarEl = document.getElementById('agenda');
        calendar = new FullCalendar.Calendar(calendarEl, {
            plugins: ['dayGrid', 'timeGrid', 'list', 'interaction', 'moment', 'luxon'],
            defaultView: 'dayGridMonth',
            header: {
                left: 'prevYear, prev, today, next, nextYear',
                center: 'title',
                right: 'dayGridMonth,timeGridWeek,timeGridDay'
            },
            locale: 'pt-br',
            eventClick: function (info) {
                console.log(info.event.extendedProps.opa)
            },
            eventMouseEnter: function (info) {
                info.el.style.backgroundColor = 'green';
            },
            eventMouseLeave: function (info) {
                info.el.style.backgroundColor = '';
            },
            windowResize: false
        });
        calendar.render();
        setTimeout(() => {
            $("#slideEsquerda").height($('#footer').offset().top - $(".nav-wrapper").height() - 1);
            if (triggerEsquerda == 1)
                $("#triggerEsquerda").css('left', ($("#slideEsquerda").width() - 165) + "px")
            else
                $("#triggerEsquerda").css('left', "-165px")
            var heightCalendar = - $('#tabs-swipe-demo').height();
            heightCalendar += $('.apenasTelasMaiores').height();
            calendar.setOption('height', heightCalendar);
            calendar.addEvent({
                id: 1,
                title: 'Teste',
                start: '2019-04-12',
                opa: 'lol'
            });
        }, 10);
    })
}

function acionarEsquerda() {
    if (estaAbrindoEsquerda) {
        document.getElementById('triggerEsquerda').style.WebkitTransition = 'left 1s'
        if (triggerEsquerda == 0)
            abrirEsquerda();
        else
            fecharEsquerda();
        var left = -triggerEsquerda * $("#slideEsquerda").width() + $("#slideEsquerda").width() - 165;
        triggerEsquerda = Math.abs(triggerEsquerda - 1);
        setTimeout(function () {
            estaAbrindoEsquerda = false;
            $(".apenasTelasMaiores").css('transition', '')
            $('.tabs').tabs();
            document.getElementById('triggerEsquerda').style.WebkitTransition = ''
        }, 1000);
    }
}
function abrirEsquerda() {
    $("#slideEsquerda").css("transition", "left 1s");
    setTimeout(function () {
        $("#slideEsquerda").css("transition", "unset");
    }, 1000)

    $("#setaUmTriggerEsquerda").rotate(-180);
    $("#setaDoisTriggerEsquerda").rotate(-180);
    $("#triggerEsquerda").css('left', (-triggerEsquerda * $("#slideEsquerda").width() + $("#slideEsquerda").width() - 165) + "px")
    $("#slideEsquerda").css('left', "0px");
    // $("#containerConteudo").css('left', $("#slideEsquerda").offset().left + $("#slideEsquerda").width() + "px");
    $("#containerConteudo").css('width', 'calc(100% - 20em)')
    if (tarefasAtivas)
        $(".apenasTelasMaiores").attr('style', 'transition: width 1s, left 1s; width: calc(100% - (20em + 360px));') //OK
    else {
        $(".apenasTelasMaiores").attr('style', 'transition: left 1s, width 1.5s; width: calc(100% - 20em);')
    }

    try {
        lidarComAberturaSliderEsquerda();
    }
    catch{ }
}
function fecharEsquerda() {
    $("#slideEsquerda").css("transition", "left 1s");
    setTimeout(function () {
        $("#slideEsquerda").css("transition", "unset");
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

jQuery.fn.rotate = function (degrees) {
    $(this).css({ 'transform': 'rotate(' + degrees + 'deg)' });
};

function ganharXP(xp, jaSomou) {
    var xpAtual = ($("#enchimentoBarra").width() / $(".barraLvlUsuario").width()) * 0.91; //aqui xpAtual é a porcentagem de xp que o usuário tem
    xpAtual *= user.xpTotal; //aqui é o xp absoluto que o usuário tem
    xpAtual += xp; //+ o que ele vai ganhar
    if (!jaSomou)
        user.XP += xp;
    if (xpAtual >= user.xpTotal) {
        xpAtual -= user.xpTotal;
        user.lvlAtual++;
        user.xpTotal *= 2.1;
        ganharXP(xpAtual, true);
    }
    else {
        $("#enchimentoBarra").css('width', `${100 * xpAtual / user.xpTotal}%`)
        $("#lvlUsuario").text(user.lvlAtual);
    }
}

//function ganharXP(xp, jaSomou) {
//    var xpAtual = ($("#enchimentoBarra").width() / $(".barraLvlUsuario").width()) * 0.91; //aqui xpAtual é a porcentagem de xp que o usuário tem
//    xpAtual *= xpTotal; //aqui é o xp absoluto que o usuário tem
//    xpAtual += xp;
//    console.log(xpAtual)
//    var xpNovo = xpAtual;
//    if (xpAtual > usuario.xpTotal) {
//        usuario.lvlAtual += Math.floor(xpAtual / usuario.xpTotal);
//        xpNovo = xpAtual % usuario.xpTotal;
//        usuario.xpTotal *= 2.1;
//    }
//    $("#enchimentoBarra").css('width', `${100 * xpNovo / usuario.xpTotal}%`)
//    $("#lvlUsuario").text(usuario.lvlAtual);
//    if (!jaSomou)
//        user.XP += xp;
//}

var forcandoRedimensionamento = false;
$(window).resize(function () {
    if (!forcandoRedimensionamento) {
        setTimeout(function () {
            $("#slideEsquerda").height($('#footer').offset().top - $(".nav-wrapper").height() - 1);
            if (triggerEsquerda == 1)
                $("#triggerEsquerda").css('left', ($("#slideEsquerda").width() - 165) + "px")
            else
                $("#triggerEsquerda").css('left', "-165px")
            var heightCalendar = - $('#tabs-swipe-demo').height();
            heightCalendar += $('.apenasTelasMaiores').height();
            calendar.setOption('height', heightCalendar);
            if (window.mobileAndTabletcheck()) {
                calendar.setOption('header', { left: '' })
            }
        }, 500)
    }
});
window.mobileAndTabletcheck = function () {
    var check = false;
    (function (a) { if (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino|android|ipad|playbook|silk/i.test(a) || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0, 4))) check = true; })(navigator.userAgent || navigator.vendor || window.opera);
    return check;
};
function setCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}
function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}
function eraseCookie(name) {
    document.cookie = name + '=; Max-Age=-99999999;';
}

function configurarFooter() {
    $("#footer").css('top', $(document).height() + 'px')
    //    $(".apenasTelasPequenas").height($(document).height() - 456);
}
///style="${$(window).width() > 600? `top: 6em;transition: left 1s, display 0.5s; top: ${$("#slideEsquerda").css('top')}px;left: ${document.getElementById('slideEsquerda').style.left + document.getElementById('slideEsquerda').style.width - 167}px;`:'display: none;'}"
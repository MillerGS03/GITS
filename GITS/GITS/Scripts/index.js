var user;
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
        var resultado = JSON.parse(getCookie("user").substring(6));
        user = resultado;
        console.log(user)
        tratar(user);
    }

    $('input.autocomplete').autocomplete({
        data: {},
        limit: 7, // The max amount of results that can be shown at once. Default: Infinity.
        onAutocomplete: function (val) {
            // Callback function when value is autcompleted.
        },
        minLength: 1, // The minimum length of the input for the autocomplete to start. Default: 1.
    });
    if ($(window).width() < 992)
        setTimeout(resize, 50)
    var tarefasLista = '';
    for (var i = 0; i < user.Tarefas.length; i++) {
        tarefasLista += `
        <li>
            <div class="collapsible-header">
                <label>
                    <input type="checkbox" />
                    <span>${user.Tarefas[i].Titulo}</span>
                </label>
                <div class="infoData valign-wrapper">
                    <span>${user.Tarefas[i].Data}</span>
                    <img src="../../Images/iconeDataTarefa.png">
                </div>
            </div>
            <div class="collapsible-body">
                <span>${user.Tarefas[i].Descricao}</span>
            </div>
        </li>
        `;
    }
    $(".collapsible").html(tarefasLista);
    //ganharXP(user.XP, true);
});
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
            <div style=""><img src=""><strong>Loja</strong></div>
        </div>
    `);


    $("#main").html($("#main").html() + `
        <div id="slideEsquerda">
        </div>
    `);
    var amigos = '';
    user.Amigos.forEach(amigo => {
        if (amigo.FoiAceito) {
            amigos += `
          <div>
            <a href="/perfil/${amigo.Id}"><img src="${amigo.FotoPerfil}" class="hoverable"></a> <a href="/perfil/${amigo.Id}"><strong>${amigo.Nome}</strong></a> <i>"${amigo.Status}"</i>
          </div>
        `
        }
    })
    $("#slideEsquerda").html(`
        <a href="perfil"><div class="imgPerfil hoverable" style="background: url('${user.FotoPerfil}') center; background-size: cover;"></div></a>
        <div class="nomeUsuario"><span><a href="perfil" class="linkSemDecoracao">${user.Nome}</a></span></div>
        <div class="tituloUsuario"><span>${user.Titulo}</span></div>
        <div class="lvlUsuario"><span id="lvlUsuario">0</span></div> <div class="barraLvlUsuario"><span id="enchimentoBarra"></span></div>
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
    $("#tarefas").height($("#slideEsquerda").height());

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
    user.XpTotal = 100;
    ganharXP(user.XP, true);
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
        if (amigos == '') {
            $(".txtAmigos").attr('style', 'display: none;')
            $("#amigos").attr('style', 'display: none;')
            $(".pesquisarAmigo").attr('style', 'display: none;')
        }
        var calendarEl = document.getElementById('agenda');
        var calendar = new FullCalendar.Calendar(calendarEl, {
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
        var heightCalendar = - $('#tabs-swipe-demo').height();
        heightCalendar += $('.apenasTelasMaiores').height();
        calendar.setOption('height', heightCalendar);
        calendar.addEvent({
            id: 1,
            title: 'Teste',
            start: '2019-04-12',
            opa: 'lol'
        });
        this.calendario = calendar;
    }, 100)
    $('.pesquisarAmigo').attr('style', `top: calc(1000px - 12.5em);`);
    $('#amigos').height(`calc((1000px - 33.5em)`);
    $("#tabAgenda").load('/_Calendario', function () {
        //setTimeout(() => {
        $("#slideEsquerda").height($('#footer').offset().top - $(".nav-wrapper").height() - 1);
        $("#tarefas").height($("#slideEsquerda").height());
            if (triggerEsquerda == 1)
                $("#triggerEsquerda").css('left', ($("#slideEsquerda").width() - 165) + "px")
            else
                $("#triggerEsquerda").css('left', "-165px")
            if ($(window).width() < 992)
                fecharEsquerda();
        //}, 10);
    })
}

function acionarEsquerda() {
    if (estaAbrindoEsquerda) {
        document.getElementById('triggerEsquerda').style.WebkitTransition = 'left 1s'
        if (triggerEsquerda == 0)
            abrirEsquerda();
        else
            fecharEsquerda();
        setTimeout(function () {
            window.calendario.setOption('height', $("#tabAgenda").height())
        }, 1000);
        var left = -triggerEsquerda * $("#slideEsquerda").width() + $("#slideEsquerda").width() - 165;
        triggerEsquerda = Math.abs(triggerEsquerda - 1);
        setTimeout(function () {
            estaAbrindoEsquerda = false;
            $(".apenasTelasMaiores").css('transition', '')
            $('.tabs').tabs();
            document.getElementById('triggerEsquerda').style.WebkitTransition = ''
            dispararResize();
            configurarCalendario();
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
    if ($(window).width() > 992) {
        $("#containerConteudo").css('width', 'calc(100% - 20em)')
        if (tarefasAtivas)
            $(".apenasTelasMaiores").attr('style', 'transition: width 1s, left 1s; width: calc(100% - (20em + 360px));') //OK
        else {
            $(".apenasTelasMaiores").attr('style', 'transition: left 1s, width 1.5s; width: calc(100% - 20em);')
        }
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
    var xpAtual = $("#enchimentoBarra").width(); //aqui xpAtual é a porcentagem de xp que o usuário tem
    xpAtual /= $(".barraLvlUsuario").width();
    xpAtual *= 0.91;
    xpAtual *= user.XpTotal; //aqui é o xp absoluto que o usuário tem
    xpAtual += xp; //+ o que ele vai ganhar
    if (!jaSomou)
        user.XP += xp;
    if (xpAtual >= user.XpTotal) {
        xpAtual -= user.XpTotal;
        user.Level++;
        user.XpTotal *= 1.1;
        ganharXP(xpAtual, true);
    }
    else {
        $("#enchimentoBarra").css('width', `${100 * xpAtual / user.XpTotal}%`)
        $("#lvlUsuario").text(user.Level);
    }
}

//function ganharXP(xp, jaSomou) {
//    var xpAtual = ($("#enchimentoBarra").width() / $(".barraLvlUsuario").width()) * 0.91; //aqui xpAtual é a porcentagem de xp que o usuário tem
//    xpAtual *= XpTotal; //aqui é o xp absoluto que o usuário tem
//    xpAtual += xp;
//    console.log(xpAtual)
//    var xpNovo = xpAtual;
//    if (xpAtual > usuario.XpTotal) {
//        usuario.Level += Math.floor(xpAtual / usuario.XpTotal);
//        xpNovo = xpAtual % usuario.XpTotal;
//        usuario.XpTotal *= 2.1;
//    }
//    $("#enchimentoBarra").css('width', `${100 * xpNovo / usuario.XpTotal}%`)
//    $("#lvlUsuario").text(usuario.Level);
//    if (!jaSomou)
//        user.XP += xp;
//}

var forcandoRedimensionamento = false;
$(window).resize(resize);
function resize() {
    if (!forcandoRedimensionamento && index) {
        M.Tabs.getInstance($(".tabs")).destroy();
        //M.Collapsible.getInstance(".collapsible").destroy();
        if (tarefasAtivas && $(window).width() < 992 && triggerEsquerda != 0) {
            $("#triggerEsquerda").click();
        }
        if ($(window).width() < 992) {
            if (triggerEsquerda == 1)
                $("#triggerEsquerda").click();
            if (tarefasAtivas)
                acionarTarefas();
            $("#containerConteudo").attr('style', '');
            $("#tabTarefas").attr('style', '');
            $(".apenasTelasMaiores").css('width', '100%');
            $(".tabs").html(`
                <li class="tab col s3"><a onclick="acionarImg()" href="#metasObjetivos"><img id="imgObjetivos" class="iconeVerticalmenteAlinhado" style="width: 1.5rem; height: 1.5rem; opacity: 0.7;" src="/Images/objetivo.png" />Metas e Objetivos</a></li>
                <li class="tab col s3"><a onclick="acionarImg()" class="active" href="#tabAgenda"><i class="material-icons iconeVerticalmenteAlinhado">today</i>Agenda</a></li>
                <li class="tab col s3"><a onclick="acionarImg()" href="#feed"><i class="material-icons iconeVerticalmenteAlinhado">forum</i>Feed</a></li>
                <li class="tab col s3"><a onclick="acionarImg()" href="#loja"><i class="material-icons iconeVerticalmenteAlinhado">shopping_cart</i>Loja</a></li>
                <li class="tab col s3"><a onclick="acionarImg()" href="#tabTarefas"><img id="imgTarefas" class="iconeVerticalmenteAlinhado" style="width: 1.5rem; height: 1.5rem; opacity: 0.7;" src="/Images/list.png">Tarefas</a></li>
            `);
            $(".infoData").css('top', '60px')
        }
        else {
            $("#tabTarefas").attr('style', 'display: none;');
            acionarTarefas();
            $("#triggerEsquerda").click();
            $(".tabs").html(`
                <li class="tab col s3"><a onclick="acionarImg()" href="#metasObjetivos"><img id="imgObjetivos" class="iconeVerticalmenteAlinhado" style="width: 1.5rem; height: 1.5rem; opacity: 0.7;" src="/Images/objetivo.png"/>Metas e Objetivos</a></li>
                <li class="tab col s3"><a onclick="acionarImg()" class="active" href="#tabAgenda"><i class="material-icons iconeVerticalmenteAlinhado">today</i>Agenda</a></li>
                <li class="tab col s3"><a onclick="acionarImg()" href="#feed"><i class="material-icons iconeVerticalmenteAlinhado">forum</i>Feed</a></li>
                <li class="tab col s3"><a onclick="acionarImg()" href="#loja"><i class="material-icons iconeVerticalmenteAlinhado">shopping_cart</i>Loja</a></li>
            `);
            $(".infoData").css('top', '4px')
        }
        $(".collapsible").collapsible();
        $(".tabs").tabs();
        setTimeout(function () {
            $("#slideEsquerda").height($('#footer').offset().top - $(".nav-wrapper").height() - 1);
            $("#tarefas").height($("#slideEsquerda").height());
            if (triggerEsquerda == 1)
                $("#triggerEsquerda").css('left', ($("#slideEsquerda").width() - 165) + "px")
            else
                $("#triggerEsquerda").css('left', "-165px")
            if (this.calendario != null) {
                var heightCalendar = - $('#tabs-swipe-demo').height();
                heightCalendar += $('#tabAgenda').height();
                this.calendario.setOption('height', $(".conteudo").height() - $("#tabs-swipe-demo").height());
                configurarCalendario();
                configurarFooter();
            }
        }, 500)
    }
    configurarFooter();
}
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
    if (index) {
        $("#footer").css('top', ($(".conteudo").offset().top + $(".conteudo").height()) + 'px');
        $("#slideEsquerda").height($('#footer').offset().top - $(".nav-wrapper").height() - 1);
        $("#tarefas").height($("#slideEsquerda").height());
        if (this.calendario != null) {
            this.calendario.setOption('height', $(".conteudo").height() - $("#tabs-swipe-demo").height());
        }
    }
    else {
        $("#footer").css('top', $(document).height() - $("#footer").height());
    }

    //    $(".apenasTelasPequenas").height($(document).height() - 456);
}
function configurarCalendario() {
    if ($("#agenda").width() < 450) {
        this.calendario.setOption('header', { left: '' })
    }
    else {
        this.calendario.setOption('header', { left: 'prevYear, prev, today, next, nextYear' });
    }
}
///style="${$(window).width() > 600? `top: 6em;transition: left 1s, display 0.5s; top: ${$("#slideEsquerda").css('top')}px;left: ${document.getElementById('slideEsquerda').style.left + document.getElementById('slideEsquerda').style.width - 167}px;`:'display: none;'}"
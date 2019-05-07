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
    var u;
    var resultado = JSON.parse(getCookie("user").substring(6));
    $.get({
        url: '/GetUsuario',
        data: {
            id: resultado
        }
    }, function (result) {
        u = JSON.parse(result);
        u.Notificacoes.forEach((n) => {
            $('#notificacoes').append(n.ToHtml());
        });
        console.log(u)
        if (index) {
            tratar(u)
            $(".tabs").tabs();
            $(".collapsible").collapsible();
            $('.modal').modal();
            noUiSlider.create(document.getElementById('dificuldadeTarefa'), {
                start: 5,
                connect: [true, false],
                step: 1,
                range: {
                    'min': 0,
                    'max': 10
                },
                format: wNumb({
                    decimals: 0
                })
            });
        }
    })

    $('input.autocomplete').autocomplete({
        data: {},
        limit: 7, // The max amount of results that can be shown at once. Default: Infinity.
        onAutocomplete: function (val) {
            // Callback function when value is autcompleted.
        },
        minLength: 1, // The minimum length of the input for the autocomplete to start. Default: 1.
    });
    setTimeout(resize, 50)
});
function tratar(user) {
    $("#nav-mobile").prepend(`<li><a class="tooltipped signout" href="/SignOut" data-tooltip="Bastidores">Sair</a></li>`);
    $("#main").css("height", "50em");
    if (user.Amigos.length > 5)
        $("#amigos").css('overflow-y', 'scroll')
    $("#slideEsquerda").height($('#footer').offset().top - $(".nav-wrapper").height() - 1);
    $("#tarefas").height($("#slideEsquerda").height());
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
    mostrarXP(user);
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
        if (user.Amigos.length == 0) {
            $(".txtAmigos").attr('style', 'display: none;')
            $("#amigos").attr('style', 'display: none;')
            $(".pesquisarAmigo").attr('style', 'display: none;')
        }
        var cliques = 0;
        var infoAnt = null;
        var calendarEl = document.getElementById('agenda');
        var calendar = new FullCalendar.Calendar(calendarEl, {
            plugins: ['dayGrid', 'timeGrid', 'list', 'interaction', 'moment', 'luxon'],
            dateClick: function (info) {
                setTimeout(function () { cliques = 0 }, 300)
                if (++cliques % 2 == 0 && info.dateStr == infoAnt) {
                    adicionarEvento(info);
                    cliques = 0;
                }
                infoAnt = info.dateStr;
            },
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
        var contEventos = 0;
        for (contEventos = 0; contEventos < user.Tarefas.length; contEventos++) {
            calendar.addEvent({
                id: contEventos,
                title: user.Tarefas[contEventos].Titulo,
                start: user.Tarefas[contEventos].Data.substring(6) + '-' +
                    user.Tarefas[contEventos].Data.substring(3, 5) + '-' +
                    user.Tarefas[contEventos].Data.substring(0, 2),
                descricao: user.Tarefas[contEventos].Descricao
            });
        }
        for (contEventos = user.Tarefas.length; contEventos < user.Acontecimentos.length + user.Tarefas.length; contEventos++) {
            calendar.addEvent({
                id: contEventos - user.Tarefas.length,
                title: user.Acontecimentos[contEventos - user.Tarefas.length].Titulo,
                start: user.Acontecimentos[contEventos - user.Tarefas.length].Data.substring(6) + '-' +
                    user.Acontecimentos[contEventos - user.Tarefas.length].Data.substring(3, 5) + '-' +
                    user.Acontecimentos[contEventos - user.Tarefas.length].Data.substring(0, 2),
                descricao: user.Acontecimentos[contEventos - user.Tarefas.length].Descricao
            });
        }
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
function mostrarXP(usuario) {
    var rets = getStatusXP(usuario.XP);
    usuario.Level = rets[1];
    $("#enchimentoBarra").css('width', `${100 * rets[0] / rets[2]}%`)
    $("#lvlUsuario").text(usuario.Level);
}

function getStatusXP(xp) {
    var ret = new Array();
    var xpNecessario = 100;
    var xpAtual = xp;
    var level = 1;
    while (xpAtual > xpNecessario) {
        xpAtual -= xpNecessario;
        xpNecessario *= 1.1;
        level++;
    }
    ret.push(xpAtual);
    ret.push(level);
    ret.push(xpNecessario);
    return ret;
}

var forcandoRedimensionamento = false;
$(window).resize(resize);
function resize() {
    if (!forcandoRedimensionamento && index) {
        M.Tabs.getInstance($(".tabs")).destroy();
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
        $("#footer").css("position", "absolute");
        $("#footer").css('top', ($(".conteudo").offset().top + $(".conteudo").height()) + 'px');
        $("#slideEsquerda").height($('#footer').offset().top - $(".nav-wrapper").height() - 1);
        $("#tarefas").height($("#slideEsquerda").height());
        if (this.calendario != null) {
            this.calendario.setOption('height', $(".conteudo").height() - $("#tabs-swipe-demo").height());
        }
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
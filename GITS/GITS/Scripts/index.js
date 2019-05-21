var index = false;
$(document).ready(function () {
    try {
        console.log(window.usuario)
        if (window.usuario == null || window.usuario == '')
            throw new DOMException();
        $.get({
            url: '/GetTema',
            success: function (tema) {
                tema = JSON.parse(tema);
                document.documentElement.style.setProperty('--tema', tema.Conteudo.substring(0, tema.Conteudo.indexOf(" ")));
                var style = tema.Conteudo.substring(tema.Conteudo.indexOf(" ") + 1)
                $("head").append(`<style>${style}</style>`);
            },
            async: false
        });
    }
    catch {
        $("#btnNotificacoes").remove();
    }
    $('#slide-out').sidenav({
        edge: 'right'
    });
    $('#slide-out-esquerda').sidenav();
    $(".dropdown-trigger").dropdown({
        constrainWidth: false
    });
    $('.tooltipped').tooltip();
    $('input.autocomplete').autocomplete({
        data: {},
        limit: 7, // The max amount of results that can be shown at once. Default: Infinity.
        onAutocomplete: function (val) {
            // Callback function when value is autcompleted.
        },
        minLength: 1, // The minimum length of the input for the autocomplete to start. Default: 1.
    });
    setTimeout(resize, 50)
})

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

var ultimoWidth = $(window).width() > 992 ? 992 : 993;
var forcandoRedimensionamento = false;
$(window).resize(resize);
function resize() {
    if (!forcandoRedimensionamento && index) {
        var tabAtiva = $(".tab a.active").attr('href').substring(1);

        if ($(window).width() <= 992 && ultimoWidth > 992) {

            //$("#slideEsquerda").sidenav('close');
            //$("#containerConteudo").attr('style', '');
            //$("#tabTarefas").attr('style', '');
            //$(".apenasTelasMaiores").css('width', '100%');
            $("#tabTarefas").removeAttr('style');

            M.Tabs.getInstance($(".tabs")).destroy();
            $(".tabs").html(`
                <li class="tab col s3"><a onclick="acionarImg()" class="${(tabAtiva == "metasObjetivos" ? "active" : "")}" href="#metasObjetivos"><img id="imgObjetivos" class="iconeVerticalmenteAlinhado" style="width: 1.5rem; height: 1.5rem; opacity: 0.7;" src="/Images/objetivo.png" />Metas e Objetivos</a></li>
                <li class="tab col s3"><a onclick="acionarImg()" class="${(tabAtiva == "tabAgenda" ? "active" : "")}" href="#tabAgenda"><i class="material-icons iconeVerticalmenteAlinhado">today</i>Agenda</a></li>
                <li class="tab col s3"><a onclick="acionarImg()" class="${(tabAtiva == "feed" ? "active" : "")}" href="#feed"><i class="material-icons iconeVerticalmenteAlinhado">forum</i>Feed</a></li>
                <li class="tab col s3"><a onclick="acionarImg()" class="${(tabAtiva == "loja" ? "active" : "")}" href="#loja"><i class="material-icons iconeVerticalmenteAlinhado">shopping_cart</i>Loja</a></li>
                <li class="tab col s3"><a onclick="acionarImg()" href="#tabTarefas"><img id="imgTarefas" class="iconeVerticalmenteAlinhado" style="width: 1.5rem; height: 1.5rem; opacity: 0.7;" src="/Images/list.png">Tarefas</a></li>
            `);

            $(".tabs").tabs();
        }
        else if ($(window).width() > 992 && ultimoWidth <= 992) {
            if (tarefasAtivas)
                acionarTarefas();
            //acionarTarefas();
            //$("#slideEsquerda").sidenav('open');

            M.Tabs.getInstance($(".tabs")).destroy();
            $(".tabs").html(`
                <li class="tab col s3"><a onclick="acionarImg()" class="${(tabAtiva == "metasObjetivos" ? "active" : "")}" href="#metasObjetivos"><img id="imgObjetivos" class="iconeVerticalmenteAlinhado" style="width: 1.5rem; height: 1.5rem; opacity: 0.7;" src="/Images/objetivo.png"/>Metas e Objetivos</a></li>
                <li class="tab col s3"><a onclick="acionarImg()" class="${(tabAtiva == "tabAgenda" ? "active" : "")}" href="#tabAgenda"><i class="material-icons iconeVerticalmenteAlinhado">today</i>Agenda</a></li>
                <li class="tab col s3"><a onclick="acionarImg()" class="${(tabAtiva == "feed" ? "active" : "")}" href="#feed"><i class="material-icons iconeVerticalmenteAlinhado">forum</i>Feed</a></li>
                <li class="tab col s3"><a onclick="acionarImg()" class="${(tabAtiva == "loja" || tabAtiva == "tabTarefas" ? "active" : "")}" href="#loja"><i class="material-icons iconeVerticalmenteAlinhado">shopping_cart</i>Loja</a></li>
            `);
            $(".infoData").css('top', '4px');

            $('.tabs').tabs();

            $("#tabTarefas").attr('style', 'display: none;');
        }/*
        $(".collapsible").collapsible();
        $(".tabs").tabs();*/
        setTimeout(function () {
            $("#slideEsquerda").height($('#footer').offset().top - $(".nav-wrapper").height() - 1);
            $("#tarefas").height($("#slideEsquerda").height());
            if (this.calendario != null) {
                var heightCalendar = - $('#tabs-swipe-demo').height();
                heightCalendar += $('#tabAgenda').height();
                this.calendario.setOption('height', $(".conteudo").height() - $("#tabs-swipe-demo").height());
                configurarCalendario();
                configurarFooter();
            }
        }, 500)
        ultimoWidth = $(window).width();
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
String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

function setRainbow() {
    $('.rainbow').html(function (i, html) {
        var chars;
        if (!html.substring(6).includes('<span>'))
            chars = $.trim(html).split("");
        else
            chars = $.trim(html).substring(6, $.trim(html).length - 7).split('</span><span>');
        return '<span>' + chars.join('</span><span>') + '</span>';
    });
}
function diferenca(a1, a2) {
    var diff = [];
    for (var i = 0; i < a1.length; i++) {
        if (!a2.includes(a1[i]))
            diff.push(a1[i])
    }
    for (var i = 0; i < a2.length; i++) {
        if (!(a1.includes(a2[i]) && diff.includes(a2[i])))
            diff.push(a2[i]);
    }
    return diff;
}

function calcUrgencia(dataCriacao, dataFim, dificuldade) {
    var diffTotal = Math.round((dataFim - dataCriacao) / (1000 * 60 * 60 * 24));
    var diffAtual = Math.round((new Date() - dataCriacao) / (1000 * 60 * 60 * 24));
    if (diffTotal > diffAtual)
        return diffAtual / diffTotal * dificuldade;
    return 10;
}
function calcRecompensa(diff) {
    return (Math.random() * (101 - 1) + 1) * diff / (Math.random() * (1 - 5) + 1);
}


function modalConfirmacao(txt, p, func, first) {
    first();
    $("body").append(`<div id="modal1" class="modal"> <div class="modal-content"> <h4>${txt}</h4> <p>${p}</p> </div> <div class="modal-footer"> <a href="#!" id="btnModalConfirmacao" class="modal-close waves-effect waves-green btn-flat">Aceitar</a> </div> </div>`);
    $("#btnModalConfirmacao").click(function () {
        func();
    })
    $('#modal1').modal({
        onCloseEnd: function () {
            $('#modal1').remove();
        }
    });
    $('#modal1').modal('open');
}
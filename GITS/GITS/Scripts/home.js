var index = true;
var tarefasAtivas = true;
function acionarTarefas() {
    $('.carousel.carousel-slider').css("transition", "width 1s");

    tarefasAtivas = !tarefasAtivas;
    if (!tarefasAtivas) {
        $("aside").css("width", "0px");
        $("#btnAcionarTarefas").css("right", "10px");
        $('.carousel.carousel-slider').css("width", "100%");
        if ($(window).width() > 992) {
            if (triggerEsquerda == 1 || estaAbrindoEsquerda)
                $(".apenasTelasMaiores").attr('style', 'transition: width 1s, left 1s; width: calc(100% - 20em); left: 20em;')
            else
                $(".apenasTelasMaiores").attr('style', 'transition: width 1s, left 1s; width: 100%; left: 0;')
        }
    }
    else {
        $("aside").css("display", "initial");
        $("aside").css("width", "360px");
        $("#btnAcionarTarefas").css("right", "370px");
        $('.carousel.carousel-slider').css("width", "calc(100% - 360px)");

        //if ($(window).width() < 992 && triggerEsquerda != 0)
        //    $("#triggerEsquerda").click();
        if ($(window).width() > 992) {
            if (triggerEsquerda == 1)
                $(".apenasTelasMaiores").attr('style', "transition: width 1s, left 1s; width: calc(100% - (20em + 360px)); left: 20em");
            else
                $(".apenasTelasMaiores").attr('style', "transition: width 1s, left 1s; left: 0; width: calc(100% - 360px)");
        }
    }

    var instance = M.Carousel.getInstance(document.getElementById("carouselImportante"));
    var index = instance.center;

    setTimeout(function () {
        dispararResize();
        $('.carousel.carousel-slider').css("transition", "unset");
        if ($("aside").width() == 0)
            $("aside").css("display", "none");
        $('.tabs').tabs();
    }, 1000);
}

function acionarImg() {
    setTimeout(function () {
        var estilo = document.getElementById('imgObjetivos').style;

        if ($("#metasObjetivos").css("display") == "none")
            estilo.opacity = 0.7;
        else
            estilo.opacity = 1;
        if (document.getElementById('imgTarefas') != null) {
            var estilo = document.getElementById('imgTarefas').style;

            if ($("#tabTarefas").css("display") == "none")
                estilo.opacity = 0.7;
            else
                estilo.opacity = 1;
        }
    }, 10)
}

function dispararResize() {
    forcandoRedimensionamento = true;

    var event = new Event('resize', {
        'view': window,
        'bubbles': true,
        'cancelable': true
    });

    var elem = document.getElementById("carouselImportante");
    elem.dispatchEvent(event);

    forcandoRedimensionamento = false;
}

function lidarComAberturaSliderEsquerda() {
    if ($(window).width() < 992 && $("aside").width() > 0)
        acionarTarefas();
}
var calendar;
setTimeout(function () {
    $('.carousel.carousel-slider').carousel({
        fullWidth: true,
        indicators: true
    });
    dispararResize();
    $("#btnAcionarTarefas").tooltip();
    $("#listaTarefas").collapsible();
    $('.tabs').tabs();
}, 1);
var metas = new Array();
var amigos = new Array();
var validChipsValues;
function adicionarEvento(info) {
    if (info.date >= new Date()) {
        $("#adicionarEvento").modal('open');
        $("#dataEvento").val(info.dateStr);
        $("#dataEvento").change(verificarCamposTarefa)
        $("#txtTitulo").change(verificarCamposTarefa)
        $("#txtTitulo").focusout(verificarCamposTarefa)
        $("#txtDescricao").change(verificarCamposTarefa)
        $("#txtDescricao").focusout(verificarCamposTarefa)
        $("#txtMeta").change(verificarCamposTarefa)
        $("#txtMeta").focusout(verificarCamposTarefa)
        $('#txtTitulo').characterCounter();
        if (document.getElementById('chkMeta').checked) {
            $("#selectMeta").css('display', 'block');
        }
        else {
            $("#selectMeta").css('display', 'none');
        }
        $("#chkMeta").change(function () {
            if (this.checked) {
                $("#selectMeta").css('display', 'block');
            }
            else {
                $("#selectMeta").css('display', 'none');
            }
        });
        $("#addEvento").on('click', adicionarTarefa)
        $('#divTarefas').css('display', 'none');
        $('#divAcontecimentos').css('display', 'none');
        $('input:radio[name="radioTipoEvento"]').change(
            function () {
                $('#continuacaoAdicaoEvento').css('display', 'block');
                if ($(this).is(':checked') && $(this).val() == 'tarefa') {
                    $('#divAcontecimentos').css('display', 'none');
                    $('#divTarefas').css('display', 'block');
                }
                else {
                    $('#divAcontecimentos').css('display', 'block');
                    $('#divTarefas').css('display', 'none');
                }
            }
        );
        $.get({
            url: 'GetUsuario/',
            data: {
                id: JSON.parse(getCookie("user").substring(6))
            }
        }, function (result) {
            var u = JSON.parse(result)
            metas = new Array();
            amigos = new Array();
            u.Metas.forEach(function (m) { metas.push(m.Titulo) })
            u.Amigos.forEach(function (a) { amigos.push(a.Nome) })
            var obj = new Object();
            for (var i = 0; i < u.Metas.length; i++)
                obj[`${u.Metas[i].Titulo}`] = null;
            $('#txtMeta').autocomplete({
                data: obj,
            });
            obj = new Object();
            validChipsValues = new Array();
            for (var i = 0; i < u.Amigos.length; i++) {
                obj[`${u.Amigos[i].Nome}`] = u.Amigos[i].FotoPerfil;
                validChipsValues[i] = u.Amigos[i].Nome
            }
            $('#conviteAmigos').chips({
                autocompleteOptions: {
                    data: obj,
                    limit: Infinity,
                    minLength: 1,
                },
                onChipAdd: function (e, chip) {
                    for (var i = 0; i < u.Amigos.length; i++)
                        if (chip.innerText.includes(u.Amigos[i].Nome)) {
                            $(chip).prepend(`<img src="${u.Amigos[i].FotoPerfil}" />`)
                            this.chipsData[this.chipsData.length - 1].img = u.Amigos[i].FotoPerfil;
                            this.chipsData[this.chipsData.length - 1].tag = u.Amigos[i].Nome;
                        }
                    if (validChipsValues.includes(this.chipsData[this.chipsData.length - 1].tag)) return;
                    $("#conviteAmigos .chip").remove(`:nth-child(${this.chipsData.length})`);
                    this.chipsData.pop();
                }
            });
            $("#conviteAmigos input").change(verificarCamposTarefa)
            $("#conviteAmigos input").focusout(verificarCamposTarefa)
            $("#conviteAmigos input").on('focus', function () { $("#conviteAmigos").parent().children().css('color', 'black'); })
            $("#conviteAmigos input").attr('style', 'width: 100% !important;')
        });
    }
}

function verificarCamposTarefa() {
    var erro = false;
    if ($("#dataEvento").val() == null || $("#dataEvento").val() == "") {
        erro = true;
        if (!$("#dataEvento").hasClass('invalid'))
            $("#dataEvento").attr('class', 'validate invalid');
    }
    if ($("#txtTitulo").val() == null || $("#txtTitulo").val().trim() == "" || $("#txtTitulo").val().trim().length > 65) {
        erro = true;
        if (!$("#txtTitulo").hasClass('invalid'))
            $("#txtTitulo").attr('class', 'validate invalid');
    }
    if (document.getElementById('chkMeta').checked && ($("#txtMeta").val() == null || $("#txtMeta").val().trim() == "" || !metas.includes($("#txtMeta").val().trim()))) {
        erro = true;
        $("#txtMeta").attr('class', 'validate invalid');
    }
    var data = new Array();
    var chips = M.Chips.getInstance(document.getElementById('conviteAmigos'));
    chips.chipsData.forEach(function (c) {
        data.push(c.tag);
    })
    if (!amigos.includes(data)) {
        erro = true;
        $("#conviteAmigos input").attr('class', 'input invalid');
        $("#conviteAmigos").parent().children().css('color', '#F44336')
    }
    if (erro)
        $(".modal-footer").html(`<button disabled class="modal-close waves-effect waves-green btn-flat" id="addEvento">Adicionar</button>`);
    else {
        $(".modal-footer").html(`<button class="modal-close waves-effect waves-green btn-flat" id="addEvento">Adicionar</button>`);
        $("#conviteAmigos").parent().children().css('color', 'black')
    }
    $("#addEvento").on('click', adicionarTarefa)
    return erro;
}

function calcUrgencia(d) {
    var data = new Date(d);
    return 5;
}
function adicionarTarefa() {
    if (!verificarCamposTarefa()) {
        var objEvento = {
            Titulo: $("#txtTitulo").val(),
            Descricao: $("#txtDescricao").val(),
            Dificuldade: document.getElementById('dificuldadeTarefa').noUiSlider.get(),
            Urgencia: calcUrgencia($("#dataEvento").val()),
            Data: $("#dataEvento").val()
        };
        var con = M.Chips.getInstance(document.getElementById('conviteAmigos')).chipsData;
        var convites = new Array();
        con.forEach(function (c) {
            convites.push(c.tag);
        });
        $.post({
            url: '/CriarTarefa',
            data: {
                evento: objEvento,
                nomeMeta: $("#txtMeta").val(),
                convites: convites
            }
        })
    }
}
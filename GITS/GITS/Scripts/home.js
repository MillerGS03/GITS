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

function adicionarEvento(info) {
    if (info.date >= new Date()) {
        verificarCamposTarefa();
        $("#adicionarEvento").modal('open');
        $("#dataEvento").val(info.dateStr);
        $("#dataEvento").change(verificarCamposTarefa)
        $("#txtTitulo").change(verificarCamposTarefa)
        $("#txtDescricao").change(verificarCamposTarefa)
        $("#txtMeta").change(verificarCamposTarefa)
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
            var obj = new Object();
            for (var i = 0; i < u.Metas.length; i++)
                obj[`${u.Metas[i].Titulo}`] = null;
            $('#txtMeta').autocomplete({
                data: obj,
            });
            obj = new Array();
            for (var i = 0; i < u.Amigos.length; i++) {
                obj[`${u.Amigos[i].Nome}`] = u.Amigos[i].FotoPerfil;
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
                        }
                }
            });
        });
    }
}

function verificarCamposTarefa() {
    var erro = false;
    if ($("#dataEvento").val() == null || $("#dataEvento").val() == "")
        erro = true;
    if ($("#txtTitulo").val() == null || $("#txtTitulo").val().trim() == "")
        erro = true;
    if (document.getElementById('chkMeta').checked && ($("#txtMeta").val() == null || $("#txtMeta").val().trim() == ""))
        erro = true;
    if (erro)
        $(".modal-footer").html(`<button disabled class="modal-close waves-effect waves-green btn-flat" id="addEvento">Adicionar</button>`);
    else
        $(".modal-footer").html(`<button class="modal-close waves-effect waves-green btn-flat" id="addEvento">Adicionar</button>`);
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
var index = true;
var tarefasAtivas = true;
var triggerEsquerda = 0;
var estaAbrindoEsquerda = false;
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
$(document).ready(function () {
    $('.carousel.carousel-slider').carousel({
        fullWidth: true,
        indicators: true
    });
    dispararResize();
    $("#btnAcionarTarefas").tooltip();
    $("#listaTarefas").collapsible();
    $('.tabs').tabs();
    console.log(window.usuario)
    tratar(window.usuario);
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
});
var metas = new Array();
var amigos = new Array();
var validChipsValues;
function adicionarEvento(info) {
    $("#adicionarEvento").modal('open');
    if (info && info.date >= new Date())
        $("#dataEvento").val(info.dateStr);
    $("#dataEvento").change(verificarCamposTarefa)
    $("#txtTitulo").change(verificarCamposTarefa)
    $("#txtTitulo").focusout(verificarCamposTarefa)
    $("#txtDescricao").change(verificarCamposTarefa)
    $("#txtDescricao").focusout(verificarCamposTarefa)
    $("#txtMeta").change(verificarCamposTarefa)
    $("#txtMeta").focusout(verificarCamposTarefa)
    $('#txtTitulo').characterCounter();
    $("#txtTitulo").val("");
    $("#txtDescricao").val("");
    $("#txtMeta").val("");
    $("#conviteAmigos input").val("");
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
    $('#divTarefas').css('display', 'none');
    $('#divAcontecimentos').css('display', 'none');
    $('input:radio[name="radioTipoEvento"]').change(
        function () {
            $('#continuacaoAdicaoEvento').css('display', 'block');
            if ($(this).is(':checked') && $(this).val() == 'tarefa') {
                $('#divAcontecimentos').css('display', 'none');
                $('#divTarefas').css('display', 'block');
                $("#addEvento").on('click', adicionarTarefa)
            }
            else {
                $('#divAcontecimentos').css('display', 'block');
                $('#divTarefas').css('display', 'none');
            }
        }
    );
    var u = window.usuario;
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
    var usuariosJaInclusos = new Array();
    var nomeComId = new Array();

    for (var i = 0; i < window.usuario.Amigos.length; i++) {
        var amigo = window.usuario.Amigos[i];

        var nomeRepetido = false;
        for (var j = 0; j < usuariosJaInclusos.length; j++)
            if (usuariosJaInclusos[j].nome == amigo.Nome) {
                usuariosJaInclusos[j].numero++;
                nomeRepetido = true;
                obj[amigo.Nome + "(" + usuariosJaInclusos[j].numero + ")"] = amigo.FotoPerfil;
                nomeComId.push({ id: amigo.Id, nome: amigo.Nome + "(" + usuariosJaInclusos[j].numero + ")" })
                break;
            }
        if (!nomeRepetido) {
            usuariosJaInclusos.push({ nome: amigo.Nome, numero: 1 });
            nomeComId.push({ nome: amigo.Nome, id: amigo.Id })
            obj[amigo.Nome] = amigo.FotoPerfil;
        }
    }
    $('#conviteAmigos').chips({
        autocompleteOptions: {
            data: obj,
            limit: Infinity,
            minLength: 1,
        },
        onChipAdd: function (e, chipEvento) {
            var chipsInstance = M.Chips.getInstance($("#conviteAmigos"));
            var chipsData = chipsInstance.chipsData;
            var chip = chipsData[chipsData.length - 1];
            var valido = false;
            for (var i = 0; i < nomeComId.length; i++)
                if (nomeComId[i].nome == chip.tag) {
                    chip.id = nomeComId[i].id;
                    var jaExiste = false;
                    for (var j = 0; j < chipsData.length - 1; j++)
                        if (chipsData[j].id == window.usuario.Amigos[i].Id) {
                            jaExiste = true;
                            break;
                        }
                    if (jaExiste)
                        break;
                    valido = true;
                    chip.image = window.usuario.Amigos[i].FotoPerfil;
                    if (!$(chipEvento).html().includes('<img>'))
                        $(chipEvento).html(`<img src="${chip.image}">` + $(chipEvento).html());
                    break;
                }
            if (!valido)
                chipsInstance.deleteChip(chipsData.length - 1);
        }
    });
    $("#conviteAmigos input").change(verificarCamposTarefa)
    $("#conviteAmigos input").focusout(verificarCamposTarefa)
    $("#conviteAmigos input").on('focus', function () { $("#conviteAmigos").parent().children().css('color', 'black'); })
    $("#conviteAmigos input").attr('style', 'width: 100% !important;')
}
function editarEvento(info) {
    $("#adicionarEvento").modal('open');
    var dataEvento = new Date(info.event.start)
    $("#dataEvento").val(dataEvento.getFullYear() + '-' + dataEvento.getMonth().toString().padStart(2, '0') + '-' + dataEvento.getDate().toString().padStart(2, '0'));
    $("#dataEvento").change(verificarCamposTarefa)
    $("#txtTitulo").change(verificarCamposTarefa)
    $("#txtTitulo").focusout(verificarCamposTarefa)
    $("#txtDescricao").change(verificarCamposTarefa)
    $("#txtDescricao").focusout(verificarCamposTarefa)
    $("#txtMeta").change(verificarCamposTarefa)
    $("#txtMeta").focusout(verificarCamposTarefa)
    $('#txtTitulo').characterCounter();
    $("#txtTitulo").val(info.event.title);
    $("#txtDescricao").val(info.event.extendedProps.descricao);
    if (info.event.extendedProps.meta != null)
        $("#txtMeta").val(info.event.extendedProps.meta.Titulo);
    if (info.event.extendedProps.meta != null) {
        $("#selectMeta").css('display', 'block');
        $("#chkMeta").prop('checked', true);
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
    $('#divTarefas').css('display', 'none');
    $('#divAcontecimentos').css('display', 'none');
    $('input:radio[name="radioTipoEvento"]').change(
        function () {
            $('#continuacaoAdicaoEvento').css('display', 'block');
            if ($(this).is(':checked') && $(this).val() == 'tarefa') {
                $('#divAcontecimentos').css('display', 'none');
                $('#divTarefas').css('display', 'block');
                $("#addTarefa").on('click', editarTarefa)
                if (info.event.extendedProps.tipo == 0)
                {
                    $("#txtTitulo").val(info.event.title);
                    $("#txtDescricao").val(info.event.extendedProps.descricao);
                    if (info.event.extendedProps.meta != null)
                        $("#txtMeta").val(info.event.extendedProps.meta.Titulo);
                    $.post({
                        url: '/GetUsuarios',
                        data: { ids: info.event.extendedProps.marcados },
                        success: function (us) {
                            us = JSON.parse(us)
                            obj = new Object();
                            var usuariosJaInclusos = new Array();
                            var nomeComId = new Array();

                            for (var i = 0; i < us.length; i++) {
                                var marcadoAtual = us[i];

                                var nomeRepetido = false;
                                for (var j = 0; j < usuariosJaInclusos.length; j++)
                                    if (usuariosJaInclusos[j].nome == marcadoAtual.Nome) {
                                        usuariosJaInclusos[j].numero++;
                                        nomeRepetido = true;
                                        obj[marcadoAtual.Nome + "(" + usuariosJaInclusos[j].numero + ")"] = marcadoAtual.FotoPerfil;
                                        nomeComId.push({ id: marcadoAtual.Id, nome: marcadoAtual.Nome + "(" + usuariosJaInclusos[j].numero + ")" })
                                        break;
                                    }
                                if (!nomeRepetido) {
                                    usuariosJaInclusos.push({ nome: marcadoAtual.Nome, numero: 1 });
                                    nomeComId.push({ nome: marcadoAtual.Nome, id: marcadoAtual.Id })
                                    obj[marcadoAtual.Nome] = marcadoAtual.FotoPerfil;
                                }
                            }
                            for (var l = 0; l < us.length; l++) {
                                M.Chips.getInstance(document.getElementById('conviteAmigos')).addChip({
                                    tag: nomeComId[l].nome,
                                    image: us[l].FotoPerfil,
                                    id: nomeComId[l].id,
                                });
                            }
                        },
                        async: false
                    })
                }
                else {
                    $("#txtTitulo").val("");
                    $("#txtDescricao").val("");
                    $("#txtMeta").val("");
                    $("#conviteAmigos input").val("");
                }
            }
            else {
                $('#divAcontecimentos').css('display', 'block');
                $('#divTarefas').css('display', 'none');
            }
        }
    );
    var u = window.usuario;
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
    var usuariosJaInclusos = new Array();
    var nomeComId = new Array();

    for (var i = 0; i < window.usuario.Amigos.length; i++) {
        var amigo = window.usuario.Amigos[i];

        var nomeRepetido = false;
        for (var j = 0; j < usuariosJaInclusos.length; j++)
            if (usuariosJaInclusos[j].nome == amigo.Nome) {
                usuariosJaInclusos[j].numero++;
                nomeRepetido = true;
                obj[amigo.Nome + "(" + usuariosJaInclusos[j].numero + ")"] = amigo.FotoPerfil;
                nomeComId.push({ id: amigo.Id, nome: amigo.Nome + "(" + usuariosJaInclusos[j].numero + ")" })
                break;
            }
        if (!nomeRepetido) {
            usuariosJaInclusos.push({ nome: amigo.Nome, numero: 1 });
            nomeComId.push({ nome: amigo.Nome, id: amigo.Id })
            obj[amigo.Nome] = amigo.FotoPerfil;
        }
    }
    window.chipsData = [];
    var primeiraVez = true;
    $('#conviteAmigos').chips({
        autocompleteOptions: {
            data: obj,
            limit: Infinity,
            minLength: 1,
        },
        onChipAdd: function (e, chipEvento) {
            var chipsInstance = M.Chips.getInstance($("#conviteAmigos"));
            var chipsData = chipsInstance.chipsData;
            var chip = chipsData[chipsData.length - 1];
            var valido = false;
            for (var i = 0; i < nomeComId.length; i++)
                if (nomeComId[i].nome == chip.tag) {
                    chip.id = nomeComId[i].id;
                    var jaExiste = false;
                    for (var j = 0; j < chipsData.length - 1; j++)
                        if (chipsData[j].id == amigos[i].Id) {
                            jaExiste = true;
                            break;
                        }
                    if (jaExiste)
                        break;
                    valido = true;
                    chip.image = amigos[i].FotoPerfil;
                    if (!$(chipEvento).html().includes('img'))
                        $(chipEvento).html(`<img src="${chip.image}">` + $(chipEvento).html());
                    break;
                }
            if (!valido) {
                primeiraVez = true;
                chipsInstance.deleteChip(chipsData.length - 1);
            }
            else
                window.chipsData.push(chip);
        },
        onChipDelete: function (e, chip) {
            if (window.chipsData != null) {
                var chipsData = M.Chips.getInstance($("#conviteAmigos")).chipsData;
                var chipDeletado = diferenca(window.chipsData, chipsData)[0];
                if (chipDeletado != null) {
                    alert(chipDeletado.id)
                }
                for (var o = 0; o < window.chipsData.length; o++)
                    if (window.chipsData[o].id == chipDeletado.id)
                        window.chipsData.splice(o, 1);
            }
            primeiraVez = false;
        }
    });
    $("#conviteAmigos input").change(verificarCamposTarefa)
    $("#conviteAmigos input").focusout(verificarCamposTarefa)
    $("#conviteAmigos input").on('focus', function () { $("#conviteAmigos").parent().children().css('color', 'black'); })
    $("#conviteAmigos input").attr('style', 'width: 100% !important;')
    if (info.event.extendedProps.tipo == 0)
        $('#radioTarefa').click();
    else
        $('#radioAcontecimento').click();
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
        data.push(c.id);
    })
    var tem = true;
    for (var i = 0; i < window.usuario.Amigos.length; i++)
        if (!data.includes(window.usuario.Amigos[i].Id))
            tem = false;
    if (!tem) {
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
            convites.push(c.id);
        });
        objEvento.IdUsuariosMarcados = convites;
        $.post({
            url: '/CriarTarefa',
            data: {
                evento: objEvento,
                nomeMeta: $("#txtMeta").val()
            },
            success: function (e) {
                window.usuario.Tarefas.push(e);
                objEvento.IdUsuariosAdmin = window.usuario.Id;
                window.calendario.addEvent({
                    title: objEvento.Titulo,
                    start: objEvento.Data,
                    descricao: objEvento.Descricao,
                    usuariosAdmin: e.IdUsuariosAdmin,
                    tipo: 0,
                    marcados: objEvento.IdUsuariosMarcados
                });
            },
            async: false
        })
    }
}
function editarTarefa() {
    //
}

function mudarTableLoja(element) {
    var index = 0;
    $(element).parent().children().each((i, e) => {
        $(e).attr('class', 'collection-item');
        if (e == element)
            index = i;
    });
    $(element).attr('class', 'collection-item active');
    $.get({
        url: '/GetItensDeTipo',
        data: {
            tipo: index
        }
    }, function (itens) {
        itens = JSON.parse(itens);
        var table = '<table><tr>'
        for (var i = 0; i < itens.length; i++) {
            if (i % 6 == 0 && i != 0)
                table += `</tr><tr>`;
            var imgPerfil = $('.imgPerfil').last().css('background').substring(22, $('.imgPerfil').last().css('background').lastIndexOf('"'));
            itens[i].ToTableHtml = itens[i].ToTableHtml.replace("url(imgPerfil)", `url('${imgPerfil}')`)
            table += `<td ${(estaEquipado(itens[i].CodItem) ? 'style="border: 3px solid green;"' : '')} onclick="mostrarItem(${index}, ${i});">${itens[i].ToTableHtml}</td>`;
        }
        table += `</tr></table>`;
        $("#atualLoja").html(table);
        setRainbow();
    })
}
function mostrarItem(tipo, index) {
    $.get({
        url: '/GetItensDeTipo',
        data: {
            tipo: tipo
        }
    }, itens => {
        itens = JSON.parse(itens);
        var imgPerfil = $('.imgPerfil').last().css('background').substring(22, $('.imgPerfil').last().css('background').lastIndexOf('"'));
        document.getElementById('atualLoja').innerHTML = itens[index].ToHtml.replace("url(imgPerfil)", `url('${imgPerfil}')`);
        if (!temItem(itens[index].CodItem))
            document.getElementById('atualLoja').innerHTML += `<center style="width: 95%;"><a class="waves-effect waves-light btn" style="background-color:var(--tema); position: relative; bottom: 1em;" onmouseout="this.innerHTML = 'Comprar';" onmouseover="this.innerHTML = 'G$ ${itens[index].Valor}';" onclick="comprarItem(${itens[index].CodItem})">Comprar</a><center>`;
        else if (!estaEquipado(itens[index].CodItem, itens[index].Conteudo))
            document.getElementById('atualLoja').innerHTML += `<center style="width: 95%;"><a class="waves-effect waves-light btn" style="background-color:var(--tema); position: relative; bottom: 1em;" onclick="equiparItem(${itens[index].CodItem}, ${itens[index].Tipo})">Equipar</a><center>`;
        else
            document.getElementById('atualLoja').innerHTML += `<center style="width: 95%;"><a class="waves-effect waves-light btn" style="background-color:var(--tema); position: relative; bottom: 1em;" onclick="desequiparItem(${itens[index].CodItem}, ${itens[index].Tipo})">Desequipar</a><center>`;
        setRainbow();
    });
}
function temItem(idItem) {
    var achou = false;
    window.usuario.Itens.forEach(function (item) {
        if (item.CodItem == idItem)
            achou = true;
    })
    return achou;
}
function estaEquipado(idItem, cont) {
    var equipado = false;
    if (window.usuario.TemaSite == idItem)
        equipado = true;
    else if (window.usuario.Insignia == idItem)
        equipado = true;
    else if (window.usuario.Decoracao == idItem)
        equipado = true;
    else if (window.usuario.Titulo != '' && (window.usuario.Titulo.split(" ")[0] == idItem || window.usuario.Titulo.split(" ").includes(cont)))
        equipado = true;
    return equipado;
}
function equiparItem(idItem, tipo) {
    $.post({
        url: '/EquiparItem',
        data: { idItem: idItem, tipo: tipo },
        success: function () { window.location.reload(); },
        async: false
    })
}
function desequiparItem(id, tipo) {
    $.post({
        url: '/DesquiparItem',
        data: { idItem: id, tipo: tipo },
        success: function () { window.location.reload(); },
        async: false
    })
}
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
        var infoAntE = null;
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
                setTimeout(function () { cliques = 0 }, 300)
                if (++cliques % 2 == 0 && info.dateStr == infoAntE && info.event.extendedProps.usuariosAdmin.toString().includes(window.usuario.Id)) {
                    editarEvento(info);
                    cliques = 0;
                }
                infoAntE = info.dateStr;
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
        window.calendario = calendar;
        var heightCalendar = - $('#tabs-swipe-demo').height();
        heightCalendar += $('.apenasTelasMaiores').height();
        calendar.setOption('height', heightCalendar);
        for (var i = 0; i < user.Tarefas.length; i++) {
            var tar = user.Tarefas[i];
            window.calendario.addEvent({
                title: tar.Titulo,
                start: tar.Data.substring(6) + '-' + tar.Data.substring(3, 5) + '-' + tar.Data.substring(0, 2),
                descricao: tar.Descricao,
                usuariosAdmin: tar.IdUsuariosAdmin,
                tipo: 0,
                meta: tar.Meta,
                marcados: tar.IdUsuariosMarcados
            });
        }
        for (var i = 0; i < user.Acontecimentos.length; i++) {
            var acont = user.Acontecimentos[i];
            window.calendario.addEvent({
                title: acont.Titulo,
                start: acont.Data.substring(6) + '-' + acont.Data.substring(3, 5) + '-' + acont.Data.substring(0, 2),
                descricao: acont.Descricao,
                usuariosAdmin: acont.IdUsuariosAdmin,
                tipo: 1
            });
        }
    }, 100)
    $('.pesquisarAmigo').attr('style', `top: calc(1000px - 12.5em);`);
    $('#amigos').height(`calc((1000px - 33.5em)`);
    $("#tabAgenda").load('/_Calendario', function () {
        $("#slideEsquerda").height($('#footer').offset().top - $(".nav-wrapper").height() - 1);
        $("#tarefas").height($("#slideEsquerda").height());
        if (triggerEsquerda == 1)
            $("#triggerEsquerda").css('left', ($("#slideEsquerda").width() - 165) + "px")
        else
            $("#triggerEsquerda").css('left', "-165px")
        if ($(window).width() < 992)
            fecharEsquerda();
    })
    configurarPostar();
    $.get({
        url: '/GetItem',
        data: {
            id: user.Decoracao
        },
        success: function (deco) {
            if (deco != '') {
                deco = JSON.parse(deco)
                //
            }
        },
        async: false
    });
    $.get({
        url: '/GetItem',
        data: {
            id: user.Insignia
        },
        success: function (insig) {
            if (insig != '') {
                insig = JSON.parse(insig)
                //
            }
        }
    })
    if (user.Titulo != '') {
        $.get({
            url: '/GetItem',
            data: {
                id: parseInt(user.Titulo.split(" ")[0])
            },
            success: function (titu) {
                if (titu != '') {
                    titu = JSON.parse(titu)
                    $("#spanTituloUsuario").html(titu.Conteudo);
                    var conteudos = user.Titulo.split(' ');

                    if (conteudos.indexOf("R") > -1)
                        $("#spanTituloUsuario").attr('class', 'rainbow');
                    if (conteudos.indexOf("B") > -1)
                        $("#spanTituloUsuario").css('font-weight', 'bold');
                    setRainbow();
                }
            },
            async: false
        })
    }
    setTimeout(function () { $(".conteudoLoja div").children().first().click(); $("#triggerEsquerda").click(); acionarTarefas(); }, 100)
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
function configurarCalendario() {
    if ($("#agenda").width() < 450) {
        this.calendario.setOption('header', { left: '' })
    }
    else {
        this.calendario.setOption('header', { left: 'prevYear, prev, today, next, nextYear' });
    }
}

function comprarItem(id) {
    $.get({
        url: '/GetItem',
        data: { id: id },
        success: function (item) {
            item = JSON.parse(item)
            if (item.Valor > window.usuario.Dinheiro)
                alert('Voc� n�o tem dinheiro o suficiente. Estude mais!')
            else
                $.post({
                    url: '/ComprarItem',
                    data: { idItem: id, tipo: item.Tipo },
                    success: function () {
                        alert(`${item.Nome} comprado com sucesso!`)
                        $("body").remove();
                        window.location.reload();
                    },
                    async: false
                })
        },
        async: false
    })
}
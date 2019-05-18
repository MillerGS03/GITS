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
            $(".apenasTelasMaiores").attr('style', 'transition: width 1s, left 1s; width: 100%; left: 0;')
        }
    }
    else {
        $("aside").css("display", "initial");
        $("aside").css("width", "360px");
        $("#btnAcionarTarefas").css("right", "370px");
        $('.carousel.carousel-slider').css("width", "calc(100% - 360px)");
        if ($(window).width() > 992) {
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
    setCarousel();
    dispararResize();
    $("#btnAcionarTarefas").tooltip();
    $("#triggerEsquerda").tooltip();
    $("#listaTarefas").collapsible();
    $('.tabs').tabs();
    $(".collapsible").collapsible();
    $('.modal').modal();
    $('.sidenav').sidenav();
    setNoUiSlider();
    tratar(window.usuario);
    var construirCalendario = setInterval(function () {
        if ($('#agenda').length) {
            setCalendario();
            clearInterval(construirCalendario);
        }
    }, 50);
});
function salvarProgresso(idMeta) {
    $.post({
        url: "/AtualizarProgressoMeta",
        data: {
            idMeta: idMeta,
            progresso: document.getElementById("progresso" + idMeta).noUiSlider.get()
        },
        success: function () {
            document.getElementById("salvarProgresso" + idMeta).setAttribute("style", "display: none;");
        }
    })
}
function adicionarMeta() {
    var data = M.Datepicker.getInstance(document.getElementById("dataMeta")).date;
    $.post({
        url: "/AdicionarMeta",
        data: {
            titulo: $("#txtTituloMeta").val().trim(),
            descricao: $("#txtDescricaoMeta").val().trim(),
            recompensa: $("#txtRecompensa").val(),
            dataTermino: data == null ? "" : data.toLocaleDateString()
        }
    })
}
function verificarCamposMeta() {
    var erro = $("#txtTituloMeta").val().trim() == "" || $("#txtDescricaoMeta").val().trim() == "" || $("#txtRecompensa").val().trim() == "";

    if (erro && !$("#addMeta").hasClass("disabled"))
        $("#addMeta").addClass("disabled");
    else if (!erro && $("#addMeta").hasClass("disabled"))
        $("#addMeta").removeClass("disabled");

    return erro;
}

var metas = new Array();
function modalEvento(info, evento, adm) {
    $("#adicionarEvento").modal('open');
    var dataEvento = '';
    if (evento) {
        $('#addEvento').html('salvar');
        dataEvento = new Date(evento.start)
        dataEvento = dataEvento.getFullYear() + '-' + (dataEvento.getMonth() + 1).toString().padStart(2, '0') + '-' + dataEvento.getDate().toString().padStart(2, '0')
    }
    else if (info && info.date >= new Date()) {
        dataEvento = info.dateStr;
        $('#addEvento').html('adicionar');
    }
    $("#dataEvento").val(dataEvento);
    $("#dataEvento").change(function () {
        verificarCamposTarefa();
    })
    $("#txtTitulo").change(function () {
        verificarCamposTarefa();
    })
    $("#txtTitulo").focusout(function () {
        verificarCamposTarefa();
    })
    $("#txtDescricao").change(function () {
        verificarCamposTarefa();
    })
    $("#txtDescricao").focusout(function () {
        verificarCamposTarefa();
    })
    $("#txtMeta").change(function () {
        verificarCamposTarefa();
    })
    $("#txtMeta").focusout(function () {
        verificarCamposTarefa();
    })

    $('#txtTitulo').characterCounter();
    if (!adm) {
        $('#txtTitulo').attr('readonly', "readonly")
        $('#dataEvento').attr('readonly', "readonly")
        $('#txtDescricao').attr('readonly', "readonly")
        $('#conviteAmigos input').attr('readonly', "readonly")
        $("#removerEvento").css('display', 'none');
        $("#reqAdmEvento").css('display', 'block');
    }
    else {
        $('#txtTitulo').removeAttr("readonly")
        $('#dataEvento').removeAttr("readonly")
        $('#txtDescricao').removeAttr("readonly")
        $('#conviteAmigos input').removeAttr("readonly")
        if (evento)
            $("#removerEvento").css('display', 'block');
        else
            $("#removerEvento").css('display', 'none');
        $("#reqAdmEvento").css('display', 'none');
    }
    if ($("#txtTitulo").val() == null || $("#txtTitulo").val().trim() == "" || $("#txtTitulo").val().trim().length > 65)
        $("#addEvento").addClass('disabled');
    if (evento) {
        $("#txtTitulo").val(evento.title);
        $("#txtDescricao").val(evento.extendedProps.descricao);
        document.getElementById('dificuldadeTarefa').noUiSlider.set(evento.extendedProps.dificuldade);
        if (evento.extendedProps.meta != null)
            $("#txtMeta").val(evento.extendedProps.meta.Titulo);
        if (evento.extendedProps.meta != null && evento.extendedProps.meta != '') {
            $("#selectMeta").css('display', 'block');
            $("#chkMeta").prop('checked', true);
        }
        else {
            $("#selectMeta").css('display', 'none');
            $("#chkMeta").prop('checked', false);
        }
    }
    else {
        $("#txtTitulo").val("");
        $("#txtDescricao").val("");
        $("#txtMeta").val("");
        $("#conviteAmigos input").val("");
    }
    $("#chkMeta").change(function () {
        if (this.checked) {
            $("#selectMeta").css('display', 'block');
        }
        else {
            $("#selectMeta").css('display', 'none');
        }
        verificarCamposTarefa();
    });
    if (evento && evento.extendedProps.tipo == 0) {
        $('#divTarefas').css('display', 'block');
        $('#divAcontecimentos').css('display', 'none');
    }
    else if (evento) {
        $('#divTarefas').css('display', 'none');
        $('#divAcontecimentos').css('display', 'block');
    }
    else {
        $('#divTarefas').css('display', 'none');
        $('#divAcontecimentos').css('display', 'none');
        $("input[type='radio'][name='radioTipoEvento']").prop('checked', false);
        $('#continuacaoAdicaoEvento').css('display', 'none');
        $("#conviteAmigos input").val('');
    }
    $('input:radio[name="radioTipoEvento"]').change(function () {
        $('#continuacaoAdicaoEvento').css('display', 'block');
        if ($(this).is(':checked') && $(this).val() == 'tarefa') {
            $('#divAcontecimentos').css('display', 'none');
            $('#divTarefas').css('display', 'block');
            $("#addEvento").unbind().click(function () {
                if (evento)
                    trabalharTarefa(evento.extendedProps.cod, adm);
                else
                    trabalharTarefa(null, adm);
            })
            $("#removerEvento").unbind().click(function () {
                if (evento)
                    removerTarefa(evento.extendedProps.cod, adm);
            })
            $("#reqAdmEvento").unbind().click(function () {
                if (evento && !adm) {
                    $.post({
                        url: '/RequisitarAdminTarefa',
                        data: { codTarefa: evento.extendedProps.cod, idUsuario: window.usuario.Id },
                        async: false
                    })
                }
            })
        }
        else {
            $('#divAcontecimentos').css('display', 'block');
            $('#divTarefas').css('display', 'none');
            $("#addEvento").unbind().click(function () {
                if (evento)
                    trabalharAcontecimento(evento.extendedProps.cod, adm);
                else
                    trabalharAcontecimento(null, adm);
            })
            $("#removerEvento").unbind().click(function () {
                if (evento)
                    removerAcontecimento(evento.extendedProps.cod, adm);
            })
            $("#reqAdmEvento").unbind().click(function () {
                if (evento && !adm) {
                    $.post({
                        url: '/RequisitarAdminAcontecimento',
                        data: { codAcontecimento: evento.extendedProps.cod, idUsuario: window.usuario.Id },
                        async: false
                    })
                }
            })
        }
    });
    metas = new Array();
    window.usuario.Metas.forEach(function (m) { metas.push(m.Titulo) })
    var obj = new Object();
    for (var i = 0; i < window.usuario.Metas.length; i++)
        obj[`${window.usuario.Metas[i].Titulo}`] = null;
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
        }
    });
    $("#conviteAmigos input").change(verificarCamposTarefa)
    $("#conviteAmigos input").focusout(verificarCamposTarefa)
    $("#conviteAmigos input").on('focus', function () { $("#conviteAmigos").parent().children().css('color', 'black'); })
    $("#conviteAmigos input").attr('style', 'width: 100% !important;')
    if (evento && evento.extendedProps.tipo == 0) {
        $('#radioTarefa').click();
        $("#addEvento").unbind().click(function () {
            if (evento)
                trabalharTarefa(evento.extendedProps.cod, adm);
            else
                trabalharTarefa(null, adm);
        })
        $("#removerEvento").unbind().click(function () {
            if (evento)
                removerTarefa(evento.extendedProps.cod, adm);
        })
        $("#reqAdmEvento").unbind().click(function () {
            if (evento && !adm) {
                $.post({
                    url: '/RequisitarAdminTarefa',
                    data: { codTarefa: evento.extendedProps.cod, idUsuario: window.usuario.Id },
                    async: false
                })
            }
        })
    }
    else if (evento) {
        $('#radioAcontecimento').click();
        $("#addEvento").unbind().click(function () {
            if (evento)
                trabalharAcontecimento(evento.extendedProps.cod, adm);
            else
                trabalharAcontecimento(null, adm);
        })
        $("#removerEvento").unbind().click(function () {
            if (evento)
                removerAcontecimento(evento.extendedProps.cod, adm);
        })
        $("#reqAdmEvento").unbind().click(function () {
            if (evento && !adm) {
                $.post({
                    url: '/RequisitarAdminAcontecimento',
                    data: { codAcontecimento: evento.extendedProps.cod, idUsuario: window.usuario.Id },
                    async: false
                })
            }
        })
    }
    if (evento) {
        $.post({
            url: '/GetUsuarios',
            data: { ids: evento.extendedProps.marcados },
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
}
function trabalharTarefa(id = 0, adm) {
    if (!verificarCamposTarefa()) {
        var data = new Date();
        data = `${data.getDate()}/${data.getMonth()}/${data.getFullYear()}`;
        var objEvento = {
            CodTarefa: id,
            Titulo: $("#txtTitulo").val(),
            Descricao: $("#txtDescricao").val() == null ? "" : $("#txtDescricao").val(),
            Dificuldade: document.getElementById('dificuldadeTarefa').noUiSlider.get(),
            Urgencia: calcUrgencia(data, $("#dataEvento").val()),
            Data: $("#dataEvento").val(),
            IdUsuariosAdmin: new Array(),
            Recompensa: calcRecompensa(document.getElementById('dificuldadeTarefa').noUiSlider.get()),
            Criacao: data
        };
        var con = M.Chips.getInstance(document.getElementById('conviteAmigos')).chipsData;
        var convites = new Array();
        con.forEach(function (c) {
            convites.push(c.id);
        });
        convites.push(window.usuario.Id);
        objEvento.IdUsuariosMarcados = convites;
        objEvento.IdUsuariosAdmin.push(window.usuario.Id);
        $.post({
            url: '/TrabalharTarefa',
            data: {
                evento: objEvento,
                nomeMeta: document.getElementById('chkMeta').checked ? $("#txtMeta").val() : null,
                adm: adm
            },
            success: function (e) {
                e = JSON.parse(e)
                var tem = false;
                for (var i = 0; i < window.usuario.Tarefas.length; i++)
                    if (window.usuario.Tarefas[i].CodTarefa == e.CodTarefa) {
                        tem = true;
                        window.usuario.Tarefas[i] = e;
                    }

                if (!tem) {
                    window.usuario.Tarefas.push(e);
                    window.calendario.addEvent({
                        cod: e.CodTarefa,
                        title: e.Titulo,
                        start: e.Data,
                        descricao: e.Descricao,
                        usuariosAdmin: e.IdUsuariosAdmin,
                        tipo: 0,
                        marcados: e.IdUsuariosMarcados,
                        dificuldade: e.Dificuldade
                    });
                }
                else {
                    for (var i = 0; i < window.calendario.getEvents().length; i++)
                        if (window.calendario.getEvents()[i].extendedProps.cod == e.CodTarefa && window.calendario.getEvents()[i].extendedProps.tipo == 0) {
                            window.calendario.getEvents()[i].setProp('title', e.Titulo);
                            dataEvento = new Date(e.Data)
                            dataEvento = dataEvento.getFullYear() + '-' + (dataEvento.getMonth() + 1).toString().padStart(2, '0') + '-' + dataEvento.getDate().toString().padStart(2, '0')
                            window.calendario.getEvents()[i].setStart(dataEvento);
                            window.calendario.getEvents()[i].setExtendedProp('descricao', e.Descricao);
                            window.calendario.getEvents()[i].setExtendedProp('usuariosAdmin', e.IdUsuariosAdmin);
                            window.calendario.getEvents()[i].setExtendedProp('dificuldade', e.Dificuldade);
                            window.calendario.getEvents()[i].setExtendedProp('marcados', e.IdUsuariosMarcados);
                            window.calendario.getEvents()[i].setExtendedProp('meta', e.Meta == null || e.Meta.Id == 0 ? null : e.Meta);
                        }
                }
            },
            async: false
        })
    }
}
function trabalharAcontecimento(id = 0, adm) {
    if (!verificarCamposAcontecimento()) {
        var objEvento = {
            CodAcontecimento: id == null || id == 0 ? 0 : id,
            Titulo: $("#txtTitulo").val(),
            Descricao: $("#txtDescricao").val() == null ? "" : $("#txtDescricao").val(),
            Data: $("#dataEvento").val(),
            IdUsuariosAdmin: new Array(),
            Tipo: 0
        };
        var con = M.Chips.getInstance(document.getElementById('conviteAmigos')).chipsData;
        var convites = new Array();
        con.forEach(function (c) {
            convites.push(c.id);
        });
        convites.push(window.usuario.Id);
        objEvento.IdUsuariosMarcados = convites;
        objEvento.IdUsuariosAdmin.push(window.usuario.Id);
        $.post({
            url: '/TrabalharAcontecimento',
            data: {
                a: objEvento,
                adm: adm
            },
            success: function (e) {
                e = JSON.parse(e)
                var tem = false;
                for (var i = 0; i < window.usuario.Acontecimentos.length; i++)
                    if (window.usuario.Acontecimentos[i].CodAcontecimento == e.CodAcontecimento) {
                        tem = true;
                        window.usuario.Acontecimentos[i] = e;
                    }

                if (!tem) {
                    window.usuario.Acontecimentos.push(e);
                    var objAdd = {
                        cod: e.CodAcontecimento,
                        title: e.Titulo,
                        start: e.Data,
                        descricao: e.Descricao,
                        usuariosAdmin: e.IdUsuariosAdmin,
                        tipo: 1,
                        marcados: e.IdUsuariosMarcados
                    }
                    window.calendario.addEvent(objAdd);
                }
                else {
                    for (var i = 0; i < window.calendario.getEvents().length; i++)
                        if (window.calendario.getEvents()[i].extendedProps.cod == e.CodAcontecimento && window.calendario.getEvents()[i].extendedProps.tipo == 1) {
                            window.calendario.getEvents()[i].setProp('title', e.Titulo);
                            dataEvento = new Date(e.Data)
                            dataEvento = dataEvento.getFullYear() + '-' + (dataEvento.getMonth() + 1).toString().padStart(2, '0') + '-' + dataEvento.getDate().toString().padStart(2, '0')
                            window.calendario.getEvents()[i].setStart(dataEvento);
                            window.calendario.getEvents()[i].setExtendedProp('descricao', e.Descricao);
                            window.calendario.getEvents()[i].setExtendedProp('usuariosAdmin', e.IdUsuariosAdmin);
                            window.calendario.getEvents()[i].setExtendedProp('marcados', e.IdUsuariosMarcados);
                            break;
                        }
                }
            },
            async: false
        })
    }
}
function removerTarefa(id = 0, adm) {
    $.post({
        url: '/RemoverTarefa',
        data: {
            id: id,
            adm: adm
        },
        success: function () {
            for (var i = 0; i < window.usuario.Tarefas; i++) {
                if (window.usuario.Tarefas[i].CodTarefa == id) {
                    window.usuario.Tarefas.splice(i, 1);
                    break;
                }
            }
            for (var i = 0; i < window.calendario.getEvents().length; i++)
                if (window.calendario.getEvents()[i].extendedProps.cod == id && window.calendario.getEvents()[i].extendedProps.tipo == 0)
                    window.calendario.getEvents()[i].remove();
        },
        async: false
    })
}
function removerAcontecimento(id = 0, adm) {
    $.post({
        url: '/RemoverAcontecimento',
        data: {
            id: id,
            adm: adm
        },
        success: function () {
            for (var i = 0; i < window.usuario.Acontecimentos; i++) {
                if (window.usuario.Acontecimentos[i].CodAcontecimento == id) {
                    window.usuario.Acontecimentos.splice(i, 1);
                    break;
                }
            }
            for (var i = 0; i < window.calendario.getEvents().length; i++)
                if (window.calendario.getEvents()[i].extendedProps.cod == id && window.calendario.getEvents()[i].extendedProps.tipo == 1)
                    window.calendario.getEvents()[i].remove();
        },
        async: false
    })
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
    var idsAmigos = new Array();
    var chips = M.Chips.getInstance(document.getElementById('conviteAmigos'));
    chips.chipsData.forEach(function (c) {
        data.push(c.id);
    })
    window.usuario.Amigos.forEach(a => idsAmigos.push(a.Id))
    var naoTem = false;
    data.forEach(d => {
        if (!idsAmigos.includes(d))
            naoTem = true;
    })
    if (data.length > 0 && naoTem) {
        erro = true;
        $("#conviteAmigos input").attr('class', 'input invalid');
        $("#conviteAmigos").parent().children().css('color', '#F44336')
    }
    if (!erro) {
        $("#conviteAmigos").parent().children().css('color', 'black')
        $("#addEvento").removeClass('disabled')
    }
    else
        $("#addEvento").addClass('disabled');
    return erro;
}
function verificarCamposAcontecimento() {
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
    var data = new Array();
    var idsAmigos = new Array();
    var chips = M.Chips.getInstance(document.getElementById('conviteAmigos'));
    chips.chipsData.forEach(function (c) {
        data.push(c.id);
    })
    window.usuario.Amigos.forEach(a => idsAmigos.push(a.Id))
    var naoTem = false;
    data.forEach(d => {
        if (!idsAmigos.includes(d))
            naoTem = true;
    })
    if (data.length > 0 && naoTem) {
        erro = true;
        $("#conviteAmigos input").attr('class', 'input invalid');
        $("#conviteAmigos").parent().children().css('color', '#F44336')
    }
    if (!erro) {
        $("#conviteAmigos").parent().children().css('color', 'black')
        $("#addEvento").removeClass('disabled')
    }
    else
        $("#addEvento").addClass('disabled');
    return erro;
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
    $('.datepicker').datepicker({
        format: 'dd/mm/yyyy',
        onstart: () => { $('.datepicker').appendTo('body'); },
        i18n: {
            months: ['Janeiro', 'Fevereiro', 'Mar�o', 'Abril', 'Maio', 'Junho', 'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'],
            monthsShort: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'],
            weekdays: ['Domingo', 'Segunda', 'Ter�a', 'Quarta', 'Quinta', 'Sexta', 'Sab�do'],
            weekdaysShort: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sab'],
            weekdaysAbbrev: ['D', 'S', 'T', 'Q', 'Q', 'S', 'S'],
            today: 'Hoje',
            clear: 'Limpar',
            close: 'Pronto',
            labelMonthNext: 'Pr�ximo m�s',
            labelMonthPrev: 'M�s anterior',
            labelMonthSelect: 'Selecione um m�s',
            labelYearSelect: 'Selecione um ano',
            selectMonths: true,
            selectYears: 15,
            cancel: 'Cancelar',
            clear: 'Limpar'
        }
    });
    $("#txtTituloMeta").on("input", verificarCamposMeta);
    $("#txtDescricaoMeta").on("change keyup paste", verificarCamposMeta);
    $("#txtRecompensa").on("input", verificarCamposMeta);

    //setTimeout(function () {
        if (user.Amigos.length == 0) {
            $(".txtAmigos").attr('style', 'display: none;')
            $("#amigos").attr('style', 'display: none;')
            $(".pesquisarAmigo").attr('style', 'display: none;')
        }
    //}, 50)
    $('.pesquisarAmigo').attr('style', `top: calc(1000px - 12.5em);`);
    $('#amigos').height(`calc((1000px - 33.5em)`);
    $("#tabAgenda").load('/_Calendario', function () {
        $("#slideEsquerda").height($('#footer').offset().top - $(".nav-wrapper").height() - 1);
        $("#tarefas").height($("#slideEsquerda").height());
    })
    configurarPostar();
    $.get({
        url: '/GetItens',
        success: function (itens) {
            if (itens != '') {
                itens = JSON.parse(itens)
                var deco = itens.find(item => item.CodItem == user.Decoracao);
                var insig = itens.find(item => item.CodItem == user.Insignia);
                var titu = itens.find(item => item.CodItem == parseInt(user.Titulo.split(" ")[0]));

                if (titu != '') {
                    $("#spanTituloUsuario").html(titu.Conteudo);
                    var conteudos = user.Titulo.split(' ');

                    if (conteudos.indexOf("R") > -1)
                        $("#spanTituloUsuario").attr('class', 'rainbow');
                    if (conteudos.indexOf("B") > -1)
                        $("#spanTituloUsuario").css('font-weight', 'bold');
                    setRainbow();
                }
            }
        },
        async: false
    });
    /*
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
    }*/
    setTimeout(function () { $(".conteudoLoja div").children().first().click();}, 100)
}
function setCalendario() {
    var cliques = 0;
    var infoAnt = null;
    var infoAntE = null;
    var calendarEl = null;
    while (calendarEl == null) calendarEl = document.getElementById('agenda');
    var calendar = new FullCalendar.Calendar(calendarEl, {
        plugins: ['dayGrid', 'timeGrid', 'list', 'interaction', 'moment', 'luxon'],
        dateClick: function (info) {
            setTimeout(function () { cliques = 0 }, 300)
            if (++cliques % 2 == 0 && info.dateStr == infoAnt) {
                modalEvento(info, null, true);
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
            if (++cliques % 2 == 0 && info.dateStr == infoAntE) {
                var adm = info.event.extendedProps.usuariosAdmin.includes(window.usuario.Id)
                modalEvento(null, info.event, adm);
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
        eventRender: function (info) {
            var tooltip = new Tooltip(info.el, {
                title: info.event.extendedProps.descricao,
                placement: 'top',
                trigger: 'hover',
                container: 'body'
            });
        },
        windowResize: false
    });

    /*var renderizar = function () {
        try {
            calendar.render();
        }
        catch { setTimeout(renderizar, 250); }
    }
    renderizar();*/

    window.calendario = calendar;
    var heightCalendar = - $('#tabs-swipe-demo').height();
    heightCalendar += $('.apenasTelasMaiores').height();
    calendar.setOption('height', heightCalendar);
    for (var i = 0; i < window.usuario.Tarefas.length; i++) {
        var tar = window.usuario.Tarefas[i];
        var objAdd = {
            cod: tar.CodTarefa,
            title: tar.Titulo,
            start: tar.Data.substring(6) + '-' + tar.Data.substring(3, 5) + '-' + tar.Data.substring(0, 2),
            descricao: tar.Descricao,
            usuariosAdmin: tar.IdUsuariosAdmin,
            tipo: 0,
            meta: tar.Meta.CodMeta == 0 ? null : tar.Meta,
            marcados: tar.IdUsuariosMarcados,
            dificuldade: tar.Dificuldade
        };
        window.calendario.addEvent(objAdd);
    }
    for (var i = 0; i < window.usuario.Acontecimentos.length; i++) {
        var acont = window.usuario.Acontecimentos[i];
        var objAdd = {
            cod: acont.CodAcontecimento,
            title: acont.Titulo,
            start: acont.Data.substring(6, 10) + '-' + acont.Data.substring(3, 5) + '-' + acont.Data.substring(0, 2),
            descricao: acont.Descricao,
            usuariosAdmin: acont.IdUsuariosAdmin,
            tipo: 1,
            marcados: acont.IdUsuariosMarcados
        }
        window.calendario.addEvent(objAdd);
    }
    window.calendario.render();
}
function configurarCalendario() {
    if ($("#agenda").width() < 450) {
        this.calendario.setOption('header', { left: '' })
    }
    else {
        this.calendario.setOption('header', { left: 'prevYear, prev, today, next, nextYear' });
    }
}
function setCarousel() {
    var tarefasImportantes = window.usuario.Tarefas.sort((a, b) => {
        return a.Urgencia = b.Urgencia
    })
    if (tarefasImportantes.length >= 4)
        tarefasImportantes.slice(0, 4)
    var cores = ['red', 'amber', 'green', 'blue', 'purple', 'orange'];
    var acontecimentosProximos = window.usuario.Acontecimentos.sort((a, b) => { return a.Data - b.Data; }).slice(0, 2)
    for (var i = 0; i < tarefasImportantes.length; i++) {
        var atual = document.createElement('div')
        atual.classList.add('carousel-item');
        atual.classList.add(cores[i]);
        atual.classList.add('white-text');
        atual.innerHTML = `<h2>${tarefasImportantes[i].Titulo}</h2><p class="white-text">${tarefasImportantes[i].Descricao}<br><br>Dificuldade: ${tarefasImportantes[i].Dificuldade}/10<br>Recompensa: <div class="gitcoin" style="filter: brightness(.6)"></div> ${tarefasImportantes[i].Recompensa}<br>Urg&ecirc;ncia: ${tarefasImportantes[i].Urgencia.toFixed(2)}/10<br>Prazo: ${tarefasImportantes[i].Data}</p>`;
        atual.codTarefa = tarefasImportantes[i].CodTarefa;
        $('#carouselImportante').append(atual);
    }
    for (var i = 0; i < acontecimentosProximos.length; i++) {
        $('#carouselImportante').append(`<div class="carousel-item ${cores[i + tarefasImportantes.length]} white-text"><h2>${acontecimentosProximos[i].Titulo}</h2><p class="white-text">${acontecimentosProximos[i].Descricao}<br><br>Data: ${acontecimentosProximos[i].Data}</p></div>`);
    }
    $('.carousel.carousel-slider').carousel({
        fullWidth: true,
        indicators: true,
        onCycleTo: function (e) {
            $('.carousel-fixed-item a').attr('href', `/tarefas/${e.codTarefa}`);
        }
    });
}
function setNoUiSlider() {
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
    var elementos = document.getElementsByClassName("progresso");
    for (var i = 0; i < elementos.length; i++) {
        noUiSlider.create(elementos[i], {
            start: parseInt(elementos[i].classList[1]),
            connect: [true, false],
            step: 1,
            range: {
                'min': 0,
                'max': 100
            },
            format: wNumb({
                decimals: 0
            }),
            id: elementos[i].id.substring(9)
        })
        elementos[i].noUiSlider.on("change", function () {
            document.getElementById("salvarProgresso" + this.options.id).removeAttribute("style");
        })
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
//ultima linha
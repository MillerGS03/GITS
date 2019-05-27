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
        //$('.carousel.carousel-slider').css("transition", "unset");
        if ($("aside").width() == 0)
            $("aside").css("display", "none");
        //$('.tabs').tabs();
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
        if ($("#tabAgenda").css("display") != "none") {
            var heightCalendar = - $('#tabs-swipe-demo').height();
            heightCalendar += $('#tabAgenda').height();
            this.calendario.setOption('height', $(".conteudo").height() - $("#tabs-swipe-demo").height());
            configurarCalendario();
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
function comecarAdicaoMeta() {
    $("#tituloAdicionarOuEditar").html("Adicionar uma meta");
    $("#addEditMeta").html("Adicionar");

    $("#txtTituloMeta").val("");
    $("#txtDescricaoMeta").val("");
    $("#txtRecompensa").val("");
    $("#dataMeta").val("");
    $("#dataMeta").datepicker("setDate", null);

    $("input").removeClass("valid");
    $("textarea").removeClass("valid");

    verificarCamposMeta();

    codMetaEditando = -1;
}

var codMetaEditando = -1;
function comecarEdicaoMeta(meta) {
    $("#tituloAdicionarOuEditar").html("Editar uma meta");
    $("#addEditMeta").html("Salvar");

    $("#txtTituloMeta").val(meta.Titulo);
    $("#txtDescricaoMeta").val(meta.Descricao);
    $("#txtRecompensa").val(meta.Recompensa);
    $("#dataMeta").val(meta.Data);

    if (meta.Data != "" && meta.Data != null) {
        var data = new Date(parseInt(meta.Data.substring(6)), parseInt(meta.Data.substr(3, 2)) - 1, parseInt(meta.Data.substr(0, 2)));
        $("#dataMeta").datepicker("setDate", data);
    }
    else {
        $("#dataMeta").val("");
        $("#dataMeta").datepicker("setDate", null);
    }

    $("input").removeClass("invalid");
    $("textarea").removeClass("invalid");

    verificarCamposMeta();
    updateLabels();

    codMetaEditando = meta.CodMeta;
}
function adicionarMeta() {
    var dataTermino = M.Datepicker.getInstance(document.getElementById("dataMeta")).date;
    var data = {
        titulo: $("#txtTituloMeta").val().trim(),
        descricao: $("#txtDescricaoMeta").val().trim(),
        recompensa: $("#txtRecompensa").val(),
        dataTermino: dataTermino == null ? "" : dataTermino.toLocaleDateString()
    };

    if (codMetaEditando == - 1)
        $.post({
            url: "/AdicionarMeta",
            data: data,
            success: function () { window.location.reload() }
        })
    else {
        data.idMeta = codMetaEditando;
        $.post({
            url: "/EditarMeta",
            data: data,
            success: function () { window.location.reload() }
        })
    }
}

var metaARemover = -1;
function removerMeta() {
    $.post({
        url: "/RemoverMeta",
        data: {
            idMeta: metaARemover
        },
        success: function () { window.location.reload() }
    })
}
function verificarCamposMeta() {
    var erro = $("#txtTituloMeta").val().trim() == "" || $("#txtDescricaoMeta").val().trim() == "" || $("#txtRecompensa").val().trim() == "";

    if (erro && !$("#addEditMeta").hasClass("disabled"))
        $("#addEditMeta").addClass("disabled");
    else if (!erro && $("#addEditMeta").hasClass("disabled"))
        $("#addEditMeta").removeClass("disabled");

    return erro;
}

function updateLabels() {
    var input_selector = 'input[type=text], input[type=password], input[type=email], input[type=url], input[type=tel], input[type=number], input[type=search], textarea';
    $(input_selector).each(function (index, element) {
        if ($(element).val().length > 0) {
            $(this).siblings('label, i').addClass('active');
        }
    });
}

var metas = new Array();
function modalEvento(info, evento, adm) {
    $("#adicionarEvento").modal('open');
    var dataEvento = '';
    if (evento) {
        $('#addEvento').html('salvar');
        dataEvento = new Date(evento.start)
        dataEvento = dataEvento.getDate().toString().padStart(2, '0') + "/" + (dataEvento.getMonth() + 1).toString().padStart(2, '0') + '/' + dataEvento.getFullYear()
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

    updateLabels();
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
            Criacao: data,
            XP: document.getElementById('dificuldadeTarefa').noUiSlider.get() * 10
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
                        dificuldade: e.Dificuldade,
                        xp: e.XP
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
                            window.calendario.getEvents()[i].setExtendedProp('xp', e.XP);
                        }
                }
                setTarefas();
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
    modalConfirmacao("Deseja realmente remover essa tarefa?", "Esta tarefa n&atilde;o aparecer&aacute; novamente para voc&ecirc; e voc&ecirc; dever&aacute; ser convidado novamente para fazer parte dela para retom&aacute;-la.", () => {
        $.post({
            url: '/RemoverTarefa',
            data: {
                id: id,
                adm: adm
            },
            success: function () {
                for (var i = 0; i < window.usuario.Tarefas.length; i++) {
                    if (window.usuario.Tarefas[i].CodTarefa == id) {
                        window.usuario.Tarefas.splice(i, 1);
                        break;
                    }
                }
                for (var i = 0; i < window.calendario.getEvents().length; i++)
                    if (window.calendario.getEvents()[i].extendedProps.cod == id && window.calendario.getEvents()[i].extendedProps.tipo == 0)
                        window.calendario.getEvents()[i].remove();
                setTarefas();
            },
            async: false
        })
    }, () => { })
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
            if (itens[i].LevelMinimo <= getStatusXP(window.usuario.XP)[1]) {
                if (i % 6 == 0 && i != 0)
                    table += `</tr><tr>`;
                var imgPerfil = $('.imgPerfil').last().css('background').substring(22, $('.imgPerfil').last().css('background').lastIndexOf('"'));
                itens[i].ToTableHtml = itens[i].ToTableHtml.replace("url(imgPerfil)", `url('${imgPerfil}')`)
                table += `<td ${(estaEquipado(itens[i].CodItem) ? 'style="border: 3px solid green;"' : '')} onclick="mostrarItem(${itens[i].CodItem});">${itens[i].ToTableHtml}</td>`;
            }
        }
        table += `</tr></table>`;
        $("#atualLoja").html(table);
        setRainbow();
    })
}
function mostrarItem(id) {
    $.get({
        url: '/GetItem',
        data: {
            id: id
        }
    }, item => {
        item = JSON.parse(item);
        var imgPerfil = $('.imgPerfil').last().css('background').substring(22, $('.imgPerfil').last().css('background').lastIndexOf('"'));
        document.getElementById('atualLoja').innerHTML = item.ToHtml.replace("url(imgPerfil)", `url('${imgPerfil}')`);
        if (!temItem(item.CodItem))
            document.getElementById('atualLoja').innerHTML += `<center style="width: 95%;"><a class="waves-effect waves-light btn" style="background-color:var(--tema); position: relative; bottom: 1em;" onmouseout="this.innerHTML = 'Comprar';" onmouseover="this.innerHTML = 'G$ ${item.Valor}';" onclick="comprarItem(${item.CodItem}, ${item.Valor})">Comprar</a><center>`;
        else if (!estaEquipado(item.CodItem, item.Conteudo))
            document.getElementById('atualLoja').innerHTML += `<center style="width: 95%;"><a class="waves-effect waves-light btn" style="background-color:var(--tema); position: relative; bottom: 1em;" onclick="mudarItemAtual(${item.CodItem}, ${item.Tipo}, true)">Equipar</a><center>`;
        else
            document.getElementById('atualLoja').innerHTML += `<center style="width: 95%;"><a class="waves-effect waves-light btn" style="background-color:var(--tema); position: relative; bottom: 1em;" onclick="mudarItemAtual(${item.CodItem}, ${item.Tipo}, false)">Desequipar</a><center>`;
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
function mudarItemAtual(idItem, tipo, equipar) {
    $.post({
        url: equipar ? '/EquiparItem' : '/DesquiparItem',
        data: { idItem: idItem, tipo: tipo },
        success: function () {
            var el;
            switch (tipo) {
                case 0:
                    $.get({
                        url: '/GetTitulo',
                        data: { idUsuario: window.usuario.Id },
                        success: function (ret) {
                            el = document.getElementById('tabTitulos');
                            window.usuario.Titulo = JSON.parse(ret);
                        },
                        async: false
                    });
                    break;
                case 1:
                    $.get({
                        url: '/GetDecoracao',
                        data: { idUsuario: window.usuario.Id },
                        success: function (ret) {
                            el = document.getElementById('tabDecoracoes');
                            window.usuario.Decoracao = JSON.parse(ret);
                        },
                        async: false
                    });
                    break;
                case 2:
                    $.get({
                        url: '/GetInsignia',
                        data: { idUsuario: window.usuario.Id },
                        success: function (ret) {
                            el = document.getElementById('tabInsignias');
                            window.usuario.Insignia = JSON.parse(ret);
                        },
                        async: false
                    });
                    break;
                case 3:
                    $.get({
                        url: '/GetTema',
                        data: { idUsuario: window.usuario.Id },
                        success: function (ret) {
                            el = document.getElementById('tabTemas');
                            window.usuario.TemaSite = JSON.parse(ret);
                        },
                        async: false
                    });
                    break;
            }
            setItens();
            mudarTableLoja(el);
        },
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
            months: ['Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho', 'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'],
            monthsShort: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'],
            weekdays: ['Domingo', 'Segunda', 'Terça', 'Quarta', 'Quinta', 'Sexta', 'Sabádo'],
            weekdaysShort: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sab'],
            weekdaysAbbrev: ['D', 'S', 'T', 'Q', 'Q', 'S', 'S'],
            today: 'Hoje',
            clear: 'Limpar',
            close: 'Pronto',
            labelMonthNext: 'Próximo mês',
            labelMonthPrev: 'Mês anterior',
            labelMonthSelect: 'Selecione um mês',
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
    $("#mudarOpcoesNotif").click(function () { alterarOpcoesNotificacao(); });
    configurarPostar();
    setItens();
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
    setTimeout(function () { $(".conteudoLoja div").children().first().click(); }, 100)
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
            dificuldade: tar.Dificuldade,
            xp: tar.XP
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
    if (acontecimentosProximos.length > 0 && tarefasImportantes.length > 0) {
        for (var i = 0; i < tarefasImportantes.length; i++) {
            var atual = document.createElement('div')
            atual.classList.add('carousel-item');
            atual.classList.add(cores[i]);
            atual.classList.add('white-text');
            atual.innerHTML = `<h2>${tarefasImportantes[i].Titulo}</h2><p class="white-text">${tarefasImportantes[i].Descricao}<br><br>Dificuldade: ${tarefasImportantes[i].Dificuldade}/10<br>Recompensa: <div class="gitcoin" style="filter: brightness(.6)"></div> ${tarefasImportantes[i].Recompensa}<br>Urg&ecirc;ncia: ${tarefasImportantes[i].Urgencia.toFixed(2)}/10<br>Prazo: ${tarefasImportantes[i].Data}</p>`;
            atual.tipo = 0;
            atual.codTarefa = tarefasImportantes[i].CodTarefa;
            $('#carouselImportante').append(atual);
        }
        for (var i = 0; i < acontecimentosProximos.length; i++) {
            var atual = document.createElement('div')
            atual.classList.add('carousel-item');
            atual.classList.add(cores[i + tarefasImportantes.length]);
            atual.classList.add('white-text');
            atual.innerHTML = `<h2>${acontecimentosProximos[i].Titulo}</h2><p class="white-text">${acontecimentosProximos[i].Descricao}<br><br>Data: ${acontecimentosProximos[i].Data}</p>`;
            atual.tipo = 1;
            atual.codAcontecimento = acontecimentosProximos[i].CodAcontecimento;
            $('#carouselImportante').append(atual);
        }
    }
    else {
        $('#carouselImportante').append(`<div class="carousel-item blue white-text"><h2>Voc&ecirc; n&atilde;o tem nenhuma tarefa!</h2></div>`);
    }
    $('.carousel.carousel-slider').carousel({
        fullWidth: true,
        indicators: true,
        onCycleTo: function (e) {
            if (e.tipo == 0)
                $('.carousel-fixed-item a').attr('href', `/tarefas/${e.codTarefa}`);
            else
                $('.carousel-fixed-item a').attr('href', `/acontecimentos/${e.codAcontecimento}`);
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
function comprarItem(id, valor) {
    if (valor > window.usuario.Dinheiro)
        alert('Você não tem dinheiro o suficiente. Estude mais!')
    else
        $.post({
            url: '/ComprarItem',
            data: { idItem: id },
            success: function (i) {
                i = JSON.parse(i)
                mudarItemAtual(i.CodItem, i.Tipo, true)
                setItens();
                window.usuario.Itens.append(i);
                mostrarItem(id);
                alert(`${i.Nome} comprado com sucesso!`)
            },
            async: false
        })
}
function setItens() {
    $.get({
        url: '/GetItensEquipados',
        success: function (itens) {
            if (itens != '') {
                itens = JSON.parse(itens)
                var titu = itens[0];
                var insig = itens[1];
                var deco = itens[2];
                /*var deco = itens.find(item => item.CodItem == window.usuario.Decoracao);
                var insig = itens.find(item => item.CodItem == window.usuario.Insignia);
                var titu = itens.find(item => item.CodItem == parseInt(window.usuario.Titulo.split(" ")[0]));*/

                var conteudos = titu.split(' ');
                $("#spanTituloUsuario").html(conteudos[0]);

                if (conteudos.indexOf("R") > -1)
                    $("#spanTituloUsuario").attr('class', 'rainbow');
                else
                    $("#spanTituloUsuario").removeAttr('class');
                if (conteudos.indexOf("B") > -1)
                    $("#spanTituloUsuario").css('font-weight', 'bold');
                else
                    $("#spanTituloUsuario").removeAttr('style');
                setRainbow();
            }
        },
        async: false
    });
}
function alterarEstadoTarefa(txt, t, estado, el) {
    modalConfirmacao(txt, '', function () {
        $.post({
            url: '/AlterarEstadoTarefa',
            data: { codTarefa: t, idUsuario: window.usuario.Id, estado: estado },
            success: function () {
                $(el).prop("checked", estado);
                aux = 'Dar'
                if (!estado)
                    aux = 'Retirar'
                $.post({
                    url: '/' + aux + 'Recompensa',
                    data: {
                        codTarefa: t,
                        idUsuario: window.usuario.Id
                    },
                    success: function (ret) {
                        ret = JSON.parse(ret);
                        window.usuario.Dinheiro += ret[0];
                        window.usuario.XP += ret[1];
                        mostrarXP(window.usuario)
                        if (estado)
                            $(el).attr('onclick', `alterarEstadoTarefa('Deseja continuar essa tarefa?',${t},false,this)`);
                        else
                            $(el).attr('onclick', `alterarEstadoTarefa('Deseja completar essa tarefa?',${t},true,this)`);
                    },
                    async: false
                })
            },
            async: false
        })
    }, function () {
        $(el).prop("checked", !estado);
    });
}

function alterarOpcoesNotificacao() {
    var notifTarefa = $("#notifTarefa").prop('checked');
    var notifAcontecimento = $("#notifAcontecimento").prop('checked')
    var notifReqAdmin = $("#notifReqAdmin").prop('checked')
    var notifAceitarAmizade = $("#notifAceitarAmizade").prop('checked')
    var notifReqAmizade = $("#notifReqAmizade").prop('checked')
}

function setTarefas() {
    tarefasHtml = '';
    window.usuario.Tarefas.forEach(function (t, i) {
        tarefasHtml += `<li style="position: relative;"><div class="collapsible-header"><label style="width: auto; max-width: 52.5%;"><input ${t.Terminada ? "checked=checked onclick=alterarEstadoTarefa(" + "'Deseja&nbsp;continuar&nbsp;essa&nbsp;tarefa?'" + "," + (t.CodTarefa) + ",false,this);" : "onclick=alterarEstadoTarefa(" + "'Deseja&nbsp;completar&nbsp;essa&nbsp;tarefa?'" + "," + (t.CodTarefa) + ",true,this)"} type="checkbox" /><span style="height: 100%;">${t.Titulo}</span></label><div class="infoData valign-wrapper"><span>${dataBr(t.Data)}</span><img src="../../Images/iconeDataTarefa.png"></div></div><div class="collapsible-body"><span>${t.Descricao}</span></div></li>`
    })
    $("#listaTarefas").html(tarefasHtml)
}
//ultima linha
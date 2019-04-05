var tarefasAtivas = true;
function acionarTarefas()
{
    $('.carousel.carousel-slider').css("transition", "width 1s");

    tarefasAtivas = !tarefasAtivas;
    if (!tarefasAtivas)
    {
        $("aside").css("width", "0px");
        $("#btnAcionarTarefas").css("right", "10px");
        $('.carousel.carousel-slider').css("width", "100%");
    }
    else
    {
        $("aside").css("display", "initial");
        $("aside").css("width", "360px");
        $("#btnAcionarTarefas").css("right", "370px"); 
        $('.carousel.carousel-slider').css("width", "calc(100% - 360px)");

        if ($(window).width() < 750 && triggerEsquerda != 0)
            $("#triggerEsquerda").click();
    }

    var instance = M.Carousel.getInstance(document.getElementById("carouselImportante"));
    var index = instance.center;

    setTimeout(function() {
        dispararResize();
        $('.carousel.carousel-slider').css("transition", "unset");
        if ($("aside").width() == 0)
            $("aside").css("display", "none");
    }, 1000);
}

function dispararResize()
{
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

function lidarComAberturaSliderEsquerda()
{
    setTimeout(dispararResize, 1000);
    if ($(window).width() < 750 && $("aside").width() > 0)
        acionarTarefas();
}

setTimeout(function() {
    $('.carousel.carousel-slider').carousel({
        fullWidth: true,
        indicators: true
      });
    dispararResize();
    $("#btnAcionarTarefas").tooltip();
    $("#listaTarefas").collapsible();
}, 1);



// menu da esquerda


function acionarEsquerda()
{
  if (estaAbrindoEsquerda)
  {
    if (triggerEsquerda == 0)
      abrirEsquerda();
    else
      fecharEsquerda();
    var left = -triggerEsquerda*$("#slideEsquerda").width() + $("#slideEsquerda").width() - 165;
    triggerEsquerda = Math.abs(triggerEsquerda - 1);
    setTimeout(function() {estaAbrindoEsquerda = false;}, 900);
  }
}
function abrirEsquerda()
{
    $("#setaUmTriggerEsquerda").rotate(0);
    $("#setaDoisTriggerEsquerda").rotate(0);
    $("#triggerEsquerda").css('left', (-triggerEsquerda*$("#slideEsquerda").width() + $("#slideEsquerda").width() - 165) + "px")
    $("#slideEsquerda").css('left', (-triggerEsquerda*$("#slideEsquerda").width() - 5) + "px");
    $("#containerConteudo").css('left', $("#slideEsquerda").offset().left + $("#slideEsquerda").width() + "px");
    $("#containerConteudo").css('width', 'calc(100% - ' + $("#slideEsquerda").width() +"px)")
    if (tarefasAtivas)
        $(".apenasTelasMaiores").width("61.75em");
    else
        $(".apenasTelasMaiores").width("81.5em");
    try
    {
        lidarComAberturaSliderEsquerda();
    }
    catch{}
}
function fecharEsquerda()
{
    $("#setaUmTriggerEsquerda").rotate(-180);
    $("#setaDoisTriggerEsquerda").rotate(-180);
    $("#triggerEsquerda").css('left', "-165px")
    $("#slideEsquerda").css('left', (-$("#slideEsquerda").width() - 5) + 'px');
    $("#containerConteudo").css('left', '0');
    $("#containerConteudo").css('width', '100%')
    $(".apenasTelasMaiores").css('left', '0')
    if (tarefasAtivas)
        $(".apenasTelasMaiores").width("81.5em");
    else
        $(".apenasTelasMaiores").width("100%");
}

jQuery.fn.rotate = function(degrees) {
  $(this).css({'transform' : 'rotate('+ degrees +'deg)'});
};
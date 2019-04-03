var tarefasAtivas = false;
function acionarTarefas()
{
    $('.carousel.carousel-slider').css("transition", "width 1s");

    tarefasAtivas = !tarefasAtivas;
    if (tarefasAtivas)
    {
        $("aside").css("width", "0px");
        $("#btnAcionarTarefas").css("right", "10px");
        $('.carousel.carousel-slider').css("width", "100%");
    }
    else
    {
        $("aside").css("width", "360px");
        $("#btnAcionarTarefas").css("right", "370px"); 
        $('.carousel.carousel-slider').css("width", "calc(100% - 360px)");
    }

    var instance = M.Carousel.getInstance(document.getElementById("carouselImportante"));
    var index = instance.center;

    setTimeout(dispararResize, 1000);
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

setTimeout(function() {
    $('.carousel.carousel-slider').carousel({
        fullWidth: true,
        indicators: true
      });
    dispararResize();
}, 1);

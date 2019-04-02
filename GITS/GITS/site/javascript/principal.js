$('.carousel.carousel-slider').carousel({
    fullWidth: true,
    indicators: true
  });

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

    setTimeout(function() {
        $('.carousel.carousel-slider').clear();
        $('.carousel.carousel-slider').carousel({
            fullWidth: true,
            indicators: true
          });
    }, 1000)
}
var index = false;

window.onload = function () {
    var freq = document.getElementById("freq").getContext("2d");
    var lembrar = document.getElementById("lembrar").getContext("2d");
    var controlar = document.getElementById("controlar").getContext("2d");
    var dificuldade = document.getElementById("dificuldade").getContext("2d");

    window.freq = criarGraficoPareto(freq, ["Algumas vezes por dia", "Não checam", "Algumas vezes por semana", "Uma vez por dia"/*, "Uma vez por semana"*/],
        [41.70, 33.30, 50.00, 100.00/*, 100.00*/], [5, 4, 2, 1/*, 0*/], '186, 30, 20', '66, 134, 244');
    window.lembrar = criarGraficoPareto(lembrar, ['Calendário', 'Memória', 'Agenda', 'Calendário, Agenda e Memória', 'Não lembram', 'Outra pessoa as lembra'],
        [41.70, 66.70, 75.00, 83.30, 91.60, 100.00], [5, 3, 1, 1, 1, 1], '186, 30, 20', '66, 134, 244');
    window.controlar = criarGraficoPareto(controlar, ['Sim', 'Não'], [66.67, 100.00], [8, 4], '186, 30, 20', '66, 134, 244');
    window.dificuldade = criarGraficoPareto(dificuldade, ["Falta de motivação", "Procrastinação", "Manter controle de tarefas"],
        [47.62, 76.19, 100.00], [10, 6, 5], '186, 30, 20', '66, 134, 244');

    $("p").prepend("&emsp;&emsp;&emsp;&emsp;")
};

function criarGraficoPareto(ctx, labels, valoresP, valores, corLinha, corBarra) {
    var data = {
        labels: labels,
        datasets: [{
            type: "line",
            label: "Acumulado",
            borderColor: `rgba(${corLinha}, 1)`,
            backgroundColor: `rgba(${corLinha}, 0.5)`,
            pointBorderWidth: 5,
            fill: false,
            data: valoresP,
            yAxisID: 'y-axis-2'
        }, {
            type: "bar",
            label: "Respostas/Opções",
            borderColor: `rgba(${corBarra}, 1)`,
            backgroundColor: `rgba(${corBarra}, 0.5)`,
            data: valores,
            yAxisID: 'y-axis-1'
        }]
    };

    var options = {
        scales: {
            xAxes: [{
                stacked: true,
                scaleLabel: {
                    display: true,
                    labelString: "Respostas/Opções"
                }
            }],

            yAxes: [{
                type: "linear",
                position: "left",
                id: "y-axis-1",
                stacked: true,
                ticks: {
                    suggestedMin: 0
                },
                scaleLabel: {
                    display: true,
                    labelString: "Pessoas"
                }
            }, {
                type: "linear",
                position: "right",
                id: "y-axis-2",
                ticks: {
                    suggestedMin: 0,
                    callback: function (value) {
                        return value + "%";
                    }
                },
                scaleLabel: {
                    display: true,
                    labelString: "Porcentagem"
                }
            }]
        }
    };
    return new Chart(ctx, {
        type: 'bar',
        data: data,
        options: options
    });
}
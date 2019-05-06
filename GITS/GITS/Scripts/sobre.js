var index = false;

$(window).resize(function () {
    $("#containerConteudo").css('height', $("#footer").offset().top - $("#containerConteudo").offset().top + 'px');
})


var data = {
    labels: ["Algumas vezes por dia", "Algumas vezes por semana", "Não checo", "Uma vez por dia"/*, "Uma vez por semana"*/],
    datasets: [{
        type: "line",
        label: "Acumulado",
        borderColor: "rgba(186, 30, 20, 1)",
        backgroundColor: "rgba(186, 30, 20, 0.5)",
        pointBorderWidth: 5,
        fill: false,
        data: [50.00, 70.00, 90.00, 100.00],
        yAxisID: 'y-axis-2'
    }, {
        type: "bar",
        label: "Algumas vezes por dia",
        borderColor: "rgba(66, 134, 244, 1)",
        backgroundColor: "rgba(66, 134, 244, 0.5)",
        data: [50, 20, 20, 10],
        yAxisID: 'y-axis-1'
    }]
};

var options = {
    scales: {
        xAxes: [{
            stacked: true,
            scaleLabel: {
                display: true,
                labelString: "Opções"
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

window.onload = function () {
    var freq = document.getElementById("freq").getContext("2d");
    var lembrar = document.getElementById("lembrar").getContext("2d");
    var controlar = document.getElementById("controlar").getContext("2d");

    window.freq = criarGraficoPareto(freq, ["Algumas vezes por dia", "Algumas vezes por semana", "Não checo", "Uma vez por dia"/*, "Uma vez por semana"*/],
        [50.00, 70.00, 90.00, 100.00/*, 100.00*/], [5, 2, 2, 1/*, 0*/], '186, 30, 20', '66, 134, 244');
    window.lembrar = criarGraficoPareto(lembrar, ['Calendário', 'Memória', 'Agenda', 'Calendário , Agenda e Memória', 'Não lembro'/*, 'Outra pessoa me lembra'*/],
        [50.00, 70.00, 80.00, 90.00, 100.00/*, 100.00*/], [5, 2, 1, 1, 1/*, 0*/], '186, 30, 20', '66, 134, 244');
    window.controlar = criarGraficoPareto(controlar, ['Sim', 'Não'], [80.00, 100.00], [8, 2], '186, 30, 20', '66, 134, 244');
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
            label: "Algumas vezes por dia",
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
(function () {
    var el = document.getElementById("question-donut-chart");
    if (!el || typeof ApexCharts === "undefined") return;

    var series = JSON.parse(el.dataset.series || "[]");
    var labels = JSON.parse(el.dataset.labels || "[]");

    function fmt(v) {
        v = Number(v) || 0;
        return v >= 1000 ? (v / 1000).toFixed(1) + "k" : String(v);
    }

    var options = {
        series: series,
        labels: labels,
        colors: ["#1A56DB", "#057A55", "#7E3AF2"],
        chart: { type: "donut", height: 305, fontFamily: "Poppins, sans-serif" },
        stroke: { colors: ["transparent"] },
        plotOptions: {
            pie: {
                donut: {
                    size: "75%",
                    labels: {
                        show: true,
                        name: {
                            show: true,
                            offsetY: 20,
                            fontSize: "13px",
                            fontWeight: 500,
                            color: "#9CA3AF",
                            formatter: function () { return "QUESTIONS"; }
                        },
                        value: {
                            show: true,
                            offsetY: -18,
                            fontSize: "40px",
                            fontWeight: 700,
                            color: "#111827",
                            formatter: fmt
                        },
                        total: {
                            show: true,
                            showAlways: true,
                            label: "INDEXED",
                            fontSize: "13px",
                            fontWeight: 500,
                            color: "#9CA3AF",
                            formatter: function (w) {
                                return fmt(w.globals.seriesTotals.reduce(function (a, b) { return a + b; }, 0));
                            }
                        }
                    }
                }
            }
        },
        dataLabels: { enabled: false },
        legend: { show: false },
        tooltip: { enabled: true, y: { formatter: fmt } }
    };

    new ApexCharts(el, options).render();
})();

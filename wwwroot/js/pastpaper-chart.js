(function () {
    var el = document.getElementById("pastpaper-donut-chart");
    if (!el || typeof ApexCharts === "undefined") return;

    var series = JSON.parse(el.dataset.series || "[]");
    var labels = JSON.parse(el.dataset.labels || "[]");

    function fmt(v) {
        v = Number(v) || 0;
        return v >= 1000 ? (v / 1000).toFixed(1) + "k" : String(v);
    }

    // Palette repeats if there are more boards than colors.
    var palette = ["#1A56DB", "#057A55", "#7E3AF2", "#E3A008", "#E02424", "#0694A2", "#FF5A1F", "#6875F5"];

    var options = {
        series: series,
        labels: labels,
        colors: palette,
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
                            formatter: function () { return "BOARDS"; }
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
                            label: "PAST PAPERS",
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
        legend: {
            show: true,
            position: "bottom",
            fontSize: "13px",
            fontFamily: "Poppins, sans-serif",
            labels: { colors: "#6B7280" },
            markers: { width: 10, height: 10, radius: 10 },
            itemMargin: { horizontal: 8, vertical: 4 }
        },
        tooltip: { enabled: true, y: { formatter: fmt } }
    };

    new ApexCharts(el, options).render();
})();

(function () {
    var el = document.getElementById("institute-area-chart");
    if (!el || typeof ApexCharts === "undefined") return;

    var series = JSON.parse(el.dataset.series || "[]");
    var labels = JSON.parse(el.dataset.labels || "[]");
    var height = parseInt(el.dataset.height, 10) || 250;

    var options = {
        chart: {
            height: height,
            type: "area",
            fontFamily: "Poppins, sans-serif",
            dropShadow: { enabled: false },
            toolbar: { show: false }
        },
        tooltip: { enabled: true, x: { show: false } },
        fill: {
            type: "gradient",
            gradient: { opacityFrom: 0.55, opacityTo: 0, shade: "#1C64F2", gradientToColors: ["#1C64F2"] }
        },
        dataLabels: { enabled: false },
        stroke: { width: 6, curve: "smooth" },
        grid: { show: true, strokeDashArray: 4, padding: { left: 2, right: 2, top: 0 } },
        series: [{ name: "Institutes", data: series, color: "#1A56DB" }],
        xaxis: {
            categories: labels,
            labels: { show: true, style: { colors: "#9CA3AF", fontFamily: "Poppins, sans-serif" } },
            axisBorder: { show: false },
            axisTicks: { show: false }
        },
        yaxis: { show: false }
    };

    new ApexCharts(el, options).render();
})();

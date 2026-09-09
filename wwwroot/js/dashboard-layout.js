// Drag-to-reorder for dashboard cards. Order is saved per institute (or per user
// for accounts without one) via the SaveLayout endpoint. Reordering is client-side;
// unknown/new cards fall back to their default position.
(function () {
    var toolbar = document.querySelector("[data-dashboard-toolbar]");
    if (!toolbar || typeof Sortable === "undefined") return;

    var sections = Array.prototype.slice.call(document.querySelectorAll("[data-sortable-section]"));
    if (!sections.length) return;

    var tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
    var saveUrl = toolbar.dataset.saveUrl;
    var resetUrl = toolbar.dataset.resetUrl;

    var saved = {};
    try { saved = JSON.parse(toolbar.dataset.layout || "{}") || {}; } catch (e) { saved = {}; }

    function sectionName(el) { return el.dataset.sortableSection; }

    function cards(section) {
        return Array.prototype.filter.call(section.children, function (c) {
            return c.dataset && c.dataset.cardKey;
        });
    }

    // Apply saved order: listed keys first (in saved order), unlisted cards keep their spot at the end.
    sections.forEach(function (section) {
        var order = saved[sectionName(section)];
        if (!Array.isArray(order) || !order.length) return;

        var byKey = {};
        cards(section).forEach(function (c) { byKey[c.dataset.cardKey] = c; });
        order.forEach(function (key) {
            if (byKey[key]) { section.appendChild(byKey[key]); delete byKey[key]; }
        });
    });

    // Charts were rendered before this ran, so nudge ApexCharts to remeasure after any re-parenting.
    window.dispatchEvent(new Event("resize"));

    function collectLayout() {
        var layout = {};
        sections.forEach(function (section) {
            layout[sectionName(section)] = cards(section).map(function (c) { return c.dataset.cardKey; });
        });
        return JSON.stringify(layout);
    }

    function postLayout(url, layout) {
        var params = new URLSearchParams();
        if (tokenInput) params.append("__RequestVerificationToken", tokenInput.value);
        if (layout != null) params.append("layout", layout);
        return fetch(url, {
            method: "POST",
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
            body: params.toString()
        });
    }

    // Dragging is enabled by default — no toggle needed.
    sections.forEach(function (section) {
        Sortable.create(section, {
            animation: 150,
            draggable: "[data-card-key]",
            ghostClass: "opacity-40",
            onEnd: function () { postLayout(saveUrl, collectLayout()); }
        });
        cards(section).forEach(function (c) { c.classList.add("cursor-move"); });
    });

    var resetBtn = toolbar.querySelector("[data-arrange-reset]");
    if (resetBtn) resetBtn.addEventListener("click", function () {
        postLayout(resetUrl, null).then(function () { window.location.reload(); });
    });
})();

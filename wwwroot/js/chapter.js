// Chapter/topic selection for the paper builder step: keeps the "select all",
// per-chapter, and per-topic checkboxes in sync (including indeterminate states).
(function () {
    const form = document.getElementById('chapter-form');
    if (!form) return;

    const master = document.getElementById('select-all');
    const allBoxes = () => [...form.querySelectorAll('.chapter-check, .topic-check')];
    const topicsOf = id => [...form.querySelectorAll(`.topic-check[data-chapter="${id}"]`)];

    form.addEventListener('change', e => {
        if (e.target === master) {
            allBoxes().forEach(b => (b.checked = master.checked));
        } else if (e.target.classList.contains('chapter-check')) {
            topicsOf(e.target.value).forEach(t => (t.checked = e.target.checked));
        }
        sync();
    });

    function sync() {
        form.querySelectorAll('.chapter-check').forEach(c => {
            const topics = topicsOf(c.value);
            if (!topics.length) {
                return;
            }
            c.checked = topics.every(t => t.checked);
            c.indeterminate = !c.checked && topics.some(t => t.checked);
        });
        const boxes = allBoxes();
        master.checked = boxes.length > 0 && boxes.every(b => b.checked);
        master.indeterminate = !master.checked && boxes.some(b => b.checked);
    }

    sync();
})();

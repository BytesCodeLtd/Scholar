(function () {
    function wire(form) {
        const instSelect = form.querySelector('[data-sg-institute]');
        const classSelect = form.querySelector('[data-sg-class]');
        const items = form.querySelectorAll('[data-sg-item]');

        // No institute picker (institute admin): show everything, nothing to filter.
        if (!instSelect) return;

        function refresh() {
            const inst = instSelect.value;

            if (classSelect) {
                Array.from(classSelect.options).forEach(opt => {
                    if (!opt.value) return; // keep the placeholder
                    const match = opt.dataset.institute === inst;
                    opt.hidden = !match;
                    opt.disabled = !match;
                });
                const cur = classSelect.selectedOptions[0];
                if (cur && cur.value && cur.dataset.institute !== inst) {
                    classSelect.value = '';
                }
            }

            items.forEach(el => {
                const match = el.dataset.institute === inst;
                el.style.display = match ? '' : 'none';
                if (!match) {
                    const cb = el.querySelector('input[type="checkbox"]');
                    if (cb) cb.checked = false;
                }
            });
        }

        instSelect.addEventListener('change', refresh);
        refresh();
    }

    document.querySelectorAll('[data-sg-form]').forEach(wire);
})();

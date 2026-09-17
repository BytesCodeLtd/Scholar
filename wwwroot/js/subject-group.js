(function () {
    function wire(form) {
        const instSelect = form.querySelector('[data-sg-institute]');
        const classSelect = form.querySelector('[data-sg-class]');
        const sectionItems = form.querySelectorAll('[data-sg-section]');
        const subjectItems = form.querySelectorAll('[data-sg-subject]');

        function setVisible(el, visible) {
            el.style.display = visible ? '' : 'none';
            if (!visible) {
                const cb = el.querySelector('input[type="checkbox"]');
                if (cb) cb.checked = false;
            }
        }

        function refreshSections() {
            const opt = classSelect ? classSelect.selectedOptions[0] : null;
            const ids = (opt && opt.dataset.sections) ? opt.dataset.sections.split(',') : [];
            sectionItems.forEach(el => setVisible(el, ids.indexOf(el.dataset.sectionId) !== -1));
        }

        function refreshInstitute() {
            if (!instSelect) return;
            const inst = instSelect.value;

            if (classSelect) {
                Array.from(classSelect.options).forEach(opt => {
                    if (!opt.value) return; // keep placeholder
                    const match = opt.dataset.institute === inst;
                    opt.hidden = !match;
                    opt.disabled = !match;
                });
                const cur = classSelect.selectedOptions[0];
                if (cur && cur.value && cur.dataset.institute !== inst) {
                    classSelect.value = '';
                }
            }

            subjectItems.forEach(el => setVisible(el, el.dataset.institute === inst));
        }

        if (instSelect) {
            // Changing institute can reset the class, so refresh sections afterwards too.
            instSelect.addEventListener('change', () => { refreshInstitute(); refreshSections(); });
        }
        if (classSelect) {
            classSelect.addEventListener('change', refreshSections);
        }

        refreshInstitute();
        refreshSections();
    }

    document.querySelectorAll('[data-sg-form]').forEach(wire);
})();

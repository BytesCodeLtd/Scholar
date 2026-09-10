// Generic searchable dropdown. Any <select> tagged with `data-searchable`
// (or the `.js-searchable` class) is upgraded to a Tom Select combobox with
// type-to-filter. Works with normal form posting since it wraps the real
// <select>, so `name`/`value` binding is unchanged.
//
// Usage:
//   <select name="instituteId" data-searchable>...</select>
//   <select data-searchable data-placeholder="Pick a subject…">...</select>
//   <select data-searchable multiple>...</select>   (multi-select works too)
(function () {
    function initSearchableSelects(root) {
        if (typeof TomSelect === 'undefined') return;

        (root || document).querySelectorAll('select[data-searchable], select.js-searchable').forEach(el => {
            if (el.tomselect) return; // already initialised

            const placeholder = el.dataset.placeholder
                || el.querySelector('option[value=""]')?.textContent?.trim()
                || 'Select…';

            new TomSelect(el, {
                placeholder,
                allowEmptyOption: false,    // ignore the empty "Select…" option so it shows as a placeholder, not a value
                maxOptions: null,           // show all matches, don't cap the list
                hideSelected: el.multiple,
                dropdownParent: 'body',     // avoids clipping inside overflow/scroll containers
                render: {
                    no_results: (data, escape) =>
                        `<div class="ts-no-results">No results for "${escape(data.input)}"</div>`
                }
            });
        });
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => initSearchableSelects());
    } else {
        initSearchableSelects();
    }

    // Expose so dynamically injected markup (modals, AJAX partials) can re-scan.
    window.initSearchableSelects = initSearchableSelects;
})();

// Remembers whether the Examination group is expanded across page loads. The
// state is driven entirely by the user's own choice (stored in localStorage) and
// is the same on every page - it never auto-opens or auto-closes based on the
// current route.
(function () {
    var KEY = 'scholar:sidebar:examination';
    var toggle = document.querySelector('[data-collapse-toggle="dropdown-papers"]');
    var menu = document.getElementById('dropdown-papers');
    if (!toggle || !menu) return;

    function isOpen() { return !menu.classList.contains('hidden'); }
    function setOpen(open) {
        menu.classList.toggle('hidden', !open);
        toggle.setAttribute('aria-expanded', open ? 'true' : 'false');
    }

    // Restore the saved state (defaults to collapsed on first ever visit).
    setOpen(localStorage.getItem(KEY) === 'open');

    // Persist the user's choice.
    toggle.addEventListener('click', function () {
        setTimeout(function () {
            localStorage.setItem(KEY, isOpen() ? 'open' : 'closed');
        }, 0);
    });
})();

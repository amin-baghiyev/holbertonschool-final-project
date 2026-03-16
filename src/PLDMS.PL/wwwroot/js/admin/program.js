const modal = document.getElementById('addProgramModal');
const form = document.getElementById("createProgramForm");

let programId = null;

const toggleModal = (show) => {
    modal.classList.toggle('hidden', !show);
    document.body.style.overflow = show ? 'hidden' : 'auto';

    if (!show) {
        programId = null;
        form.reset();
        document.querySelectorAll('[data-valmsg-for]').forEach(x => x.textContent = '');
        document.getElementById("modalTitle").innerText = "Create New Program";
    }
};

document.addEventListener("click", (e) => {
    const target = e.target;

    if (target.closest(".add-program")) return toggleModal(true);
    if (target.closest(".modal-overlay") || target.closest(".cancel-btn")) return toggleModal(false);

    const editBtn = target.closest(".edit-btn");
    if (editBtn) {
        programId = editBtn.dataset.id;
        form.Name.value = editBtn.dataset.name;
        form.Duration.value = editBtn.dataset.duration;

        document.getElementById("modalTitle").innerText = "Update Program";
        toggleModal(true);
    }
});

form.addEventListener('submit', async e => {
    e.preventDefault();

    const url = programId
        ? `/Admin/Program/Update?id=${programId}`
        : '/Admin/Program/Create';

    const res = await fetch(url, {
        method: programId ? 'PUT' : 'POST',
        body: new FormData(form),
        headers: { 'Accept': 'application/json' }
    });

    if (res.ok) return location.reload();

    const data = await res.json();
    showValidationErrors(data.errors);
});

function showValidationErrors(errors) {
    document.querySelectorAll('[data-valmsg-for]').forEach(x => x.textContent = '');

    if (!errors) return;

    Object.entries(errors).forEach(([key, value]) => {
        const span = document.querySelector(`[data-valmsg-for="${key}"]`);
        if (span) span.textContent = value[0];
    });
}

document.addEventListener('keydown', e => {
    if (e.key === 'Escape' && !modal.classList.contains('hidden'))
        toggleModal(false);
});
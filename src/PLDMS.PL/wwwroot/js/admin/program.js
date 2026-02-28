const modal = document.getElementById('addProgramModal');
let isEdit = false;
let programId;

const openModal = () => {
    modal.classList.remove('hidden');
    document.body.style.overflow = 'hidden';
}

const closeModal = () => {
    modal.classList.add('hidden');
    document.body.style.overflow = 'auto';

    isEdit = false;
    programId = null;

    const form = document.getElementById("createProgramForm");
    form.reset();

    document.querySelectorAll('[data-valmsg-for]').forEach(span => span.textContent = '');

    document.getElementById("modalTitle").innerText = "Create New Program";
}

const handleEditClick = (btn) => {
    programId = btn.dataset.id;
    const name = btn.dataset.name;
    const duration = btn.dataset.duration;

    const form = document.getElementById("createProgramForm");
    form.querySelector('[name="Name"]').value = name;
    form.querySelector('[name="Duration"]').value = duration;

    document.getElementById("modalTitle").innerText = "Update Program";

    openModal();
};

document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape' && !modal.classList.contains('hidden')) {
        closeModal();
    }
});

document.addEventListener("click", (e) => {
    const target = e.target;

    if (target.closest(".modal-overlay") || target.closest(".cancel-btn")) {
        closeModal();
        return;
    }

    if (target.closest(".add-program") || target.closest(".save-program")) {
        openModal();
        return;
    }

    const editBtn = target.closest(".edit-btn");
    if (editBtn) {
        handleEditClick(editBtn);
    }
});

function showValidationErrors(errors) {
    document.querySelectorAll('[data-valmsg-for]')
        .forEach(span => span.textContent = '');

    for (const key in errors) {
        const span = document.querySelector(`[data-valmsg-for="${key}"]`);
        if (span) {
            span.textContent = errors[key][0];
        }
    }
}

document.addEventListener('submit', async (e) => {
    const form = e.target;

    if (form && form.id === 'createProgramForm') {
        e.preventDefault();

        const formData = new FormData(form);

        if (programId) {
            formData.append('Id', programId);
        }

        isEdit = !!formData.get('Id');
        const url = isEdit ? '/Admin/Program/Update' : '/Admin/Program/Create';

        const response = await fetch(url, {
            method: isEdit ? 'PUT' : 'POST',
            body: formData
        });

        const result = await response.json();

        if (response.ok) {
            if (result.success) {
                window.location.reload();
            }
        } else {
            showValidationErrors(result.errors);
        }
    }
});
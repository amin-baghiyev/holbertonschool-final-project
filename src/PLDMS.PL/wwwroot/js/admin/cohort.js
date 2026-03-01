const modal = document.getElementById('addCohortModal');
let isEdit = false;
let cohortId;

const fetchPrograms = () => {
    return fetch('/Admin/Cohort/GetPrograms')
        .then(res => res.json())
        .then(data => {
            const select = document.getElementById("ProgramId");
            select.innerHTML = "<option value=''>Select Program</option>";
            data.forEach(p => {
                const option = document.createElement("option");
                option.value = p.id;
                option.textContent = p.name;
                select.appendChild(option);
            });
        });
}
const openModal = () => {
    modal.classList.remove('hidden');
    document.body.style.overflow = 'hidden';
}

const closeModal = () => {
    modal.classList.add('hidden');
    document.body.style.overflow = 'auto';

    isEdit = false;
    cohortId = null;

    const form = document.getElementById("createCohortForm");
    form.reset();

    document.querySelectorAll('[data-valmsg-for]').forEach(span => span.textContent = '');

    document.getElementById("modalTitle").innerText = "Create New Cohort";
}

const handleEditClick = (btn) => {
    cohortId = btn.dataset.id;
    const name = btn.dataset.name;
    const programId = btn.dataset.programId;
    const startDate = btn.dataset.startDate;
    const endDate = btn.dataset.endDate;

    const form = document.getElementById("createCohortForm");

    form.querySelector('[name="Name"]').value = name;
    form.querySelector('[name="ProgramId"]').value = programId;
    form.querySelector('[name="StartDate"]').value = new Date(startDate).toISOString().split('T')[0];
    form.querySelector('[name="EndDate"]').value = new Date(endDate).toISOString().split('T')[0];

    document.getElementById("modalTitle").innerText = "Update Cohort";

    openModal();
};

document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape' && !modal.classList.contains('hidden')) {
        closeModal();
    }
});

document.addEventListener("click", async(e) => {
    const target = e.target;

    if (target.closest(".modal-overlay") || target.closest(".cancel-btn")) {
        closeModal();
        return;
    }

    if (target.closest(".add-cohort") || target.closest(".save-cohort")) {
        await fetchPrograms();
        openModal();
        return;
    }

    const editBtn = target.closest(".edit-btn");
    if (editBtn) {
        await fetchPrograms();
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

    if (form && form.id === 'createCohortForm') {
        e.preventDefault();

        const formData = new FormData(form);

        if (cohortId) {
            formData.append('Id', cohortId);
        }

        isEdit = !!formData.get('Id');
        const url = isEdit ? '/Admin/Cohort/Update' : '/Admin/Cohort/Create';

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
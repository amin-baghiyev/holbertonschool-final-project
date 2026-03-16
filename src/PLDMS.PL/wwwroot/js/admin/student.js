const modal = document.getElementById('studentModal');
const form = document.getElementById('studentForm');
const globalErrorContainer = document.getElementById('globalErrorContainer');
let studentId = null;

const openModal = () => {
    modal.classList.remove('hidden');
    document.body.style.overflow = 'hidden';
};

const closeModal = () => {
    modal.classList.add('hidden');
    document.body.style.overflow = 'auto';
    studentId = null;
    form.reset();
    document.querySelectorAll('[data-valmsg-for]').forEach(span => span.textContent = '');
    globalErrorContainer.classList.add('hidden');
    document.getElementById("modalTitle").innerText = "Create New Student";
};

document.addEventListener("click", async (e) => {
    const target = e.target;

    if (target.closest(".add-student-btn")) openModal();

    if (target.closest(".modal-overlay") || target.closest(".cancel-btn")) closeModal();

    const editBtn = target.closest(".edit-btn");
    if (editBtn) {
        studentId = editBtn.dataset.id;
        form.querySelector('[name="FullName"]').value = editBtn.dataset.fullname;
        form.querySelector('[name="Email"]').value = editBtn.dataset.email;
        form.querySelector('[name="UserName"]').value = editBtn.dataset.username;

        document.getElementById("modalTitle").innerText = "Update Student";
        openModal();
    }
});

form.addEventListener('submit', async (e) => {
    e.preventDefault();
    const formData = new FormData(form);
    const isEdit = !!studentId;
    const url = isEdit ? `/Admin/Student/Update?id=${studentId}` : '/Admin/Student/Create';

    try {
        const response = await fetch(url, {
            method: isEdit ? 'PUT' : 'POST',
            body: formData,
            headers: {
                'Accept': 'application/json',
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        const contentType = response.headers.get("content-type");
        if (contentType && contentType.indexOf("application/json") !== -1) {
            const result = await response.json();

            if (response.ok && result.success) {
                window.location.reload();
            } else {
                showValidationErrors(result.errors);
            }
        } else {
            showValidationErrors({ "Error": ["Server returned an unexpected response format."] });
        }
    } catch (err) {
        showValidationErrors({ "Error": ["A network error occurred. Please try again."] });
    }
});
function showValidationErrors(errors) {
    document.querySelectorAll('[data-valmsg-for]').forEach(span => span.textContent = '');
    globalErrorContainer.classList.add('hidden');

    if (!errors) return;

    for (const key in errors) {
        const span = document.querySelector(`[data-valmsg-for="${key}"]`);
        if (span) {
            span.textContent = errors[key][0];

            if (key === "Error") {
                globalErrorContainer.classList.remove('hidden');
            }
        }
    }
}

document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape' && !modal.classList.contains('hidden')) closeModal();
});
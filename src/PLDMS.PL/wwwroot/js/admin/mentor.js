const modal = document.getElementById('mentorModal');
const form = document.getElementById('mentorForm');
const globalErrorContainer = document.getElementById('globalErrorContainer');
let mentorId = null;

const openModal = () => {
    modal.classList.remove('hidden');
    document.body.style.overflow = 'hidden';
};

const closeModal = () => {
    modal.classList.add('hidden');
    document.body.style.overflow = 'auto';
    mentorId = null;
    form.reset();
    document.querySelectorAll('[data-valmsg-for]').forEach(span => span.textContent = '');
    globalErrorContainer.classList.add('hidden');
    document.getElementById("modalTitle").innerText = "Create New Mentor";
};

// Global Click Handlers
document.addEventListener("click", async (e) => {
    const target = e.target;

    // Open Create Modal
    if (target.closest(".add-mentor-btn")) {
        openModal();
    }

    // Close Modal
    if (target.closest(".modal-overlay") || target.closest(".cancel-btn")) {
        closeModal();
    }

    // Open Edit Modal
    const editBtn = target.closest(".edit-btn");
    if (editBtn) {
        mentorId = editBtn.dataset.id;
        form.querySelector('[name="FullName"]').value = editBtn.dataset.fullname;
        form.querySelector('[name="Email"]').value = editBtn.dataset.email;
        form.querySelector('[name="UserName"]').value = editBtn.dataset.username;

        document.getElementById("modalTitle").innerText = "Update Mentor";
        openModal();
    }
});

form.addEventListener('submit', async (e) => {
    e.preventDefault();

    const formData = new FormData(form);
    const isEdit = !!mentorId;

    let url = '/Admin/Mentor/Create';
    if (isEdit) {
        url = `/Admin/Mentor/Update?id=${mentorId}`;
    }

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

// Escape key to close
document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape' && !modal.classList.contains('hidden')) closeModal();
});
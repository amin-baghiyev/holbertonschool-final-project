const modal = document.getElementById('studentModal');
const form = document.getElementById('studentForm');
let studentId = null;

const toggleModal = (show) => {
    modal.classList.toggle('hidden', !show);
    document.body.style.overflow = show ? 'hidden' : 'auto';

    if (!show) {
        mentorId = null;
        form.reset();
        document.querySelectorAll('[data-valmsg-for]').forEach(x => x.textContent = '');
        document.getElementById("modalTitle").innerText = "Create New Program";
    }
};
document.addEventListener("click", (e) => {
    const target = e.target;

    if (target.closest(".add-student-btn")) return toggleModal(true);
    if (target.closest(".modal-overlay") || target.closest(".cancel-btn")) return toggleModal(false);

    const editBtn = target.closest(".edit-btn");
    if (editBtn) {
        studentId = editBtn.dataset.id;
        form.FullName.value = editBtn.dataset.fullname;
        form.Email.value = editBtn.dataset.email;
        form.UserName.value = editBtn.dataset.username;
        document.getElementById("modalTitle").innerText = "Update Student";
        toggleModal(true);
    }
});

form.addEventListener('submit', async e => {
    e.preventDefault();

    const url = studentId
        ? `/Admin/Student/Update?id=${studentId}`
        : '/Admin/Student/Create';

    const res = await fetch(url, {
        method: studentId ? 'PUT' : 'POST',
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
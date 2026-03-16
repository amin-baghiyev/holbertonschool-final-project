const modal = document.getElementById('addCohortModal');
const studentModal = document.getElementById('addStudentModal');
const form = document.getElementById("createCohortForm");
let cohortId = null;
let allStudents = [];
let selectedStudentIds = new Set();

const fetchExistingStudents = async (id) => {
    try {
        const res = await fetch(`/Admin/Cohort/GetStudentsByCohortId?id=${id}`);
        const existingStudents = await res.json();

        existingStudents.forEach(s => {
            selectedStudentIds.add(s.id.toString());
        });

        document.getElementById('selectedCountText').textContent = `${selectedStudentIds.size} students selected`;
    } catch (err) {
        console.error("Failed to fetch existing students:", err);
    }
};

const openStudentModal = async (id) => {
    cohortId = id;
    studentModal.classList.remove('hidden');
    document.body.style.overflow = 'hidden';

    selectedStudentIds.clear();

    await fetchExistingStudents(id);

    if (allStudents.length === 0) {
        await fetchStudents();
    } else {
        renderStudentList(allStudents);
    }
};
const closeStudentModal = () => {
    studentModal.classList.add('hidden');
    document.body.style.overflow = 'auto';
    cohortId = null;
    selectedStudentIds.clear();
};

const fetchStudents = async () => {
    const container = document.getElementById('studentListContainer');
    try {
        const res = await fetch('/Admin/Cohort/GetStudents');
        allStudents = await res.json();
        renderStudentList(allStudents);
    } catch (err) {
        container.innerHTML = `<div class="text-red-500 text-center py-4">Failed to load students.</div>`;
    }
};
const renderStudentList = (students) => {
    const container = document.getElementById('studentListContainer');

    if (students.length === 0) {
        container.innerHTML = `<div class="text-gray-500 text-center py-8">No students found.</div>`;
        return;
    }

    container.innerHTML = students.map(student => `
        <div class="flex items-center justify-between p-3 hover:bg-gray-50 rounded-lg transition-colors border-b border-gray-50">
            <div class="flex items-center gap-3">
                <input type="checkbox" 
       class="student-checkbox w-5 h-5 rounded border-gray-300 text-brand-red focus:ring-brand-red" 
       value="${student.id}"
       ${selectedStudentIds.has(student.id.toString()) ? 'checked' : ''}>
                <div>
                    <p class="font-medium text-gray-800">${student.fullName}</p>
                    <p class="text-sm text-gray-500">${student.email || ''}</p>
                </div>
            </div>
        </div>
    `).join('');

    document.getElementById('selectedCountText').textContent = `${selectedStudentIds.size} students selected`;
};

document.getElementById('studentListContainer').addEventListener('change', (e) => {
    if (e.target.classList.contains('student-checkbox')) {
        const id = e.target.value;
        if (e.target.checked) {
            selectedStudentIds.add(id);
        } else {
            selectedStudentIds.delete(id);
        }
        document.getElementById('selectedCountText').textContent = `${selectedStudentIds.size} students selected`;
    }
});
document.getElementById('confirmAddStudents').addEventListener('click', async () => {
    const payload = {
        cohortId: parseInt(cohortId),
        studentIds: Array.from(selectedStudentIds)
    };

    const res = await fetch('/Admin/Cohort/AddStudentsToCohort', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    });

    if (res.ok) return location.reload();

    const data = await res.json();
    showValidationErrors(data.errors);
});

const fetchPrograms = async () => {
    const res = await fetch('/Admin/Cohort/GetPrograms');
    const data = await res.json();
    const select = document.getElementById("ProgramId");
    select.innerHTML = "<option value=''>Select Program</option>";
    data.forEach(p => {
        const option = document.createElement("option");
        option.value = p.id;
        option.textContent = p.name;
        select.appendChild(option);
    });
}

const openModal = () => {
    modal.classList.remove('hidden');
    document.body.style.overflow = 'hidden';
}
const closeModal = () => {
    modal.classList.add('hidden');
    document.body.style.overflow = 'auto';

    cohortId = null;

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

    const addStudentsBtn = target.closest(".open-add-students-modal-btn");
    if (addStudentsBtn) {
        const id = addStudentsBtn.dataset.id;
        await openStudentModal(id);
        return;
    }
    
    if (target.closest(".open-add-students-modal-btn")) openStudentModal();

    if (target.closest(".student-modal-overlay") || target.closest(".student-cancel-btn") || target.closest(".student-x-btn")) {
        closeStudentModal();
        return;
    }

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

form.addEventListener('submit', async e => {
    e.preventDefault();

    const url = cohortId
        ? `/Admin/Cohort/Update?id=${cohortId}`
        : '/Admin/Cohort/Create';

    const res = await fetch(url, {
        method: cohortId ? 'PUT' : 'POST',
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

document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape' && !modal.classList.contains('hidden')) closeModal();
    if (e.key === 'Escape' && !studentModal.classList.contains('hidden')) closeStudentModal();
});
const modal = document.getElementById('addExerciseModal');
const testCasesContainer = document.getElementById('testCasesContainer');
const testCaseTemplate = document.getElementById('testCaseTemplate');
let isEdit = false;

// Populate Programs Dropdown
const fetchPrograms = async () => {
    const res = await fetch('/Mentor/Exercise/GetPrograms');
    const data = await res.json();
    const select = document.getElementById("ProgramId");
    select.innerHTML = "<option value=''>Select Program</option>";
    data.forEach(p => {
        select.add(new Option(p.name, p.id));
    });
};

const openModal = () => {
    modal.classList.remove('hidden');
    document.body.style.overflow = 'hidden';

    // Ensure at least one test case exists when opening new
    if (testCasesContainer.children.length === 0) {
        addTestCaseRow();
    }
};

const closeModal = () => {
    modal.classList.add('hidden');
    document.body.style.overflow = 'auto';
    isEdit = false;

    document.getElementById("createExerciseForm").reset();
    document.getElementById("ExerciseId").value = '';
    testCasesContainer.innerHTML = '';
    document.querySelectorAll('[data-valmsg-for]').forEach(span => span.textContent = '');
    document.getElementById("modalTitle").innerText = "Create New Exercise";
};

// Test Case Management
const addTestCaseRow = (inputVal = '', outputVal = '') => {
    const clone = testCaseTemplate.content.cloneNode(true);
    clone.querySelector('.tc-input').value = inputVal;
    clone.querySelector('.tc-output').value = outputVal;
    testCasesContainer.appendChild(clone);
};

document.getElementById('addTestCaseBtn').addEventListener('click', () => addTestCaseRow());

testCasesContainer.addEventListener('click', (e) => {
    if (e.target.closest('.remove-test-case')) {
        e.target.closest('.test-case-row').remove();
    }
});

// Edit Button Click (Requires Fetching Data)
const handleEditClick = async (id) => {
    // Show loading state if desired
    try {
        const response = await fetch(`/Mentor/Exercise/GetForEdit/${id}`);
        if (!response.ok) throw new Error('Failed to fetch exercise data');

        const data = await response.json();
        const form = document.getElementById("createExerciseForm");

        document.getElementById("ExerciseId").value = id;
        form.querySelector('[name="Name"]').value = data.name;
        form.querySelector('[name="Description"]').value = data.description;
        form.querySelector('[name="ProgramId"]').value = data.programId;

        // Match enum value (0: Easy, 1: Medium, 2: Hard)
        form.querySelector('[name="Difficulty"]').value = data.difficulty;

        // Check language checkboxes
        document.querySelectorAll('input[name="Languages"]').forEach(cb => {
            cb.checked = data.languages.includes(parseInt(cb.value));
        });

        // Populate Test Cases
        testCasesContainer.innerHTML = '';
        if (data.testCases && data.testCases.length > 0) {
            data.testCases.forEach(tc => addTestCaseRow(tc.input, tc.output));
        } else {
            addTestCaseRow();
        }

        document.getElementById("modalTitle").innerText = "Update Exercise";
        isEdit = true;
        openModal();
    } catch (error) {
        console.error(error);
        alert("Error loading exercise details.");
    }
};

// Global Listeners
document.addEventListener("click", async (e) => {
    const target = e.target;

    if (target.closest(".modal-overlay") || target.closest(".cancel-btn")) {
        closeModal();
        return;
    }

    if (target.closest(".add-exercise")) {
        await fetchPrograms();
        openModal();
        return;
    }

    const editBtn = target.closest(".edit-btn");
    if (editBtn) {
        await fetchPrograms();
        await handleEditClick(editBtn.dataset.id);
    }
});

document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape' && !modal.classList.contains('hidden')) closeModal();
});

document.getElementById('createExerciseForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const form = e.target;

    // Construct DTO payload
    const payload = {
        name: form.querySelector('[name="Name"]').value,
        description: form.querySelector('[name="Description"]').value,
        programId: parseInt(form.querySelector('[name="ProgramId"]').value) || 0,
        difficulty: parseInt(form.querySelector('[name="Difficulty"]').value),
        languages: [],
        testCases: []
    };

    // Grab checked languages
    form.querySelectorAll('input[name="Languages"]:checked').forEach(cb => {
        payload.languages.push(parseInt(cb.value));
    });

    // Grab test cases
    form.querySelectorAll('.test-case-row').forEach(row => {
        const input = row.querySelector('.tc-input').value.trim();
        const output = row.querySelector('.tc-output').value.trim();
        if (input || output) {
            payload.testCases.push({ input, output });
        }
    });

    const exerciseId = document.getElementById("ExerciseId").value;
    const url = isEdit ? `/Mentor/Exercise/Update/${exerciseId}` : '/Mentor/Exercise/Create';
    const method = isEdit ? 'PUT' : 'POST';

    try {
        const response = await fetch(url, {
            method: method,
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify(payload)
        });

        if (response.ok) {
            window.location.reload();
        } else {
            const result = await response.json();
            showValidationErrors(result.errors);
        }
    } catch (error) {
        console.error("Submission failed", error);
    }
});

function showValidationErrors(errors) {
    document.querySelectorAll('[data-valmsg-for]').forEach(span => span.textContent = '');
    if (!errors) return;
    for (const key in errors) {
        const span = document.querySelector(`[data-valmsg-for="${key}"]`);
        if (span) span.textContent = errors[key][0];
    }
}
const modal = document.getElementById('addSessionModal');
const form = document.getElementById('createSessionForm');
let isEdit = false;
let sessionId = null;

const openModal = () => {
    modal.classList.remove('hidden');
    document.body.style.overflow = 'hidden';
};

const closeModal = () => {
    modal.classList.add('hidden');
    document.body.style.overflow = 'auto';

    isEdit = false;
    sessionId = null;

    form.reset();

    // Clear validation messages
    document.querySelectorAll('[data-valmsg-for]').forEach(span => span.textContent = '');

    // Reset multiple select (if you use a library like Select2, trigger change here)
    const exerciseSelect = form.querySelector('[name="ExercisesIds"]');
    if (exerciseSelect) {
        Array.from(exerciseSelect.options).forEach(opt => opt.selected = false);
    }

    document.getElementById('modalTitle').innerText = 'Create New Session';
};

const handleEditClick = async (btn) => {
    sessionId = btn.dataset.id;

    try {
        const response = await fetch(`/Mentor/Session/GetForEdit?id=${sessionId}`);
        const result = await response.json();

        if (result.success) {
            const data = result.data;

            // Populate standard inputs
            form.querySelector('[name="Name"]').value = data.name;
            form.querySelector('[name="CohortId"]').value = data.cohortId;
            form.querySelector('[name="StudentCountPerGroup"]').value = data.studentCountPerGroup;

            // Format dates for datetime-local inputs (slice off seconds/milliseconds if needed)
            form.querySelector('[name="StartDate"]').value = data.startDate.slice(0, 16);
            form.querySelector('[name="EndDate"]').value = data.endDate.slice(0, 16);

            // Populate multiple select for Exercises
            const exerciseSelect = form.querySelector('[name="ExercisesIds"]');
            Array.from(exerciseSelect.options).forEach(opt => {
                opt.selected = data.exercisesIds.includes(parseInt(opt.value, 10));
            });

            document.getElementById('modalTitle').innerText = 'Update Session';
            openModal();
        } else {
            alert(result.message || "Failed to load session details.");
        }
    } catch (error) {
        console.error("Error fetching session data:", error);
        alert("An error occurred while loading session details.");
    }
};

const handleDeleteClick = async (btn) => {
    const id = btn.dataset.id;
    const name = btn.dataset.name;

    if (!confirm(`Are you sure you want to delete the session "${name}"?`)) {
        return;
    }

    const token = form.querySelector('input[name="__RequestVerificationToken"]').value;

    try {
        const response = await fetch(`/Mentor/Session/Delete?id=${id}`, {
            method: 'DELETE',
            headers: {
                'RequestVerificationToken': token
            }
        });

        const result = await response.json();

        if (response.ok && result.success) {
            window.location.reload();
        } else {
            alert(result.message || "Failed to delete session.");
        }
    } catch (error) {
        console.error("Error deleting session:", error);
        alert("An unexpected error occurred.");
    }
};

// --- Event Listeners ---

document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape' && !modal.classList.contains('hidden')) {
        closeModal();
    }
});

document.addEventListener('click', (e) => {
    const target = e.target;

    // Handle Modal Close
    if (target.closest('.modal-overlay') || target.closest('.cancel-btn')) {
        closeModal();
        return;
    }

    // Handle Add Button
    if (target.closest('.add-session')) {
        openModal();
        return;
    }

    // Handle Edit Button
    const editBtn = target.closest('.edit-btn');
    if (editBtn) {
        handleEditClick(editBtn);
        return;
    }

    // Handle Delete Button
    const deleteBtn = target.closest('.trash-btn');
    if (deleteBtn) {
        handleDeleteClick(deleteBtn);
        return;
    }
});

function showValidationErrors(errors) {
    document.querySelectorAll('[data-valmsg-for]').forEach(span => span.textContent = '');

    // The backend might return errors as an array or object. 
    // If it's an array of strings (like in our controller), display the first one globally or map them.
    if (Array.isArray(errors)) {
        // Fallback alert if generic array is sent instead of key-value dictionary
        alert(errors.join('\n'));
    } else {
        for (const key in errors) {
            const span = document.querySelector(`[data-valmsg-for="${key}"]`);
            if (span) {
                span.textContent = errors[key][0];
            }
        }
    }
}

document.addEventListener('submit', async (e) => {
    if (e.target && e.target.id === 'createSessionForm') {
        e.preventDefault();

        const formData = new FormData(form);
        const token = formData.get('__RequestVerificationToken');

        // Build a JSON object manually since we have numbers and arrays
        const payload = {
            Name: formData.get('Name'),
            CohortId: parseInt(formData.get('CohortId'), 10),
            StudentCountPerGroup: parseInt(formData.get('StudentCountPerGroup'), 10),
            StartDate: formData.get('StartDate'),
            EndDate: formData.get('EndDate'),
            // FormData.getAll extracts all selected values from the multiple select
            ExercisesIds: formData.getAll('ExercisesIds').map(id => parseInt(id, 10))
        };

        const url = sessionId ? `/Mentor/Session/Edit?id=${sessionId}` : `/Mentor/Session/Create`;

        try {
            const response = await fetch(url, {
                method: sessionId ? 'PUT' : 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify(payload)
            });

            const result = await response.json();

            if (response.ok && result.success) {
                window.location.reload();
            } else {
                if (result.errors) {
                    showValidationErrors(result.errors);
                } else if (result.message) {
                    alert(result.message);
                }
            }
        } catch (error) {
            console.error("Submission error:", error);
            alert("An unexpected error occurred while saving the session");
        }
    }
});
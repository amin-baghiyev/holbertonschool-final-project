const modal = document.getElementById('addSessionModal');
const form = document.getElementById('createSessionForm');
let isEdit = false;
let sessionId = null;

let exerciseCache = {};
let selectedExerciseIds = new Set();
let debounceTimer;

const fetchExistingExercises = async (id) => {
    try {
        const res = await fetch(`/Mentor/Session/GetExercisesBySessionId?id=${id}`);
        const existingExercises = await res.json();
        
        if (Array.isArray(existingExercises)) {
            existingExercises.forEach(e => {
                selectedExerciseIds.add(e.id);
                exerciseCache[e.id] = e;
            });
        }
        return existingExercises;
    } catch (err) {
        console.error("Failed to fetch existing exercises:", err);
        return [];
    }
};

const handleExerciseSearch = async () => {
    const searchTerm = document.getElementById('exerciseSearchInput').value.trim();
    const diffFilter = document.getElementById('exerciseDifficultyFilter').value;
    const progFilter = document.getElementById('exerciseProgramFilter').value;
    const langFilter = document.getElementById('exerciseLanguageFilter').value;

    if (!searchTerm && !diffFilter && !progFilter && !langFilter) {
        // If everything is empty, just render selected
        const selectedExercises = Object.values(exerciseCache).filter(e => selectedExerciseIds.has(e.id));
        renderExerciseList(selectedExercises);
        return;
    }

    const container = document.getElementById('exerciseListContainer');
    container.innerHTML = `<div class="flex justify-center py-8"><div class="animate-spin rounded-full h-8 w-8 border-b-2 border-brand-red"></div></div>`;
    
    try {
        let qs = [];
        if (searchTerm) qs.push(`q=${encodeURIComponent(searchTerm)}`);
        if (diffFilter) qs.push(`difficulty=${encodeURIComponent(diffFilter)}`);
        if (progFilter) qs.push(`programId=${encodeURIComponent(progFilter)}`);
        if (langFilter) qs.push(`language=${encodeURIComponent(langFilter)}`);
        
        const url = `/Mentor/Session/SearchExercises?${qs.join('&')}`;
        const res = await fetch(url);
        const exercises = await res.json();
        
        exercises.forEach(e => exerciseCache[e.id] = e);
        
        // We merge with selected exercise items so they aren't hidden un-intuitively
        const mergedMap = new Map();
        selectedExerciseIds.forEach(id => {
            if (exerciseCache[id]) mergedMap.set(id, exerciseCache[id]);
        });
        exercises.forEach(e => mergedMap.set(e.id, e));

        renderExerciseList(Array.from(mergedMap.values()));
    } catch (err) {
        container.innerHTML = `<div class="text-red-500 text-center py-4">Failed to search exercises.</div>`;
    }
};

function renderExerciseList(exercisesToRender = []) {
    const container = document.getElementById('exerciseListContainer');
    if (!container) return;

    container.innerHTML = '';
    
    const filtered = exercisesToRender.sort((a, b) => {
        const aSel = selectedExerciseIds.has(a.id) ? -1 : 1;
        const bSel = selectedExerciseIds.has(b.id) ? -1 : 1;
        if (aSel !== bSel) return aSel;
        return a.name.localeCompare(b.name);
    });

    filtered.forEach(e => {
        const isSelected = selectedExerciseIds.has(e.id);
        
        const div = document.createElement('div');
        div.className = `flex items-center justify-between p-3 rounded-lg border cursor-pointer transition-colors ${isSelected ? 'bg-red-50 border-brand-red' : 'bg-white border-gray-200 hover:border-gray-300'}`;
        
        div.addEventListener('click', (ev) => {
            if (ev.target.tagName !== 'INPUT') {
                toggleExercise(e.id);
            }
        });
        
        let eDiffName = e.difficulty;
        let eDiffColor = eDiffName === "Easy" ? "text-green-600 bg-green-100" : eDiffName === "Medium" ? "text-yellow-600 bg-yellow-100" : "text-red-600 bg-red-100";

        div.innerHTML = `
            <div class="flex items-center gap-3 pointer-events-none">
                <input type="checkbox" class="w-4 h-4 text-brand-red rounded focus:ring-brand-red pointer-events-auto cursor-pointer" ${isSelected ? 'checked' : ''} onchange="toggleExercise(${e.id})">
                <div class="flex flex-col">
                    <span class="font-medium text-gray-900 text-sm">${e.name}</span>
                    <span class="text-xs text-gray-500">${e.programName || 'Unknown'} • ${(e.languages || []).join(', ')}</span>
                </div>
            </div>
            <div>
                 <span class="text-[10px] font-bold px-2 py-1 rounded-full uppercase tracking-wider ${eDiffColor}">${eDiffName}</span>
            </div>
        `;
        container.appendChild(div);
    });

    if (filtered.length === 0) {
        container.innerHTML = `<div class="p-6 text-center text-sm text-gray-500 italic">No exercises found.</div>`;
    }

    document.getElementById('exerciseVisibleCount').textContent = `${filtered.length} showing`;
    document.getElementById('exerciseSelectionCount').textContent = `${selectedExerciseIds.size} selected`;
}

window.toggleExercise = function(id) {
    if (selectedExerciseIds.has(id)) {
        selectedExerciseIds.delete(id);
    } else {
        selectedExerciseIds.add(id);
    }
    handleExerciseSearch();
};

document.getElementById('exerciseSearchInput')?.addEventListener('input', () => {
    clearTimeout(debounceTimer);
    debounceTimer = setTimeout(handleExerciseSearch, 300);
});

['exerciseDifficultyFilter', 'exerciseProgramFilter', 'exerciseLanguageFilter'].forEach(id => {
    document.getElementById(id)?.addEventListener('change', handleExerciseSearch);
});


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

    const nameInput = form.querySelector('[name="Name"]');
    nameInput.removeAttribute("readonly");
    nameInput.classList.remove("bg-gray-100", "cursor-not-allowed", "text-gray-500");

    // Clear validation messages
    document.querySelectorAll('[data-valmsg-for]').forEach(span => span.textContent = '');

    selectedExerciseIds.clear();
    exerciseCache = {};

    ['exerciseSearchInput', 'exerciseDifficultyFilter', 'exerciseProgramFilter', 'exerciseLanguageFilter'].forEach(id => {
        const el = document.getElementById(id);
        if (el) el.value = "";
    });

    handleExerciseSearch(); // Load initial state

    document.getElementById('modalTitle').innerText = 'Create New Session';
};

const handleEditClick = async (btn) => {
    sessionId = btn.dataset.id;

    try {
        const response = await fetch(`/Mentor/Session/GetForEdit?id=${sessionId}`);
        const result = await response.json();

        if (result.data) {
            const data = result.data;

            // Populate standard inputs
            const nameInput = form.querySelector('[name="Name"]');
            nameInput.value = data.name;
            nameInput.setAttribute("readonly", "true");
            nameInput.classList.add("bg-gray-100", "cursor-not-allowed", "text-gray-500");

            form.querySelector('[name="CohortId"]').value = data.cohortId;
            form.querySelector('[name="StudentCountPerGroup"]').value = data.studentCountPerGroup;

            // Format dates for datetime-local inputs (slice off seconds/milliseconds if needed)
            form.querySelector('[name="StartDate"]').value = data.startDate.slice(0, 16);
            form.querySelector('[name="EndDate"]').value = data.endDate.slice(0, 16);

            selectedExerciseIds.clear();
            exerciseCache = {};
            
            // Note: the backend GetForEdit doesn't return exercise details, 
            // so we will query GetExercisesBySessionId directly to populate selected states accurately.
            await fetchExistingExercises(sessionId);
            await handleExerciseSearch();

            document.getElementById('modalTitle').innerText = 'Update Session';
            openModal();
        } else {
            alert(result.message || "Failed to load session details.");
        }
    } catch (error) {
        console.error("Error fetching session data:", error);
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
            // Map Set to Array from the custom UI
            ExercisesIds: Array.from(selectedExerciseIds)
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

            if (response.ok) {
                window.location.reload();
            } else {
                const result = await response.json();

                if (result.errors) {
                    showValidationErrors(result.errors);
                }
            }
        } catch (error) {
            console.error("Submission error:", error);
        }
    }
});
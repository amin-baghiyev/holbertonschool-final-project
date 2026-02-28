const confirmModal = document.getElementById('confirmModal');
const openConfirmModal = (id, type, itemName) => {

    const config = {
        delete: {
            url: `/Admin/Program/Delete/${id}`,
            title: "Delete Program",
            verb: "permanently delete",
            icon: "ph-trash text-red-600",
            bg: "bg-red-100",
            btn: "bg-red-600 hover:bg-red-700",
            btnText: "Yes, Delete"
        },
        disable: {
            url: `/Admin/Program/Deactivate/${id}`,
            title: "Deactivate Program",
            verb: "deactivate",
            icon: "ph-warning-circle text-orange-600",
            bg: "bg-orange-100",
            btn: "bg-orange-600 hover:bg-orange-700",
            btnText: "Yes, Deactivate"
        },
        enable: {
            url: `/Admin/Program/Activate/${id}`,
            title: "Activate Program",
            verb: "activate",
            icon: "ph-check-circle text-green-600",
            bg: "bg-green-100",
            btn: "bg-green-600 hover:bg-green-700",
            btnText: "Yes, Activate"
        }
    };

    const action = config[type];
    if (!action) return;

    confirmModal.dataset.actionType = type;
    confirmModal.dataset.actionUrl = action.url;

    document.getElementById('actionTitle').innerText = action.title;
    document.getElementById('actionVerb').innerText = action.verb;
    document.getElementById('actionProgramName').innerText = itemName;

    document.getElementById('actionIcon').className =
        `ph ${action.icon} text-3xl`;

    document.getElementById('actionIconContainer').className =
        `mx-auto flex items-center justify-center h-16 w-16 rounded-full ${action.bg} mb-4`;

    const confirmBtn = document.getElementById('actionConfirmBtn');
    confirmBtn.className =
        `px-6 py-2.5 text-white rounded-lg font-medium shadow-sm transition-colors ${action.btn}`;
    confirmBtn.innerText = action.btnText;

    confirmModal.classList.remove('hidden');
    document.body.style.overflow = 'hidden';
};

const closeConfirmModal = () => {
    confirmModal.classList.add('hidden');
    document.body.style.overflow = 'auto';
}

document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape' && !confirmModal.classList.contains('hidden')) {
        closeConfirmModal();
    }
});

document.addEventListener('click', (e) => {

    const btn =
        e.target.closest(".trash-btn") ||
        e.target.closest(".power-btn");

    if (btn) {
        const id = btn.dataset.id;
        const type = btn.dataset.type;
        const name = btn.dataset.name;

        openConfirmModal(id, type, name);
        return;
    }

    if (
        e.target.closest(".confirm-modal-overlay") ||
        e.target.closest(".confirm-modal-cancel-btn")
    ) {
        closeConfirmModal();
    }
});

document
    .getElementById('actionProgramForm')
    .addEventListener('submit', async function (e) {

        e.preventDefault();

        const formData = new FormData(this);
        const type = confirmModal.dataset.actionType;
        const method =
            type === 'delete' ? 'DELETE' : 'PATCH';
        const url = confirmModal.dataset.actionUrl;
        
        try {
            const response = await fetch(url, {
                method: method,
                body: formData,
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            });

            if (response.ok) {
                window.location.reload();
            } else {
                alert("Operation failed. Please try again.");
            }

        } catch (error) {
            console.error("Error:", error);
        }
    });
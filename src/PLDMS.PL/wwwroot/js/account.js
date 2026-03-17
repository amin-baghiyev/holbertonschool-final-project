document.querySelector("form[id=login]").addEventListener("submit", async e => {
    e.preventDefault();

    clearErrors();

    const formData = new FormData(e.target);
    const data = Object.fromEntries(formData.entries());

    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    const response = await fetch("/Account/Index", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "RequestVerificationToken": token
        },
        body: JSON.stringify(data)
    });

    const result = await response.json();

    if (!response.ok) {
        if (result.errors) {
            showErrors(result.errors);
        }

        return;
    }
    
    window.location.href = result.redirectUrl;
})

function clearErrors() {
    document.querySelectorAll("[data-valmsg-for]").forEach(el => {
        el.textContent = "";
    });
}

function showErrors(errors) {
    for (const key in errors) {
        const messages = errors[key];

        const span = document.querySelector(`[data-valmsg-for="${key}"]`);

        if (span) {
            span.textContent = messages[0];
        }
    }
}
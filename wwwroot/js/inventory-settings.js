document.addEventListener("click", function (e) {
    if (!e.target || e.target.id !== "generate-token-btn") return;

    const idInput = document.getElementById("inv-id");
    if (!idInput) return;

    const id = idInput.value;

    const tokenInput = document.querySelector("input[name='__RequestVerificationToken']");
    const antiForgery = tokenInput ? tokenInput.value : null;

    fetch(`/Inventory/GenerateApiTokenAjax/${id}`, {
        method: "POST",
        headers: {
            "RequestVerificationToken": antiForgery
        }
    })
        .then(r => r.json())
        .then(data => {
            const block = document.getElementById("api-token-block");
            if (!block) return;

            block.innerHTML = `
                <div class="alert alert-info mt-2">
                    <strong>Your API Token:</strong>
                    <code>${data.token}</code>
                </div>
            `;
        })
        .catch(err => console.error(err));
});

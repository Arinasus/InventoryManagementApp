document.addEventListener("click", function (e) {
    if (!e.target || e.target.id !== "generate-token-btn") return;

    const id = document.getElementById("inv-id").value;
    const antiForgery = document.querySelector("input[name='__RequestVerificationToken']").value;

    fetch(`/Inventory/GenerateApiTokenAjax/${id}`, {
        method: "POST",
        headers: {
            "RequestVerificationToken": antiForgery
        }
    })
        .then(r => r.json())
        .then(data => {
            const block = document.getElementById("api-token-block");

            block.innerHTML = `
                <div class="alert alert-info mt-2">
                    <strong>Your API Token:</strong>
                    <code>${data.token}</code>
                </div>
                <a class="btn btn-outline-secondary mt-2"
                   href="/api/inventory/${data.token}"
                   target="_blank">
                    Open Aggregated Data
                </a>
            `;
        })
        .catch(err => console.error(err));
});

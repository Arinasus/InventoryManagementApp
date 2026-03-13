console.log("autosave loaded");

let lastSavedState = null;

const input = document.getElementById('inv-tags');
const tagify = new Tagify(input, {
    whitelist: [],
    dropdown: {
        enabled: 1,
        maxItems: 10,
        position: "text",
        closeOnSelect: false
    }
});


tagify.on('input', function (e) {
    fetch('/Tags/SearchTags?query=' + e.detail.value)
        .then(async r => r.ok ? r.json() : [])
        .then(list => {
            tagify.settings.whitelist = list;
            tagify.dropdown.show(e.detail.value);
        });
});

function getCurrentState() {
    return {
        id: document.getElementById("inv-id").value,
        title: document.getElementById("inv-title").value,
        description: document.getElementById("inv-desc").value,
        category: document.getElementById("inv-category").value,
        isPublic: document.getElementById("inv-public").checked,
        tags: tagify.value.map(t => t.value),
        rowVersion: document.getElementById("inv-rowversion").value        ,
    };
}

function statesEqual(a, b) {
    return JSON.stringify(a) === JSON.stringify(b);
}

function showStatus(text) {
    document.getElementById("autosave-status").innerText = text;
}

setInterval(() => {
    const current = getCurrentState();

    if (lastSavedState && statesEqual(current, lastSavedState))
        return;

    fetch('/Inventory/AutoSave', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(current)
    })
        .then(async r => {
            try { return await r.json(); }
            catch { return { success: false }; }
        })

        .then(res => {
            if (res.success) {
                lastSavedState = current;
                document.getElementById("inv-rowversion").value = res.rowVersion;
                showStatus("Сохранено: " + new Date().toLocaleTimeString());
            } else {
                showStatus("Конфликт изменений! Обновите страницу.");
            }
        });

}, 8000);

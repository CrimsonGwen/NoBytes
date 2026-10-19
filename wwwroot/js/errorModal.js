document.addEventListener("DOMContentLoaded", function () {
    const modal = document.getElementById("errorModal");
    const closeBtn = document.getElementById("closeModalBtn");

    if (modal) {
        // Lees het data-attribuut uit om te kijken of we de modal moeten openen
        const shouldShow = modal.getAttribute("data-show-errors") === "True";
        
        if (shouldShow) {
            modal.showModal();
        }

        // Koppel de sluit-knop
        if (closeBtn) {
            closeBtn.addEventListener("click", function () {
                modal.close();
            });
        }
    }
});
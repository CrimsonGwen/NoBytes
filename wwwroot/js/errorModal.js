document.addEventListener("DOMContentLoaded", function () {
    const modal = document.getElementById("errorModal");
    const closeBtn = document.getElementById("closeModalBtn");

    if (modal) {
        const shouldShow = modal.getAttribute("data-show-errors") === "True";
        
        if (shouldShow) {
            modal.showModal();
        }

        if (closeBtn) {
            closeBtn.addEventListener("click", function () {
                modal.close();
            });
        }
    }
});
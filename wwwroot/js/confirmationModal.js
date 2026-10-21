document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("registerForm");
    const confirmModal = document.getElementById("confirmModal");
    const confirmBtn = document.getElementById("confirmSubmitBtn");
    const cancelBtn = document.getElementById("cancelModalBtn");

    if (form && confirmModal) {
        let isConfirmed = false;

        form.addEventListener("submit", function (e) {
            if (!isConfirmed) {
                e.preventDefault();
                document.getElementById("confVoornaam").innerText = document.querySelector("[name='Voornaam']").value;
                document.getElementById("confAchternaam").innerText = document.querySelector("[name='Achternaam']").value;
                document.getElementById("confGebruikersnaam").innerText = document.querySelector("[name='Gebruikersnaam']").value;
                document.getElementById("confEmail").innerText = document.querySelector("[name='Email']").value;
                document.getElementById("confAdres").innerText = document.querySelector("[name='Adres']").value;
                document.getElementById("confHuisnummer").innerText = document.querySelector("[name='Huisnummer']").value;
                document.getElementById("confPostcode").innerText = document.querySelector("[name='Postcode']").value;
                document.getElementById("confGemeente").innerText = document.querySelector("[name='Gemeente']").value;

                confirmModal.showModal();
            }
        });

        confirmBtn.addEventListener("click", function () {
            isConfirmed = true;
            confirmModal.close();
            form.submit();
        });

        cancelBtn.addEventListener("click", function () {
            confirmModal.close();
        });
    }
});
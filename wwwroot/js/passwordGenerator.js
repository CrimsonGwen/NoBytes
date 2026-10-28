
document.addEventListener("DOMContentLoaded", function () {
    const generateBtn = document.getElementById("generateBtn");
    const passwordInput = document.getElementById("wachtwoordInput");
    const toggleBtn = document.getElementById("togglePasswordBtn");

    if (!generateBtn || !passwordInput) return;

    const uppercase = "ABCDEFGHJKLMNPQRSTUVWXYZ";
    const lowercase = "abcdefghijkmnopqrstuvwxyz";
    const numbers = "23456789";
    const symbols = "!@#$%&*?";

    const allCharacters = uppercase + lowercase + numbers + symbols;

    function randomIndex(max) {
        const values = new Uint32Array(1);
        const limit = Math.floor(4294967296 / max) * max;

        do {
            crypto.getRandomValues(values);
        } while (values[0] >= limit);

        return values[0] % max;
    }

    function randomChar(characters) {
        return characters[randomIndex(characters.length)];
    }

    generateBtn.addEventListener("click", function () {
        const password = [
            randomChar(uppercase),
            randomChar(lowercase),
            randomChar(numbers),
            randomChar(symbols)
        ];

        while (password.length < 16) {
            password.push(randomChar(allCharacters));
        }

        for (let i = password.length - 1; i > 0; i--) {
            const j = randomIndex(i + 1);

            [password[i], password[j]] =
                [password[j], password[i]];
        }

        passwordInput.value = password.join("");

        passwordInput.dispatchEvent(
            new Event("input", { bubbles: true })
        );
    });

    if (toggleBtn) {
        toggleBtn.addEventListener("click", function () {
            const isHidden = passwordInput.type === "password";

            passwordInput.type = isHidden ? "text" : "password";

            toggleBtn.textContent = isHidden
                ? "Verberg wachtwoord"
                : "Toon wachtwoord";
        });
    }
});

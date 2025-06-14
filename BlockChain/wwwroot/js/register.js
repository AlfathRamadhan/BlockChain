// register.js

// Fungsi validasi email dengan domain khusus @gmail.com
function validateEmail(email) {
    const regex = /^[a-zA-Z0-9._-]+@gmail\.com$/;  // Email harus berakhiran @gmail.com
    return regex.test(email);
}

function validatePassword(password) {
    const regex = /^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*(),.?":{}|<>]).{8,}$/;
    return regex.test(password);
}

document.addEventListener('DOMContentLoaded', function () {
    const form = document.querySelector('form');
    const passwordInput = document.querySelector('input[name="KataSandi"]');

    form.addEventListener('submit', function (event) {
        // Validasi email
        const emailInput = document.getElementById('email');
        if (!validateEmail(emailInput.value)) {
            event.preventDefault();
            document.getElementById('emailError').textContent = 'Hanya email dengan domain @gmail.com yang diterima.';
            return;
        }

        // Validasi password
        if (!validatePassword(passwordInput.value)) {
            event.preventDefault();
            alert("Password harus minimal 8 karakter dan mengandung huruf kapital, angka, dan karakter spesial (misal: !@#?).");
        }
    });
});


// Event listener ketika dokumen sudah siap
document.addEventListener('DOMContentLoaded', function () {
    const form = document.querySelector('form');
    const emailInput = document.getElementById('email');
    const emailError = document.getElementById('emailError');

    // Validasi saat mengetik (realtime feedback)
    emailInput.addEventListener('input', function () {
        if (!validateEmail(emailInput.value) && emailInput.value !== "") {
            emailError.textContent = 'Hanya email dengan domain @gmail.com yang diterima.';
        } else {
            emailError.textContent = '';
        }
    });

    // Validasi saat form disubmit
    form.addEventListener('submit', function (event) {
        if (!validateEmail(emailInput.value)) {
            event.preventDefault(); // Mencegah form terkirim jika tidak valid
            emailError.textContent = 'Hanya email dengan domain @gmail.com yang diterima.';
        }
    });

    $(document).ready(function () {
        $("input[name='NoHp']").on("input", function () {
            this.value = this.value.replace(/[^0-9]/g, '');
        });
    });

});

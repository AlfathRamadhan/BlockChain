// register.js

// Fungsi validasi email dengan domain khusus @gmail.com
function validateEmail(email) {
    const regex = /^[a-zA-Z0-9._-]+@gmail\.com$/;  // Email harus berakhiran @gmail.com
    return regex.test(email);
}

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

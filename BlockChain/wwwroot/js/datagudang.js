function openModal() {
    console.log("Modal dibuka");
    document.getElementById("tambahModal").style.display = "block";
    const kodeInput = document.getElementById("kodeProdukInput");
    if (kodeInput) {
        kodeInput.value = generateKodeProduk();
    }
}

function closeModal() {
    document.getElementById("tambahModal").style.display = "none";
}

window.onclick = function (event) {
    const modal = document.getElementById("tambahModal");
    if (event.target === modal) {
        modal.style.display = "none";
    }

    if (!event.target.matches('.date-filter')) {
        const dropdown = document.getElementById("dropdownOptions");
        if (dropdown && dropdown.style.display === "block") {
            dropdown.style.display = "none";
        }
    }
}

function toggleDropdown() {
    const dropdown = document.getElementById("dropdownOptions");
    dropdown.style.display = dropdown.style.display === "block" ? "none" : "block";
}

// ================= PENCARIAN =====================
const searchInput = document.getElementById("searchInput");
const tableBody = document.getElementById("produkTableBody");

searchInput.addEventListener("input", function () {
    const term = this.value;
    fetch(`/DataGudang/SearchJson?term=${encodeURIComponent(term)}`)
        .then(response => response.json())
        .then(data => {
            tableBody.innerHTML = "";
            data.forEach((item, index) => {
                const row = `<tr>
                            <td>${index + 1}</td>
                            <td>${item.kodeProduk}</td>
                            <td>${item.nama}</td>
                            <td>${item.stok}</td>
                            <td>${item.tanggalMasuk}</td>
                            <td>${item.tanggalExpired}</td>
                            <td><a href="#"><i class="fas fa-external-link-alt"></i></a></td>
                        </tr>`;
                tableBody.insertAdjacentHTML("beforeend", row);
            });
        });
});

// ================= KODE PRODUK OTOMATIS =====================
function generateKodeProduk() {
    const timestamp = Date.now().toString().slice(-6);
    const random = Math.floor(100 + Math.random() * 900);
    return `PRD-${timestamp}${random}`;
}

// ================= DELETE PRODUK =====================
let deleteId = 0;

function confirmDelete(id) {
    deleteId = id;
    document.getElementById("confirmDeleteModal").style.display = "block";
}

function deleteConfirmed() {
    console.log('Menghapus produk dengan ID: ' + deleteId);
    fetch(`/DataGudang/Hapus?id=${deleteId}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ id: deleteId })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                const row = document.getElementById(`row-${deleteId}`);
                if (row) row.remove();
                closeDeleteConfirmModal();

                // Refresh pagination setelah penghapusan
                showPage(currentPage);
            } else {
                alert("Gagal menghapus produk: " + data.message);
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert("Terjadi kesalahan saat menghapus produk");
        });
}

function closeDeleteConfirmModal() {
    document.getElementById("confirmDeleteModal").style.display = "none";
}

// ================= KONFIRMASI PRODUK =====================
function konfirmasiProduk() {
    const selectedIds = [];
    document.querySelectorAll('.produk-checkbox:checked').forEach(cb => {
        selectedIds.push(cb.value);
    });

    if (selectedIds.length === 0) {
        alert("Pilih setidaknya satu produk.");
        return;
    }

    const query = selectedIds.map(id => `ids=${id}`).join('&');
    window.location.href = `/DataGudang/KonfirmasiProduk?${query}`;
}

document.getElementById("checkAll").addEventListener("change", function () {
    const isChecked = this.checked;
    document.querySelectorAll('.produk-checkbox').forEach(cb => cb.checked = isChecked);
});

// ================= PAGINASI TABEL =====================
let currentPage = 1;
const rowsPerPage = 5;

function showPage(page) {
    const allRows = Array.from(document.querySelectorAll("#produkTableBody tr"));
    const totalPages = Math.ceil(allRows.length / rowsPerPage);

    // Jika tidak ada data, sembunyikan semua paginasi dan keluar
    if (allRows.length === 0) {
        document.getElementById('prevPage').style.display = 'none';
        document.getElementById('nextPage').style.display = 'none';
        document.getElementById('pageNumbers').style.display = 'none';
        return;
    }

    // Sesuaikan halaman agar tidak lebih besar dari total halaman
    if (page > totalPages) {
        page = totalPages;
    }

    allRows.forEach((row, index) => {
        row.style.display = (index >= (page - 1) * rowsPerPage && index < page * rowsPerPage) ? '' : 'none';
    });

    currentPage = page;
    updatePageNumbers(totalPages);
}

function updatePageNumbers(totalPages) {
    const pageNumbersContainer = document.getElementById('pageNumbers');
    const prevBtn = document.getElementById('prevPage');
    const nextBtn = document.getElementById('nextPage');

    pageNumbersContainer.innerHTML = '';

    // Selalu tampilkan tombol prev/next
    prevBtn.style.display = '';
    nextBtn.style.display = '';
    pageNumbersContainer.style.display = '';

    // Disable tombol jika di halaman pertama/terakhir
    prevBtn.disabled = currentPage === 1;
    nextBtn.disabled = currentPage === totalPages;

    for (let i = 1; i <= totalPages; i++) {
        const btn = document.createElement('button');
        btn.textContent = i;
        btn.className = 'pagination-btn';
        if (i === currentPage) {
            btn.classList.add('active');
        }
        btn.onclick = () => showPage(i);
        pageNumbersContainer.appendChild(btn);
    }

    // Jika totalPages == 0, tetap nonaktifkan tombol
    if (totalPages === 0) {
        prevBtn.disabled = true;
        nextBtn.disabled = true;
        pageNumbersContainer.style.display = 'none';
    }
}


document.getElementById('prevPage').onclick = () => {
    if (currentPage > 1) showPage(currentPage - 1);
};

document.getElementById('nextPage').onclick = () => {
    const allRows = Array.from(document.querySelectorAll("#produkTableBody tr"));
    const totalPages = Math.ceil(allRows.length / rowsPerPage);
    if (currentPage < totalPages) showPage(currentPage + 1);
};

// Inisialisasi pertama
showPage(1);

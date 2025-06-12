let currentNotifId = null;

function filterNotifications(event, kategori) {
    const items = document.querySelectorAll('.notification-item');
    const tabs = document.querySelectorAll('.notification-tabs .tab');

    tabs.forEach(tab => tab.classList.remove('active'));
    event.target.classList.add('active');

    items.forEach(item => {
        if (kategori === 'semua') {
            item.style.display = 'flex';
        } else {
            item.style.display = item.classList.contains(kategori) ? 'flex' : 'none';
        }
    });
}

function openModal(button) {
    const id = button.getAttribute("data-id");
    currentNotifId = button.getAttribute("data-id");

    const jsonString = button.getAttribute("data-detail");

    let parsedData;
    try {
        parsedData = JSON.parse(jsonString);
    } catch (e) {
        console.error("Gagal parsing data:", e);
        return;
    }

    const tbody = document.getElementById("produkTableBody");
    tbody.innerHTML = "";
    let subtotal = 0;

    for (let i = 0; i < parsedData.Produk.length; i++) {
        const nama = parsedData.Produk[i];
        const jumlah = parsedData.Stok[i] ?? 0;
        const satuan = parsedData.Satuan[i] ?? '-';
        const expired = parsedData.TanggalExpired[i] ?? '-';
        const harga = parsedData.HargaSatuan[i] ?? 0;
        const total = parsedData.TotalHarga[i] ?? (jumlah * harga);

        subtotal += total;

        const row = document.createElement("tr");
        row.innerHTML = `
                    <td>${nama}</td>
                    <td>${jumlah}</td>
                    <td>${satuan}</td>
                    <td>${expired}</td>
                    <td>Rp ${harga.toLocaleString('id-ID')}</td>
                    <td>Rp ${total.toLocaleString('id-ID')}</td>
                `;
        tbody.appendChild(row);
    }

    document.getElementById("subtotalHarga").innerHTML = `<strong>Rp ${subtotal.toLocaleString('id-ID')}</strong>`;

    const modal = document.getElementById("detailModal");
    modal.classList.add("show");
    modal.style.display = "block";
}

function closeModal() {
    const modal = document.getElementById("detailModal");
    modal.classList.remove("show");
    modal.style.display = "none";
}

window.onclick = function (event) {
    const modal = document.getElementById("detailModal");
    if (event.target == modal) {
        closeModal();
    }
}

function deleteNotification(id) {
    if (!confirm("Yakin ingin menghapus notifikasi ini?")) return;

    fetch('/NotifikasiOwner/DeleteNotification/' + id, {
        method: 'DELETE',
        headers: {
            'RequestVerificationToken': getAntiForgeryToken()
        }
    })
        .then(response => {
            if (response.ok) {
                const notifDiv = document.querySelector(`.notification-item[data-id='${id}']`);
                if (notifDiv) notifDiv.remove();
                Swal.fire({
                    icon: 'success',
                    title: 'Sukses',
                    text: 'Notifikasi berhasil dihapus.',
                    timer: 2000,
                    showConfirmButton: false
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Gagal menghapus notifikasi.',
                    timer: 2000,
                    showConfirmButton: false
                });
            }
        })
        .catch(error => {
            console.error("Error:", error);
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Terjadi kesalahan saat menghapus notifikasi.',
                timer: 2000,
                showConfirmButton: false
            });
        });
}

function konfirmasiPembelian() {
    if (!currentNotifId) return Swal.fire({
        icon: 'error',
        title: 'Error',
        text: 'Notifikasi tidak valid.',
        timer: 2000,
        showConfirmButton: false
    });

    // var token = $('input[name="__RequestVerificationToken"]').val();
    console.log(currentNotifId);
    $.ajax({
        url: '/NotifikasiOwner/KonfirmasiPembelian',
        type: 'POST',
        data: { id: currentNotifId },
        datatype: "json",
        // headers: {
        //     'RequestVerificationToken': token
        // },
        success: function (data) {
            closeModal();
            removeNotificationById(currentNotifId);
            Swal.fire({
                icon: 'success',
                title: 'Berhasil',
                text: 'Pembelian berhasil dikonfirmasi.',
                timer: 2000,
                showConfirmButton: false
            });
        },
        error: function (textStatus, errorThrown) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Gagal mengonfirmasi pembelian.',
                timer: 2000,
                showConfirmButton: false
            });
        }
    });
}

function tolakPembelian() {
    if (!currentNotifId) return Swal.fire({
        icon: 'error',
        title: 'Error',
        text: 'Notifikasi tidak valid.',
        timer: 2000,
        showConfirmButton: false
    });

    fetch(`/Notifikasi/TolakPembelian/${currentNotifId}`, {
        method: 'POST',
        headers: {
            'RequestVerificationToken': getAntiForgeryToken()
        }
    })
        .then(response => {
            if (response.ok) {
                closeModal();
                removeNotificationById(currentNotifId);
                Swal.fire({
                    icon: 'success',
                    title: 'Berhasil',
                    text: 'Pembelian berhasil ditolak.',
                    timer: 2000,
                    showConfirmButton: false
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Gagal menolak pembelian.',
                    timer: 2000,
                    showConfirmButton: false
                });
            }
        })
        .catch(() => {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Terjadi kesalahan saat menolak pembelian.',
                timer: 2000,
                showConfirmButton: false
            });
        });
}

function removeNotificationById(id) {
    const notifDiv = document.querySelector(`.notification-item[data-id='${id}']`);
    if (notifDiv) notifDiv.remove();
}

function showGudangProdukModalFromAttr(button) {
    const jsonData = button.getAttribute('data-json');
    if (!jsonData) return;

    try {
        const produkList = JSON.parse(jsonData);
        const tbody = document.getElementById('gudangProdukDetailBody');
        tbody.innerHTML = ''; // Kosongkan dulu

        produkList.forEach(p => {
            const row = `
                <tr>
                    <td>${p.Nama}</td>
                    <td>${p.Jumlah}</td>
                    <td>${p.Satuan}</td>
                    <td>${p.Expired}</td>
                </tr>
            `;
            tbody.innerHTML += row;
        });

        document.getElementById('gudangProdukModal').style.display = 'block';
    } catch (e) {
        console.error("Gagal parse JSON detail:", e);
        alert("Gagal menampilkan detail produk.");
    }
}

function closeGudangProdukModal() {
    document.getElementById("gudangProdukModal").style.display = "none";
}
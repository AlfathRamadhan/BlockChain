function openInvoice(id) {
    fetch(`/Pesanan/GetInvoiceById?id=${id}`)
        .then(res => res.json())
        .then(data => {
            let produkRows = '';
            if (data.Produk && data.Produk.length > 0) {
                data.Produk.forEach((item, index) => {
                    produkRows += `
                                <tr>
                                    <td style="padding:10px; text-align:center;">${index + 1}</td>
                                    <td style="padding:10px;">${item.NamaProduk ?? '-'}</td>
                                    <td style="padding:10px;">${item.Satuan ?? '-'}</td>
                                    <td style="padding:10px; text-align:center;">${item.JumlahUnit ?? 1}</td>
                                    <td style="padding:10px;">${formatRupiah(item.HargaSatuan)}</td>
                                    <td style="padding:10px;">${formatRupiah(item.HargaSatuan * item.JumlahUnit)}</td>
                                </tr>
                            `;
                });
            } else {
                produkRows = `<tr><td colspan="6" style="text-align:center;">Tidak ada produk</td></tr>`;
            }

            let subtotal = 0;
            if (data.Produk && data.Produk.length > 0) {
                subtotal = data.Produk.reduce((sum, item) => sum + (item.HargaSatuan * item.JumlahUnit), 0);
            } else {
                subtotal = data.Jumlah; // fallback
            }

            const html = `
                        <h2><i class="fa fa-file-invoice"></i> Detail Pesanan</h2>
                        <p>Dibuat pada ${formatTanggalIndonesia(data.Tanggal)}</p>
                        <div class="invoice-card-container">
                            <div class="invoice-card">
                                <h4>Informasi Pelanggan</h4>
                                <p>${data.Supplier}</p>
                                <p>Jl. Contoh Alamat, Bengkong</p>
                                <p>08123456789</p>
                            </div>
                            <div class="invoice-card">
                                <h4>Informasi Pesanan</h4>
                                <p>Pembayaran: ${data.MetodePembayaran}</p>
                                <p>No. Pembayaran: ${data.NomorPembayaran}</p>
                                <p>Status: <span style="color:${getStatusColor(data.Status)}; font-weight:bold;">${data.Status}</span></p>
                            </div>
                        </div>
                        <h3 style="color:white; font-weight:bold; margin-bottom: 10px;">Item Pesanan</h3>
                        <table style="width:100%; max-width:700px; margin:0 auto; border-collapse:collapse; background-color:#EDE8D5; color:#333; border-radius:5px; margin-bottom:20px;">
                            <thead>
                                <tr>
                                    <th style="padding:10px; width:40px; text-align:center;">No</th>
                                    <th style="padding:10px;">Produk</th>
                                    <th style="padding:10px;">Satuan</th>
                                    <th style="padding:10px; width:40px; text-align:center;">Jumlah</th>
                                    <th style="padding:10px;">Harga</th>
                                    <th style="padding:10px;">Total</th>
                                </tr>
                            </thead>
                            <tbody>
                                ${produkRows}
                                <tr>
                                    <td></td><td></td><td></td><td></td>
                                    <td style="padding:10px; text-align:center;" class="subtotal-label">Sub Total</td>
                                    <td style="padding:10px; text-align:center; font-weight:bold;" class="subtotal-value">${formatRupiah(subtotal)}</td>
                                </tr>
                            </tbody>
                        </table>
                        <div style="display:flex; justify-content:flex-end; gap:10px; margin-top:16px;">
                            <button class="btn-close" onclick="closeInvoiceModal()"><i class="fa fa-times"></i> Tutup</button>
                            <button class="btn-print" onclick="printPDF(${data.ID})"><i class="fa fa-file-pdf"></i> Cetak PDF</button>
                        </div>
                    `;

            document.getElementById("invoiceContent").innerHTML = html;
            document.getElementById("invoiceModal").style.display = "flex";
        })
        .catch(err => alert("Gagal memuat data invoice: " + err));
}

function closeInvoiceModal() {
    document.getElementById("invoiceModal").style.display = "none";
}

function printPDF(id) {
    window.open(`/Pesanan/PrintPesananPDF?id=${id}`, '_blank');
}

function formatTanggalIndonesia(tanggalStr) {
    const bulan = ["Januari", "Februari", "Maret", "April", "Mei", "Juni",
        "Juli", "Agustus", "September", "Oktober", "November", "Desember"];
    const tanggal = new Date(tanggalStr);
    return `${tanggal.getDate()} ${bulan[tanggal.getMonth()]} ${tanggal.getFullYear()}`;
}

function formatRupiah(angka) {
    if (!angka) return "Rp 0";
    return "Rp " + angka.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
}

function getStatusColor(status) {
    if (!status) return "#333";
    switch (status.toLowerCase()) {
        case "sudah dibayar": return "#2ecc71"; // hijau
        case "belum dibayar": return "#f1c40f"; // kuning
        case "gagal": return "#e74c3c"; // merah
        default: return "#333";
    }
}

function lihatBuktiPembayaran(id) {
    // TODO: Implementasi untuk menampilkan bukti pembayaran
    alert("Fitur melihat bukti pembayaran belum tersedia.");
}

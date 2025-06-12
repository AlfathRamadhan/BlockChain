let chart;

function loadChart(tahun, bulan) {
    fetch(`/Owner/DataPemakaianProduk?tahun=${tahun}&bulan=${bulan}`)
        .then(res => res.json())
        .then(data => {
            const labels = data.map(d => d.namaProduk); // sesuaikan nama properti
            const values = data.map(d => d.total);      // sesuaikan nama properti

            if (chart) {
                chart.destroy();
            }

            const ctx = document.getElementById("chartPemakaian").getContext("2d");
            chart = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'Jumlah Pemakaian',
                        data: values,
                        backgroundColor: '#5c3b24'
                    }]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: { display: false },
                        title: {
                            display: true,
                            text: 'Pemakaian Produk Bulanan'
                        }
                    }
                }
            });
        });
}

// default tahun dan bulan saat load
document.addEventListener("DOMContentLoaded", () => {
    const tahun = parseInt(document.getElementById("tahunTampil").innerText);
    const bulanIndex = parseInt(document.getElementById("bulanTampil").dataset.bulanIndex || new Date().getMonth());
    loadChart(tahun, bulanIndex + 1);
});

// navigasi bulan
document.querySelectorAll("#bulanSelector button").forEach((btn, idx) => {
    btn.dataset.bulanIndex = idx;
    btn.addEventListener("click", () => {
        document.querySelectorAll("#bulanSelector button").forEach(b => b.classList.remove("active"));
        btn.classList.add("active");

        const tahun = parseInt(document.getElementById("tahunTampil").innerText);
        loadChart(tahun, idx + 1);
    });
});

// navigasi tahun
document.getElementById("prevTahun").addEventListener("click", () => {
    const tahunSpan = document.getElementById("tahunTampil");
    tahunSpan.innerText = parseInt(tahunSpan.innerText) - 1;
    refreshChart();
});

document.getElementById("nextTahun").addEventListener("click", () => {
    const tahunSpan = document.getElementById("tahunTampil");
    tahunSpan.innerText = parseInt(tahunSpan.innerText) + 1;
    refreshChart();
});

function refreshChart() {
    const tahun = parseInt(document.getElementById("tahunTampil").innerText);
    const bulan = parseInt(document.getElementById("bulanTampil").dataset.bulanIndex) + 1;
    loadChart(tahun, bulan);
}

const bulanNames = [
    "Januari", "Februari", "Maret", "April",
    "Mei", "Juni", "Juli", "Agustus",
    "September", "Oktober", "November", "Desember"
];

document.getElementById("prevBulan").addEventListener("click", () => {
    const bulanSpan = document.getElementById("bulanTampil");
    let index = parseInt(bulanSpan.dataset.bulanIndex);
    index = (index - 1 + 12) % 12;
    bulanSpan.dataset.bulanIndex = index;
    bulanSpan.innerText = bulanNames[index];
    refreshChart();
});

document.getElementById("nextBulan").addEventListener("click", () => {
    const bulanSpan = document.getElementById("bulanTampil");
    let index = parseInt(bulanSpan.dataset.bulanIndex);
    index = (index + 1) % 12;
    bulanSpan.dataset.bulanIndex = index;
    bulanSpan.innerText = bulanNames[index];
    refreshChart();
});

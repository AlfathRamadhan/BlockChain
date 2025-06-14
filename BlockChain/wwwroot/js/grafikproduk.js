let chart;
const bulanNames = [
    "Januari", "Februari", "Maret", "April",
    "Mei", "Juni", "Juli", "Agustus",
    "September", "Oktober", "November", "Desember"
];

function loadChart(tahun, bulan) {
    fetch(`/Owner/DataPemakaianProduk?tahun=${tahun}&bulan=${bulan}`)
        .then(res => res.json())
        .then(data => {
            const labels = data.map(d => d.namaProduk);
            const values = data.map(d => d.total);
            const satuan = data.map(d => d.satuan); // ambil satuan

            if (chart) chart.destroy();

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
                        },
                        datalabels: {
                            color: '#fff',
                            font: {
                                weight: 'bold'
                            },
                            formatter: function (value, context) {
                                return `${value} ${satuan[context.dataIndex]}`;
                            },
                            anchor: 'center',
                            align: 'center'
                        }
                    }
                },
                plugins: [ChartDataLabels]
            });
        })

}

function refreshChart() {
    const tahun = parseInt(document.getElementById("selectTahun").value);
    const bulan = parseInt(document.getElementById("selectBulan").value);
    loadChart(tahun, bulan);
}

function initDropdowns() {
    const tahunNow = new Date().getFullYear();
    const bulanNow = new Date().getMonth(); // 0-based

    const tahunSelect = document.getElementById("selectTahun");
    for (let i = tahunNow - 5; i <= tahunNow + 5; i++) {
        const option = document.createElement("option");
        option.value = i;
        option.textContent = i;
        if (i === tahunNow) option.selected = true;
        tahunSelect.appendChild(option);
    }

    const bulanSelect = document.getElementById("selectBulan");
    bulanNames.forEach((name, index) => {
        const option = document.createElement("option");
        option.value = index + 1;
        option.textContent = name;
        if (index === bulanNow) option.selected = true;
        bulanSelect.appendChild(option);
    });

    tahunSelect.addEventListener("change", refreshChart);
    bulanSelect.addEventListener("change", refreshChart);

    // Load chart pertama kali
    loadChart(tahunNow, bulanNow + 1);
}

document.addEventListener("DOMContentLoaded", initDropdowns);

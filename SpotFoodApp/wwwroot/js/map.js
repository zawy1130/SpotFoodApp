let map;
let userMarker;
let poiMarkers = [];

// ================= INIT MAP =================
window.initMap = function () {
    map = L.map('map', {
        zoomControl: true,
        attributionControl: true
    }).setView([10.762622, 106.660172], 15);   // fallback TP.HCM

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap contributors'
    }).addTo(map);

    // Fix map không hiển thị đúng kích thước trong MAUI Blazor
    setTimeout(() => {
        map.invalidateSize();
    }, 500);
};

// ================= GET USER LOCATION =================
window.getUserLocation = function (dotnetHelper) {
    if (!navigator.geolocation) {
        console.error("Geolocation không được hỗ trợ trên thiết bị này.");
        return;
    }

    navigator.geolocation.getCurrentPosition(
        (pos) => {
            const lat = pos.coords.latitude;
            const lng = pos.coords.longitude;

            console.log(`📍 User location: ${lat}, ${lng}`);

            // Di chuyển map đến vị trí người dùng + zoom cao hơn
            map.setView([lat, lng], 17);

            // Xóa marker cũ nếu có
            if (userMarker) {
                map.removeLayer(userMarker);
            }

            // Thêm marker người dùng (màu xanh nổi bật)
            userMarker = L.marker([lat, lng], {
                icon: L.icon({
                    iconUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-icon.png',
                    shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-shadow.png',
                    iconSize: [25, 41],
                    iconAnchor: [12, 41]
                })
            })
                .addTo(map)
                .bindPopup("<b>Bạn đang ở đây</b>")
                .openPopup();

            // Gọi C# để lấy danh sách POI gần nhất
            dotnetHelper.invokeMethodAsync('ReceiveLocation', lat, lng)
                .catch(err => console.error("Lỗi gọi ReceiveLocation:", err));
        },
        (error) => {
            console.error("Lỗi lấy vị trí:", error.message);
            alert("Không thể lấy vị trí. Vui lòng kiểm tra quyền Location và bật GPS.");
        },
        {
            enableHighAccuracy: true,   // Quan trọng trên mobile
            timeout: 10000,
            maximumAge: 0
        }
    );
};

// ================= SHOW POIs =================
window.showPOIs = function (pois) {
    // Xóa marker POI cũ
    poiMarkers.forEach(m => map.removeLayer(m));
    poiMarkers = [];

    pois.forEach(p => {
        const marker = L.marker([p.latitude, p.longitude])
            .addTo(map)
            .bindPopup(`<b>${p.name}</b><br>Khoảng cách: ${p.distance} m`);

        poiMarkers.push(marker);
    });
};

// ================= BOTTOM SHEET (giữ nguyên hoặc cải thiện nhẹ) =================
window.initBottomSheet = function () {
    const sheet = document.querySelector(".bottom-sheet");
    if (!sheet) return;

    let startY = 0;
    let currentY = 0;
    let isDragging = false;

    sheet.addEventListener("touchstart", (e) => {
        isDragging = true;
        startY = e.touches[0].clientY;
    });

    sheet.addEventListener("touchmove", (e) => {
        if (!isDragging) return;
        currentY = e.touches[0].clientY;
        const diff = currentY - startY;
        if (diff > 0) {
            sheet.style.transform = `translateY(${diff}px)`;
        }
    });

    sheet.addEventListener("touchend", () => {
        isDragging = false;
        if (currentY - startY > 120) {
            sheet.classList.remove("expanded");
        } else {
            sheet.classList.add("expanded");
        }
        sheet.style.transform = "";
    });
};
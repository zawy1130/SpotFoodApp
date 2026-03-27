let map;
let userMarker;
let lastLatLng = null;
let isFollowing = true; 


const userIcon = L.icon({
    iconUrl: 'img/location_icon.png',
    iconSize: [50, 60],
    iconAnchor: [20, 40]
});

window.initMap = function () {
    map = L.map('map', {
        zoomControl: true
    }).setView([10.762622, 106.660172], 15);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap',
        maxZoom: 19, // 🔥 zoom tối đa
        minZoom: 5   // 🔥 zoom tối thiểu
    }).addTo(map);

    // 👉 Khi user kéo map → tắt follow
    map.on('dragstart', function () {
        isFollowing = false;
    });
};

window.updateUserLocation = function (lat, lng) {
    if (!map) return;

    lastLatLng = [lat, lng];

    if (!userMarker) {
        userMarker = L.marker(lastLatLng, { icon: userIcon }).addTo(map);
        map.setView(lastLatLng, 17);
    } else {
        userMarker.setLatLng(lastLatLng);
    }

    // 🔥 chỉ follow khi bật
    if (isFollowing) {
        map.setView(lastLatLng);
    }
};

// 🔥 NÚT QUAY LẠI VỊ TRÍ
window.goToMyLocation = function () {
    if (!map || !lastLatLng) return;

    isFollowing = true;
    map.setView(lastLatLng, 17);
};
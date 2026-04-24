window.mapHelper = {
    map: null,
    marker: null,
    userLatLng: null,
    isFollowing: true,
    placeMarkers: [],
    dotNetRef: null,
    currentAudio: null,
    currentPoiData: null,

    // ================= ICON =================
    createIcon: (url, size = 30) => L.icon({
        iconUrl: url,
        iconSize: [size, size],
        iconAnchor: [size / 2, size]
    }),

    getIconByCategory(categoryId) {
        switch (categoryId) {
            case 1: return this.createIcon('/img/icon/nhahang_icon.png');
            case 2: return this.createIcon('/img/icon/quanan_icon.png');
            case 3: return this.createIcon('/img/icon/cafeshop_icon.png');
            case 4: return this.createIcon('/img/icon/khachsan_icon.png');
            case 5: return this.createIcon('/img/icon/chua_icon.png');
            case 6: return this.createIcon('/img/icon/nhatho_icon.png');
            default: return this.createIcon('/img/icon/nhahang_icon.png');
        }
    },

    userIcon: L.icon({
        iconUrl: '/img/icon/user_icon.png',
        iconSize: [45, 45],
        iconAnchor: [22, 45]
    }),


    // ================= HIGHLIGHT =================
    currentlyHighlightedMarker: null,
    originalIcon: null,

    highlightPoi: function (poiId) {
        this.clearHighlight();

        const marker = this.placeMarkers.find(m => m.poiId === poiId);
        if (!marker) return;

        this.currentlyHighlightedMarker = marker;
        this.originalIcon = marker.getIcon();

        const highlightIcon = L.icon({
            iconUrl: this.originalIcon.options.iconUrl,
            iconSize: [48, 48],
            iconAnchor: [24, 48],
            className: 'highlighted-poi-marker'
        });

        marker.setIcon(highlightIcon);
        marker.openTooltip();

        this.map.flyTo(marker.getLatLng(), 17.5, {
            duration: 1.2,
            easeLinearity: 0.25
        });
    },

    clearHighlight: function () {
        if (this.currentlyHighlightedMarker && this.originalIcon) {
            this.currentlyHighlightedMarker.setIcon(this.originalIcon);
            this.currentlyHighlightedMarker.closeTooltip();
        }
        this.currentlyHighlightedMarker = null;
        this.originalIcon = null;
    },

    // ================= BOTTOM SHEET & MAP INIT =================
    initBottomSheet() {
        const sheet = document.getElementById("bottomSheet");
        const dragBar = document.getElementById("dragBar");

        let startY = 0;
        let isDragging = false;

        let currentTranslate = 65; // trạng thái ban đầu
        const MAX = 65;  // đóng
        const MIN = 0;   // mở tối đa (70% màn hình)

        const setTranslate = (value) => {
            currentTranslate = Math.min(Math.max(value, MIN), MAX);
            sheet.style.transform = `translateY(${currentTranslate}%)`;
        };

        const start = (y) => {
            startY = y;
            isDragging = true;
        };

        const move = (y) => {
            if (!isDragging) return;

            const delta = y - startY;
            const percentMove = (delta / window.innerHeight) * 100;

            setTranslate(currentTranslate + percentMove);
        };

        const end = () => {
            isDragging = false;

            // SNAP (chỉ 2 mức)
            if (currentTranslate < 35) {
                setTranslate(0); // mở tối đa (70%)
                sheet.classList.add("expanded");
            } else {
                setTranslate(65); // đóng
                sheet.classList.remove("expanded");
            }
        };

        // TOUCH
        dragBar.addEventListener("touchstart", e => start(e.touches[0].clientY));
        dragBar.addEventListener("touchmove", e => move(e.touches[0].clientY));
        dragBar.addEventListener("touchend", end);

        // MOUSE
        dragBar.addEventListener("mousedown", e => start(e.clientY));
        window.addEventListener("mousemove", e => move(e.clientY));
        window.addEventListener("mouseup", end);
    },

    initMap(lat, lng, places, dotnetHelper) {
        this.dotNetRef = dotnetHelper;
        this.userLatLng = [lat, lng];

        this.map = L.map('map', { center: this.userLatLng, zoom: 16, zoomControl: false });

        L.tileLayer('https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png').addTo(this.map);

        this.marker = L.marker(this.userLatLng, { icon: this.userIcon }).addTo(this.map);

        this.updatePlaces(places);
        this.initBottomSheet();

        this.map.on('dragstart', () => this.isFollowing = false);
    },

    updatePlaces(places) {
        this.placeMarkers.forEach(m => this.map.removeLayer(m));
        this.placeMarkers = [];

        let bounds = [];

        places.forEach(p => {
            const marker = L.marker([p.lat, p.lng], {
                icon: this.getIconByCategory(p.categoryId)
            }).addTo(this.map);

            marker.poiId = p.poiId;

            marker.bindTooltip(p.name, {
                direction: 'top',
                offset: [0, -8],
                className: 'poi-label'
            });

            marker.on("click", () => this.openDetail(p.poiId));

            this.placeMarkers.push(marker);
            bounds.push([p.lat, p.lng]);
        });

        if (bounds.length > 0) {
            this.map.fitBounds(bounds, { padding: [50, 50], maxZoom: 17 });
        }
    },

    focusToLocation(lat, lng) {
        if (!this.map) return;
        this.map.setView([lat, lng], 17, { animate: true });
    },

    async openDetail(id) {
        if (!this.dotNetRef) return;
        try {
            await this.dotNetRef.invokeMethodAsync("OpenPoiDetail", id);
        } catch (err) {
            console.error("Open detail error:", err);
        }
    },

    updateUserLocation(lat, lng) {
        this.userLatLng = [lat, lng];
        if (this.marker) this.marker.setLatLng(this.userLatLng);

        if (this.isFollowing) {
            this.map.setView(this.userLatLng, this.map.getZoom(), { animate: true });
        }
    },

    goToCurrentLocation() {
        if (!this.map || !this.userLatLng) return;
        this.isFollowing = true;
        this.map.setView(this.userLatLng, 16, { animate: true });
    },

    // ================= AUDIO =================
    playAudio(audioUrl) {
        const API = "https://sony-site.somee.com";
        const fullUrl = audioUrl.startsWith('http') ? audioUrl
            : API + (audioUrl.startsWith('/') ? '' : '/') + audioUrl;

        if (this.currentAudio) {
            this.currentAudio.pause();
            this.currentAudio = null;
        }

        this.currentAudio = new Audio(fullUrl);
        this.currentAudio.play().catch(err => console.error("Audio play error:", err));

        this.setAudioButtonState("playing-audio");

        this.currentAudio.onended = () => {
            this.setAudioButtonState("idle");
            this.currentAudio = null;
        };
    },

    stopCurrentAudio: function () {
        if (this.currentAudio) {
            this.currentAudio.pause();
            this.currentAudio = null;
        }
        this.setAudioButtonState("idle");
    },

    setAudioButtonState(state) {
        const btn = document.getElementById("btn-audio");
        if (!btn) return;

        if (state === "idle") {
            btn.innerHTML = `🔊 Nghe thuyết minh`;
            btn.style.background = "#ff5722";
            btn.onclick = async () => {
                if (this.dotNetRef && this.currentPoiData) {
                    await this.dotNetRef.invokeMethodAsync("HandleAudioLogic", this.currentPoiData);
                }
            };
        } else if (state === "playing-audio") {
            btn.innerHTML = `⏸️ Tạm dừng`;
            btn.style.background = "#ff8a65";
            btn.onclick = () => this.stopCurrentAudio();
        } else if (state === "speaking-tts") {
            btn.innerHTML = `⏹️ Dừng`;
            btn.style.background = "#d32f2f";
            btn.onclick = async () => {
                if (this.dotNetRef) await this.dotNetRef.invokeMethodAsync("StopTTS");
            };
        }
    },

    showPoiDetail(data) {
        this.currentPoiData = data;
        document.getElementById("poi-name").innerText = data.name || data.Name || "";
        document.getElementById("poi-desc").innerText = data.description || data.Description || "";
        document.getElementById("poi-address").innerText = data.address || data.Address || "";
        document.getElementById("poi-image").src = data.imageUrl || data.ImageUrl || "/img/default.png";
        document.getElementById("poi-map").href = data.mapLink || data.MapLink || "#";

        document.getElementById("poi-overlay").classList.remove("hidden");
        this.setAudioButtonState("idle");
    },

    closeOverlay() {
        document.getElementById("poi-overlay").classList.add("hidden");

        if (this.currentAudio) {
            this.currentAudio.pause();
            this.currentAudio = null;
        }
        if (this.dotNetRef) {
            this.dotNetRef.invokeMethodAsync("StopTTS").catch(() => { });
        }
        this.setAudioButtonState("idle");
    }
};

function toggleLangDropdown() {
    const dropdown = document.getElementById("langDropdown");
    dropdown.classList.toggle("show");
}

// click ngoài để đóng
window.addEventListener("click", function (e) {
    const box = document.querySelector(".language-box");
    if (!box.contains(e.target)) {
        document.getElementById("langDropdown").classList.remove("show");
    }
});
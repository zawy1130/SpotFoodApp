window.mapHelper = {
    map: null,
    marker: null,
    userLatLng: null,
    isFollowing: true,
    placeMarkers: [],
    dotNetRef: null,
    currentAudio: null,
    currentPoiData: null,
    isTTSMode: false,

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

    // ================= BOTTOM SHEET =================
    initBottomSheet() {
        const sheet = document.getElementById("bottomSheet");
        const dragBar = document.getElementById("dragBar");

        let startY = 0;
        let isDragging = false;

        const start = (y) => {
            startY = y;
            isDragging = true;
        };

        const move = (y) => {
            if (!isDragging) return;
            const delta = y - startY;
            sheet.style.transform = `translateY(${Math.max(0, 68 + delta / 5)}%)`;
        };

        const end = () => {
            isDragging = false;
            if (sheet.style.transform.includes("0%")) {
                sheet.classList.add("expanded");
            } else {
                sheet.classList.remove("expanded");
            }
            sheet.style.transform = "";
        };

        // Mobile
        dragBar.addEventListener("touchstart", e => start(e.touches[0].clientY));
        dragBar.addEventListener("touchmove", e => move(e.touches[0].clientY));
        dragBar.addEventListener("touchend", end);

        // Desktop (fallback)
        dragBar.addEventListener("mousedown", e => start(e.clientY));
        window.addEventListener("mousemove", e => move(e.clientY));
        window.addEventListener("mouseup", end);
    },

    // ================= INIT MAP =================
    initMap(lat, lng, places, dotnetHelper) {
        this.dotNetRef = dotnetHelper;
        this.userLatLng = [lat, lng];

        this.map = L.map('map', {
            center: this.userLatLng,
            zoom: 16,
            zoomControl: false
        });

        L.tileLayer('https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png')
            .addTo(this.map);

        // Marker vị trí user
        this.marker = L.marker(this.userLatLng, { icon: this.userIcon }).addTo(this.map);

        // Render POI ban đầu
        this.updatePlaces(places);

        // Kéo map → tắt follow
        this.map.on('dragstart', () => this.isFollowing = false);

        // Ẩn/hiện tooltip theo zoom
        this.map.on('zoomend', () => {
            const zoom = this.map.getZoom();
            this.placeMarkers.forEach(m =>
                zoom >= 16 ? m.openTooltip() : m.closeTooltip()
            );
        });

        this.initBottomSheet();
    },

    // ================= POI =================
    updatePlaces(places) {
        // Xóa marker cũ
        this.placeMarkers.forEach(m => this.map.removeLayer(m));
        this.placeMarkers = [];

        let bounds = [];

        places.forEach(p => {
            const marker = L.marker([p.lat, p.lng], {
                icon: this.getIconByCategory(p.categoryId)
            }).addTo(this.map);

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
            this.map.fitBounds(bounds, {
                padding: [50, 50],
                maxZoom: 17
            });
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

    // ================= LOCATION =================
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

    // ================= AUDIO & TTS =================
    playAudio(audioUrl) {
        const btn = document.getElementById("btn-audio");
        const API = "http://10.0.2.2:5205";

        const fullUrl = audioUrl.startsWith('http')
            ? audioUrl
            : API + (audioUrl.startsWith('/') ? '' : '/') + audioUrl;

        // Dừng audio cũ nếu đang phát
        if (this.currentAudio) {
            this.currentAudio.pause();
            this.currentAudio = null;
        }

        this.currentAudio = new Audio(fullUrl);

        this.currentAudio.play().catch(err => {
            console.error("Audio play error:", err);
            this.setAudioButtonState("idle");
        });

        this.setAudioButtonState("playing-audio");   // Thêm trạng thái riêng cho audio

        this.currentAudio.onended = () => {
            this.setAudioButtonState("idle");
            this.currentAudio = null;
        };
    },

    // Trạng thái nút Audio/TTS
    setAudioButtonState(state) {
        const btn = document.getElementById("btn-audio");
        if (!btn) return;

        if (state === "idle") {
            btn.innerHTML = `🔊 Nghe thuyết minh`;
            btn.style.background = "#ff5722";
            btn.style.color = "white";
            btn.onclick = async () => {
                if (!this.dotNetRef || !this.currentPoiData) return;
                await this.dotNetRef.invokeMethodAsync("HandleAudioLogic", this.currentPoiData);
            };
            this.isTTSMode = false;
        }
        else if (state === "playing-audio") {
            btn.innerHTML = `⏸️ Tạm dừng`;
            btn.style.background = "#ff8a65";
            btn.onclick = () => {
                if (this.currentAudio) {
                    this.currentAudio.pause();
                    this.currentAudio = null;
                }
                this.setAudioButtonState("idle");
            };
        }
        else if (state === "speaking-tts") {
            btn.innerHTML = `⏹️ Dừng`;
            btn.style.background = "#d32f2f";
            btn.onclick = async () => {
                if (this.dotNetRef) {
                    await this.dotNetRef.invokeMethodAsync("StopTTS");
                }
            };
            this.isTTSMode = true;
        }
    },

    // Hiển thị chi tiết POI
    showPoiDetail(data) {
        this.currentPoiData = data;

        document.getElementById("poi-name").innerText = data.name || data.Name || "";
        document.getElementById("poi-desc").innerText = data.description || data.Description || "";
        document.getElementById("poi-address").innerText = data.address || data.Address || "";
        document.getElementById("poi-image").src = data.imageUrl || data.ImageUrl || "/img/default.png";
        document.getElementById("poi-map").href = data.mapLink || data.MapLink || "#";

        const btn = document.getElementById("btn-audio");
        btn.style.display = "flex";

        this.setAudioButtonState("idle");   // Luôn reset về idle khi mở popup mới

        document.getElementById("poi-overlay").classList.remove("hidden");
    },

    // Đóng overlay + dừng hết audio & TTS
    closeOverlay() {
        const overlay = document.getElementById("poi-overlay");
        overlay.classList.add("hidden");

        // Dừng Audio
        if (this.currentAudio) {
            this.currentAudio.pause();
            this.currentAudio = null;
        }

        // Dừng TTS (gọi C#)
        if (this.dotNetRef) {
            this.dotNetRef.invokeMethodAsync("StopTTS").catch(() => { });
        }

        this.setAudioButtonState("idle");
    }
};
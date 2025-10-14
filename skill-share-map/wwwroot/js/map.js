// Leaflet Map Integration for Blazor

window.mapHelper = {
    map: null,
    markers: [],

    // Initialize map
    initMap: function (mapId, lat, lng, zoom) {
        if (this.map) {
            this.map.remove();
        }

        this.map = L.map(mapId).setView([lat, lng], zoom);

        // Add OpenStreetMap tiles
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
            maxZoom: 19
        }).addTo(this.map);

        return true;
    },

    // Add marker
    addMarker: function (id, lat, lng, title, type) {
        const iconUrl = type === 'task'
            ? 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-blue.png'
            : 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-orange.png';

        const customIcon = L.icon({
            iconUrl: iconUrl,
            iconSize: [25, 41],
            iconAnchor: [12, 41],
            popupAnchor: [1, -34],
        });

        const marker = L.marker([lat, lng], { icon: customIcon })
            .bindPopup(title)
            .addTo(this.map);

        marker._id = id;
        this.markers.push(marker);

        marker.on('click', function() {
            DotNet.invokeMethodAsync('skill-share-map', 'OnMarkerClicked', id);
        });

        return true;
    },

    // Clear all markers
    clearMarkers: function () {
        this.markers.forEach(marker => {
            this.map.removeLayer(marker);
        });
        this.markers = [];
        return true;
    },

    // Fit bounds to show all markers
    fitBounds: function () {
        if (this.markers.length > 0) {
            const group = L.featureGroup(this.markers);
            this.map.fitBounds(group.getBounds().pad(0.1));
        }
        return true;
    },

    // Set view to specific location
    setView: function (lat, lng, zoom) {
        if (this.map) {
            this.map.setView([lat, lng], zoom);
        }
        return true;
    },

    // Get current map center
    getCenter: function () {
        if (this.map) {
            const center = this.map.getCenter();
            return { lat: center.lat, lng: center.lng };
        }
        return null;
    }
};

// Leaflet Map Integration for Blazor
// Support multiple map instances

window.mapHelper = {
    maps: {}, // Store multiple map instances by mapId

    // Initialize map
    initMap: function (mapId, lat, lng, zoom, dotNetHelper) {
        // Clean up existing map if it exists
        if (this.maps[mapId]) {
            if (this.maps[mapId].map) {
                this.maps[mapId].map.remove();
            }
            if (this.maps[mapId].dotNetHelper) {
                this.maps[mapId].dotNetHelper.dispose();
            }
        }

        // Create new map instance
        const map = L.map(mapId).setView([lat, lng], zoom);

        // Add OpenStreetMap tiles
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
            maxZoom: 19
        }).addTo(map);

        // Store map instance with its own markers and helper
        this.maps[mapId] = {
            map: map,
            markers: [],
            dotNetHelper: dotNetHelper
        };

        return true;
    },

    // Get map instance
    getMapInstance: function (mapId) {
        return this.maps[mapId] || this.maps['mainMap'] || Object.values(this.maps)[0];
    },

    // Add marker
    addMarker: function (id, lat, lng, title, type) {
        const mapInstance = this.getMapInstance();
        if (!mapInstance) return false;

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
            .addTo(mapInstance.map);

        marker._id = id;
        mapInstance.markers.push(marker);

        marker.on('click', () => {
            if (mapInstance.dotNetHelper) {
                mapInstance.dotNetHelper.invokeMethodAsync('HandleMarkerClick', id);
            }
        });

        return true;
    },

    // Clear all markers
    clearMarkers: function () {
        const mapInstance = this.getMapInstance();
        if (!mapInstance) return false;

        mapInstance.markers.forEach(marker => {
            mapInstance.map.removeLayer(marker);
        });
        mapInstance.markers = [];
        return true;
    },

    // Fit bounds to show all markers
    fitBounds: function () {
        const mapInstance = this.getMapInstance();
        if (!mapInstance) return false;

        if (mapInstance.markers.length > 0) {
            const group = L.featureGroup(mapInstance.markers);
            mapInstance.map.fitBounds(group.getBounds().pad(0.1));
        }
        return true;
    },

    // Set view to specific location
    setView: function (lat, lng, zoom) {
        const mapInstance = this.getMapInstance();
        if (mapInstance && mapInstance.map) {
            mapInstance.map.setView([lat, lng], zoom);
        }
        return true;
    },

    // Get current map center
    getCenter: function () {
        const mapInstance = this.getMapInstance();
        if (mapInstance && mapInstance.map) {
            const center = mapInstance.map.getCenter();
            return { lat: center.lat, lng: center.lng };
        }
        return null;
    }
};

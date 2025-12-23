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
        const tileLayer = L.tileLayer('https://{s}.basemaps.cartocdn.com/rastertiles/voyager/{z}/{x}/{y}{r}.png', {
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors &copy; <a href="https://carto.com/attributions">CARTO</a>',
            subdomains: 'abcd',
            maxZoom: 20
        });

        tileLayer.addTo(map);

        tileLayer.on('tileerror', (errorEvent) => {
            console.warn('Map tile failed to load. Check internet access or tile server availability.', errorEvent);
        });

        // Ensure map properly renders when container size changes (e.g., flex layout)
        setTimeout(() => {
            map.invalidateSize();
        }, 150);
        const markerLayerGroup = L.layerGroup().addTo(map);
        // Store map instance with its own markers and helper
        this.maps[mapId] = {
            map: map,
            markerLayer: markerLayerGroup,
            dotNetHelper: dotNetHelper
        };

        return true;
    },

    // Get map instance
    getMapInstance: function (mapId) {
        if (mapId && this.maps[mapId]) {
            return this.maps[mapId];
        }

        if (this.maps['mainMap']) {
            return this.maps['mainMap'];
        }

        const mapEntries = Object.values(this.maps);
        return mapEntries.length > 0 ? mapEntries[0] : null;
    },

    // Add marker
    // Add marker
    addMarker: function (mapId, id, lat, lng, title, type) {
        const mapInstance = this.getMapInstance(mapId);
        if (!mapInstance) return false;

        const primaryColor = '#4a75b0';
        const secondaryColor = '#d31638';
        const markerColor = type === 'task' ? primaryColor : secondaryColor;
        const detailUrl = type === 'task' ? `/task/${id}` : `/job/${id}`;

        // 1. custom SVG icon
        const svgHtml = `
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="40" height="40">
                <circle cx="12" cy="9" r="3.5" fill="white" />
                <path fill="${markerColor}" stroke="none" 
                      d="M12 2C8.13 2 5 5.13 5 9c0 5.25 7 13 7 13s7-7.75 7-13c0-3.87-3.13-7-7-7zm0 9.5c-1.38 0-2.5-1.12-2.5-2.5s1.12-2.5 2.5-2.5 2.5 1.12 2.5 2.5-1.12 2.5-2.5 2.5z"/>
            </svg>`;

        const customIcon = L.divIcon({
            className: 'custom-svg-marker',
            html: svgHtml,
            iconSize: [40, 40],
            iconAnchor: [20, 40],
            popupAnchor: [0, -30]
        });

        // 2. my design for popup content
        const popupContent = `
            <div class="modern-popup-card">
                <div class="popup-title">${title}</div>
                <div class="popup-subtitle">Broadway Mall, Ultimo</div> 
                
                <div class="popup-info-row">
                    <div class="info-item">
                        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <rect x="3" y="4" width="18" height="18" rx="2" ry="2"></rect>
                            <line x1="16" y1="2" x2="16" y2="6"></line>
                            <line x1="8" y1="2" x2="8" y2="6"></line>
                            <line x1="3" y1="10" x2="21" y2="10"></line>
                        </svg>
                        <span>DDL 2025.12.25</span>
                    </div>
                    <div class="info-item">
                        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <rect x="1" y="4" width="22" height="16" rx="2" ry="2"></rect>
                            <line x1="1" y1="10" x2="23" y2="10"></line>
                        </svg>
                        <span>$200</span>
                    </div>
                </div>

                <div class="popup-desc-title">Description</div>
                <ul class="popup-desc-content">
                    <li>Need help debugging a Python script for data analysis project</li>
                </ul>

                <a href="${detailUrl}" class="popup-btn">
                    View Details
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" style="margin-left:4px;">
                        <line x1="5" y1="12" x2="19" y2="12"></line>
                        <polyline points="12 5 19 12 12 19"></polyline>
                    </svg>
                </a>
            </div>
        `;

        // 3. create Marker
        const marker = L.marker([lat, lng], { icon: customIcon })
            .bindPopup(popupContent, {
                closeButton: false,
                autoPan: true
            });

        mapInstance.markerLayer.addLayer(marker);

        marker.on('click', () => {
            if (mapInstance.dotNetHelper) {
                mapInstance.dotNetHelper.invokeMethodAsync('HandleMarkerClick', id);
            }
        });

        return true;
    },
    // Clear all markers
    clearMarkers: function (mapId) {
        const mapInstance = this.getMapInstance(mapId);
        if (!mapInstance || !mapInstance.markerLayer) return false;

        mapInstance.markerLayer.clearLayers();

        return true;
    },

    // Fit bounds to show all markers
    fitBounds: function (mapId) {
        const mapInstance = this.getMapInstance(mapId);
        if (!mapInstance || !mapInstance.markerLayer) return false;

        const layers = mapInstance.markerLayer.getLayers();

        if (layers.length > 0) {
            const group = L.featureGroup(layers);
            mapInstance.map.fitBounds(group.getBounds().pad(0.1));
        }

        mapInstance.map.invalidateSize();
        return true;
    },

    // Set view to specific location
    setView: function (mapId, lat, lng, zoom) {
        const mapInstance = this.getMapInstance(mapId);
        if (mapInstance && mapInstance.map) {
            mapInstance.map.setView([lat, lng], zoom);
        }
        return true;
    },

    // Get current map center
    getCenter: function (mapId) {
        const mapInstance = this.getMapInstance(mapId);
        if (mapInstance && mapInstance.map) {
            const center = mapInstance.map.getCenter();
            return { lat: center.lat, lng: center.lng };
        }
        return null;
    }
};

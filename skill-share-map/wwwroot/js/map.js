// Leaflet Map Integration for Blazor
// Support multiple map instances

window.mapHelper = {
    maps: {},

    initMap: function (mapId, lat, lng, zoom, dotNetHelper) {
        if (this.maps[mapId]) {
            if (this.maps[mapId].map) this.maps[mapId].map.remove();
            if (this.maps[mapId].dotNetHelper) this.maps[mapId].dotNetHelper.dispose();
        }

        const map = L.map(mapId).setView([lat, lng], zoom);

        const tileLayer = L.tileLayer('https://{s}.basemaps.cartocdn.com/rastertiles/voyager/{z}/{x}/{y}{r}.png', {
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors &copy; <a href="https://carto.com/attributions">CARTO</a>',
            subdomains: 'abcd',
            maxZoom: 20
        });
        tileLayer.addTo(map);

        tileLayer.on('tileerror', (errorEvent) => {
            console.warn('Map tile failed to load.', errorEvent);
        });

        setTimeout(() => { map.invalidateSize(); }, 150);

        const markerLayerGroup = L.layerGroup().addTo(map);

        this.maps[mapId] = {
            map: map,
            markerLayer: markerLayerGroup,
            dotNetHelper: dotNetHelper
        };

        return true;
    },

    getMapInstance: function (mapId) {
        if (mapId && this.maps[mapId]) return this.maps[mapId];
        if (this.maps['mainMap']) return this.maps['mainMap'];
        const entries = Object.values(this.maps);
        return entries.length > 0 ? entries[0] : null;
    },

    addMarker: function (mapId, id, lat, lng, title, type) {
        const mapInstance = this.getMapInstance(mapId);
        if (!mapInstance) return false;

        const taskColor = '#0B4E99';
        const jobColor = '#EB1D26';
        const markerColor = type === 'task' ? taskColor : jobColor;

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

        const marker = L.marker([lat, lng], { icon: customIcon });
        mapInstance.markerLayer.addLayer(marker);

        marker.on('click', (e) => {
            // Zoom in to the marker on click (smooth animation)
            const currentZoom = mapInstance.map.getZoom();
            const targetZoom = Math.max(currentZoom, 16); // Zoom to at least 16
            mapInstance.map.setView(e.latlng, targetZoom, { animate: true, duration: 0.5 });

            // After the zoom/pan completes, get the screen-relative position
            // Use a short delay to let the animation settle
            setTimeout(() => {
                if (mapInstance.dotNetHelper) {
                    // Get the map container's position on screen
                    const containerEl = mapInstance.map.getContainer();
                    const containerRect = containerEl.getBoundingClientRect();
                    
                    // Get the marker's pixel position relative to the container
                    const point = mapInstance.map.latLngToContainerPoint(e.latlng);
                    
                    // Convert to screen-relative coords
                    const screenX = containerRect.left + point.x;
                    const screenY = containerRect.top + point.y;

                    mapInstance.dotNetHelper.invokeMethodAsync('HandleMarkerClick', id, screenX, screenY);
                }
            }, 550); // Wait for zoom animation to finish
        });

        return true;
    },

    clearMarkers: function (mapId) {
        const mapInstance = this.getMapInstance(mapId);
        if (!mapInstance || !mapInstance.markerLayer) return false;
        mapInstance.markerLayer.clearLayers();
        return true;
    },

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

    setView: function (mapId, lat, lng, zoom) {
        const mapInstance = this.getMapInstance(mapId);
        if (mapInstance && mapInstance.map) {
            mapInstance.map.setView([lat, lng], zoom, { animate: true });
        }
        return true;
    },

    getCenter: function (mapId) {
        const mapInstance = this.getMapInstance(mapId);
        if (mapInstance && mapInstance.map) {
            const center = mapInstance.map.getCenter();
            return { lat: center.lat, lng: center.lng };
        }
        return null;
    }
};

// Geolocation helper functions for getting user's current location

window.geolocationHelper = {
    // Get current position using browser's Geolocation API
    getCurrentPosition: function() {
        return new Promise((resolve, reject) => {
            if (!navigator.geolocation) {
                reject(new Error('Geolocation is not supported by this browser'));
                return;
            }

            navigator.geolocation.getCurrentPosition(
                (position) => {
                    resolve({
                        latitude: position.coords.latitude,
                        longitude: position.coords.longitude,
                        accuracy: position.coords.accuracy
                    });
                },
                (error) => {
                    let errorMessage = 'Unknown error occurred';
                    switch(error.code) {
                        case error.PERMISSION_DENIED:
                            errorMessage = 'User denied the request for Geolocation';
                            break;
                        case error.POSITION_UNAVAILABLE:
                            errorMessage = 'Location information is unavailable';
                            break;
                        case error.TIMEOUT:
                            errorMessage = 'The request to get user location timed out';
                            break;
                    }
                    reject(new Error(errorMessage));
                },
                {
                    enableHighAccuracy: true,
                    timeout: 10000,
                    maximumAge: 0
                }
            );
        });
    },

    // Reverse geocode coordinates to address using Nominatim (OpenStreetMap)
    reverseGeocode: async function(lat, lng) {
        try {
            const response = await fetch(
                `https://nominatim.openstreetmap.org/reverse?format=json&lat=${lat}&lon=${lng}&zoom=18&addressdetails=1`,
                {
                    headers: {
                        'Accept-Language': 'en'
                    }
                }
            );

            if (!response.ok) {
                throw new Error('Geocoding request failed');
            }

            const data = await response.json();

            // Format address from components
            let address = '';
            if (data.address) {
                const parts = [];
                if (data.address.road) parts.push(data.address.road);
                if (data.address.suburb) parts.push(data.address.suburb);
                if (data.address.city || data.address.town) parts.push(data.address.city || data.address.town);
                if (data.address.state) parts.push(data.address.state);
                if (data.address.postcode) parts.push(data.address.postcode);

                address = parts.join(', ');
            }

            // Fallback to display_name if we couldn't build address
            if (!address && data.display_name) {
                address = data.display_name;
            }

            return address || 'Address not found';
        } catch (error) {
            console.error('Reverse geocoding error:', error);
            return `${lat.toFixed(6)}, ${lng.toFixed(6)}`;
        }
    },

    // Get current location with address
    getCurrentLocationWithAddress: async function() {
        try {
            const position = await this.getCurrentPosition();
            const address = await this.reverseGeocode(position.latitude, position.longitude);

            return {
                latitude: position.latitude,
                longitude: position.longitude,
                address: address,
                accuracy: position.accuracy
            };
        } catch (error) {
            throw error;
        }
    }
};

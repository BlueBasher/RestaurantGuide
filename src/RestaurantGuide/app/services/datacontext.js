(function () {
    'use strict';

    var serviceId = 'datacontext';
    angular.module('app').factory(serviceId, ['$http', 'common', datacontext]);

    function datacontext($http, common) {

        var service = {
            searchRestaurants: searchRestaurants,
            suggestRestaurants: suggestRestaurants,
            getRestaurants: getRestaurants,
            getRestaurant: getRestaurant,
            createRestaurant: createRestaurant,
            editRestaurant: editRestaurant,
            deleteRestaurant: deleteRestaurant
        };

        return service;

        function searchRestaurants(searchTerm, filter) {
            return $http.get('api/search', {
                params: {   
                    searchTerm: searchTerm,
                    michelinStars: filter.michelinStars,
                    cuisines: filter.cuisines
                }
            });
        }

        function suggestRestaurants(searchText) {
            return $http.get('api/suggest', {
                params: {
                    searchText: searchText
                }
            });
        }

        function getRestaurants() {
            return $http.get('api/restaurant/');
        }

        function getRestaurant(id) {
            return $http.get('api/restaurant/', {
                params: {
                    id: id
                }
            });
        }

        function createRestaurant(restaurant) {
            return $http.post('api/restaurant/', restaurant);
        }

        function editRestaurant(restaurant) {
            return $http.put('api/restaurant/' + restaurant.id, restaurant)
                .success(function () {
                    common.logger.logSuccess("Restaurant updated.", restaurant, serviceId, true);
                }).error(function () {
                    common.logger.logError("Error updating restaurant.", restaurant, serviceId, true);
                });
        }

        function deleteRestaurant(restaurant) {
            return $http.delete('api/restaurant/' + restaurant.id)
                .success(function () {
                    common.logger.logSuccess("Restaurant deleted.", restaurant, serviceId, true);
                }).error(function () {
                    common.logger.logError("Error deleting restaurant.", restaurant, serviceId, true);
                });
        }
    }
})();
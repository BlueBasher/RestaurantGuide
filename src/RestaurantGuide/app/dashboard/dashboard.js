(function () {
    'use strict';
    var controllerId = 'dashboard';
    angular.module('app').controller(controllerId, ['common', '$modal', 'config', 'datacontext', dashboard]);

    function dashboard(common, $modal, config, datacontext) {
        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);

        var vm = this;
        vm.title = 'Dashboard'; 
        vm.searchResult = null;
        vm.searchTerm = '';
        vm.filter = {};
        vm.newRatingrating = {};

        vm.search = function () {
            vm.resetFilter();
            vm.doSearch();
        }

        vm.addRating = function (restaurant) {
            var modalInstance = $modal.open({
                animation: true,
                templateUrl: 'addRating.html',
                controller: 'CommentInstanceCtrl',
                resolve: {
                    title: function () {
                        return "Beoordeel restaurant"
                    },
                    restaurant: function () {
                        return restaurant;
                    },
                    rating: function () {
                        return null;
                    }
                }
            });

            modalInstance.result.then(function (restaurant) {
                common.$broadcast(config.events.spinnerToggle, { show: true });
                return datacontext.editRestaurant(restaurant)
                    .then(function () {
                        //TODO
                    }).finally(function () {
                        common.$broadcast(config.events.spinnerToggle, { show: false });
                    });
            }, function () {
                // Canceled
            });
        }

        vm.replyRating = function(restaurant, rating) {

            var r = restaurant;
            var modalInstance = $modal.open({
                animation: true,
                templateUrl: 'addRating.html',
                controller: 'CommentInstanceCtrl',
                resolve: {
                    title: function() {
                        return "Beoordeel rating"
                    },
                    restaurant: function() {
                        return restaurant;
                    },
                    rating: function() {
                        return rating;
                    }
                }
            });

            modalInstance.result.then(function(restaurant) {
                common.$broadcast(config.events.spinnerToggle, { show: true });
                return datacontext.editRestaurant(restaurant)
                    .then(function() {
                        //TODO
                    }).finally(function() {
                        common.$broadcast(config.events.spinnerToggle, { show: false });
                    });
            }, function() {
                // Canceled
            });
        }

        vm.replyComment = function(restaurant, comment) {

            var r = restaurant;
            var modalInstance = $modal.open({
                animation: true,
                templateUrl: 'addRating.html',
                controller: 'CommentInstanceCtrl',
                resolve: {
                    title: function() {
                        return "Beoordeel rating"
                    },
                    restaurant: function() {
                        return restaurant;
                    },
                    rating: function() {
                        return comment;
                    }
                }
            });

            modalInstance.result.then(function(restaurant) {
                common.$broadcast(config.events.spinnerToggle, { show: true });
                return datacontext.editRestaurant(restaurant)
                    .then(function() {
                        //TODO
                    }).finally(function() {
                        common.$broadcast(config.events.spinnerToggle, { show: false });
                    });
            }, function() {
                // Canceled
            });
        }

        vm.doSearch = function () {
            common.$broadcast(config.events.spinnerToggle, { show: true });
            datacontext.searchRestaurants(vm.searchTerm, vm.filter)
                .then(function (data) {
                    vm.searchResult = data.data;
                }).finally(function () {
                    common.$broadcast(config.events.spinnerToggle, { show: false });
                });
        }

        vm.selectFacet = function (facetName, facetValue) {
            if (facetName == 'MichelinStars') {
                vm.filter.michelinStars = facetValue;
            }

            if (facetName == 'Cuisines') {
                vm.filter.cuisines.push(facetValue);
            }

            vm.doSearch();
        }

        vm.getSuggestions = function (searchText) {
            return datacontext.suggestRestaurants(searchText)
                .then(function (data) {
                    return data.data;
                });
        }

        vm.getRatings = function (document) {
            return datacontext.getRestaurant(document.id)
                .then(function (data) {
                    document.ratings = data.data.ratings;
                })
        }

        vm.getNumber = function (loopCount) {
            var a = [];
            for (var i = 0; i < loopCount; i++) {
                a.push(i);
            }

            return a;
        }

        vm.resetFilter = function() {
            vm.filter = {
                michelinStars: null,
                cuisines: []
            };
        }

        activate();

        function activate() {
            vm.resetFilter();

            common.activateController([], controllerId)
                .then(function () { log('Activated Dashboard View'); });
        }
    }

    angular.module('app').controller('CommentInstanceCtrl', function ($scope, $modalInstance, title, restaurant, rating) {

        $scope.title = title;
        $scope.showStars = !rating;
        var newRating = {};

        if (rating) {
            if (!rating.comments) {
                rating.comments = [];
            }
            rating.comments.push(newRating);
        } else {
            if (!restaurant.ratings) {
                restaurant.ratings = [];
            }
            restaurant.ratings.push(newRating);
        }

        $scope.restaurant = restaurant;
        $scope.rating = newRating;

        $scope.ok = function () {
            $modalInstance.close($scope.restaurant);
        };

        $scope.getNumber = function (num) {
            return new Array(num);
        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
    });
})();
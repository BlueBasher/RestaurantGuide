    (function () {
    'use strict';
    var controllerId = 'admin';
    angular.module('app').controller(controllerId, ['$modal', 'config', 'common', 'datacontext', admin]);

    function admin($modal, config, common, datacontext) {
        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);

        var vm = this;
        vm.title = 'Admin';
        vm.restaurants = [];
        vm.getNumber = function (num) {
            return new Array(num);
        };

        vm.createRestaurant = function () {
            var modalInstance = $modal.open({
                animation: true,
                templateUrl: 'editRestaurant.html',
                controller: 'ModalInstanceCtrl',
                resolve: {
                    title: function () {
                        return "Create restaurant"
                    },
                    restaurant: function () {
                        return {};
                    },
                    getNumber: function () {
                        return vm.getNumber;
                    }
                }
            });

            modalInstance.result.then(function (restaurant) {
                common.$broadcast(config.events.spinnerToggle, { show: true });
                return datacontext.createRestaurant(restaurant)
                    .then(function (data) {
                        var newRestaurant = data.data;
                        newRestaurant.isExpanded = true;
                        vm.restaurants.push(newRestaurant);
                    }).finally(function () {
                        common.$broadcast(config.events.spinnerToggle, { show: false });
                    });
            }, function () {
                // Canceled
            });
        };

        vm.editRestaurant = function (restaurant) {
            var modalInstance = $modal.open({
                animation: true,
                templateUrl: 'editRestaurant.html',
                controller: 'ModalInstanceCtrl',
                resolve: {
                    title: function () {
                        return "Edit " + restaurant.name
                    },
                    restaurant: function () {
                        return angular.copy(restaurant);
                    },
                    getNumber: function () {
                        return vm.getNumber;
                    }
                }
            });

            modalInstance.result.then(function (restaurant) {
                common.$broadcast(config.events.spinnerToggle, { show: true });
                return datacontext.editRestaurant(restaurant)
                    .then(function () {
                        for (var i = 0; i < vm.restaurants.length; i++) {
                            if (vm.restaurants[i].id === restaurant.id) {
                                vm.restaurants[i] = restaurant;
                                vm.restaurants[i].isExpanded = true;
                            }
                        }
                    }).finally(function () {
                        common.$broadcast(config.events.spinnerToggle, { show: false });
                    });
            }, function () {
                // Canceled
            });
        }

        vm.removeRestaurant = function (restaurant) {
            return datacontext.deleteRestaurant(restaurant)
                .then(function () {
                    var index = vm.restaurants.indexOf(restaurant);
                    vm.restaurants.splice(index, 1);
                });
        }

        activate();

        function activate() {
            common.activateController([getRestaurants()], controllerId)
                .then(function () { log('Activated Admin View'); });
        }

        function getRestaurants() {
            return datacontext.getRestaurants()
                .then(function (data) {
                    vm.restaurants = data.data;
                });
        }
    }

    angular.module('app').controller('ModalInstanceCtrl', function ($scope, $modalInstance, title, restaurant, getNumber) {

        $scope.title = title;
        $scope.restaurant = restaurant;
        $scope.newCuisine = '';

        $scope.getNumber = getNumber;
        $scope.addCuisine = function () {
            if ($scope.newCuisine) {
                if (!$scope.restaurant.cuisines) {
                    $scope.restaurant.cuisines = [];
                }
                $scope.restaurant.cuisines.push($scope.newCuisine);
                $scope.newCuisine = '';
            }
        }

        $scope.removeCuisine = function (cuisine) {
            var index = $scope.restaurant.cuisines.indexOf(cuisine);
            $scope.restaurant.cuisines.splice(index, 1);
        }

        $scope.ok = function () {
            $modalInstance.close($scope.restaurant);
        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
    });
})();
'use strict';

var user = angular.module('tipExpert.user');

user.controller('navigationController', ['$scope', '$state', 'authService', 'alertService', function ($scope, $state, authService, alertService) {

    $scope.user = authService.user;

    $scope.logout = function() {
        authService.logout()
            .then(function() {
                $state.go('home');
            })
            .catch(alertService.error);
    };

    $scope.loginOauth = function(provider) {
        $window.location.href = '/auth/' + provider;
    };
    
    $scope.alerts = alertService.alerts;

    $scope.closeAlert = function(index) {
        alertService.closeAlert(index);
    };
}]);
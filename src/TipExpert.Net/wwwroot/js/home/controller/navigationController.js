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

    $scope.login = function() {
        $scope.submitted = true;
        
        if ($scope.loginForm.$invalid)
                return;

        var user = {
            email: $scope.loginForm.email.$modelValue,
            password: $scope.loginForm.password.$modelValue
        };

        authService.login(user)
            .then(function() {
                $state.go('games.overview');
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
'use strict';

var user = angular.module('tipExpert.user');

user.controller('navigationController', ['$scope', '$state', 'authService', 'alertService', function ($scope, $state, authService, alertService) {

    $scope.user = authService.user;

    $scope.logout = function() {
        authService.logout(function () {
            $state.go('home');
        },
        alertService.error);
    };

    $scope.login = function() {
        $scope.submitted = true;
        
        if ($scope.loginForm.$invalid)
                return;

        authService.login({
                //Email: $scope.loginForm.email.$modelValue,
                //Password: $scope.loginForm.password.$modelValue
                Email: 'T@T.COM',
                Password: 'T@T.COM'
            },
            function(res) {
                $state.go('games.overview');
            },
            alertService.error);
    };

    $scope.loginOauth = function(provider) {
        $window.location.href = '/auth/' + provider;
    };
    
    $scope.alerts = alertService.alerts;

    $scope.closeAlert = function(index) {
        alertService.closeAlert(index);
    };
}]);
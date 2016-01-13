'use strict';

var homeModule = angular.module('tipExpert.home');

homeModule.controller('homeController', [
    '$scope', '$state', '$window', '$modal', 'authService', 'alertService',
    function ($scope, $state, $window, $modal, authService, alertService) {

        $scope.user = {
            name: '',
            email: '',
            password: ''
        };

        $scope.openLogin = function () {
            $modal.open({
                templateUrl: '/js/home/views/loginDialog.html',
                controller: 'loginController'
            });
        };

        $scope.openSignup = function() {
            $modal.open({
                templateUrl: '/js/home/views/signUpDialog.html',
                controller: 'signUpController'
            });
        };

        $scope.loginOauth = function(provider) {
            $window.location.href = '/auth/' + provider;
        };
    }
]);
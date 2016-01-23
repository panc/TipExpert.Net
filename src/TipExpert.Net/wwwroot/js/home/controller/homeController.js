'use strict';

var homeModule = angular.module('tipExpert.home');

homeModule.controller('homeController', [
    '$scope', '$state', '$window', '$uibModal',
    function ($scope, $state, $window, $uibModal) {

        $scope.user = {
            name: '',
            email: '',
            password: ''
        };

        $scope.openLogin = function () {
            $uibModal.open({
                templateUrl: '/js/home/views/loginDialog.html',
                controller: 'loginController'
            });
        };

        $scope.openSignup = function() {
            $uibModal.open({
                templateUrl: '/js/home/views/signUpDialog.html',
                controller: 'signUpController'
            });
        };

        $scope.loginOauth = function(provider) {
            $window.location.href = '/auth/' + provider;
        };
    }
]);
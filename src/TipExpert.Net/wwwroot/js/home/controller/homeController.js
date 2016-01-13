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

        $scope.signup = function() {
            $scope.submitted = true;

            if ($scope.submitForm.$invalid)
                return;

            authService.signup($scope.user)
				.then(function() {
					$state.go('games.overview');
				})
				.catch(alertService.error);
        };

        $scope.loginOauth = function(provider) {
            $window.location.href = '/auth/' + provider;
        };
    }
]);
'use strict';

var homeModule = angular.module('tipExpert.home');

homeModule.controller('loginController', [
    '$scope', '$state', '$modalInstance', 'authService', 'alertService',
    function ($scope, $state, $modalInstance, authService, alertService) {

        $scope.user = {
            name: '',
            email: '',
            password: ''
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
                .then(function () {
                    $scope.cancel();
                    $state.go('games.overview');
                })
                .catch(alertService.error);
        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
    }
]);
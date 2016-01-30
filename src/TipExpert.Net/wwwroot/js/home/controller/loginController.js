'use strict';

var homeModule = angular.module('tipExpert.home');

homeModule.controller('loginController', [
    '$rootScope', '$scope', '$state', '$uibModalInstance', 'authService', 'alertService',
    function ($rootScope, $scope, $state, $uibModalInstance, authService, alertService) {

        $scope.user = {
            name: '',
            email: '',
            password: ''
        };

        $scope.login = function() {
            $scope.submitted = true;

            if ($scope.loginForm.$invalid)
                return;

            authService.login($scope.user)
                .then(function () {
                    $scope.cancel();

                    if ($rootScope.returnTo) 
                        $state.go($rootScope.returnTo.state, $rootScope.returnTo.params);
                    else 
                        $state.go('games.overview');

                    $rootScope.returnTo = null;
                })
                .catch(alertService.error);
        };

        $scope.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }
]);
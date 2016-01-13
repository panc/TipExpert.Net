'use strict';

var homeModule = angular.module('tipExpert.home');

homeModule.controller('signUpController', [
    '$scope', '$state', '$modalInstance', 'authService', 'alertService',
    function ($scope, $state, $modalInstance, authService, alertService) {

        $scope.user = {
            name: '',
            email: '',
            password: ''
        };

        $scope.signup = function() {
            $scope.submitted = true;

            if ($scope.signUpForm.$invalid)
                return;

            authService.signup($scope.user)
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
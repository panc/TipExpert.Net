'use strict';

var user = angular.module('tipExpert.user');

user.controller('userProfileController', [
    '$scope', '$stateParams', 'userService', 'alertService', function($scope, $stateParams, userService, alertService) {

        $scope.user = {};
        $scope.hideRole = true;

        if ($stateParams.userId) {
            userService.loadProfile($stateParams.userId)
                .success(function(user) {
                    $scope.user = user;
                    $scope.hideRole = user.role == userConfig.roles.user;
                })
                .error(alertService.error);
        }
    }
]);
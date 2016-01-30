'use strict';

var game = angular.module('tipExpert.game');

game.controller('invitationController', [
    '$scope', '$stateParams', 'gameService', 'alertService', function($scope, $stateParams, gameService, alertService) {

        $scope.user = {};
        $scope.hideRole = true;

        if ($stateParams.token) {

            $scope.token = $stateParams.token;
        }
    }
]);
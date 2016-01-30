'use strict';

var game = angular.module('tipExpert.game');

game.controller('invitationController', [
    '$scope', '$stateParams', '$state', 'gameService', function ($scope, $stateParams, $state, gameService) {

        $scope.user = {};
        $scope.hideRole = true;

        var token = "";
        if ($stateParams.token)
            token = $stateParams.token;

        $scope.acceptInvitation = function() {
            gameService.acceptInvitation(token)
                .then(function() {
                    $state.go('games.overview');
                });
        };
    }
]);
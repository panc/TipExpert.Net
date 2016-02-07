'use strict';

var game = angular.module('tipExpert.game');

game.controller('invitationController', [
    '$scope', '$stateParams', '$state', 'invitationService', 'alertService', 
    function ($scope, $stateParams, $state, invitationService, alertService) {

        $scope.user = {};
        $scope.hideRole = true;

        var token = "";

        if ($stateParams.token) {
            token = $stateParams.token;

            invitationService.getDetails(token)
                .success(function(details) {
                    $scope.details = details;
                })
                .error(function(e) {
                    $state.go('games.overview');
                    alertService.error(e);
                });
        }

        $scope.acceptInvitation = function() {
            invitationService.acceptInvitation(token)
                .success(function(result) {
                    $state.go('games.show', { gameId: result.gameId });
                })
                .error(alertService.error);
        };
    }
]);
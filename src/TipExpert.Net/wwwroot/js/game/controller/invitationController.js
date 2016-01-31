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
                .error(alertService.error);
        }

        $scope.acceptInvitation = function() {
            invitationService.acceptInvitation(token)
                .success(function() {
                    $state.go('games.overview');
                })
                .error(alertService.error);
        };
    }
]);
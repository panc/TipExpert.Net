'use strict';

var game = angular.module('tipExpert.game');

game.controller('myGamesController', [
    '$scope', '$modal', 'gameService', 'alertService', function($scope, $modal, gameService, alertService) {
        $scope.createdGames = [];
        $scope.invitedGames = [];

        $scope.createGame = function() {
            $modal.open({
                templateUrl: '/js/game/views/addGameDialog.html',
                controller: 'addGameController'
            });
        };

        gameService.loadGamesCreatedByCurrentUser()
            .success(function(games) {
                $scope.createdGames = games;
            })
            .error(alertService.error);

        gameService.loadGamesForCurrentUser()
            .success(function(games) {
                $scope.invitedGames = games;
            })
            .error(alertService.error);
    }
]);


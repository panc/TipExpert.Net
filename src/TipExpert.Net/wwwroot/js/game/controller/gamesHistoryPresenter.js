'use strict';

var game = angular.module('tipExpert.game');

game.controller('gamesHistoryController', ['$scope', '$modal', 'gameService', 'alertService', function($scope, $modal, gameService, alertService) {
    $scope.games = [];

    gameService.loadFinishedGamesForCurrentUser()
        .success(function (games) {
            $scope.games = games;
        })
        .error(alertService.error);
}]);


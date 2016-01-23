'use strict';

var game = angular.module('tipExpert.game');

game.controller('gamesHistoryController', ['$scope', '$uibModal', 'gameService', 'alertService', function($scope, $uibModal, gameService, alertService) {
    $scope.games = [];

    gameService.loadFinishedGamesForCurrentUser()
        .success(function (games) {
            $scope.games = games;
        })
        .error(alertService.error);
}]);


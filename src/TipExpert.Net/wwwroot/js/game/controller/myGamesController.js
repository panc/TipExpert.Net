'use strict';

var game = angular.module('tipExpert.game');

game.controller('myGamesController', [
    '$scope', '$uibModal', 'gameService', 'alertService', function ($scope, $uibModal, gameService, alertService) {
        $scope.createdGames = [];
        $scope.invitedGames = [];

        $scope.createGame = function () {
            var game = { isNew: true };
            openEditDialog(game);
        };

        $scope.editGame = function (game) {
            openEditDialog(game);
        };

        function openEditDialog(game) {
            var modalInstance = $uibModal.open({
                templateUrl: '/js/game/views/addOrEditGameDialog.html',
                controller: 'addOrEditGameController',
                resolve: {
                    game: function () {
                        return game;
                    }
                }
            });

            modalInstance.result.then(function () {
                alertService.info('Changes successfully saved!');
                reload();
            }, function () {
                // canceld -> nothing to do
            });
        };

        function reload() {
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
        };

        reload();
    }
]);


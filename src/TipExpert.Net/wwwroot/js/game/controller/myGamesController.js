'use strict';

var game = angular.module('tipExpert.game');

game.controller('myGamesController', [
    '$scope', '$uibModal', '$location', 'gameService', 'alertService', 'gameIdToEdit',
    function ($scope, $uibModal, $location, gameService, alertService, gameIdToEdit) {
        $scope.createdGames = [];
        $scope.invitedGames = [];

        if (gameIdToEdit !== null) {

            gameService.load(gameIdToEdit)
                .success(openEditDialog)
                .error(alertService.error);
        }

        $scope.createGame = function () {
            var game = { isNew: true, title: '<NEW>', id: '000000' };
            openEditDialog(game);
        };

        $scope.editGame = function (game) {
            openEditDialog(game);
        };

        function openEditDialog(game) {

//            var url = $location.path();
//            $location.path(url + '/' + game.id + '/edit');

            var modalInstance = $uibModal.open({
                templateUrl: '/js/game/views/addOrEditGameDialog.html',
                controller: 'addOrEditGameController',
                windowClass: 'edit-game-modal-window',
                resolve: {
                    game: function () {
                        return game;
                    }
                }
            });

            modalInstance.result.then(function () {
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


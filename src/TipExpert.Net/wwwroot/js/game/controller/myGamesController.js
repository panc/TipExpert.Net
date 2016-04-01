'use strict';

var game = angular.module('tipExpert.game');

game.controller('myGamesController', [
    '$scope', '$uibModal', '$location', '$state', 'authService', 'gameService', 'alertService', 'gameIdToEdit', 'gameIdToPlay',
    function ($scope, $uibModal, $location, $state, authService, gameService, alertService, gameIdToEdit, gameIdToPlay) {

        $scope.games = [];
        $scope.userId = authService.user.id;

        if (gameIdToEdit !== null)
            loadGameForEdit(gameIdToEdit, false);
        
        else if (gameIdToPlay !== null)
            loadGameForPlay(gameIdToPlay, false);

        $scope.createGame = function () {
            var game = { isNew: true, title: '<NEW>', id: '000000' };
            openEditGameDialog(game, true);
        };

        $scope.editGame = function (game) {
            loadGameForEdit(game.id, true);
        };

        $scope.playGame = function (game) {
            loadGameForPlay(game.id, true);
        };

        function loadGameForEdit(gameId, updateUrl) {

            gameService.loadForEdit(gameId)
                .success(function(game) { openEditGameDialog(game, updateUrl); })
                .error(function (e) {
                    alertService.error(e);
                    changeUrlToOverview();
                });
        }

        function loadGameForPlay(gameId, updateUrl) {

            gameService.load(gameId)
                .success(function (game) { openPlayGameDialog(game, updateUrl); })
                .error(function (e) {
                    alertService.error(e);
                    changeUrlToOverview();
                });
        }

        function openEditGameDialog(game, updateUrl) {

            if (updateUrl)
                $state.go("games.edit", { gameId: game.id }, { location: true, notify: false, reload: false });
            
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
                changeUrlToOverview();
            }, function () {
                changeUrlToOverview();
            });
        };

        function openPlayGameDialog(game, updateUrl) {

            if (updateUrl)
                $state.go("games.play", { gameId: game.id }, { location: true, notify: false, reload: false });

            var modalInstance = $uibModal.open({
                templateUrl: '/js/game/views/game.html',
                controller: 'gameController',
                windowClass: 'edit-game-modal-window',
                resolve: {
                    game: function () {
                        return game;
                    }
                }
            });

            modalInstance.result.then(function () {
                reload();
                changeUrlToOverview();
            }, function () {
                changeUrlToOverview();
            });
        };

        function changeUrlToOverview() {
            $state.go("games.overview", { gameIdToEdit: null }, { location: true, notify: false, reload: false });
        }

        function reload() {
            gameService.loadGamesForCurrentUser()
                .success(function(games) {
                    $scope.games = games;
                })
                .error(alertService.error);
        };

        reload();
    }
]);


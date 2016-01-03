'use strict';

var game = angular.module('tipExpert.game');

game.controller('editGameController', [
    '$scope', '$state', '$stateParams', '$modal', 'gameService', 'alertService',
    function($scope, $state, $stateParams, $modal, gameService, alertService) {

        $scope.game = {};

        if ($stateParams.gameId) {
            gameService.loadForEdit($stateParams.gameId)
                .success(function (game) {
                    $scope.game = game;
                })
                .error(alertService.error);
        }

        $scope.save = function() {
            $scope.submitted = true;

            if ($scope.submitForm.$invalid)
                return;

            gameService.updateGameData($scope.game)
                .success(function(updatedGame) {
                    $scope.game = updatedGame;
                    alertService.info('Successfully saved!');
                })
                .error(alertService.error);
        };

        $scope.delete = function() {
            if ($scope.game.isFinished) {
                alertService.error('Can not delete a finished game!');
                return;
            }

            // todo: ask whether the game should realy be deleted

            gameService.delete($scope.game.id)
                .success(function() {
                    alertService.info('Successfully deleted!');
                    $state.go('games.overview');
                })
                .error(alertService.error);
        };

        $scope.addMatch = function() {
            var modalInstance = $modal.open({
                templateUrl: '/js/game/views/selectMatchesDialog.html',
                controller: 'selectMatchesController',
                resolve: {
                    game: function() {
                        return $scope.game;
                    }
                }
            });

            modalInstance.result.then(function(updatedGame) {
                $scope.game = updatedGame;
                alertService.info('Changes successfully saved!');

            }, function() {
                // canceld -> nothing to do
            });
        };

        $scope.addPlayer = function() {
            var modalInstance = $modal.open({
                templateUrl: '/js/game/views/selectPlayersDialog.html',
                controller: 'selectPlayersController',
                resolve: {
                    game: function() {
                        return $scope.game;
                    }
                }
            });

            modalInstance.result.then(function(updatedGame) {
                $scope.game = updatedGame;
                alertService.info('Changes successfully saved!');

            }, function() {
                // canceld -> nothing to do
            });
        };
    }
]);

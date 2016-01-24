'use strict';

var game = angular.module('tipExpert.game');

game.controller('addOrEditGameController', [
    '$scope', '$uibModalInstance', '$state', '$location', 'gameService', 'alertService', 'game',
    function ($scope, $uibModalInstance, $state, $location, gameService, alertService, game) {

        game.matchSelectionMode = 'em2016';

        $scope.game = game;
        $scope.isNewGame = game.isNew === true;
        $scope.selectedTab = 1;
        $scope.matchSelectionMode = game.matchSelectionMode;

        var matchSelections = {};

        if (!$scope.isNewGame) {
            gameService.loadForEdit(game.id)
                .success(function (game) {
                    $scope.game = game;
                })
                .error(alertService.error);
        }

        $scope.registerMatchSelection = function (name, getMetadataCallback) {
            matchSelections[name] = {
                getMetadata: getMetadataCallback
            }
        }

        $scope.save = function() {
            $scope.submitted = true;

            if (this.submitForm.$invalid)
                return;

            var ms = matchSelections[$scope.matchSelectionMode];
            if (ms && ms.getMetadata)
                $scope.game.matchesMetadata = ms.getMetadata();

            var result = $scope.isNewGame
                ? gameService.create($scope.game)
                : gameService.updateGameData($scope.game);

            result.success(function(newGame) {
                    alertService.info('Changes successfully saved!');
                    $uibModalInstance.close();
                })
                .error(alertService.error);
        };

        $scope.delete = function () {
            if ($scope.game.isFinished) {
                alertService.error('Can not delete a finished game!');
                return;
            }

            // todo: ask whether the game should realy be deleted

            gameService.delete($scope.game.id)
                .success(function () {
                    alertService.info('Successfully deleted!');
                    $uibModalInstance.close();
                })
                .error(alertService.error);
        };

        $scope.cancel = function() {
            $uibModalInstance.dismiss('cancel');
        };
    }
]);

'use strict';

var game = angular.module('tipExpert.game');

game.controller('addOrEditGameController', [
    '$scope', '$modalInstance', '$state', 'gameService', 'alertService', 'game',
    function($scope, $modalInstance, $state, gameService, alertService, game) {

        $scope.game = game;
        $scope.isNewGame = game.isNew === true;

        if (!$scope.isNewGame) {
            gameService.loadForEdit(game.id)
                .success(function (game) {
                    $scope.game = game;
                })
                .error(alertService.error);
        }

        $scope.save = function() {
            $scope.submitted = true;

            if (this.submitForm.$invalid)
                return;

            var result = $scope.isNewGame
                ? gameService.create($scope.game)
                : gameService.updateGameData($scope.game);

            result.success(function(newGame) {
                    $modalInstance.close();
                })
                .error(alertService.error);
        };

        $scope.cancel = function() {
            $modalInstance.dismiss('cancel');
        };
    }
]);

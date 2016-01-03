'use strict';

var game = angular.module('tipExpert.game');

game.controller('gameController', [
    '$scope', '$modal', '$stateParams', 'gameService', 'alertService',
    function ($scope, $modal, $stateParams, gameService, alertService) {

        $scope.game = {};
        $scope.submitted = true;
        $scope.editStake = false;

        $scope.cancelEditStake = function() {
            $scope.stake = $scope.game.player.stake;
            $scope.editStake = false;
        };

        $scope.saveStake = function() {
            gameService.updateStake($scope.game.id, $scope.game.player.stake)
                .success(function(game) {
                    $scope.game = game;
                    $scope.editStake = false;
                })
                .error(alertService.error);
        };

        $scope.saveTip = function(tip) {

            gameService.updateTip($scope.game.id, tip.match, tip,
                function(homeTip, guestTip) {
                    tip.oldHomeTip = homeTip;
                    tip.oldGuestTip = guestTip;

                    tip.showSaveButton = false;
                },
                alertService.error);
        };

        $scope.cancelTipEditing = function(tip) {

            tip.homeTip = tip.oldHomeTip;
            tip.guestTip = tip.oldGuestTip;

            tip.showSaveButton = false;
        };

        if ($stateParams.gameId) {
            gameService.load($stateParams.gameId)
                .success(function(game) {
                    $scope.game = game;
                })
                .error(alertService.error);
        }
    }
]);
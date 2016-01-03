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

        $scope.saveTip = function(mt) {
            gameService.updateTip($scope.game.id, mt)
                .success(function(game) {
                    $scope.game = game;

                    mt.showSaveButton = false;
                    mt.tipOfPlayer.oldHomeScore = mt.tipOfPlayer.homeScore;
                    mt.tipOfPlayer.oldGuestScore = mt.tipOfPlayer.guestScore;
                })
                .error(alertService.error);
        };

        $scope.cancelTipEditing = function(mt) {
            mt.showSaveButton = false;
            mt.tipOfPlayer.homeScore = mt.tipOfPlayer.oldHomeScore;
            mt.tipOfPlayer.guestScore = mt.tipOfPlayer.oldGuestScore;
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
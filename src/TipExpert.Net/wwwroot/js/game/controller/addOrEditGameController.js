'use strict';

var game = angular.module('tipExpert.game');

game.controller('addOrEditGameController', [
    '$scope', '$uibModalInstance', '$window', 'gameService', 'alertService', 'game',
    function ($scope, $uibModalInstance, $window, gameService, alertService, game) {

        game.matchSelectionMode = 'em2016';

        $scope.game = game;
        $scope.isNewGame = game.isNew === true;
        $scope.selectedTab = 1;
        $scope.matchSelectionMode = game.matchSelectionMode;
        
        var matchSelections = {};

        $scope.registerMatchSelection = function (name, getMetadataCallback) {
            matchSelections[name] = {
                getMetadata: getMetadataCallback
            }
        }

        $scope.invitePlayer = function(userNameOrEmail) {
            var user = {
                email: userNameOrEmail
            };

            if (!$scope.game.invitedPlayers)
                $scope.game.invitedPlayers = [];

            $scope.game.invitedPlayers.push(user);
        };

        $scope.removeInvitation = function(player) {
            if ($window.confirm("Do you really want to delete invitation for '" + player.email + "'?")) {
                var index = $scope.game.invitedPlayers.indexOf(player);
                $scope.game.invitedPlayers.splice(index, 1);
            }
        };

        $scope.removePlayer = function (player) {
            if ($scope.game.creatorId === player.userId) {
                $window.alert("The creator of a game can not be deleted!");
                return;
            }

            if ($window.confirm("Do you really want to delete user '" + player.name + "'?")) {
                var index = $scope.game.players.indexOf(player);
                $scope.game.players.splice(index, 1);
            }
        };

        $scope.save = function() {
            $scope.submitted = true;

            if (this.submitForm.$invalid)
                return;
            
            $scope.game.matchSelectionMode = $scope.matchSelectionMode;

            var ms = matchSelections[$scope.game.matchSelectionMode];
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

'use strict';

var game = angular.module('tipExpert.game');

game.controller('selectPlayersController', [
    '$scope', '$uibModalInstance', 'authService', 'userService', 'gameService', 'alertService', 'game',
    function($scope, $uibModalInstance, authService, userService, gameService, alertService, game) {

        var selectedplayers = game.players.slice(0);
        
        $scope.toggleUserSelection = function(user) {

            // current user always has to be part of the players list
            if (authService.user != null && user.userId === authService.user.id)
                return;

            user.selected = !user.selected;

            if (user.selected) {
                // if selected add the user to the player-list of the game
                selectedplayers.push(user);
            } else {
                // if not selected remove the user from the player-list of the game
                angular.forEach(selectedplayers, function(player) {

                    if (user.userId === player.userId) {
                        var index = selectedplayers.indexOf(player);
                        selectedplayers.splice(index, 1);
                    }
                });
            }
        };

        $scope.save = function() {

            gameService.updatePlayers(game.id, selectedplayers)
                .success(function (updatedGame) {
                    $uibModalInstance.close(updatedGame);
                })
                .error(alertService.error);
        };

        $scope.cancel = function() {
            $uibModalInstance.dismiss('cancel');
        };

        userService.loadFriendsForUser(authService.currentUser)
            .success(function (users) {
                $scope.users = users;

                angular.forEach(users, function(user) {
                    angular.forEach(selectedplayers, function(player) {

                        if (user.userId === player.userId)
                            user.selected = true;
                    });
                });
            })
            .error(alertService.error);
    }
]);

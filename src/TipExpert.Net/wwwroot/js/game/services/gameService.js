'use strict';

var game = angular.module('tipExpert.game');

game.factory('gameService', ['$http', '$q', function ($http, $q) {

    return {
        loadGamesForCurrentUser: function() {
            return $http.get('/api/games/invited');
        },

        loadGamesCreatedByCurrentUser: function() {
            return $http.get('/api/games/created');
        },

        loadFinishedGamesForCurrentUser: function() {
            return $http.get('/api/games/finished');
        },

        load: function(gameId) {
            return $http.get('/api/games/' + gameId);
        },

        loadForEdit: function(gameId) {
            return $http.get('/api/games/' + gameId + '/edit');
        },

        create: function(newGame) {
            return $http.post('/api/games', newGame);
        },

        updateGameData: function (game) {
            return $http.put('/api/games/' + game.id + '/edit/data', game);
        },

        updateStake: function(gameId, newStake) {
            return $http.put('/api/games/' + gameId + '/stake', newStake);
        },

        updateTip: function(gameId, matchTip) {
            return $http.put('/api/games/' + gameId + '/tip', matchTip);
        },

        delete: function(gameId) {
            return $http.delete('/api/games/' + gameId + '/edit');
        },

        acceptInvitation: function(token) {
            return $http.post('/api/invitation/accept', { value: token });
        }
    };
}]);
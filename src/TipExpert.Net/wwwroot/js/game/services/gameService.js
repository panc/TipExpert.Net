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
            var gameData = {
                id: game.id,
                title: game.title,
                description: game.description,
                minStake: game.minStake
            }

            return $http.put('/api/games/' + game.id + '/edit/data', gameData);
        },

        updatePlayers: function (gameId, players) {
            return $http.put('/api/games/' + gameId + '/edit/players', players);
        },

        updateMatches: function(gameId, matches) {
            return $http.put('/api/games/' + gameId + '/edit/matches', matches);
        },

        updateStake: function(gameId, newStake) {
            return $http.put('/api/games/' + gameId + '/stake', newStake);
        },

        updateTip: function(gameId, matchTip) {
            return $http.put('/api/games/' + gameId + '/tip', matchTip);
        },

        delete: function(gameId) {
            return $http.delete('/api/games/' + gameId + '/edit');
        }
    };
}]);
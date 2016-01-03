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

        load: function(gameId, success, error) {
            $http.get('/api/games/' + gameId)
                .success(function(data, status, headers, config) {
                    success(data);
                })
                .error(error);
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

            return $http.put('/api/games/' + game.id + '/data', gameData);
        },

        updatePlayers: function (gameId, players) {
            return $http.put('/api/games/' + gameId + '/players', players);
        },

        updateMatches: function(gameId, matches) {
            return $http.put('/api/games/' + gameId + '/matches', matches);
        },

        updateStake: function(gameId, playerId, newStake, success, error) {
            $http.put('/api/games/' + gameId + '/stake',
                {
                    playerId: playerId,
                    stake: newStake
                })
                .success(function(data, status, headers, config) {
                    success();
                })
                .error(error);
        },

        updateTip: function(gameId, matchId, tip, success, error) {
            $http.put('/api/games/' + gameId + '/tip',
                {
                    tip: tip.id,
                    match: matchId,
                    homeTip: tip.homeTip,
                    guestTip: tip.guestTip
                })
                .success(function(data, status, headers, config) {
                    success(data.homeTip, data.guestTip);
                })
                .error(error);
        },

        delete: function(gameId) {
            return $http.delete('/api/games/' + gameId);
        }
    };
}]);
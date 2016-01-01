'use strict';

var game = angular.module('tipExpert.game');

game.factory('gameService', ['$http', '$q', function ($http, $q) {

    return {
        loadGamesForCurrentUser: function() {
            var deferred = $q.defer();

            $http.get('/api/games/invited')
                .success(deferred.resolve)
                .error(deferred.reject);

            return deferred.promise;
        },

        loadGamesCreatedByCurrentUser: function() {
            var deferred = $q.defer();

            $http.get('/api/games/created')
                .success(deferred.resolve)
                .error(deferred.reject);

            return deferred.promise;
        },

        loadFinishedGamesForCurrentUser: function() {
            var deferred = $q.defer();

            $http.get('/api/games/finished')
                .success(deferred.resolve)
                .error(deferred.reject);

            return deferred.promise;
        },

        load: function(gameId, success, error) {
            $http.get('/api/games/' + gameId)
                .success(function(data, status, headers, config) {
                    success(data);
                })
                .error(error);
        },

        loadForEdit: function(gameId) {
            var deferred = $q.defer();

            $http.get('/api/games/' + gameId + '/edit')
                .success(deferred.resolve)
                .error(deferred.reject);

            return deferred.promise;
        },

        create: function(newGame) {
            var deferred = $q.defer();
            
            $http.post('/api/games', newGame)
                .success(deferred.resolve)
                .error(deferred.reject);

            return deferred.promise;
        },

        updateGameData: function (game) {
            var deferred = $q.defer();

            var gameData = {
                id: game.id,
                title: game.title,
                description: game.description,
                minState: game.minState
            }

            $http.put('/api/games/' + game.id + '/data', gameData)
                .success(deferred.resolve)
                .error(deferred.reject);

            return deferred.promise;
        },

        updatePlayers: function (gameId, players) {
            var deferred = $q.defer();

            $http.put('/api/games/' + gameId + '/players', players)
                .success(deferred.resolve)
                .error(deferred.reject);

            return deferred.promise;
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

        delete: function(game, success, error) {
            $http.delete('/api/games/' + game.id + '/edit')
                .success(success)
                .error(error);
        }
    };
}]);
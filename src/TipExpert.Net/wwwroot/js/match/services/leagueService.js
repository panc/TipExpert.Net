'use strict';

var matchModule = angular.module('tipExpert.match');

matchModule.factory('leagueService', ['$http', '$q', function ($http, $q) {

    return {
        load: function() {
            var deferred = $q.defer();

            $http.get('/api/leagues')
                .success(deferred.resolve)
                .error(deferred.reject);

            return deferred.promise;
        },

        create: function(newLeague, success, error) {
            $http.post('api/leagues/', newLeague)
                .success(function(league) {
                    leagues.push(league);
                    success(league);
                })
                .error(error);
        },

        update: function(league, success, error) {
            $http.put('/api/leagues/' + league.id, league)
                .success(function(newLeague) {
                    success();
                })
                .error(error);
        },

        delete: function(league, error) {
            $http.delete('api/leagues/' + league.id)
                .success(function() {
                    var index = leagues.indexOf(league);
                    leagues.splice(index, 1);
                })
                .error(error);
        }
    };
}]);
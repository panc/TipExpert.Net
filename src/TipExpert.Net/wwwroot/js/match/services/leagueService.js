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

        create: function(newLeague) {
            var deferred = $q.defer();
            
            $http.post('api/leagues/', newLeague)
                .success(deferred.resolve)
                .error(deferred.reject);

            return deferred.promise;
        },

        update: function(league) {
            var deferred = $q.defer();

            $http.put('/api/leagues/' + league.id, league)
                .success(deferred.resolve)
                .error(deferred.reject);

            return deferred.promise;
        },

        delete: function(league) {
            var deferred = $q.defer();

            $http.delete('api/leagues/' + league.id)
                .success(deferred.resolve)
                .error(deferred.reject);

            return deferred.promise;
        }
    };
}]);
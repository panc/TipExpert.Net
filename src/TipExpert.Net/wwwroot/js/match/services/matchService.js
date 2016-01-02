'use strict';

var matchModule = angular.module('tipExpert.match');

matchModule.factory('matchService', ['$http', '$q', function ($http, $q) {

    // todo:
    // use cache for the matches...

    return {
        load: function(league) {
            var deferred = $q.defer();

            $http.get('/api/leagues/' + league.id + '/matches')
                .success(deferred.resolve)
                .error(deferred.reject);

            return deferred.promise;
        },

        loadForSelection: function(league) {
            var deferred = $q.defer();

            $http.get('/api/leagues/' + league.id + '/matches/notfinished')
                .success(deferred.resolve)
                .error(deferred.reject);

            return deferred.promise;
        },
        
        create: function (newMatch) {
            var deferred = $q.defer();

            $http.post('api/matches/', newMatch)
                .success(deferred.resolve)
                .error(deferred.reject);

            return deferred.promise;
        },
        
        update: function (match) {
            var deferred = $q.defer();
            
            $http.put('/api/matches/' + match.id, match)
                .success(deferred.resolve)
                .error(deferred.reject);
          
            return deferred.promise;
        },
        
        delete: function (match) {
            var deferred = $q.defer();
            
            $http.delete('api/matches/' + match.id)
                .success(deferred.resolve)
                .error(deferred.reject);

            return deferred.promise;
        }
    };
}]);
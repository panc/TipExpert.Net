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
        
        create: function (newMatch, success, error) {
            $http.post('api/matches/', newMatch)
                .success(function(match) {
                    success(match);
                })
                .error(error);
        },
        
        update: function (match, success, error) {
            $http.put('/api/matches/' + match.id, match)
                .success(function(updatedMatch) {
                    success(updatedMatch);
                })
                .error(error);
        },
        
        delete: function (match, error) {
            $http.delete('api/matches/' + match.id)
                .error(error);
        }
    };
}]);
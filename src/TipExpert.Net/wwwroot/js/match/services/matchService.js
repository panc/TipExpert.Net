'use strict';

var matchModule = angular.module('tipExpert.match');

matchModule.factory('matchService', ['$http', function($http) {

    // todo:
    // use cache for the matches...

    return {
        load: function(league, success, error) {
            $http.get('/api/leagues/' + league.id + '/matches')
                .success(function(data) {
                    
                    success(data);
                })
                .error(error);
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
'use strict';

var matchModule = angular.module('tipExpert.match');

matchModule.factory('matchService', ['$http', function ($http) {

    return {
        load: function(league) {
            return $http.get('/api/leagues/' + league.id + '/matches');
        },

        loadForSelection: function(league) {
            return $http.get('/api/leagues/' + league.id + '/matches/notfinished');
        },
        
        create: function (newMatch) {
            return $http.post('api/matches/', newMatch);
        },
        
        update: function (match) {
            return $http.put('/api/matches/' + match.id, match);
        },
        
        delete: function (match) {
            return $http.delete('api/matches/' + match.id);
        }
    };
}]);
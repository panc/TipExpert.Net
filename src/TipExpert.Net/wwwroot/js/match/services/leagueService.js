'use strict';

var matchModule = angular.module('tipExpert.match');

matchModule.factory('leagueService', ['$http', function ($http) {

    return {
        load: function() {
            return $http.get('/api/leagues');
        },

        create: function(newLeague) {
            return $http.post('api/leagues/', newLeague);
        },

        update: function(league) {
            return $http.put('/api/leagues/' + league.id, league);
        },

        delete: function(league) {
            return $http.delete('api/leagues/' + league.id);
        }
    };
}]);
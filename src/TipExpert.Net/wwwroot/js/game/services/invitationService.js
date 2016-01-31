'use strict';

var game = angular.module('tipExpert.game');

game.factory('invitationService', ['$http', '$q', function ($http) {

    return {
        getDetails: function(token) {
            return $http.get('/api/invitation/' + token);
        },

        acceptInvitation: function(token) {
            return $http.post('/api/invitation/accept', JSON.stringify(token));
        }
    };
}]);
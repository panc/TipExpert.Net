'use strict';

var user = angular.module('tipExpert.user');

user.factory('userService', ['$http', '$q', function($http, $q) {

    return {
        loadAllUser: function() {
            return $http.get('/api/account');
        },

        loadFriendsForUser: function(user) {
            return $http.get('/api/account/friends');
        },

        loadProfile: function(userId) {
            return $http.get('/api/account/' + userId);
        },

        update: function(usersToSave) {
            var deferred = $q.defer();
            var promises = [];
            
            angular.forEach(usersToSave, function(user) {
                promises.push($http.put('/api/account/' + user.id, user));
            });

            $q.all(promises)
                .then(deferred.resolve)
                .catch(function(result) {
                    deferred.reject(result.data);
                });

            return deferred.promise;
        }
    };
}]);
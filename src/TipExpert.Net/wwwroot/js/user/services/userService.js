'use strict';

var user = angular.module('tipExpert.user');

user.factory('userService', ['$http', '$q', function($http, $q) {

    return {
        loadAllUser: function() {
            var deferred = $q.defer();

            $http.get('/api/account')
                .success(deferred.resolve)
                .error(deferred.reject);

            return deferred.promise;
        },

        loadFriendsForUser: function(user) {
            var deferred = $q.defer();

            $http.get('/api/account/friends')
                .success(deferred.resolve)
                .error(deferred.reject);

            return deferred.promise;
        },

        loadProfile: function(userId) {
            var deferred = $q.defer();

            $http.get('/api/account/' + userId)
                .success(deferred.resolve)
                .error(deferred.reject);

            return deferred.promise;
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
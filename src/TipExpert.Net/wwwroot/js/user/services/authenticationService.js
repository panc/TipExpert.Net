'use strict';

var userModule = angular.module( 'tipExpert.user' );

userModule.factory('authService', ['$http', '$q', '$cookieStore', 'userService', function($http, $q, $cookieStore, userService) {

    var accessLevels = userConfig.accessLevels;
    var userRoles = userConfig.roles;

    var currentUser = $cookieStore.get('user') || { id: '', name: '', role: userRoles.public, email: '' };
    currentUser.isLoggedIn = currentUser.role == userRoles.user || currentUser.role == userRoles.admin;

    var changeUser = function(user) {
        $cookieStore.remove('user');

        currentUser.id = user.id;
        currentUser.name = user.name;
        currentUser.role = user.role;
        currentUser.email = user.email;
        currentUser.picture = user.picture;
        currentUser.coins = user.coins;

        currentUser.isLoggedIn = user.role == userRoles.user || user.role == userRoles.admin;
        currentUser.isAdmin = user.role == userRoles.admin;

        $cookieStore.put('user', currentUser);
    };

    var reloadProfile = function() {
        var id = currentUser.id;
        if (!id || id == '')
            return;

        userService.loadProfile(id)
            .success(changeUser)
            .error(function() {
                console.log('Failed to reload user profile!');
            });
    };

    reloadProfile();

    return {
        authorize: function(accessLevel) {
            var role = currentUser.role;

            if (accessLevel == accessLevels.admin)
                return role == userRoles.admin;

            if (accessLevel == accessLevels.user)
                return role == userRoles.admin || role == userRoles.user;

            return true;
        },

        signup: function(user) {
            var deferred = $q.defer();

            $http.post('/api/account/signup', user)
                .success(function(user) {
                    changeUser(user);
                    deferred.resolve(user);
                })
                .error(deferred.reject);
			
            return deferred.promise;
        },

        login: function (user) {
            var deferred = $q.defer();

            $http.post('/api/account/login', user)
                .success(function(user) {
                    changeUser(user);
                    deferred.resolve(user);
                })
                .error(deferred.reject);

            return deferred.promise;
        },

        logout: function () {
            var deferred = $q.defer();

            $http.post('api/account/logout').success(function () {
                changeUser({
                    username: '',
                    role: userRoles.public
                });

                deferred.resolve();
            })
            .error(deferred.reject);

            return deferred.promise;
        },

        reloadCurrentUserProfile: reloadProfile,
        user: currentUser
    };
}]);
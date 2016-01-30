'use strict';

// define all available modules
angular.module('tipExpert.user', [ 'ngCookies' ]);
angular.module('tipExpert.match', [ ]);
angular.module('tipExpert.game', [ ]);
angular.module('tipExpert.home', [ ]);

var tipExpert = angular.module('tipExpert', ['tipExpert.home', 'tipExpert.user', 'tipExpert.match', 'tipExpert.game', 'ui.bootstrap', 'ui.router', 'pascalprecht.translate', 'ngCookies']);

// configure the main module
tipExpert.config([
    '$stateProvider', '$urlRouterProvider', '$locationProvider', '$httpProvider',
    function($stateProvider, $urlRouterProvider, $locationProvider, $httpProvider) {

        var accessLevels = userConfig.accessLevels;
        var abstractView = {
            'header': {
                templateUrl: '/js/home/views/header.html',
                controller: 'navigationController'
            },
            'main': {
                // Note: abstract still needs a ui-view for its children to populate.
                // We can simply add it inline here.
                template: '<ui-view/>'
            },
            'footer': {
                templateUrl: '/js/home/views/footer.html',
                controller: 'languageController'
            }
        };

        $stateProvider

            // routes for user module
            .state('user', {
                url: '/user',
                views: abstractView,
                abstract: true
            })
            .state('user.overview', {
                title: 'Users',
                url: '',
                templateUrl: '/js/user/views/user.html',
                controller: 'userController',
                access: accessLevels.user
            })
            .state('user.profile', {
                title: 'User profile',
                url: '/:userId',
                templateUrl: '/js/user/views/profile.html',
                controller: 'userProfileController',
                access: accessLevels.user
            })

            // routes for match module
            .state('matches', {
                url: '/matches',
                views: abstractView,
                abstract: true
            })
            .state('matches.overview', {
                title: 'Matches',
                url: '',
                templateUrl: '/js/match/views/matches.html',
                controller: 'matchController',
                access: accessLevels.admin
            })
            .state('matches.finished', {
                title: 'Not implemented yet!',
                url: '/finished',
                templateUrl: 'todo',
                controller: 'finishedMatchesController',
                access: accessLevels.admin
            })

            // routes for game module
            .state('games', {
                url: '/games',
                views: abstractView,
                abstract: true
            })
            .state('games.overview', {
                title: 'Games',
                url: '',
                templateUrl: '/js/game/views/myGames.html',
                controller: 'myGamesController',
                resolve: {
                    gameIdToEdit: function () {
                        return null;
                    }
                },
                access: accessLevels.user // todo
            })
            .state('games.history', {
                title: 'Games History',
                url: '/history',
                templateUrl: '/js/game/views/gamesHistory.html',
                controller: 'gamesHistoryController',
                access: accessLevels.user // todo
            })
            .state('games.create', {
                title: 'Create game',
                url: '/create',
                templateUrl: '/js/game/views/editGame.html',
                controller: 'editGameController',
                access: accessLevels.user // todo
            })
            .state('games.edit', {
                title: 'Edit game',
                url: '/:gameId/edit',
                templateUrl: '/js/game/views/myGames.html',
                controller: 'myGamesController',
                resolve: {
                    gameIdToEdit: ['$stateParams', function ($stateParams) {
                        return $stateParams.gameId;
                    }]
                },
                access: accessLevels.user // todo
            })
            .state('games.show', {
                title: 'Game',
                url: '/:gameId',
                templateUrl: '/js/game/views/game.html',
                controller: 'gameController',
                access: accessLevels.user // todo
            })

            // routes for invitation module
            .state('invitation', {
                url: '/invitation',
                views: abstractView,
                abstract: true
            })
            .state('invitation.token', {
                title: 'Invitation',
                url: '/:token',
                templateUrl: '/js/game/views/invitation.html',
                controller: 'invitationController',
                access: accessLevels.user // todo
            })

            // routes for home module
            .state('home', {
                title: 'Home',
                url: '/',
                views: {
                    'header': {
                        templateUrl: '/js/home/views/landingPageHeader.html',
                        controller: 'navigationController',
                    },
                    'main': {
                        templateUrl: '/js/home/views/landingPage.html',
                        controller: 'homeController'
                    },
                    'footer': {
                        templateUrl: '/js/home/views/footer.html',
                        controller: 'languageController'
                    }
                },
                access: accessLevels.public
            });

        $urlRouterProvider.otherwise('/');

        $locationProvider.html5Mode({
            enabled: true,
            requireBase: false
        });

        $httpProvider.interceptors.push([
            '$q', '$location', function($q, $location) {
                return {
                    'responseError': function(response) {
                        if (response.status === 401 || response.status === 403) {
                            $location.path('/');
                            return $q.reject(response);
                        } else {
                            return $q.reject(response);
                        }
                    }
                };
            }
        ]);
    }
]);

tipExpert.config([
    '$translateProvider', function($translateProvider) {

        $translateProvider
            .fallbackLanguage('en')
            .registerAvailableLanguageKeys(['en', 'de'], {
                'en_US': 'en',
                'en_UK': 'en',
                'de_DE': 'de',
                'de_CH': 'de',
                'de_AT': 'de'
            });

        $translateProvider.useCookieStorage();
        $translateProvider.determinePreferredLanguage();

        var origin = window.location.origin || window.location.protocol + '//' + window.location.host;

        $translateProvider.useStaticFilesLoader({
            prefix: origin + '/locales/',
            suffix: '.json'
        });
    }
]);

tipExpert.run([
    '$rootScope', '$location', '$state', 'authService', 'alertService', 
    function($rootScope, $location, $state, authService, alertService) {

        $rootScope.$state = $state;

        $rootScope.$on('$stateChangeStart', function(event, toState, toParams, fromState, fromParams) {

            var isLoggedIn = authService.user.isLoggedIn;

            if (!authService.authorize(toState.access)) {
                event.preventDefault();

                if (!isLoggedIn) {
                    $rootScope.returnTo = {
                        state: toState.name,
                        params: toParams
                    };

                    $state.go('home');
                } else {
                    if (!fromState.controller)
                        $state.go('games.overview');

                    alertService.error('You are not allowed to access this page');
                }
            } else if (toState.name == 'home' && isLoggedIn) {

                event.preventDefault();
                $state.go('games.overview');
            }
        });
    }
]);

'use strict';

var game = angular.module('tipExpert.game');

game.directive('teMatchSelectionLeagues',
    function() {
        return {
            restrict: 'E',
            scope: {
                game: '='
            },
            templateUrl: '/js/game/directives/matchSelectionLeagues.html',
            controller: ['$element', '$scope',
                function($element, $scope) {
                    
                }
            ]
        };
    }
);

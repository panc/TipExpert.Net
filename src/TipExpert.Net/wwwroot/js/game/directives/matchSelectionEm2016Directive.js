'use strict';

var game = angular.module('tipExpert.game');

game.directive('teMatchSelectionEm2016', 
    function() {
        return {
            restrict: 'E',
            scope: {
                game: '='
            },
            templateUrl: '/js/game/directives/matchSelectionEm2016.html',
            controller: ['$element', '$scope',
                function($element, $scope) {
                    
                }
            ]
        };
    }
);

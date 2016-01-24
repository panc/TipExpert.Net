'use strict';

var game = angular.module('tipExpert.game');

game.directive('teMatchSelectionEm2016', 
    function() {
        return {
            restrict: 'E',
            scope: {
                game: '=',
                registerMatchSelection: '=',
            },
            templateUrl: '/js/game/directives/matchSelectionEm2016.html',
            controller: ['$element', '$scope',
                function($element, $scope) {

                    $scope.registerMatchSelection('em2016', updateMetadata);

                    $scope.selection = $scope.game.matchesMetadata == null
                        ? getDefaultSelection()
                        : JSON.parse($scope.game.matchesMetadata);


                    function updateMetadata() {
                        return JSON.stringify($scope.selection);
                    }

                    function getDefaultSelection() {
                        return {
                            groupA: false, 
                            groupB: false,
                            groupC: false,
                            groupD: false,
                            groupE: false,
                            groupF: false,
                            roundOfLast16: false,
                            quaterFinal: false,
                            semiFinal: false,
                            final: false
                        };
                    }
                }
            ]
        };
    }
);

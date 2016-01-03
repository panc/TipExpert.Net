'use strict';

var game = angular.module('tipExpert.game');

game.controller('selectMatchesController', [
    '$scope', '$modalInstance', 'leagueService', 'matchService', 'gameService', 'alertService', 'game',
    function ($scope, $modalInstance, leagueService, matchService, gameService, alertService, game) {

        var selectedMatches = game.matches.slice(0);

        $scope.loadMatches = function(league) {
            matchService.loadForSelection(league)
                .success(function (matches) {
                    $scope.matches = matches;

                    angular.forEach(matches, function(match) {

                        angular.forEach(selectedMatches, function(selectedMatch) {

                            if (match.id === selectedMatch.matchId)
                                match.selected = true;
                        });
                    });

                })
                .error(alertService.error);
        };

        $scope.toggleMatchSelection = function(match) {
            match.selected = !match.selected;

            if (match.selected) {
                // if selected add the match to the match-list of the game
                selectedMatches.push({ matchId: match.id });
            } else {
                // if not selected remove the match from the match-list of the game
                angular.forEach(selectedMatches, function (selectedMatch) {

                    if (match.id === selectedMatch.matchId) {
                        var index = selectedMatches.indexOf(selectedMatch);
                        selectedMatches.splice(index, 1);
                    }
                });
            }
        };

        $scope.save = function() {

            gameService.updateMatches(game.id, selectedMatches)
                .success(function (updatedGame) {
                    $modalInstance.close(updatedGame);
                })
                .error(alertService.error);
        };

        $scope.cancel = function() {
            $modalInstance.dismiss('cancel');
        };

        leagueService.load()
            .success(function (leagues) {
                $scope.leagues = leagues;

                if (leagues.length > 0) {
                    $scope.league = leagues[0];
                    $scope.loadMatches($scope.league);
                }
            })
            .error(alertService.error);
    }
]);

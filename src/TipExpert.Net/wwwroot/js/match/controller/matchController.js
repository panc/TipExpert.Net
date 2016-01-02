'use strict';

var match = angular.module('tipExpert.match');

match.controller('matchController', [
    '$scope', '$modal', 'leagueService', 'matchService', 'alertService', function($scope, $modal, leagueService, matchService, alertService) {

        $scope.selectedMatch = { homeTeam: '', guestTeam: '', dueDate: new Date() };
        $scope.leagues = [];
        $scope.newLeague = { name: '' };

        // leagues

        var reloadLeagues = function() {
            leagueService.load()
                .success(function(leagues) {
                    $scope.leagues = leagues;

                    angular.forEach($scope.leagues, function(league) {
                        league.editorEnabled = false;
                    });

                    if ($scope.leagues.length > 0)
                        $scope.loadMatches($scope.leagues[0]);
                })
                .error(alertService.error);
        }

        $scope.addLeague = function() {

            leagueService.create($scope.newLeague)
                .success(function (league) {
                    league.editorEnabled = false;
                    $scope.leagues.push(league);

                    $scope.newLeague = "";
                })
                .error(alertService.error);
        };

        $scope.removeLeague = function(league) {
            leagueService.delete(league)
                .success(reloadLeagues)
                .error(alertService.error);
        };

        $scope.editLeague = function(league) {
            league.editorEnabled = true;
            league.editableName = league.name;
        };

        $scope.cancelEditLeague = function(league) {
            league.editorEnabled = false;
        };

        $scope.saveLeague = function(league) {
            league.name = league.editableName;

            leagueService.update(league)
                .success(function (updatedLeague) {
                    league.editorEnabled = false;
                })
                .error(alertService.error);
        };

        // matches

        $scope.loadMatches = function(league) {
            $scope.selectedLeague = league;

            matchService.load(league)
                .success(function (matches) {
                    $scope.matches = matches;
                })
                .error(alertService.error);
        };

        $scope.addMatch = function() {
            var match = {
                homeTeam: '',
                guestTeam: '',
                dueDate: Date.now(),
                leagueId: $scope.selectedLeague.id
            };

            $scope.editMatch(match);
        };

        $scope.editMatch = function(match) {
            var modalInstance = $modal.open({
                templateUrl: '/js/match/views/editMatchDialog.html',
                controller: 'editMatchController',
                resolve: {
                    match: function() {
                        return match;
                    }
                }
            });

            modalInstance.result
                .then(function(newOrUpdatedMatch) {
                    $scope.loadMatches($scope.selectedLeague);
                }, function() {
                    // canceld -> nothing to do
                });
        };

        reloadLeagues();
    }
]);
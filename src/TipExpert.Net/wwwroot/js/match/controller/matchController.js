'use strict';

var match = angular.module('tipExpert.match');

match.controller('matchController', [
    '$scope', '$modal', 'leagueService', 'matchService', 'alertService', function($scope, $modal, leagueService, matchService, alertService) {

        $scope.selectedMatch = { homeTeam: '', guestTeam: '', dueDate: new Date() };
        $scope.leagues = [];
        $scope.newLeague = { name: '' };

        // leagues

        $scope.addLeague = function() {

            leagueService.create($scope.newLeague,
                    function(league) {
                        league.editorEnabled = false;
                        $scope.newLeague = "";
                    }),
                function(data) {
                    // todo   
                    alert(data);
                };
        };

        $scope.removeLeague = function(league) {
            leagueService.delete(league,
                function(data) {
                    // todo
                    alert(data);
                });
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

            leagueService.update(league,
                function() {
                    league.editorEnabled = false;
                },
                alertService.error);
        };

        // matches

        var showEditMatchDialog = function(match, onSavedCallback) {
            var modalInstance = $modal.open({
                templateUrl: '/js/match/views/editMatchDialog.html',
                controller: 'editMatchController',
                resolve: {
                    match: function() {
                        return match;
                    }
                }
            });

            modalInstance.result.then(function(newOrUpdatedMatch) {

                if (onSavedCallback)
                    onSavedCallback(newOrUpdatedMatch);

            }, function() {
                // canceld -> nothing to do
            });
        };

        $scope.loadMatches = function(league) {
            $scope.selectedLeague = league;

            matchService.load(league)
                .then(function(matches) {
                    $scope.matches = matches;
                })
                .catch(alertService.error);
        };

        $scope.addMatch = function() {
            var match = {
                homeTeam: '',
                guestTeam: '',
                dueDate: Date.now(),
                leagueId: $scope.selectedLeague.id
            };

            showEditMatchDialog(match, function(newMatch) {
                $scope.matches.push(newMatch);
            });
        };

        $scope.editMatch = function(match) {
            showEditMatchDialog(match);
        };

        leagueService.load()
            .then(function(leagues) {
                $scope.leagues = leagues;

                angular.forEach($scope.leagues, function(league) {
                    league.editorEnabled = false;
                });

                if ($scope.leagues.length > 0)
                    $scope.loadMatches($scope.leagues[0]);
            })
            .catch(alertService.error);
    }
]);
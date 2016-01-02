'use strict';

/* Use the Match module */

var match = angular.module('tipExpert.match');

match.controller('editMatchController', [
    '$scope', '$modalInstance', 'matchService', 'alertService', 'match', function($scope, $modalInstance, matchService, alertService, match) {

        $scope.match = match;

        $scope.save = function() {

            var promise = (match.id)
                ? matchService.update(match)
                : matchService.create(match);

            promise.success(function (newOrUpdatedMatch) {
                    $modalInstance.close(newOrUpdatedMatch);
                })
                .error(alertService.error);
        };

        $scope.cancel = function() {
            $modalInstance.dismiss('cancel');
        };
    }
]);
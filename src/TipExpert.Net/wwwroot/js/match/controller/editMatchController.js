'use strict';

/* Use the Match module */

var match = angular.module('tipExpert.match');

match.controller('editMatchController', [
    '$scope', '$uibModalInstance', 'matchService', 'alertService', 'match', function($scope, $uibModalInstance, matchService, alertService, match) {

        $scope.match = match;

        $scope.save = function() {

            var promise = (match.id)
                ? matchService.update(match)
                : matchService.create(match);

            promise.success(function (newOrUpdatedMatch) {
                    $uibModalInstance.close(newOrUpdatedMatch);
                })
                .error(alertService.error);
        };

        $scope.cancel = function() {
            $uibModalInstance.dismiss('cancel');
        };
    }
]);
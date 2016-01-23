'use strict';

var user = angular.module('tipExpert.user');

user.factory('alertService', ['$timeout', function($timeout) {

    var alerts = [];

    addAlert("tst", "success");
    addAlert("tst", "success");
    addAlert("tst", "success");
    addAlert("tst", "success");

    function addAlert(message, type) {
        var text = 'An error occoured';
		
		if (message != undefined) {
			if (message.msg != undefined)
				text = message.msg;
			else if (message.errors != undefined)
				text = message.errors[0];
			else
				text = message;
		}
			
        var item = { msg: text, type: type };
        alerts.push(item);
    };

    function closeAlert(item) {
        if (!item)
            return;

        if (item.timeout)
            $timeout.cancel(item.timeout);

        var index = alerts.indexOf(item);
        alerts.splice(index, 1);
    };

    return {
        alerts: alerts,

        error: function(message) {
            addAlert(message, 'warning');
        },

        info: function(message) {
            addAlert(message, 'success');
        },

        closeAlert: function (item) {
            closeAlert(item);
        }
    };
}]);
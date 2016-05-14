var gabsApp = angular.module("gabsApp", ['ngResource', 'ui.bootstrap']);


gabsApp.controller("MeasureCtrl", ['$scope', '$http', function ($scope, $http) {
    $scope.measures = {
        items: [{
            length: '',
            measurename: '',
            width: ''
        }]
    };

    $scope.addItem = function () {
        $scope.measures.items.push({
            length: '',
            measurename: '',
            width: ''
        });
    },

    $scope.removeItem = function (index) {
        $scope.measures.items.splice(index, 1);
    },

    $scope.total = function () {
        var total = 0;
        angular.forEach($scope.measures.items, function (item) {
            total += (item.length * item.width) / 144;
        })
        $scope.Math = window.Math;
        return Math.ceil(total);
    }

    $scope.TotalInput = function () {
        if ($scope.total() > 0)
            return true;
        else
            return false;
    };

    $scope.showhide = true;
    $scope.toggleShowhide = function () {
        $scope.showhide = $scope.showhide === false ? true : false;
    };

}]);

gabsApp.controller("SinkCtrl", ['$scope', '$http', function ($scope, $http) {

    $scope.sinks = {
        items: [{
            sinkID: '',
            sinkqty: '1',
        }]
    };

    $scope.addItem = function () {
        $scope.sinks.items.push({
            sinkID: '',
            sinkqty: '1',
        });
    },

    $scope.setDdlSink = function (index, SinkID) {
        //$scope.sinks.items.item.sinkID
    },

    $scope.removeItem = function (index) {
        $scope.sinks.items.splice(index, 1);
    },

    $scope.showhide = true;
    $scope.toggleShowhide = function () {
        $scope.showhide = $scope.showhide === false ? true : false;
    };

}]);


gabsApp.filter('ceil', function () {
    return function (input) {
        return Math.ceil(input);
    };
});

gabsApp.directive('ngModelOnblur', function() {
    return {
        restrict: 'A',
        require: 'ngModel',
        priority: 1,
        link: function(scope, elm, attr, ngModelCtrl) {
            if (attr.type === 'radio' || attr.type === 'checkbox') return;
            
            elm.unbind('input').unbind('keydown').unbind('change');
            elm.bind('blur', function() {
                scope.$apply(function() {
                    ngModelCtrl.$setViewValue(elm.val());
                });         
            });
        }
    };
});

gabsApp.directive('elastic', [
    '$timeout',
    function ($timeout) {
        return {
            restrict: 'A',
            link: function ($scope, element) {
                var resize = function () {
                    return element[0].style.height = "" + element[0].scrollHeight + "px";
                };
                element.on("blur keyup change", resize);
                $timeout(resize, 0);
            }
        };
    }
]);
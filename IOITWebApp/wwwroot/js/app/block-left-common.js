
myApp.controller('BlockLeftCommonController', ['$scope', '$http', '$mdDialog', '$mdToast', 'config', 'cfpLoadingBar', 'md5', '$window', 'app', '$rootScope', function BlockLeftCommonController($scope, $http, $mdDialog, $mdToast, config, cfpLoadingBar, md5, $window, app, $rootScope) {
    $scope.IsLogin = false;
    $scope.init = function () {
        if (app.data.CustomerId === -1 && app.data.CustomerId === undefined) {
            $scope.IsLogin = true;
        }
    };

    $rootScope.$on("UpdateCart", function () {
        var quantity = JSON.parse($window.localStorage.getItem("Order"));
        if (quantity !== undefined) {
            $scope.quantity = quantity.length;
        }
        else {
            $scope.quantity = 0;
        }
    });
}]);

myApp.controller('OwnerFarmController', ['$scope', '$http', '$mdDialog', '$mdToast', 'config', 'cfpLoadingBar', 'md5', '$window', 'app', function OwnerFarmController($scope, $http, $mdDialog, $mdToast, config, cfpLoadingBar, md5, $window, app) {
    $scope.init = function () {
        $scope.customerId = app.data.CustomerId;
    };

    $scope.goOwnerFarm = function (id) {
        $window.location.href = '/chu-trai-ca-' + id + '.html';
    };

    $scope.goListFishFarm = function () {
        $window.location.href = '/danh-sach-trai-ca-1.html';
    };
}]);

//myApp.controller('KoiController', ['$scope', '$http', '$mdDialog', '$mdToast', 'config', 'cfpLoadingBar', 'md5', '$window', 'app', function KoiController($scope, $http, $mdDialog, $mdToast, config, cfpLoadingBar, md5, $window, app) {
//    $scope.init = function (arr) {
//        $scope.arr = arr;
//        $scope.koi = {};
//    }

//    $scope.ShowKoi = function (ManufacturerId, Name) {
//        $scope.Name = Name;
//        for (var i = 0; i < $scope.arr.length; i++) {
//            if (ManufacturerId == $scope.arr[i].ManufacturerId) {
//                $scope.koi = $scope.arr[i];
//                break;
//            }
//        }
//    }
//}]);
myApp.controller('OrderResultsController', ['$scope', '$http', '$mdDialog', '$mdToast', 'config', 'cfpLoadingBar', 'md5', '$window', 'app', '$rootScope', '$uibModal', function OrderResultsController($scope, $http, $mdDialog, $mdToast, config, cfpLoadingBar, md5, $window, app, $rootScope, $uibModal) {
    $scope.CheckRule = false;
    $scope.order = {};
    $scope.regexEmail = config.regexEmail;
    $scope.regexPhone = config.regexPhone;
    $scope.disableBtn = { btLogin: false };

    $scope.init = function () {
        $scope.giatridonhang = JSON.parse($window.localStorage.getItem("SumPriceOrder"));
        console.log($scope.giatridonhang);
    }

}]);
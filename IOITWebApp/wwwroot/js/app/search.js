
myApp.controller('SearchController', ['$scope', '$http', '$mdDialog', 'config', 'cfpLoadingBar', 'app', '$cookies', '$rootScope', '$window', function SearchController($scope, $http, $mdDialog, config, cfpLoadingBar, app, $cookies, $rootScope, $window) {
    $scope.page = 1;
    $scope.page_size = 10;
    $scope.query = "1=1";
    $scope.q = {};
    $scope.orderby = "";
    $scope.item_count = 0;

    $scope.init = function () {
        cfpLoadingBar.start();
        console.log($scope.sName);
        console.log($scope.sType);
        console.log($scope.categoryId);
        //$scope.query=
        //if ($scope.sType === 1)
        //    $scope.loadNews();
        //else if ($scope.sType === 2)
            $scope.loadProduct();
        //else if ($scope.sType === 3)
        //    $scope.loadKoi();
        //else if ($scope.sType === 4)
        //    $scope.loadNews();
        //else 
        //    $scope.loadNews();
    };

    //$scope.loadNews = function () {
    //    $scope.query = ($scope.sName !== undefined && $scope.sName !== "") ? 'Title.Contains(\"' + $scope.sName + '\")' : "1!=1";
    //    $http.get("/web/search/news/"+ $scope.sType +"?page=" + $scope.page + "&page_size=" + $scope.page_size + "&query=" + $scope.query + "&order_by=" + $scope.orderby, {
    //        headers: {}
    //    }).then(function (data, status, headers) {
    //        cfpLoadingBar.complete();
    //        if (data.data.meta.error_code === 200) {
    //            $scope.item_count = data.data.metadata;
    //            $scope.metadata = data.data.metadata;
    //            $scope.listNews = data.data.data;
    //        }
    //    });
    //};

    $scope.loadProduct = function () {
        //if ($scope.categoryId !== -1)
        //    $scope.query = ($scope.sName !== undefined && $scope.sName !== "") ? 'Name.Contains(\"' + $scope.sName + '\") And CategoryId=' + $scope.categoryId : '1!=1';
        //else
        //    $scope.query = ($scope.sName !== undefined && $scope.sName !== "") ? 'Name.Contains(\"' + $scope.sName + '\")' : '1!=1';
        $scope.search = $scope.sName !== undefined && $scope.sName !== "" ? $scope.sName : '';
        if ($scope.categoryId !== -1)
            $scope.query = 'CategoryId=' + $scope.categoryId;
        else
            $scope.query = '1=1';
        $http.get("/web/search/product?page=" + $scope.page + "&page_size=" + $scope.page_size + "&query=" + $scope.query + "&search=" + $scope.search +"&order_by=" + $scope.orderby, {
            headers: {}
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.item_count = data.data.metadata;
                $scope.metadata = data.data.metadata;
                $scope.listProduct = data.data.data;
            }
        });
    };

    //$scope.loadKoi = function () {
    //    $scope.query = ($scope.sName !== undefined && $scope.sName !== "") ? 'Name.Contains(\"' + $scope.sName + '\") And TypeProduct=2' : "1!=1";
    //    $http.get("/web/search/product?page=" + $scope.page + "&page_size=" + $scope.page_size + "&query=" + $scope.query + "&order_by=" + $scope.orderby, {
    //        headers: {}
    //    }).then(function (data, status, headers) {
    //        cfpLoadingBar.complete();
    //        if (data.data.meta.error_code === 200) {
    //            $scope.item_count = data.data.metadata;
    //            $scope.metadata = data.data.metadata;
    //            $scope.listKoi = data.data.data;
    //        }
    //    });
    //};

    $scope.ParseNumberToArray = function () {
        var floor = Math.floor($scope.item_count / $scope.page_size);
        var LayDu = $scope.item_count % $scope.page_size;
        floor = LayDu > 0 ? floor + 1 : floor;
        floor = floor === 0 ? 1 : floor;
        $scope.NumberOfPage = floor;
        return new Array(floor);
    };

    $scope.ChangePage = function (cs, page) {
        switch (cs) {
            case 1:
                $scope.page = $scope.page - 1;
                break;
            case 2:
                $scope.page = page;
                break;
            case 3:
                $scope.page = $scope.page + 1;
                break;
            case 4:
                $scope.page = $scope.page - 1;
                break;
            default:
                break;
        }

        //if ($scope.sType === 1)
        //    $scope.loadNews();
        //else if ($scope.sType === 2)
        //    $scope.loadProduct();
        //else if ($scope.sType === 3)
        //    $scope.loadKoi();
        //else if ($scope.sType === 4)
        //    $scope.loadNews();
        $scope.loadProduct();
    };

    $scope.AddProductOrder = function (product) {
        $scope.order = JSON.parse($window.localStorage.getItem("Order"));
        product.quantity = 1;
        if ($scope.order !== undefined) {
            var loop = false;
            angular.forEach($scope.order, function (item, key) {
                if (item.ProductId === product.ProductId) {
                    item.quantity = item.quantity + product.quantity;
                    loop = true;
                }
            });

            if (!loop) $scope.order.push(product);
        }
        else {
            $scope.order = [];
            $scope.order.push(product);
        }

        $window.localStorage.setItem("Order", JSON.stringify($scope.order));
        $rootScope.$emit("UpdateCountOrder", {});
        $window.location.href = '/gio-hang.html';
    }
}]);
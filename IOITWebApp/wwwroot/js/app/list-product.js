
myApp.controller('ListProductController', ['$scope', '$http', '$mdDialog', '$mdToast', 'config', 'cfpLoadingBar', 'md5', '$window', 'app', '$rootScope', '$uibModal', function ListProductController($scope, $http, $mdDialog, $mdToast, config, cfpLoadingBar, md5, $window, app, $rootScope, $uibModal) {
    $scope.page = 1;
    $scope.page_size = 12;
    $scope.query = "1=1";
    $scope.q = {};
    $scope.orderby = "";
    $scope.item_count = 0;

    $scope.init = function (CategoryId) {

        $scope.listOrder = JSON.parse($window.localStorage.getItem("Order"));
        $scope.giatridonhang = JSON.parse($window.localStorage.getItem("SumPriceOrder"));

        var quantity = JSON.parse($window.localStorage.getItem("Order"));

        if (quantity !== null) {
            $scope.quantity = quantity.length;
        }


        if (app.data.CustomerId === -1 || app.data.CustomerId === undefined) {
            $scope.CustomerId = undefined;
        }
        else $scope.CustomerId = app.data.CustomerId;

        $scope.CategoryId = CategoryId;
       // this.GetProductByCate();
    };

    $scope.AddProductOrder = function (product) {
        $scope.order = JSON.parse($window.localStorage.getItem("Order"));
        product.quantity = 1;
        if ($scope.order !== null) {
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
    };

    // add product
    $scope.AddProduct = function (item) {
    
        $scope.order = [];
        $scope.order = JSON.parse($window.localStorage.getItem("Order"));
        console.log($scope.order);
        if ($scope.order != null || $scope.order != undefined) {
            $scope.order = JSON.parse($window.localStorage.getItem("Order"));
            console.log($scope.order);
            $scope.dem = 0;
            for (let i = 0; i < $scope.order.length; i++) {
                if ($scope.order[i].ProductId === item.ProductId) {
                    $scope.dem = $scope.dem + 1;
                    $scope.order[i].quantity = $scope.order[i].quantity + 1;
                    $mdToast.show($mdToast.simple()
                        .theme("success-toast")
                        .textContent('Thêm vào giỏ hàng thành công!')
                        .position('fixed bottom right')
                        .hideDelay(3500));

                }
            }
            console.log($scope.dem);
            if ($scope.dem < 1) {
                item.quantity = 1;
                $scope.order.push(item);
                $mdToast.show($mdToast.simple()
                    .theme("success-toast")
                    .textContent('Thêm vào giỏ hàng thành công!')
                    .position('fixed bottom right')
                    .hideDelay(3500));
            }
            $window.localStorage.setItem("Order", JSON.stringify($scope.order));
            $rootScope.$emit("UpdateCountOrder", {});
            console.log($scope.order);
        } else {
            $scope.order = [];
            item.quantity = 1;
            $scope.order.push(item);
            $mdToast.show($mdToast.simple()
                .theme("success-toast")
                .textContent('Thêm vào giỏ hàng thành công!')
                .position('fixed bottom right')
                .hideDelay(3500));
            $window.localStorage.setItem("Order", JSON.stringify($scope.order));
            $rootScope.$emit("UpdateCountOrder", {});
            console.log($scope.order);
        }
    }
    $scope.LoveProduct = function (ProductId,type) {
        if ($scope.CustomerId === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng đăng nhập để thực hiện chức năng này!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;
        }

        cfpLoadingBar.start();

        var post = $http({
            method: 'POST',
            url: '/web/customer/LoveProduct/' + $scope.CustomerId + "/" + ProductId,
            data: undefined,
            headers: { 'Authorization': 'bearer ' + app.data.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                console.log(type)
                if (type == 10) {
                    var idd = 'active_' + ProductId;
                    var idd1 = 'loveactive_' + ProductId;
                    document.getElementById(idd).style.display = "block";
                    document.getElementById(idd1).style.display = "none";

                }
                if (type == 1) {
                    var iddd = 'active_' + ProductId;
                    var iddd1 = 'loveactive_' + ProductId;
                 
                    document.getElementById(iddd).style.display = "none";
                    document.getElementById(iddd1).style.display = "block"; 

                }
                if (type == 11) {
                    var classw1 = 'yeuthich1_' + ProductId;
                    var classw2 = 'yeuthich2_' + ProductId;
                    document.getElementsByClassName(classw1)[0].style.display = "none";
                    document.getElementsByClassName(classw2)[0].style.display = "block";

                }
                $mdToast.show($mdToast.simple()
                    .theme("success-toast")
                    .textContent(data.meta.error_message)
                    .position('fixed bottom right')
                    .hideDelay(3500));
                
            }
            else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent(data.meta.error_message)
                        .ok('Đóng')
                        .fullscreen(true)
                );
            }
        }).error(function (data, status, headers, config) {
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xả ra lỗi! Xin vui lòng thử lại sau!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
        });
    };

    $scope.RemoveLikeProduct = function (ProductId, type) {
        if ($scope.CustomerId === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng đăng nhập để thực hiện chức năng này!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;
        }

        cfpLoadingBar.start();

        var post = $http({
            method: 'POST',
            url: '/web/customer/RemoveLoveProduct/' + app.data.CustomerId + "/" + ProductId,
            data: undefined,
            headers: { 'Authorization': 'bearer ' + app.data.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                $mdToast.show($mdToast.simple()
                    .theme("success-toast")
                    .textContent(data.meta.error_message)
                    .position('fixed bottom right')
                    .hideDelay(3500));

                
                if (type == 10) {
                    var idd = 'active_' + ProductId;
                    var idd1 = 'loveactive_' + ProductId;
                   
                    document.getElementById(idd).style.display = "none";
                    document.getElementById(idd1).style.display = "block";

                }
                if (type == 1) {
                    var idd = 'active_' + ProductId;
                    var idd1 = 'loveactive_' + ProductId;
                    document.getElementById(idd).style.display = "block";
                    document.getElementById(idd1).style.display = "none";

                }
                if (type == 20) {
                    location.reload();

                }
                if (type == 11) {
                    var classw1 = 'yeuthich1_' + ProductId;
                    var classw2 = 'yeuthich2_' + ProductId;
                    document.getElementsByClassName(classw1)[0].style.display = "block";
                    document.getElementsByClassName(classw2)[0].style.display = "none";

                }
                // $scope.GetLikeProducts(app.data.CustomerId);

            }
            else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent(data.meta.error_message)
                        .ok('Đóng')
                        .fullscreen(true)
                );
            }
        }).error(function (data, status, headers, config) {
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xả ra lỗi! Xin vui lòng thử lại sau.')
                    .ok('Đóng')
                    .fullscreen(true)
            );
        });
    };

    $scope.GetProductByCate = function () {
        $http.get("/web/product/GetByPageCateProduct/" + $scope.CategoryId + "?page=" + $scope.page + "&page_size=" + $scope.page_size + "&query=" + $scope.query + "&order_by=" + $scope.orderby, {
            headers: {}
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listProduct = data.data.data;
                $scope.item_count = data.data.metadata;
            }
        });
    };

    $scope.QueryChange = function (ManufacturerId) {
        $scope.query = ManufacturerId !== 0 ? "ManufacturerId = " + ManufacturerId : "1=1";
        $scope.GetProductByCate();
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
            default:
                break;
        }

        $scope.GetProductByCate();
    };

    $scope.ParseNumberToArray = function () {
        var floor = Math.floor($scope.item_count / $scope.page_size);
        var LayDu = $scope.item_count % $scope.page_size;
        floor = LayDu > 0 ? floor + 1 : floor;
        floor = floor === 0 ? 1 : floor;
        $scope.NumberOfPage = floor;
        return new Array(floor);
    };
}]);

myApp.controller('ListKoiController', ['$scope', '$http', '$mdDialog', '$mdToast', 'config', 'cfpLoadingBar', 'md5', '$window', 'app', '$rootScope', '$uibModal', function ListKoiController($scope, $http, $mdDialog, $mdToast, config, cfpLoadingBar, md5, $window, app, $rootScope, $uibModal) {
    $scope.disableBtn = {};

    $scope.init = function () {
        $scope.customerId = app.data.CustomerId;
        if (app.data.CustomerId === -1 || app.data.CustomerId === undefined) {
            $scope.CustomerId = undefined;
        }
        else $scope.CustomerId = app.data.CustomerId;
    };

    $scope.FollowKoi = function (ProductId, Type) {
        if ($scope.CustomerId === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng đăng nhập để thực hiện chức năng này!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;
        }

        $scope.disableBtn.btnFollow = true;
        cfpLoadingBar.start();

        var post = $http({
            method: 'POST',
            url: '/web/customer/FollowProduct/' + $scope.CustomerId + "/" + ProductId,
            data: undefined,
            headers: { 'Authorization': 'bearer ' + app.data.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btnFollow = false;
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                $mdToast.show($mdToast.simple()
                    .theme("success-toast")
                    .textContent(data.meta.error_message)
                    .position('fixed bottom right')
                    .hideDelay(3500));
                if (Type === 1) {
                    var ElementId = "Product-" + ProductId;
                    angular.element(document.querySelector('#' + ElementId)).css('display', 'none');
                    var ElementUnfollowId = "ProductUn-" + ProductId;
                    angular.element(document.querySelector('#' + ElementUnfollowId)).removeClass('hidden');
                    angular.element(document.querySelector('#' + ElementUnfollowId)).css('display', 'block');
                }
                else if (Type === 2) {
                    var ElementId1 = "ProductMb-" + ProductId;
                    angular.element(document.querySelector('#' + ElementId1)).css('display', 'none');
                    var ElementUnfollowId1 = "ProductMbUn-" + ProductId;
                    angular.element(document.querySelector('#' + ElementUnfollowId1)).removeClass('hidden');
                    angular.element(document.querySelector('#' + ElementUnfollowId1)).css('display', 'block');
                }
            }
            else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent(data.meta.error_message)
                        .ok('Đóng')
                        .fullscreen(true)
                );
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btnFollow = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xả ra lỗi! Xin vui lòng thử lại sau.')
                    .ok('Đóng')
                    .fullscreen(true)
            );
        });
    };

    $scope.UnFollowKoi = function (ProductId, Type) {
        if ($scope.CustomerId === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng đăng nhập để thực hiện chức năng này!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;
        }

        cfpLoadingBar.start();

        var post = $http({
            method: 'POST',
            url: '/web/customer/RemoveKoiFollow/' + $scope.CustomerId + "/" + ProductId,
            data: undefined,
            headers: { 'Authorization': 'bearer ' + app.data.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                $mdToast.show($mdToast.simple()
                    .theme("success-toast")
                    .textContent(data.meta.error_message)
                    .position('fixed bottom right')
                    .hideDelay(3500));

                if (Type === 1) {
                    var ElementId = "Product-" + ProductId;
                    angular.element(document.querySelector('#' + ElementId)).removeClass('hidden');
                    angular.element(document.querySelector('#' + ElementId)).css('display', 'block');
                    var ElementUnfollowId = "ProductUn-" + ProductId;
                    angular.element(document.querySelector('#' + ElementUnfollowId)).css('display', 'none');
                }
                else if (Type === 2) {
                    var ElementId1 = "ProductMb-" + ProductId;
                    angular.element(document.querySelector('#' + ElementId1)).removeClass('hidden');
                    angular.element(document.querySelector('#' + ElementId1)).css('display', 'block');
                    var ElementUnfollowId1 = "ProductMbUn-" + ProductId;
                    angular.element(document.querySelector('#' + ElementUnfollowId1)).css('display', 'none');
                }
            }
            else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent(data.meta.error_message)
                        .ok('Đóng')
                        .fullscreen(true)
                );
            }
        }).error(function (data, status, headers, config) {
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xả ra lỗi! Xin vui lòng thử lại sau.')
                    .ok('Đóng')
                    .fullscreen(true)
            );
        });
    };

    var popUpKoi;
    $scope.OpenPopUpKoi = function (item) {
        if (item === undefined) return;
        $scope.videoId = "https://www.youtube.com/embed/" + item.LinkYoutube;
        $scope.koiName = item.Name;

        popUpKoi = $uibModal.open({
            templateUrl: '/popup/popup-koi.html',
            windowClass: 'modal-backdrop in custom-width popup-koi',
            scope: $scope,
            size: 'lg'
        });
    };

    $scope.closePopUpKoi = function () {
        popUpKoi.dismiss('cancel');
    };

}]);
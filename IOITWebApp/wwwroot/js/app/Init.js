myApp.controller('InitController', ['$scope', '$http', '$mdDialog', '$mdToast', 'config', 'cfpLoadingBar', 'md5', '$window', 'app', '$cookies', '$rootScope', function InitController($scope, $http, $mdDialog, $mdToast, config, cfpLoadingBar, md5, $window, app, $cookies, $rootScope) {
    $scope.quantity = 0;
    $scope.totalPriceOrder = 0;

    $scope.init = function (CustomerLogin) {
        $scope.listOrder = JSON.parse($window.localStorage.getItem("Order"));
        $scope.giatridonhang = JSON.parse($window.localStorage.getItem("SumPriceOrder"));

        var hours = 10;
        var now = new Date().getTime();
        var timeout = $window.localStorage.getItem("Timeout");
        if (now - timeout > hours * 60 * 60 * 1000) {
            //$window.localStorage.removeItem("Timeout");
            //$window.localStorage.removeItem("Order");

            //auto login 
            $scope.userInfo = {};
            //load cookie
            var email = $cookies.get('email');
            var password = $cookies.get('password');
            var isRemember = $cookies.get('is_remember');
            $scope.IsRemember = isRemember;
            if (email !== undefined && email !== '' && password !== undefined && password !== '' && isRemember === 'true') {
                $scope.userInfo = { 'email': email, 'password': password };
                //$scope.login();
            }
        }

        //console.log(CustomerLogin);

        app.updateData(CustomerLogin.CustomerId, CustomerLogin.Email, CustomerLogin.FullName, CustomerLogin.Avata, CustomerLogin.Address, CustomerLogin.Password, CustomerLogin.PhomeNumber, CustomerLogin.access_token, CustomerLogin.Sex);

        $scope.CustomerId = app.data.CustomerId;
        $scope.FullName = app.data.FullName;
        $scope.Email = app.data.Email;
        $scope.Avata = app.data.Avata;
        $scope.Address = app.data.Address;
        $scope.Phone = app.data.PhomeNumber;
        $scope.Sex = app.data.Sex;
        
        var quantity = JSON.parse($window.localStorage.getItem("Order"));
        
        if (quantity !== null) {
            $scope.quantity = quantity.length;
        }

    };

    //$scope.ShowDetailCart = function () {

    //    $scope.listOrder = JSON.parse($window.localStorage.getItem("Order"));
    //    $scope.giatridonhang = JSON.parse($window.localStorage.getItem("SumPriceOrder"));


    //};
    $scope.signOut = function () {
        $window.localStorage.removeItem("Timeout");
        $window.localStorage.removeItem("Order");
        $http.get("/web/customer/logout", {
            headers: {}
        }).then(function (data, status, headers) {
            if (data.data.meta.error_code === 200) {
                $window.localStorage.removeItem("nickName");
                $window.localStorage.removeItem("checkNickName");
                $scope.access_token = '';
                $scope.customerId = -1;
                $scope.nickName = '';
                $window.checkNickName = undefined;
                $window.location.href = '/';
            }
        });
    };

    $rootScope.$on("UpdateCountOrder", function () {
        var quantity = JSON.parse($window.localStorage.getItem("Order"));
        console.log("Cập nhật số lượng giỏ hàng: " + quantity);
        if (quantity !== undefined) {
            $scope.quantity = quantity.length;
        }
        else {
            $scope.quantity = 0;
        }
    });

    $scope.ShowDetailCart = function () {
      
        $scope.totalPriceOrder = 0;
        $scope.listOrder = JSON.parse($window.localStorage.getItem("Order"));
        if ($scope.listOrder !== undefined) {
            angular.forEach($scope.listOrder, function (item, key) {
                if (item.PriceSpecial) {
                    $scope.totalPriceOrder = $scope.totalPriceOrder + (item.PriceSpecial * item.quantity);
                }
                else {
                    check = false;
                }
            });
        }



        console.log($scope.totalPriceOrder);
    };

    $scope.RemoveProductOrder = function (ProductId) {
        $scope.listOrder = [];
        $scope.listOrder = JSON.parse($window.localStorage.getItem("Order"));
        if ($scope.listOrder !== undefined) {
            angular.forEach($scope.listOrder, function (item, key) {
                if (item.ProductId === ProductId) {
                    $scope.listOrder.splice(key, 1);
                }
            });

            if ($scope.listOrder.length === 0) $scope.listOrder = null;
        }

        
        var totalPriceOrder = null;
        var quantity = 0;
        let check = true;
        if ($scope.listOrder !== undefined) {
            angular.forEach($scope.listOrder, function (item, key) {
                if (item.PriceSpecial) {
                    totalPriceOrder = totalPriceOrder + (item.PriceSpecial * item.quantity);
                }
                else {
                    check = false;
                }
            });
            
            if ($scope.listOrder != null) {
                quantity = $scope.listOrder.length;
            } else {
                quantity = 0;
            }
           
        }

        $scope.quantity = quantity;
        $scope.totalPriceOrder = check ? totalPriceOrder : null;
        $window.localStorage.setItem("Order", JSON.stringify($scope.listOrder));
        $rootScope.$emit("UpdateCountOrder", {});
    };

  

    //Đi tới trang giỏ hàng
    $scope.GoCart = function () {
        //if ($scope.quantity == 0 || $scope.quantity == undefined) {
        //    $mdDialog.show(
        //        $mdDialog.alert()
        //            .clickOutsideToClose(true)
        //            .title('Thông tin')
        //            .textContent('Đơn hàng của bạn chưa có sản phẩm nào.')
        //            .ok('Đóng')
        //            .fullscreen(true)
        //    );
        //    return;
        //}

        $window.location.href = '/gio-hang.html';
    };

    //Đi tới trang thông tin đặt hàng
    $scope.GoOrderInformation = function () {
        if ($scope.quantity === 0 || $scope.quantity === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đơn hàng của bạn chưa có sản phẩm nào.')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;
        }

        $window.location.href = '/thong-tin-dat-hang.html';
    };

    //Lắng nghe khi thay đổi thông tin tài khoản thì cập nhật lại thông tin
    $rootScope.$on("UpdateUserInit", function () {
        //console.log($scope.FullName);
        $scope.FullName = app.data.FullName;
        $scope.CustomerId = app.data.CustomerId;
    });

}]);


//Danh mục sản phẩm
myApp.controller('CategoryProductController', ['$scope', '$http', '$mdDialog', '$mdToast', 'config', 'cfpLoadingBar', 'md5', '$window', 'app', '$rootScope', function CategoryProductController($scope, $http, $mdDialog, $mdToast, config, cfpLoadingBar, md5, $window, app, $rootScope) {
    $scope.query = "1=1";

    $scope.init = function (CategoryId) {
        $scope.CategoryId = CategoryId;
        $scope.listProduct = [];
        //$scope.GetProductByCate();
    };

    $scope.InitMobile = function (CategoryId) {
        $scope.CategoryId = CategoryId;
        $scope.listProduct = [];
        //$scope.GetProductByCate();
    };

    $scope.GetProductByCate = function () {
        $http.get("/web/product/GetProductByCate/" + $scope.CategoryId + "?page=1&page_size=10&query=" + $scope.query + "&order_by=UpdatedAt desc", {
            headers: {}
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listProduct = data.data.data;
            }
        });
    };

    $scope.QueryChange = function (ManufacturerId) {
        $scope.query = ManufacturerId !== 0 ? "ManufacturerId = " + ManufacturerId : "1=1";
        $scope.GetProductByCate();
    };

    $scope.OnInit = function () {
        //$scope.da = angular.copy($scope.listProduct);
        console.log("OnInit");
        if (this.listProduct.length === 0) {
            this.GetProductByCate();
        }
    };
}]);
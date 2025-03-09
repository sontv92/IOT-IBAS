myApp.controller('ShoppingCartController', ['$scope', '$http', '$mdDialog', '$mdToast', 'config', 'cfpLoadingBar', 'md5', '$window', 'app', '$rootScope', '$uibModal', function ShoppingCartController($scope, $http, $mdDialog, $mdToast, config, cfpLoadingBar, md5, $window, app, $rootScope, $uibModal) {
    $scope.CheckRule = false;
    $scope.order = {};
    $scope.regexEmail = config.regexEmail;
    $scope.regexPhone = config.regexPhone;
    $scope.disableBtn = { btLogin: false };

    $scope.init = function () {
        $scope.listOrder = JSON.parse($window.localStorage.getItem("Order"));
        //console.log($scope.listOrder);
        $scope.calculatorOrder();
        $scope.order.ReceiverName = app.data.FullName;
        $scope.order.ReceiverPhone = app.data.PhomeNumber;
        $scope.order.ReceiverEmail = app.data.Email;
        $scope.order.BillingAddress = app.data.Address;

        var quantity1 = JSON.parse($window.localStorage.getItem("Order"));

        if (quantity1 !== null) {
            $scope.quantity1 = quantity1.length;
        }
        
        
    }

    $scope.ChangeQuantity = function (ProductId, type) {
        if (type == 1) {
            var quantity = +angular.element(document.querySelector('#quantity' + ProductId)).val() - 1;
        } else {
            var quantity = +angular.element(document.querySelector('#quantity' + ProductId)).val() + 1;
        }
       
        console.log(quantity);
        if (quantity == undefined || quantity == "") return;
        if ($scope.listOrder != undefined) {
            angular.forEach($scope.listOrder, function (item, key) {
                if (ProductId == item.ProductId) {
                    item.quantity = quantity;
                }
            });
        }
        $window.localStorage.setItem("Order", JSON.stringify($scope.listOrder));
        $rootScope.$emit("UpdateCountOrder", {});
        $scope.calculatorOrder();
       
    }

    $scope.RemoveProductOrder = function (ProductId) {
        if ($scope.listOrder != undefined) {
            angular.forEach($scope.listOrder, function (item, key) {
                if (item.ProductId == ProductId) {
                    $scope.listOrder.splice(key, 1);
                }
            });

            if ($scope.listOrder.length == 0) $scope.listOrder = null;
        }

        $window.localStorage.setItem("Order", JSON.stringify($scope.listOrder));
        if ($scope.listOrder != undefined) {
            angular.forEach($scope.listOrder, function (item, key) {
                if (ProductId == item.ProductId) {
                    var quantity = angular.element(document.querySelector('#quantity' + ProductId)).val();
                    item.quantity = quantity;
                }
            });
        }
        $scope.calculatorOrder();
        $rootScope.$emit("UpdateCountOrder", {});

        var quantity1 = JSON.parse($window.localStorage.getItem("Order"));

        if (quantity1 !== null) {
            $scope.quantity1 = quantity1.length;
        }
    }

    $scope.GoPayment = function () {
        if ($scope.CheckRule == false || $scope.CheckRule == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn vui lòng đọc điều khoản của chúng tôi!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;
        }

        if ($scope.listOrder == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa có sản phẩm nào trong giỏ hàng của bạn!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;
        }

        if ($scope.listOrder.length == 0) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa có sản phẩm nào trong giỏ hàng của bạn!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;
        }

        if (app.data.CustomerId == -1) {
            $scope.openAddLoginModal();
            return;
        }

        if ($scope.order.ReceiverName == '' || $scope.order.ReceiverName == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng nhập họ tên!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                $scope.focusElement("ReceiverName");
            });
            return;
        }

        if ($scope.order.ReceiverPhone == '' || $scope.order.ReceiverPhone == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập Số điện thoại hoặc Số điện thoại không chính xác!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                $scope.focusElement("ReceiverPhone");
            });
            return;
        }

        if ($scope.order.ReceiverEmail == '' || $scope.order.ReceiverEmail == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập Email hoặc Email không chính xác!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                $scope.focusElement("ReceiverEmail");
            });
            return;
        }

        if ($scope.order.BillingAddress == '' || $scope.order.BillingAddress == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập Địa chỉ nhận hàng!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                $scope.focusElement("BillingAddress");
            });
            return;
        }

        $scope.order.OrderTotal = $scope.totalPriceOrder != null ? $scope.totalPriceOrder : undefined;
        $scope.order.CustomerId = app.data.CustomerId;
        $scope.order.listOrderItem = [];
        angular.forEach($scope.listOrder, function (item, key) {
            var orderItem = {};
            orderItem.ProductId = item.ProductId;
            orderItem.Quantity = item.quantity;
            orderItem.Price = item.PriceSpecial;
            orderItem.PriceTotal = item.total;
            $scope.order.listOrderItem.push(orderItem);
        });

        cfpLoadingBar.start();
        var post = $http({
            method: 'POST',
            url: '/web/order/PostOrder',
            data: $scope.order,
            headers: { 'Authorization': 'bearer ' + app.data.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            cfpLoadingBar.complete();
            if (data.meta.error_code == 200) {
                $window.localStorage.setItem("Order", null);
                $rootScope.$emit("UpdateCountOrder", {});
                $window.location.href = '/hoan-tat-dat-hang-' + data.data.OrderId + '.html';
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
    }

    $scope.calculatorOrder = function () {
        let check = true;
        $scope.totalPriceOrder = null;
        if ($scope.listOrder != undefined) {
            angular.forEach($scope.listOrder, function (item, key) {
                if (item.PriceSpecial) {
                    item.total = item.PriceSpecial * item.quantity;
                    $scope.totalPriceOrder = $scope.totalPriceOrder + item.total;
                }
                else {
                    check = false;
                }
            });
        }

        $scope.totalPriceOrder = check ? $scope.totalPriceOrder : null;
        console.log($scope.listOrder.length);
        $scope.tongdonhang = 0;
        for (let i = 0; i < $scope.listOrder.length; i++) {
            if ($scope.listOrder[i].PriceSpecial != null) {
                $scope.tongdonhang += $scope.listOrder[i].total;
            }
           
        }
        console.log($scope.listOrder);
        $window.localStorage.setItem("SumPriceOrder", JSON.stringify($scope.tongdonhang));
    }

    $rootScope.$on("UpdateCountOrder", function () {
        $scope.init();
    });

    $scope.focusElement = function (id) {
        document.getElementById(id).focus();
    }

    var modalAddLogin;
    $scope.openAddLoginModal = function () {
        $scope.login = {};
        modalAddLogin = $uibModal.open({
            templateUrl: '/popup/login.html',
            windowClass: 'fade login',
            backdrop: 'static',
            scope: $scope,
            size: 'lg'
        });
    };

    $scope.closeAddLoginModal = function () {
        modalAddLogin.dismiss('cancel');
    };

    $scope.cusLogin = function () {
        var email = angular.element(document.querySelector('#txtLoginEmail')).val();
        var password = angular.element(document.querySelector('#txtLoginPassword')).val();
        if (email == '' || email == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng nhập email đăng nhập!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                $scope.focusElement("txtLoginEmail");
            });
            return;
        }

        if (password == '' || password == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng nhập mật khẩu!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                $scope.focusElement("txtLoginPassword");
            });
            return;
        }

        $scope.login = {
            "email": email,
            "password": md5.createHash(password || '')
        };
        $scope.disableBtn.btLogin = true;
        cfpLoadingBar.start();

        var post = $http({
            method: 'POST',
            url: '/web/customer/login',
            data: $scope.login,
            headers: {}
        });

        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btLogin = false;
            cfpLoadingBar.complete();

            if (data.meta.error_code == 200) {
                $scope.login = {};
                let CustomerLogin = data.data;
                console.log(CustomerLogin);
                app.updateData(CustomerLogin.CustomerId, CustomerLogin.Email, CustomerLogin.FullName, CustomerLogin.Avata, CustomerLogin.Address, CustomerLogin.Password, CustomerLogin.PhomeNumber, CustomerLogin.access_token, CustomerLogin.Sex);
                $rootScope.$emit("UpdateUserInit", {});
                $scope.init();
                $scope.closeAddLoginModal();
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
            $scope.disableBtn.btLogin = false;
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

   
}]);
myApp.controller('CheckoutController', ['$scope', '$http', '$mdDialog', '$mdToast', 'config', 'cfpLoadingBar', 'md5', '$window', 'app', '$rootScope', '$uibModal', function CheckoutController($scope, $http, $mdDialog, $mdToast, config, cfpLoadingBar, md5, $window, app, $rootScope, $uibModal) {
    $scope.CheckRule = false;
    $scope.order = {};
    $scope.regexEmail = config.regexEmail;
    $scope.regexPhone = config.regexPhone;
    $scope.disableBtn = { btLogin: false };
    $scope.ListAddress = [];
    $scope.listWard = [];
    $scope.ListProduct = [];
    $scope.ListProductTG = {};
    $scope.customerAdd = {};
    $scope.OrderWeb = {};
    //
    $scope.itemPayment = {};
    $scope.itemPaymentCallback = {};
    $scope.domain = config.domain;
    $scope.domainPay = config.domainPay;
    $scope.lang = config.lang;
    $scope.title = config.title;
    $scope.againLink = config.againLink;
    $scope.paymentLink = config.paymentLink;
    $scope.resultfLink = config.resultfLink;
    $scope.CardList = config.cardList;
    $scope.exchangeRate = config.exchangeRate;

    $scope.init = function () {
        //$scope.customerId = app.data.CustomerId;
        //$scope.access_token = app.data.access_token;
        //console.log($scope.customerId);
        //$scope.giatridonhang = JSON.parse($window.localStorage.getItem("SumPriceOrder"));
        //$scope.danhsachsp = JSON.parse($window.localStorage.getItem("Order"));
        //$scope.IdAddress = JSON.parse($window.localStorage.getItem("IdAddress"));
        $scope.loadProvince();
        if ($scope.customerId !== -1)
            $scope.loadListAddress();
        //
        //$scope.OrderWeb.PaymentMethodId = "100";
        //$scope.checkPayND = false;
    };

    $scope.LoadPayment = function (cart) {
        //$scope.customerId = app.data.CustomerId;
        //$scope.access_token = app.data.access_token;
        //console.log($scope.customerId);
        //$scope.giatridonhang = JSON.parse($window.localStorage.getItem("SumPriceOrder"));
        //$scope.danhsachsp = JSON.parse($window.localStorage.getItem("Order"));
        //console.log($scope.giatridonhang);
        $scope.danhsachsp = cart.Cart.ListItem;
        //console.log($scope.danhsachsp);
        $scope.IdAddress = JSON.parse($window.localStorage.getItem("IdAddress"));
        //$scope.loadProvince();
        //if ($scope.customerId !== -1)
        //    $scope.loadListAddress();
        //
        $scope.OrderWeb.PaymentMethodId = "100";
        $scope.checkPayND = false;
    };

    $scope.ChangeProvince = function (provinID) {
        //$scope.access_token = $window.localStorage.getItem("access_token");
        $http.get("/web/other/listDistrict?page=1&query=ProvinceId=" + provinID + "&order_by=", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listDistrict = data.data.data;
            }
        });

    };

    // Load danh sách địa chỉ
    $scope.loadListAddress = function () {
        //$scope.access_token = $window.localStorage.getItem("access_token");
        //$scope.customerId2 = $window.localStorage.getItem("customerId");
        $http.get("/web/customerAddress/Getbypage?page=1&query=CustomerId=" + $scope.customerId + "&order_by=", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.ListAddress = data.data.data;
                $scope.md = 0;
                for (let i = 0; i < $scope.ListAddress.length; i++) {
                    if ($scope.ListAddress[i].IsMain == true) {
                        $scope.md = $scope.md + 1;
                        $scope.ListAddress[i].isCheck = true;
                        $window.localStorage.setItem("IdAddress", JSON.stringify($scope.ListAddress[i].CustomerAddressId));
                    } else {
                        $scope.ListAddress[i].isCheck = false;
                    }
                }
                if ($scope.md < 1) {
                    for (let i = 0; i < $scope.ListAddress.length; i++) {


                        if (i == 0) {
                            $scope.ListAddress[0].isCheck = true;
                            $window.localStorage.setItem("IdAddress", JSON.stringify($scope.ListAddress[0].CustomerAddressId));
                        } else {
                            $scope.ListAddress[i].isCheck = false;
                        }

                    }
                }
                console.log($scope.ListAddress);

                console.log($scope.ListAddress.length);
                if ($scope.ListAddress.length <= 0) {
                    var element34 = document.getElementById("step1");
                    element34.style.display="block";

                    var element44 = document.getElementById("step2");
                    element44.style.display = "none";
                } else {
                    var element34 = document.getElementById("step1");
                    element34.style.display = "none";

                    var element44 = document.getElementById("step2");
                    element44.style.display = "block";
                }
            }
        });

    };

    // thay doi dia chi giao hang khi chon
    $scope.ChangeAddress = function (item) {
        for (let i = 0; i < $scope.ListAddress.length; i++) {
            if (item.CustomerAddressId === $scope.ListAddress[i].CustomerAddressId) {
                $scope.ListAddress[i].isCheck = true;
                $window.localStorage.setItem("IdAddress", JSON.stringify($scope.ListAddress[i].CustomerAddressId));
            } else {
                $scope.ListAddress[i].isCheck = false;
            }
        }
        //console.log($scope.ListAddress);

    };

    //List tỉnh thành
    $scope.loadProvince = function () {
        //$scope.access_token = $window.localStorage.getItem("access_token");
        $http.get("/web/other/listProvince?page=1&query=1=1&order_by=", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listProvince = data.data.data;
            }
        });
    };

    //List quận huyện
    $scope.loadDistrict = function () {
        //$scope.access_token = $window.localStorage.getItem("access_token");
        $http.get("/web/other/listDistrict?page=1&query=1=1&order_by=", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listDistrict = data.data.data;
            }
        });
    };

    // Thêm địa chỉ
    $scope.SaveAddress = function (fa) {

        var filter1 = /[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$/;
        var filterPhone = /^(0|84)([-. ]?[0-9]{1}){9,10}[-. ]?$/;

        if ($scope.customerAdd.Name === '' || $scope.customerAdd.Name === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng nhập tên!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;
        } else if ($scope.customerAdd.Phone === '' || $scope.customerAdd.Phone === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng nhập số điện thoại!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;

        }
            else if (!filterPhone.test($scope.customerAdd.Phone)) {
            console.log('ok');
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng nhập đúng định dạng số điện thoại!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;

        } else if ($scope.customerId == -1 && $scope.customerAdd.Email == '') {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng nhập email!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;

        } else if ($scope.customerAdd.ProvinceId <= 0 || $scope.customerAdd.ProvinceId === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng chọn tỉnh thành phố!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;

        } else if ($scope.customerAdd.DistrictId <= 0 || $scope.customerAdd.DistrictId === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng chọn quận huyện!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;

        } else if ($scope.customerAdd.Address === '' || $scope.customerAdd.Address === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng nhập địa chỉ!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;

        }
        //$scope.access_token = $window.localStorage.getItem("access_token");
       // $scope.customerId = $window.localStorage.getItem("customerId");
        $scope.customerAdd.CustomerId = $scope.customerId;

        var post = $http({
            method: 'POST',
            url: '/web/customerAddress',
            data: $scope.customerAdd,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                $scope.loadListAddress();
                $scope.customerAdd = {};
                $scope.customerAdd.ProvinceId = '-1';
                $scope.customerAdd.DistrictId = '-1';
                if (fa == 1) {
                    var element34 = document.getElementById("step11");
                    element34.style.display = "none";

                    var element44 = document.getElementById("step12");
                    var element55 = document.getElementById("addresslist");
                    element44.style.display = "block";
                    element55.style.display = "block";
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
    //end

    //Huy luu
    $scope.CancelSaveAddress = function (type) {
        if (type == 1) {
            var element34 = document.getElementById("step11");
            element34.style.display = "none";

            var element44 = document.getElementById("step12");
            var element55 = document.getElementById("addresslist");
            element44.style.display = "block";
            element55.style.display = "block";
        } else {
            var v1 = document.getElementById("step1");
            var v2 = document.getElementById("step2");
            v1.style.display = "none";
            v2.style.display = "block";
        }
        $scope.customerAdd = {};
        $scope.customerAdd.ProvinceId = '-1';
        $scope.customerAdd.DistrictId = '-1';
    };
    $scope.AddNewAddress = function () {
        var element34 = document.getElementById("step1");
        element34.style.display = "block";

        var element44 = document.getElementById("step2");
        element44.style.display = "none";
    };

    $scope.AddressList = function () {
        var element34 = document.getElementById("step11");
        element34.style.display = "block";

        var element44 = document.getElementById("step12");
        var element55 = document.getElementById("addresslist");
        element44.style.display = "none";
        element55.style.display = "none";
    };

    //Open Sửa địa chỉ
    var modalEditAddress;
    $scope.EditAddress = function (itemAd) {
        console.log(itemAd);
        $scope.itemAddressEdit = itemAd;
        $scope.ChangeProvince(itemAd.ProvinceId);
        $scope.itemAddressEdit.ProvinceId = $scope.itemAddressEdit.ProvinceId + '';
        $scope.itemAddressEdit.DistrictId = $scope.itemAddressEdit.DistrictId + '';

        modalEditAddress = $uibModal.open({
            templateUrl: '/popup/editAddress.html',
            windowClass: '',
            backdrop: 'static',
            scope: $scope,
            size: 'lg'
        });
    };

    $scope.closeEditAddModal = function () {
        modalEditAddress.dismiss('cancel');
    };

    // Save edit dia chi
    $scope.UpdateAddress = function () {
        //$scope.access_token = $window.localStorage.getItem("access_token");
        var UpdateAddress = $http({
            method: 'PUT',
            url: '/web/customerAddress/' + $scope.itemAddressEdit.CustomerAddressId,
            data: $scope.itemAddressEdit,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        UpdateAddress.success(function successCallback(data, status, headers, config) {
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                modalEditAddress.dismiss('cancel');
               
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Sửa địa chỉ thành công !')
                        .ok('Đóng')
                        .fullscreen(true)
                );
                console.log('off');
                $scope.loadListAddress();
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

    // Xóa địa chỉ
    $scope.showConfirmDeletaAddress = function (itemid) {
        var confirm = $mdDialog.confirm()
            .title('Thông báo')
            .textContent('Bạn có chắc muốn xóa địa chỉ này không?')
            .ok('Đồng ý')
            .cancel('Hủy bỏ');

        $mdDialog.show(confirm).then(function () {
            $scope.deleteAddressItem(itemid);
        }, function () {
        });
    };

    $scope.deleteAddressItem = function (itemid) {
        //$scope.access_token = $window.localStorage.getItem("access_token");
        var deleteitemad = $http({
            method: 'DELETE',
            url: '/web/customerAddress/' + itemid,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        deleteitemad.success(function successCallback(data, status, headers, config) {
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {

                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Xóa thành công!')
                        .ok('Đóng')
                        .fullscreen(true)
                );
                $scope.loadListAddress();
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

    //tiến hành thanh toán
    $scope.Payment = function () {
        console.log($scope.customerId);
        if ($scope.danhsachsp !== undefined && $scope.danhsachsp !== null) {
            if ($scope.danhsachsp.length <= 0) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Giỏ hàng không có sản phẩm!')
                        .ok('Đóng')
                        .fullscreen(true)
                );
                return;
            }
        }
        else {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Giỏ hàng không có sản phẩm!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;
        }

        $scope.AdressPay = JSON.parse($window.localStorage.getItem("AdressPay"));
        
        //console.log($scope.danhsachsp);
        //console.log($scope.AdressPay);
        //console.log($scope.giatridonhang);
        // thong tin danh sach sp
        for (let i = 0; i < $scope.danhsachsp.length; i++) {
            $scope.ListProductTG = {};
            $scope.ListProductTG.ProductId = $scope.danhsachsp[i].ProductId;
            $scope.ListProductTG.ProductName = $scope.danhsachsp[i].ProductName;
            $scope.ListProductTG.Quantity = $scope.danhsachsp[i].Quantity;
            if ($scope.danhsachsp[i].PriceSpecial !== null && $scope.danhsachsp[i].PriceSpecial !== undefined)
                $scope.ListProductTG.Price = $scope.danhsachsp[i].PriceSpecial;
            else if ($scope.danhsachsp[i].Price !== null && $scope.danhsachsp[i].Price !== undefined)
                $scope.ListProductTG.Price = $scope.danhsachsp[i].Price;
            else
                $scope.ListProductTG.Price = 0;
            $scope.ListProductTG.PriceDiscount = 0;
            $scope.ListProductTG.PriceTotal = $scope.ListProductTG.Quantity * $scope.ListProductTG.Price;
            $scope.ListProductTG.Status = $scope.danhsachsp[i].Status;
            $scope.ListProduct.push($scope.ListProductTG);
        }
        $scope.OrderWeb.listOrderItem = $scope.ListProduct;
        //lay thong tin khach hang
        $scope.OrderWeb.CustomerId = $scope.customerId;
        if ($scope.OrderWeb.CustomerId > 0) {
            $scope.AdressPay = JSON.parse($window.localStorage.getItem("IdAddress"));
            if ($scope.AdressPay === null || $scope.AdressPay === undefined) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Vui lòng chọn địa chỉ giao hàng!')
                        .ok('Đóng')
                        .fullscreen(true)
                );
                return;
            }
            else
                $scope.OrderWeb.CustomerAddressId = $scope.AdressPay;
        }
        else {
            $scope.OrderWeb.FullName = $scope.AdressPay.Name;
            $scope.OrderWeb.Phone = $scope.AdressPay.Phone;
            $scope.OrderWeb.Email = $scope.AdressPay.Email;
            $scope.OrderWeb.Address = $scope.AdressPay.Address;
            $scope.OrderWeb.ProvinceId = $scope.AdressPay.ProvinceId;
            $scope.OrderWeb.DistrictId = $scope.AdressPay.DistrictId;
        }
        $scope.OrderWeb.OrderTotal = $scope.giatridonhang;
        //payment
        if ($scope.OrderWeb.PaymentMethodId !== "100") {
            $scope.OrderWeb.IpAdress = $scope.ipAddress;
            $scope.OrderWeb.Locale = $scope.lang;
            $scope.OrderWeb.ReturnUrl = $scope.domain + $scope.resultfLink;
            $scope.OrderWeb.CardList = "";
            $scope.OrderWeb.AgainLink = $scope.againLink;
            if ($scope.OrderWeb.PaymentMethodId === "87") {
                $scope.OrderWeb.CardList = $scope.CardList;
            }
        }
        document.getElementById('Payment').disabled = true;

        //
        //console.log($scope.OrderWeb);

        var post = $http({
            method: 'POST',
            url: '/web/order/PostOrder/',
            data: $scope.OrderWeb,
            headers: {}
        });

        post.success(function successCallback(data, status, headers) {
            cfpLoadingBar.complete();
            document.getElementById('Payment').disabled = false;
            if (data.meta.error_code === 200) {

                //Xóa giỏ hàng
                //$http.get("/web/ShoppingCart/clearCart", {
                //    headers: { }
                //}).then(function (data, status, headers) {
                //    cfpLoadingBar.complete();
                //    //if (data.data.meta.error_code === 200) {
                //    //    $scope.listProvince = data.data.data;
                //    //}
                //});
                //$window.localStorage.removeItem('Order');
                //$window.localStorage.removeItem('SumPriceOrder');
                $scope.giatridonhang = 0;
                //console.log('oke');
                if ($scope.OrderWeb.PaymentMethodId !== "100") {
                    $scope.OrderWeb.CardList = "";
                    var urlPayment = "";
                    if ($scope.OrderWeb.PaymentMethodId === "87") {
                        $scope.OrderWeb.CardList = $scope.CardList;
                        urlPayment = $scope.domainPay
                            + "?AgainLink=" + $scope.OrderWeb.AgainLink
                            + "&Title=" + $scope.title
                            + "&vpc_AccessCode=" + config.opAccessCode
                            + "&vpc_Amount=" + data.data.OrderTotal + "00"
                            + "&vpc_CardList=" + $scope.OrderWeb.CardList
                            + "&vpc_Command=pay"
                            + "&vpc_Locale=" + $scope.OrderWeb.Locale
                            + "&vpc_MerchTxnRef=" + data.data.PaymentHistoryId
                            + "&vpc_Merchant=" + config.opMerchant
                            + "&vpc_OrderInfo=" + data.data.Code
                            + "&vpc_ReturnURL=" + $scope.OrderWeb.ReturnUrl
                            + "&vpc_TicketNo=" + data.data.IpAdress
                            + "&vpc_Version=2"
                            + "&vpc_SecureHash=" + data.data.HashKey;
                    }
                    else {
                        urlPayment = $scope.domainPay
                            + "?AgainLink=" + $scope.OrderWeb.AgainLink
                            + "&Title=" + $scope.title
                            + "&vpc_AccessCode=" + config.opAccessCode
                            + "&vpc_Amount=" + data.data.OrderTotal + "00"
                            + "&vpc_Command=pay"
                            + "&vpc_Locale=" + $scope.OrderWeb.Locale
                            + "&vpc_MerchTxnRef=" + data.data.PaymentHistoryId
                            + "&vpc_Merchant=" + config.opMerchant
                            + "&vpc_OrderInfo=" + data.data.Code
                            + "&vpc_ReturnURL=" + $scope.OrderWeb.ReturnUrl
                            + "&vpc_TicketNo=" + data.data.IpAdress
                            + "&vpc_Version=2"
                            + "&vpc_SecureHash=" + data.data.HashKey;
                    }

                    $scope.OrderWeb = {};
                    
                    //console.log(urlPayment);
                    $window.location.href = urlPayment;
                }
                else {
                    $window.location.href = "/" + $scope.resultfLink + "?vpc_OrderInfo=" + data.data.Code;
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

    // Tien hang thanh toan khong co user id
    $scope.PayNoUserId = function () {
        var filterPhone = /^(0|84)([-. ]?[0-9]{1}){9,10}[-. ]?$/;
        if ($scope.customerAdd.Name === '' || $scope.customerAdd.Name === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng nhập tên!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;
        } else if ($scope.customerAdd.Phone === '' || $scope.customerAdd.Phone === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng nhập số điện thoại!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;

        }
        
        else if (!filterPhone.test($scope.customerAdd.Phone)) {
            console.log('ok');
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng nhập đúng định dạng số điện thoại!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;

        }
        else if ($scope.customerAdd.Email === '' || $scope.customerAdd.Email === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng nhập email!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;

        } else if ($scope.customerAdd.ProvinceId <= 0 || $scope.customerAdd.ProvinceId === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng chọn tỉnh thành phố!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;

        } else if ($scope.customerAdd.DistrictId <= 0 || $scope.customerAdd.DistrictId === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng chọn quận huyện!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;

        } else if ($scope.customerAdd.Address === '' || $scope.customerAdd.Address === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng nhập địa chỉ!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;

        }
        console.log($scope.customerAdd);
        ///hinh-thuc-thanh-toan.html
        $window.localStorage.setItem("AdressPay", JSON.stringify($scope.customerAdd));
        $window.location.href = "hinh-thuc-thanh-toan.html";
    };

    //payment
    //get callback payment
    $scope.LoadUrlPayment = function () {
        //$scope.LoadExchangeRate();
        var url_string = window.location.href;
        var url = new URL(url_string);
        var orderId = url.searchParams.get("id");
        var type = url.searchParams.get("type");
        console.log(type);
        $scope.OrderWeb = {};
        $scope.OrderWeb.PaymentMethodId = "100";
        $scope.PaymentError = false;
        $scope.PaymentError1 = true;
        $scope.PaymentMessage = "";
        if (type !== undefined)
            type = parseInt(type);
        else
            type = 200;

        if (type === 201) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = true;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Giao dịch không thành công, "
                    + "Ngân hàng phát hành thẻ không cấp phép cho "
                    + "giao dịch hoặc thẻ chưa được kích hoạt dịch vụ "
                    + "thanh toán trên Internet. Vui lòng liên hệ ngân "
                    + "hàng theo số điện thoại sau mặt thẻ được hỗ "
                    + "trợ chi tiết.";
            }
            else {
                $scope.PaymentMessage = "The transaction is unsuccessful. This transaction has been declined by issuer bank or card have been not registered online payment services. Please contact your bank for further clarification.";
            }
        }
        else if (type === 202) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = true;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Giao dịch không thành công, "
                    + "Ngân hàng phát hành thẻ từ chối cấp phép cho "
                    + "giao dịch.Vui lòng liên hệ ngân hàng theo số "
                    + "điện thoại sau mặt thẻ để biết chính xác nguyên "
                    + "nhân Ngân hàng từ chối.";
            }
            else {
                $scope.PaymentMessage = "The transaction is unsuccessful. This transaction has been declined by issuer bank. Please contact your bank for further clarification.";
            }
        }
        else if (type === 203) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = true;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Giao dịch không thành công, "
                    + "Cổng thanh toán không nhận được kết quả trả "
                    + "về từ ngân hàng phát hành thẻ.Vui lòng liên hệ "
                    + "với ngân hàng theo số điện thoại sau mặt thẻ "
                    + "để biết chính xác trạng thái giao dịch và thực "
                    + "hiện thanh toán lại.";

            }
            else {
                $scope.PaymentMessage = "The transaction is unsuccessful. OnePAY did not received payment result from Issuer bank. Please contact your bank for details and try again.";
            }
        }
        else if (type === 204) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = true;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Giao dịch không thành công do thẻ hết hạn sử "
                    + "dụng hoặc nhập sai thông tin tháng / năm hết "
                    + "hạn của thẻ.Vui lòng kiểm tra lại thông tin và "
                    + "thanh toán lại.";
            }
            else {
                $scope.PaymentMessage = "The transaction is unsuccessful. Your card is expired or You have entered incorrect expired date. Please check and try again.";
            }
        }
        else if (type === 205) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = true;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Giao dịch không thành công, "
                    + "Thẻ không đủ hạn mức hoặc tài khoản không "
                    + "đủ số dư để thanh toán.Vui lòng kiểm tra lại "
                    + "thông tin và thanh toán lại";
            }
            else {
                $scope.PaymentMessage = "The transaction is unsuccessful. This transaction cannot be processed due to insufficient funds. Please try another card.";
            }
        }
        else if (type === 206) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = true;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Giao dịch không thành công, "
                    + "Quá trình xử lý giao dịch phát sinh lỗi từ ngân "
                    + "hàng phát hành thẻ.Vui lòng liên hệ ngân hàng "
                    + "theo số điện thoại sau mặt thẻ được hỗ trợ chi tiết.";
            }
            else {
                $scope.PaymentMessage = "The transaction is unsuccessful. An error was encountered while processing your transaction. Please contact your bank for further clarification.";
            }
        }
        else if (type === 207) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = true;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Giao dịch không thành công, "
                    + "Đã có lỗi phát sinh trong quá trình xử lý giao "
                    + "dịch.Vui lòng thực hiện thanh toán lại.";
            }
            else {
                $scope.PaymentMessage = "The transaction is unsuccessful. An error was encountered while processing your transaction. Please contact your bank for further clarification.";
            }
        }
        else if (type === 208) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = true;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Giao dịch không thành công. Số thẻ không "
                +"đúng.Vui lòng kiểm tra và thực hiện thanh toán lại.";
            }
            else {
                $scope.PaymentMessage = "The transaction is unsuccessful. You have entered incorrect card number. Please try again.";
            }
        }
        else if (type === 209) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = true;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Giao dịch không thành công. Tên chủ thẻ "
                    + "không đúng.Vui lòng kiểm tra và thực hiện"
                    + "thanh toán lại.";
            }
            else {
                $scope.PaymentMessage = "The transaction is unsuccessful. You have entered incorrect card holder name. Please try again.";
            }
        }
        else if (type === 210) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = true;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Giao dịch không thành công. Thẻ hết hạn/Thẻ "
                +"bị khóa.Vui lòng kiểm tra và thực hiện thanh "
                +"toán lại.";
            }
            else {
                $scope.PaymentMessage = "The transaction is unsuccessful. The card is expired/locked. Please try again.";
            }
        }
        else if (type === 211) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = true;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Giao dịch không thành công. Thẻ chưa đăng ký "
                    + "sử dụng dịch vụ thanh toán trên Internet. Vui "
                    + "lòng liên hê ngân hàng theo số điện thoại sau "
                    + "mặt thẻ để được hỗ trợ.";
            }
            else {
                $scope.PaymentMessage = "The transaction is unsuccessful. You have been not registered online payment services. Please contact your bank for details.";
            }
        }
        else if (type === 212) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = true;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Giao dịch không thành công. Ngày phát "
                    + "hành / Hết hạn không đúng.Vui lòng kiểm tra và "
                    + "thực hiện thanh toán lại.";
            }
            else {
                $scope.PaymentMessage = "The transaction is unsuccessful. You have entered incorrect Issue date or Expire date. Please try again.";
            }
        }
        else if (type === 213) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = true;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Giao dịch không thành công. thẻ/ tài khoản đã "
                    + "vượt quá hạn mức thanh toán.Vui lòng kiểm "
                    + "tra và thực hiện thanh toán lại.";
            }
            else {
                $scope.PaymentMessage = "The transaction is unsuccessful. The transaction amount exceeds the maximum transaction/amount limit. Please try another card.";
            }
        }
        else if (type === 221) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = true;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Giao dịch không thành công. Số tiền không đủ "
                    + "để thanh toán.Vui lòng kiểm tra và thực hiện "
                    + "thanh toán lại.";
            }
            else {
                $scope.PaymentMessage = "The transaction is unsuccessful. This transaction cannot be processed due to insufficient funds in your account. Please try another card.";
            }
        }
        else if (type === 222) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = true;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Giao dịch không thành công. Thông tin tài "
                    + "khoản không đúng.Vui lòng kiểm tra và thực "
                    + "hiện thanh toán lại.";
            }
            else {
                $scope.PaymentMessage = "The transaction is unsuccessful. This transaction cannot be processed due to invalid account. Please try again.";
            }
        }
        else if (type === 223) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = true;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Giao dịch không thành công. Tài khoản bị khóa. "
                +"Vui lòng liên hê ngân hàng theo số điện thoại "
                +"sau mặt thẻ để được hỗ trợ.";
            }
            else {
                $scope.PaymentMessage = "The transaction is unsuccessful. This transaction cannot be processed due to account locked. Please contact your bank for further clarification.";
            }
        }
        else if (type === 224) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = true;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Giao dịch không thành công. Thông tin thẻ "
                    + "không đúng.Vui lòng kiểm tra và thực hiện "
                    + "thanh toán lại.";
            }
            else {
                $scope.PaymentMessage = "The transaction is unsuccessful. You have entered incorrect card number. Please try again.";
            }
        }
        else if (type === 225) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = true;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Giao dịch không thành công. OTP không đúng. "
                    + "Vui lòng kiểm tra và thực hiện thanh toán lại.";
            }
            else {
                $scope.PaymentMessage = "The transaction is unsuccessful. You have entered incorrect OTP. Please try again.";
            }
        }
        else if (type === 226) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = true;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Giao dịch không thành công. Quá thời gian "
                    + "thanh toán.Vui lòng thực hiện thanh toán lại.";
            }
            else {
                $scope.PaymentMessage = "The transaction is unsuccessful. Transaction timed out. Please try again.";
            }
        }
        else if (type === 227) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = true;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Giao dịch không thành công. Người sử dụng "
                    + "hủy giao dịch.";
            }
            else {
                $scope.PaymentMessage = "The transaction is unsuccessful. The transaction has been cancelled by card holder. Please try again.";
            }
        }
        else if (type === 228) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = true;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Giao dịch không thành công do không xác thực "
                +"được 3D - Secure.Vui lòng liên hệ ngân hàng "
                +"theo số điện thoại sau mặt thẻ được hỗ trợ chi tiết.";
            }
            else {
                $scope.PaymentMessage = "The transaction is unsuccessful. The card used in this transaction is not authorized 3D-Secure complete. Please contact your bank for further clarification.";
            }
        }
        else if (type === 229) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = true;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Giao dịch không thành công do nhập sai CSC "
                    + "(Card Security Card) hoặc ngân hàng từ chối "
                    + "cấp phép cho giao dịch.Vui lòng liên hệ ngân "
                    + "hàng theo số điện thoại sau mặt thẻ được hỗ "
                    + "trợ chi tiết.";
            }
            else {
                $scope.PaymentMessage = "The transaction is unsuccessful. You have entered wrong CSC or Issuer Bank declided transaction. Please contact your bank for further clarification.";
            }
        }
        else if (type === 230) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = true;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Giao dịch không thành công do không xác thực "
                    + "được 3D - Secure.Vui lòng liên hệ ngân hàng "
                    + "theo số điện thoại sau mặt thẻ được hỗ trợ chi tiết.";
            }
            else {
                $scope.PaymentMessage = "The transaction is unsuccessful. Due to 3D Secure Authentication Failed. Please contact your bank for further clarification.";
            }
        }
        else if (type === 231) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = true;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Giao dịch không thành công do vi phạm quy "
                    + "định của hệ thống.Vui lòng liên hệ với OnePAY "
                    + "để được hỗ trợ (Hotline: 1900 633 927).";
            }
            else {
                $scope.PaymentMessage = "The transaction is unsuccessful. Transaction restricted due to OFD’s policies. Please contact OnePAY for details (Hotline 1900 633 927).";
            }
        }
        else if (type === 232) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = true;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Giao dịch không thành công. Vui lòng liên hệ "
                +"với OnePAY để được hỗ trợ(Hotline: 1900 633 927).";
            }
            else {
                $scope.PaymentMessage = "The transaction is unsuccessful. Please contact OnePAY for details (Hotline 1900 633 927).";
            }
        }
        else if (type === 233) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = true;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Giao dịch thành công, "
                    + "nhưng chúng tôi không gửi thông tin đơn hàng được cho bạn. "
                    + "Bạn vui lòng kiểm tra lại địa chỉ Email.";
            }
            else {
                $scope.PaymentMessage = "You have successfully declared your electronic visa "
                    + "but we could not send order email information to you. "
                    + "Please check and update your Email address";
            }
        }
        else if (type === 404) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = false;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Đơn hàng không tồn tại";
            }
            else {
                $scope.PaymentMessage = "Payment order does not exist";
            }
        }
        else if (type === 500 || type === 400) {
            $scope.PaymentError = true;
            $scope.PaymentError1 = false;
            if ($scope.lang === "vn") {
                $scope.PaymentMessage = "Đơn hàng đang trong quá trình xử lý, bạn vui lòng kiểm tra trạng thái thanh toán theo đường dẫn bên dưới.";
            }
            else {
                $scope.PaymentMessage = "Orders are in the process of payment processing, please check the status of payment orders by the link below";
            }
        }

        //console.log($scope.itemEvisa);
        $scope.currencyName = "";
        $http.get('web/order/getById?id=' + orderId, {
            headers: {}
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.itemPaymentCallback = data.data.data;
                $scope.currencyName = "$";
                if ($scope.itemPaymentCallback.PaymentStatusId === 2) {
                    $scope.PaymentError = true;
                    $scope.PaymentError1 = false;
                    if ($scope.lang === "vn") {
                        $scope.PaymentMessage = "Đơn hàng đã được thanh toán";
                    }
                    else {
                        $scope.PaymentMessage = "The order has been paid";
                    }
                }
            }
        });
    };

    //get resuft payment
    $scope.LoadUrlResuft = function () {
        var url_string = window.location.href;
        var url = new URL(url_string);
        $scope.itemPayment.Command = url.searchParams.get("vpc_Command");
        $scope.itemPayment.Locale = url.searchParams.get("vpc_Locale");
        $scope.itemPayment.CurrencyCode = url.searchParams.get("vpc_CurrencyCode");
        $scope.itemPayment.MerchTxnRef = url.searchParams.get("vpc_MerchTxnRef");
        $scope.itemPayment.Merchant = url.searchParams.get("vpc_Merchant");
        $scope.itemPayment.OrderInfo = url.searchParams.get("vpc_OrderInfo");
        $scope.itemPayment.Amount = url.searchParams.get("vpc_Amount");
        $scope.itemPayment.TxnResponseCode = url.searchParams.get("vpc_TxnResponseCode");
        $scope.itemPayment.TransactionNo = url.searchParams.get("vpc_TransactionNo");
        $scope.itemPayment.Message = url.searchParams.get("vpc_Message");
        $scope.itemPayment.Card = url.searchParams.get("vpc_Card");
        $scope.itemPayment.PayChannel = url.searchParams.get("vpc_PayChannel");
        $scope.itemPayment.CardUid = url.searchParams.get("vpc_CardUid");
        $scope.code = url.searchParams.get("vpc_OrderInfo");
        console.log($scope.itemPayment);
        console.log($scope.itemPayment.Command);

        $scope.currencyName = "đ";
        $scope.PaymentMessage = "";
        $scope.EmailAddress = "";

        if ($scope.itemPayment.Command === null || $scope.itemPayment.Command === undefined) {
            $scope.currencyName = "";
            $http.get('web/order/getById?id=' + $scope.code, {
                headers: {}
            }).then(function (data, status, headers) {
                cfpLoadingBar.complete();
                if (data.data.meta.error_code === 200) {
                    $scope.orderResults = data.data.data;
                    
                    if ($scope.lang === "vn") {
                        $scope.PaymentMessage = "Bạn đã tạo đơn hàng thành công";
                        $scope.EmailAddress = ($scope.orderResults.Email !== undefined || $scope.orderResults.Email === "") ? ("Đơn hàng đã được gửi vào địa chỉ Email: " + $scope.orderResults.Email) : "";
                    }
                    else {
                        $scope.PaymentMessage = "You have successfully declared your order";
                    }
                }
                else {
                    if ($scope.lang === "vn") {
                        $scope.PaymentMessage = "Bạn đã tạo đơn hàng thành công";
                    }
                    else {
                        $scope.PaymentMessage = "You have successfully declared your order";
                    }
                }
            });
        }
        else {
            var post = $http({
                method: 'POST',
                url: '/web/order/updatePayment/' + $scope.itemPayment.OrderInfo,
                data: $scope.itemPayment,
                headers: {}
            });

            post.success(function successCallback(data, status, headers) {
                //$scope.disableBtn.btRegister = false;
                cfpLoadingBar.complete();
                $scope.disableBtn.btCreateEVisa = false;
                if (data.meta.error_code === 200) {
                    //Lấy chi tiết đơn hàng
                    $scope.currencyName = "";
                    $http.get('web/order/getById?id=' + $scope.code, {
                        headers: {}
                    }).then(function (data, status, headers) {
                        cfpLoadingBar.complete();
                        if (data.data.meta.error_code === 200) {
                            $scope.orderResults = data.data.data;
                            //console.log($scope.orderResults);
                            if ($scope.lang === "vn") {
                                $scope.PaymentMessage = "Bạn đã tạo đơn hàng thành công";
                                $scope.EmailAddress = ($scope.orderResults.Email !== undefined || $scope.orderResults.Email === "") ? ("Đơn hàng đã được gửi vào địa chỉ Email: " + $scope.orderResults.Email) : "";
                            }
                            else {
                                $scope.PaymentMessage = "You have successfully declared your order";
                            }
                        }
                        else {
                            if ($scope.lang === "vn") {
                                $scope.PaymentMessage = "Bạn đã tạo đơn hàng thành công";
                                $scope.EmailAddress = ($scope.orderResults.Email !== undefined || $scope.orderResults.Email === "") ? ("Đơn hàng đã được gửi vào địa chỉ Email: " + $scope.orderResults.Email) : "";
                            }
                            else {
                                $scope.PaymentMessage = "You have successfully declared your order";
                            }
                        }
                    });
                }
                else if (data.meta.error_code === 233) {
                    if ($scope.lang === "vn") {
                        $scope.PaymentMessage = "Giao dịch thành công, "
                            + "nhưng chúng tôi không gửi thông tin đơn hàng được cho bạn. "
                            + "Bạn vui lòng kiểm tra lại địa chỉ Email.";
                    }
                    else {
                        $scope.PaymentMessage = "You have successfully declared your electronic visa "
                            + "but we could not send order email information to you, "
                            + "Please check and update your Email address";
                    }
                }
                else if (data.meta.error_code === 404) {
                    if ($scope.lang === "vn") {
                        $scope.PaymentMessage = "Đơn hàng không tồn tại";
                    }
                    else {
                        $scope.PaymentMessage = "Payment order does not exist";
                    }
                }
                else if (data.meta.error_code === 500 || data.meta.error_code === 400) {
                    if ($scope.lang === "vn") {
                        $scope.PaymentMessage = "Đơn hàng đang trong quá trình xử lý, bạn vui lòng kiểm tra trạng thái thanh toán theo đường dẫn bên dưới.";
                    }
                    else {
                        $scope.PaymentMessage = "Orders are in the process of payment processing, please check the status of payment orders by the link below";
                    }
                }
                else {
                    var urlPayment = $scope.domain + $scope.paymentLink + "?id=" + $scope.itemPayment.OrderInfo + "&type=" + data.meta.error_code;
                    $window.location.href = urlPayment;
                }
            }).error(function (data, status, headers) {
                cfpLoadingBar.complete();
                $scope.disableBtn.btCreateEVisa = false;
                if ($scope.lang === "vn") {
                    $scope.PaymentMessage = "Đơn hàng đang trong quá trình xử lý, bạn vui lòng kiểm tra trạng thái thanh toán theo đường dẫn bên dưới.";
                }
                else {
                    $scope.PaymentMessage = "Orders are in the process of payment processing, please check the status of payment orders by the link below";
                }
            });
        }
    };

    $scope.RePayment = function () {

        $scope.itemPaymentCallback.PaymentMethodId = $scope.OrderWeb.PaymentMethodId;
        $scope.itemPaymentCallback.IpAdress = $scope.ipAddress;
        $scope.itemPaymentCallback.Locale = $scope.lang;
        $scope.itemPaymentCallback.ReturnUrl = $scope.domain + $scope.resultfLink;
        $scope.itemPaymentCallback.CardList = "";
        $scope.itemPaymentCallback.AgainLink = $scope.againLink;
        if ($scope.itemPaymentCallback.PaymentMethodId === "87") {
            $scope.itemPaymentCallback.CardList = $scope.CardList;
        }
        //console.log($scope.itemPaymentCallback);

        $scope.disableBtn.btCreateEVisa = true;
        var post = $http({
            method: 'POST',
            url: '/web/order/createPayment/' + $scope.itemPaymentCallback.OrderId,
            data: $scope.itemPaymentCallback,
            headers: {}
        });

        post.success(function successCallback(data, status, headers) {
            //$scope.disableBtn.btRegister = false;
            cfpLoadingBar.complete();
            $scope.disableBtn.btCreateEVisa = false;
            if (data.meta.error_code === 200) {
                var urlPayment = "";
                //console.log($scope.itemPaymentCallback);
                if ($scope.OrderWeb.PaymentMethodId !== "100") {
                    if ($scope.itemPaymentCallback.PaymentMethodId === "87") {
                        $scope.itemPaymentCallback.CardList = $scope.CardList;

                        urlPayment = $scope.domainPay
                            + "?AgainLink=" + $scope.itemPaymentCallback.AgainLink
                            + "&Title=" + $scope.title
                            + "&vpc_AccessCode=" + config.opAccessCode
                            + "&vpc_Amount=" + data.data.OrderTotal + "00"
                            + "&vpc_CardList=" + $scope.itemPaymentCallback.CardList
                            + "&vpc_Command=pay"
                            + "&vpc_Locale=" + $scope.itemPaymentCallback.Locale
                            + "&vpc_MerchTxnRef=" + data.data.PaymentHistoryId
                            + "&vpc_Merchant=" + config.opMerchant
                            + "&vpc_OrderInfo=" + data.data.Code
                            + "&vpc_ReturnURL=" + $scope.itemPaymentCallback.ReturnUrl
                            + "&vpc_TicketNo=" + data.data.IpAdress
                            + "&vpc_Version=2"
                            + "&vpc_SecureHash=" + data.data.HashKey;
                    }
                    else {
                        urlPayment = $scope.domainPay
                            + "?AgainLink=" + $scope.itemPaymentCallback.AgainLink
                            + "&Title=" + $scope.title
                            + "&vpc_AccessCode=" + config.opAccessCode
                            + "&vpc_Amount=" + data.data.OrderTotal + "00"
                            + "&vpc_Command=pay"
                            + "&vpc_Locale=" + $scope.itemPaymentCallback.Locale
                            + "&vpc_MerchTxnRef=" + data.data.PaymentHistoryId
                            + "&vpc_Merchant=" + config.opMerchant
                            + "&vpc_OrderInfo=" + data.data.Code
                            + "&vpc_ReturnURL=" + $scope.itemPaymentCallback.ReturnUrl
                            + "&vpc_TicketNo=" + data.data.IpAdress
                            + "&vpc_Version=2"
                            + "&vpc_SecureHash=" + data.data.HashKey;
                    }
                    //console.log(urlPayment);
                    $window.location.href = urlPayment;
                }
                else {
                    $window.location.href = "/" + $scope.resultfLink + "?vpc_OrderInfo=" + data.data.Code;
                }
            }
            else if (data.meta.error_code === 201) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Message')
                        .textContent('The order has been paid!')
                        .ok('Close')
                        .fullscreen(true)
                );
            }
            else if (data.meta.error_code === 404) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Message')
                        .textContent('Order does not exist!')
                        .ok('Close')
                        .fullscreen(true)
                );
            }
            else if (data.meta.error_code === 400 || data.meta.error_code === 500) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Message')
                        .textContent('There was an error! Please try again!')
                        .ok('Close')
                        .fullscreen(true)
                );
            }
            else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Message')
                        .textContent('There was an error! Please try again!')
                        .ok('Close')
                        .fullscreen(true)
                );
            }
        }).error(function (data, status, headers) {
            cfpLoadingBar.complete();
            $scope.disableBtn.btCreateEVisa = false;
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Message')
                    .textContent('There was an error! Please try again!')
                    .ok('Close')
                    .fullscreen(true)
            );
        });
    };

    $scope.checkPayment = function (type) {
        //if (type === 2)
        //    $scope.checkTT = true;
        //if (type === 3)
        //    $scope.checkDK = true;
        $scope.OrderWeb.PaymentMethodId = type+"";
        if ($scope.OrderWeb.PaymentMethodId === "87")
            $scope.checkPayND = true;
        else
            $scope.checkPayND = false;

        //if ($scope.checkTT && $scope.checkDK
        //    && ($scope.itemEvisa.PaymentMethodId === "86" || $scope.itemEvisa.PaymentMethodId === "87"
        //        || $scope.itemEvisa.PaymentMethodId === "88" || $scope.itemEvisa.PaymentMethodId === "89")) {
        //    $scope.checkPay = true;
        //}
    };

    $scope.checkPaymentATM = function (type) {
        $scope.CardList = type + "";
    };

    // set mac dinh dia chi trong so dia chi
    $scope.SetDefaultAddress = function (item) {
        item.IsMain = true;
        $http.put("/web/CustomerAddress/" + item.CustomerAddressId, item, {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {

                $scope.loadListAddress();

            }
        });
    };

}]);
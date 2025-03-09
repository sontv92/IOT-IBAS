
myApp.controller('CustomerController', ['$scope', '$http', '$mdDialog', '$mdToast', 'config', 'cfpLoadingBar', 'md5', '$window', 'app', function CustomerController($scope, $http, $mdDialog, $mdToast, config, cfpLoadingBar, md5, $window, app) {
    $scope.page = 1;
    $scope.page_size = 9;
    $scope.query = "1=1";
    $scope.q = {};
    $scope.EditCustomer = {};
    $scope.orderby = "";
    $scope.item_count = 0;
    $scope.disableBtn = { btRegister: false, btLogin: false, btResetPass: false, submitRecover: false };
    $scope.regexEmail = config.regexEmail;
    $scope.regexPhone = config.regexPhone;

    $scope.init = function (data) {
        //console.log(data.Avata);
        $scope.customerId = data.CustomerId;
        $scope.EditCustomer.Email = data.Email;
        $scope.EditCustomer.FullName = data.FullName;
        $scope.EditCustomer.Avata = data.Avata;
        $scope.EditCustomer.Address = data.Address;
        $scope.EditCustomer.PhomeNumber = data.PhomeNumber;
        $scope.EditCustomer.Sex = data.Sex;
        $scope.access_token = data.access_token;
        $scope.StatutOrId = 1;
        $scope.CustomerListOrder();
        $scope.ChangeStatutsOrder($scope.StatutOrId);

        //$scope.listOrder = JSON.parse($window.localStorage.getItem("Order"));
        //console.log($scope.customerId);

        var url_string = window.location.href;
        var url = new URL(url_string);
        var type = url.searchParams.get("key");
        $scope.typee = type;
        //console.log($scope.typee);

        if ($scope.typee == 'dk') {
            var element = document.getElementById("dangky");
            document.getElementById("creatacc").style.display='block';
            document.getElementById("haveacc").style.display='none';
            element.classList.add("active");
           
        }

        if ($scope.typee == 'dn') {
            var element = document.getElementById("dangky");
            var element1 = document.getElementById("dangnhap");
            element1.classList.add("active");
            document.getElementById("haveacc").style.display = 'block';
            document.getElementById("creatacc").style.display = 'none';
           
        }

        //if (app.data.CustomerId !== -1 && app.data.CustomerId !== undefined) {
        //    $window.location.href = '/';
        //}

        //$scope.current_url = current_url;
        //console.log($scope.current_url);
        $scope.register = {};
        $scope.login = {};
        $scope.resetPass = {};
        $scope.reset = false;

        //console.log($scope.EditCustomer.Avata);
    };

    $scope.showResetPass = function () {
        $scope.reset = !$scope.reset;
    };

    $scope.submitLogin = function () {
        var email = angular.element(document.querySelector('#txtLoginEmail')).val();
        var password = angular.element(document.querySelector('#txtLoginPassword')).val();
        if (email === '' || email === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập email đăng nhập!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                $scope.focusElement("txtLoginEmail");
            });
            return;
        }

        if (password === '' || password === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mật khẩu!')
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
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                cfpLoadingBar.complete();
                $window.location.href = '/';
                $scope.login = {};
                //$scope.nickName = data.data.FullName !== undefined ? data.data.FullName : "";
                //$scope.checkNickName = undefined;
                //$window.localStorage.setItem("nickName", data.data.FullName);
                //$window.localStorage.removeItem("checkNickName");

                //get nickname
               // $scope.getNickName();

                //set timeout
                //var now = new Date().getTime();
                //$window.localStorage.setItem("Timeout", now);
                //$scope.goHome();
                
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
                return;
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btLogin = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xảy ra lỗi! Xin vui lòng thử lại sau.')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;
        });

    };

    //$scope.goHome = function () {
    //    $window.location.href = '/';
    //};

    // Get danh sach đơn hàng
    $scope.CustomerListOrder = function () {
        $http.get("/web/order/list?page=1&query=CustomerId=" + $scope.customerId + "&order_by=", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.ListMyOrder = data.data.metadata;


            }
        });
    };

    $scope.submitRecover = function () {
        var email = angular.element(document.querySelector('#txtEmailRecover')).val();
        if (email === '' || email === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn vui lòng nhập email để lấy lại mật khẩu!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;
        }

        let obj = {
            "email": email
        };

        $scope.disableBtn.submitRecover = true;
        cfpLoadingBar.start();

        var post = $http({
            method: 'POST',
            url: '/web/customer/RecoverPasssword',
            data: obj,
            headers: {}
        });

        post.success(function successCallback(data, status, headers, config) {
         
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                var confirm = $mdDialog.confirm()
                    .title('Thông báo')
                    .textContent(data.meta.error_message)
                    .ok('Về trang chủ')   
                    .cancel('Đóng');

                $mdDialog.show(confirm).then(function () {
                    $scope.goHome();
                });
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
            //$scope.disableBtn.submitRecover = false;
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

    $scope.focusElement = function (id) {
        document.getElementById(id).focus();
    };

    //$scope.getNickName = function () {
    //    $http.get("/web/aution/getNickName/" + $scope.sessionId + "/" + $scope.customerId + "/" + $scope.productId, {
    //        headers: {}
    //    }).then(function (data, status, headers) {
    //        cfpLoadingBar.complete();
    //        if (data.data.meta.error_code === 200) {
    //            $window.localStorage.setItem("nickName", $scope.nickName);
    //            $window.localStorage.setItem("checkNickName", true);
    //            $scope.checkNickName = true;
    //        }
    //        else {
    //            $scope.checkNickName = false;
    //        }
    //    });
    //};
    // DANG KY USER

    $scope.RegisterMember = function (type) {
        if ($scope.register.FullName === '' || $scope.register.FullName === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng nhập họ tên!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("FullNameDk");
                        break;
                    case 2:
                        $scope.focusElement("FullNameMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }

        //if ($scope.register.PhomeNumber === '' || $scope.register.PhomeNumber === undefined) {
        //    $mdDialog.show(
        //        $mdDialog.alert()
        //            .clickOutsideToClose(true)
        //            .title('Thông báo')
        //            .textContent('Chưa nhập Số điện thoại hoặc Số điện thoại đã nhập không chính xác!')
        //            .ok('Đóng')
        //            .fullscreen(true)
        //    ).finally(function () {
        //        switch (type) {
        //            case 1:
        //                $scope.focusElement("PhoneDk");
        //                break;
        //            case 2:
        //                $scope.focusElement("PhoneMb");
        //                break;
        //            default:
        //                break;
        //        }
        //    });
        //    return;
        //}

        if ($scope.register.Email === '' || $scope.register.Email === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập email hoặc email đã nhập chưa chính xác!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("EmailDk");
                        break;
                    case 2:
                        $scope.focusElement("EmailMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }

        if ($scope.register.PhomeNumber === '' || $scope.register.PhomeNumber === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập số điện thoại hoặc nhập sai định dạng điện thoại!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;
        }

        if ($scope.register.Password === '' || $scope.register.Password === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mật khẩu!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("PasswordDk");
                        break;
                    case 2:
                        $scope.focusElement("PasswordMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }

        if ($scope.register.ConfirmPassword === '' || $scope.register.ConfirmPassword === undefined || $scope.register.Password !== $scope.register.ConfirmPassword) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Mật khẩu xác nhận chưa nhập hoặc nhập chưa chính xác!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("ConfirmPasswordDk");
                        break;
                    case 2:
                        $scope.focusElement("ConfirmPasswordMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }

        //if (type === 1) {
        //    if (!$scope.confirm) {
        //        $mdDialog.show(
        //            $mdDialog.alert()
        //                .clickOutsideToClose(true)
        //                .title('Thông báo')
        //                .textContent('Để đăng ký thành viên. Bạn cần đồng ý với các điều khoản của chúng tôi!')
        //                .ok('Đóng')
        //                .fullscreen(true)
        //        );
        //        return;
        //    }
        //}

        $scope.disableBtn.btRegister = true;
        cfpLoadingBar.start();
        var obj = angular.copy(this.register);

        var post = $http({
            method: 'POST',
            url: '/web/customer/register',
            data: obj,
            headers: {}
        });

        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btRegister = false;
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                var key = md5.createHash(data.data.KeyRandom || '');
                $window.location.href = '/xac-nhan-dang-ky-' + key + '-' + data.data.CustomerId + '.html';
            }
            else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông tin')
                        .textContent(data.meta.error_message)
                        .ok('Đóng')
                        .fullscreen(true)
                );
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btRegister = false;
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

    // dang xuat
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

    // gio hang mini
    $scope.ShowDetailCart = function () {
        let check = true;
        $scope.totalPriceOrder = null;
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

        $scope.totalPriceOrder = check ? $scope.totalPriceOrder : null;

        $('.giohang').asidebar('open');
    };

    $scope.RemoveProductOrder = function (ProductId) {
        $scope.listOrder = JSON.parse($window.localStorage.getItem("Order"));
        if ($scope.listOrder !== undefined) {
            angular.forEach($scope.listOrder, function (item, key) {
                if (item.ProductId === ProductId) {
                    $scope.listOrder.splice(key, 1);
                }
            });

            if ($scope.listOrder.length === 0) $scope.listOrder = null;
        }

        $window.localStorage.setItem("Order", JSON.stringify($scope.listOrder));
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
            quantity = $scope.listOrder.length;
        }

        $scope.quantity = quantity;
        $scope.totalPriceOrder = check ? totalPriceOrder : null;
        $rootScope.$emit("ListenMiniOrder", {});
    };

    //Luu thong tin sua user
    $scope.SaveEditUser = function () {
        if ($scope.EditCustomer.FullName == '' || $scope.EditCustomer.FullName == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng nhập tên người dùng!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;
        } else if ($scope.EditCustomer.Email == '' || $scope.EditCustomer.Email == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng nhập email!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;
        } else if ($scope.EditCustomer.PhomeNumber == '' || $scope.EditCustomer.PhomeNumber == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng nhập số điện thoại!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;
        } else if ($scope.EditCustomer.Address == '' || $scope.EditCustomer.Address == undefined) {
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


        $scope.EditCustomer.CustomerId = $scope.customerId;
        var post = $http({
            method: 'POST',
            url: '/web/Customer/UpdateInfoCustomer/' + $scope.customerId,
            data: $scope.EditCustomer,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Cập nhật thông tin thành công')
                        .ok('Đóng')
                        .fullscreen(true)
                );

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

    // upload avata
    $scope.uploadAvatar = function (e) {
        if (e === undefined) return;
        if (e.files.length <= 0) return;

        var fd = new FormData();
        fd.append("file", e.files[0]);
        cfpLoadingBar.start();
        var post = $http({
            method: 'POST',
            url: '/web/upload/uploadImage/6',
            data: fd,
            headers: {
                "Content-Type": undefined
            }
        });

        post.success(function successCallback(data, status, headers, config) {
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                $scope.EditCustomer.Avata = data.data[0];
                var oFReader = new FileReader();
                oFReader.readAsDataURL(document.getElementById("uploadImage").files[0]);
                oFReader.onload = function (oFREvent) {
                    document.getElementById("uploadPreview").src = oFREvent.target.result;
                };
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

    // Lọc trang thai don hang
    $scope.ChangeStatutsOrder = function (id) {
        $scope.StatutOrId = id;
        $scope.listOrderByStatust = [];
        $http.get("/web/order/list?page=" + $scope.page + "&page_size=" + $scope.page_size + "&query=CustomerId=" + $scope.customerId + " and OrderStatusId=" + $scope.StatutOrId + "&order_by=", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.ListMyOrderByStatus = data.data.data;
                $scope.item_count = data.data.metadata.Count;
                $scope.metadata = data.data.metadata.Count;
            }
        });

    };
    // Phan trang

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
        $scope.ChangeStatutsOrder($scope.StatutOrId);
    };
}]);
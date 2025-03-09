myApp.controller('BlUserController', ['$scope', '$http', '$mdDialog', '$mdToast', 'config', 'cfpLoadingBar', 'md5', '$window', 'app', '$rootScope', function BlUserController($scope, $http, $mdDialog, $mdToast, config, cfpLoadingBar, md5, $window, app, $rootScope) {
    $scope.disableBtn = {};

    $scope.init = function (IndexRouter) {
        if (app.data.CustomerId === -1 && app.data.CustomerId === undefined) {
            $window.location.href = '/';
        }

        $scope.IndexRouter = IndexRouter;
        $scope.CustomerFullName = app.data.FullName;
        $scope.CustomerAvata = app.data.Avata;
    };

    $rootScope.$on("UpdateInfoUser", function () {
        $scope.init($scope.IndexRouter);
    });
}]);

myApp.controller('MainUserController', ['$scope', '$http', '$mdDialog', '$mdToast', 'config', 'cfpLoadingBar', 'md5', '$window', 'app', '$rootScope', function MainUserController($scope, $http, $mdDialog, $mdToast, config, cfpLoadingBar, md5, $window, app, $rootScope) {
    $scope.regexEmail = config.regexEmail;
    $scope.regexPhone = config.regexPhone;
    $scope.user = {};
    $scope.password = {};
    $scope.disableBtn = {};
    $scope.init = function (data) {
        
        app.data.CustomerId = data.CustomerId;
        app.data.access_token = data.access_token;
        console.log(data);
        if (data.CustomerId === -1 || data.CustomerId === undefined) {
            $window.location.href = '/';
        }
        $scope.user.CustomerId = data.CustomerId;
        $scope.user.FullName = data.FullName;
        $scope.user.Avata = data.Avata;
        $scope.user.Email = data.Email;
        $scope.user.Address = data.Address;
        $scope.user.PhomeNumber = data.PhomeNumber;
        $scope.user.Sex = data.Sex;
    };

    $scope.ChangeInfo = function () {
        if ($scope.user.FullName === '' || $scope.user.FullName === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng nhập họ tên!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                $scope.focusElement("FullName");
            });
            return;
        }

        if ($scope.user.Email === '' || $scope.user.Email === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập email hoặc email đã nhập chưa chính xác!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                $scope.focusElement("Email");
            });
            return;
        }

        //if ($scope.user.PhomeNumber === '' || $scope.user.PhomeNumber === undefined) {
        //    $mdDialog.show(
        //        $mdDialog.alert()
        //            .clickOutsideToClose(true)
        //            .title('Thông báo')
        //            .textContent('Bạn chưa nhập Số điện thoại hoặc Số điện thoại đã nhập không chính xác!')
        //            .ok('Đóng')
        //            .fullscreen(true)
        //    ).finally(function () {
        //        $scope.focusElement("PhomeNumber");
        //    });
        //    return;
        //}

        //if ($scope.user.Address === '' || $scope.user.Address === undefined) {
        //    $mdDialog.show(
        //        $mdDialog.alert()
        //            .clickOutsideToClose(true)
        //            .title('Thông báo')
        //            .textContent('Bạn chưa nhập Địa chỉ !')
        //            .ok('Đóng')
        //            .fullscreen(true)
        //    ).finally(function () {
        //        $scope.focusElement("Address");
        //    });
        //    return;
        //}

        //if ($scope.user.Sex === '' || $scope.user.Sex === undefined) {
        //    $mdDialog.show(
        //        $mdDialog.alert()
        //            .clickOutsideToClose(true)
        //            .title('Thông báo')
        //            .textContent('Bạn chưa chọn Giới tính!')
        //            .ok('Đóng')
        //            .fullscreen(true)
        //    );
        //    return;
        //}

        $scope.disableBtn.btSubmit = true;
        cfpLoadingBar.start();

        var post = $http({
            method: 'POST',
            url: '/web/customer/UpdateInfoCustomer/' + $scope.user.CustomerId,
            data: $scope.user,
            headers: { 'Authorization': 'bearer ' + app.data.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {

                //$window.localStorage.setItem("CustomerFullName", data.data.FullName);
                //$window.localStorage.setItem("CustomerAvata", data.data.Avata);
                //$window.localStorage.setItem("CustomerAddress", data.data.Address);
                //$window.localStorage.setItem("CustomerPhoneNumber", data.data.PhomeNumber);
                //$window.localStorage.setItem("CustomerSex", data.data.Sex);

                app.updateData(app.data.CustomerId, app.data.Email, data.data.FullName, data.data.Avata, data.data.Address, app.data.Password, data.data.PhomeNumber, app.data.access_token, data.data.Sex);

                $rootScope.$emit("UpdateInfoUser", {});
                $rootScope.$emit("UpdateUserInit", {});

                $mdToast.show($mdToast.simple()
                    .theme("success-toast")
                    .textContent('Cập nhật thông tin tài khoản thành công!')
                    .position('fixed bottom right')
                    .hideDelay(5000));
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
            $scope.disableBtn.btSubmit = false;
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
                $scope.user.Avata = data.data[0];
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

    $scope.ChangePassword = function () {
        if ($scope.password.Password === '' || $scope.password.Password === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mật khẩu hiện tại!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                $scope.focusElement("password");
            });
            return;
        }

        if ($scope.password.NewPassword === '' || $scope.password.NewPassword === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mật khẩu mới cho tài khoản!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                $scope.focusElement("new-password");
            });
            return;
        }

        if ($scope.password.ConfirmPassword === '' || $scope.password.ConfirmPassword === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mật khẩu xác nhận!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                $scope.focusElement("confirm-password");
            });
            return;
        }

        if ($scope.password.NewPassword !== $scope.password.ConfirmPassword) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Mật khẩu xác nhận phải trùng với mật khẩu mới!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                $scope.focusElement("confirm-password");
            });
            return;
        }

        $scope.disableBtn.btSubmit = true;
        cfpLoadingBar.start();
        let obj = {
            "UserId": $scope.user.CustomerId,
            "PasswordOld": $scope.password.Password,
            "PasswordNew": $scope.password.NewPassword
        };

        var post = $http({
            method: 'POST',
            url: '/web/customer/ChangePasssword',
            data: obj,
            headers: { 'Authorization': 'bearer ' + app.data.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                $scope.password = {};
                var confirm = $mdDialog.confirm()
                    .title('Thông báo')
                    .textContent(data.meta.error_message)
                    .ok('Về trang cá nhân')
                    .cancel('Hủy');

                $mdDialog.show(confirm).then(function () {
                    $window.location.href = '/trang-ca-nhan.html';
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
            $scope.disableBtn.btSubmit = false;
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
}]);

myApp.controller('WishListController', ['$scope', '$http', '$mdDialog', '$mdToast', 'config', 'cfpLoadingBar', 'md5', '$window', 'app', '$rootScope', function WishListController($scope, $http, $mdDialog, $mdToast, config, cfpLoadingBar, md5, $window, app, $rootScope) {
    $scope.init = function () {
        app.data.CustomerId = $scope.customerId;
        app.data.access_token = $scope.access_token;
        if (app.data.CustomerId === -1 || app.data.CustomerId === undefined) {
            $window.location.href = '/';
        }
        $scope.GetLikeProducts(app.data.CustomerId);
    };

    $scope.GetLikeProducts = function (CustomerId) {
        $http.get("/web/customer/WishList/" + CustomerId, {
            headers: { 'Authorization': 'bearer ' + app.data.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.products = data.data.data;
            }
        });
    };

    $scope.RemoveLikeProduct = function (ProductId) {
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
                $scope.GetLikeProducts(app.data.CustomerId);

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
}]);

myApp.controller('KoiFollowController', ['$scope', '$http', '$mdDialog', '$mdToast', 'config', 'cfpLoadingBar', 'md5', '$window', 'app', '$rootScope', '$uibModal', function KoiFollowController($scope, $http, $mdDialog, $mdToast, config, cfpLoadingBar, md5, $window, app, $rootScope, $uibModal) {
    $scope.init = function () {
        $scope.customerId = app.data.CustomerId;
        if (app.data.CustomerId === -1 || app.data.CustomerId === undefined) {
            $window.location.href = '/';
        }
        $scope.GetKoiFollow(app.data.CustomerId);
    };

    $scope.GetKoiFollow = function (CustomerId) {
        $http.get("/web/customer/KoiFollowList/" + CustomerId, {
            headers: { 'Authorization': 'bearer ' + app.data.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.kois = data.data.data;
            }
        });
    };

    $scope.RemoveLikeProduct = function (ProductId) {
        cfpLoadingBar.start();

        var post = $http({
            method: 'POST',
            url: '/web/customer/RemoveKoiFollow/' + app.data.CustomerId + "/" + ProductId,
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
                $scope.GetKoiFollow(app.data.CustomerId);
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
myApp.controller('RegisterController', ['$scope', '$http', '$mdDialog', '$mdToast', 'config', 'cfpLoadingBar', 'md5', '$window', 'app', function RegisterController($scope, $http, $mdDialog, $mdToast, config, cfpLoadingBar, md5, $window, app) {
    $scope.confirm = false;
    $scope.disableBtn = {};
    $scope.regexEmail = config.regexEmail;
    $scope.regexPhone = config.regexPhone;

    $scope.init = function () {
        if (app.data.CustomerId !== -1 && app.data.CustomerId !== undefined) {
            $window.location.href = '/';
        }

        $scope.register = {};
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
                $scope.register.Avata = data.data[0];
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

        if ($scope.register.Password === '' || $scope.register.Password === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng nhập mật khẩu!')
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
                    .textContent('Mật khẩu xác nhận nhập chưa chính xác!')
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

        if (type === 1) {
            if (!$scope.confirm) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Để đăng ký thành viên. Bạn cần đồng ý với các điều khoản của chúng tôi!')
                        .ok('Đóng')
                        .fullscreen(true)
                );
                return;
            }
        }

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

    $scope.focusElement = function (id) {
        document.getElementById(id).focus();
    };
}]);
myApp.controller('AutionContactController', ['$scope', '$http', '$mdDialog', '$mdToast', 'config', 'cfpLoadingBar', 'md5', '$window', 'app', '$uibModal', function AutionContactController($scope, $http, $mdDialog, $mdToast, config, cfpLoadingBar, md5, $window, app, $uibModal) {
    $scope.regexEmail = config.regexEmail;
    $scope.regexPhone = config.regexPhone;
    $scope.contact = {};
    $scope.disableBtn = { btSendContact: false };

    $scope.init = function () {
        $scope.loadListBranch();
    };

    $scope.loadListBranch = function () {
        //var query = "TypeOriginId=3";
        $http.get("/web/contact/getBranch", {
            headers: {}
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listBranch = data.data.data;
            }
        });
    };

    $scope.addContact = function () {
        
                    if ($scope.contact.FullName === '' || $scope.contact.FullName === undefined) {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông báo')
                            .textContent('Chưa nhập Họ tên!')
                            .ok('Đóng')
                            .fullscreen(true)
                    ).finally(function () {
                        $scope.focusElement("hoten");
                    });
                    return;
                } else if ($scope.contact.Email === '' || $scope.contact.Email === undefined) {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông báo')
                            .textContent('Chưa nhập Email hoặc Email đã nhập không chính xác!')
                            .ok('Đóng')
                            .fullscreen(true)
                    ).finally(function () {
                        $scope.focusElement("email");
                    });
                    return;
                }
                //else if ($scope.contact.Phone === '' || $scope.contact.Phone === undefined) {
                //    $mdDialog.show(
                //        $mdDialog.alert()
                //            .clickOutsideToClose(true)
                //            .title('Thông báo')
                //            .textContent('Chưa nhập Số điện thoại hoặc Số điện thoại không chính xác!')
                //            .ok('Đóng')
                //            .fullscreen(true)
                //    ).finally(function () {
                //        $scope.focusElement("dienthoai");
                //    });
                //    return;
                //} else if ($scope.contact.Note === '' || $scope.contact.Note === undefined) {
                //    $mdDialog.show(
                //        $mdDialog.alert()
                //            .clickOutsideToClose(true)
                //            .title('Thông báo')
                //            .textContent('Chưa nhập Nội dung!')
                //            .ok('Đóng')
                //            .fullscreen(true)
                //    ).finally(function () {
                //        $scope.focusElement("noidung");
                //    });
                //    return;
                //}
                this.contact.TypeContactId = 3;

        $scope.disableBtn.btSendContact = true;
        cfpLoadingBar.start();
        var obj = angular.copy(this.contact);

        var post = $http({
            method: 'POST',
            url: '/web/contact/SendContactAution',
            data: obj,
            headers: {}
        });

        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btSendContact = false;
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                let text = "Đặt lịch tới xem cá và tham khảo báo giá thành công!";
                $mdToast.show($mdToast.simple()
                    .theme("success-toast")
                    .textContent(text)
                    .position('fixed top right')
                    .hideDelay(5000));
                $scope.closeAddContactModal();
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
            $scope.disableBtn.btSendContact = false;
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

    //aution contact
    var modalAddContact;
    $scope.openAddContactModal = function () {
        $scope.login = {};
        modalAddContact = $uibModal.open({
            templateUrl: '/popup/aution-contact.html',
            windowClass: 'fade aution-contact',
            backdrop: 'static',
            scope: $scope,
            size: 'lg'
        });
    };

    $scope.closeAddContactModal = function () {
        modalAddContact.dismiss('cancel');
    };

}]);
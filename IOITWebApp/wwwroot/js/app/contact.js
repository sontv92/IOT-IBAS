myApp.controller('ContactController', ['$scope', '$http', '$mdDialog', '$mdToast', 'config', 'cfpLoadingBar', 'md5', '$window', 'app', function ContactController($scope, $http, $mdDialog, $mdToast, config, cfpLoadingBar, md5, $window, app) {
    $scope.regexEmail = config.regexEmail;
    $scope.regexPhone = config.regexPhone;
    $scope.contact = {};
    $scope.disableBtn = {};

    $scope.init = function () {
        $scope.loadListBranch();
    };

    $scope.addContact = function (cs) {
        switch (cs) {
            case 1:
                if ($scope.contact.Title === '' || $scope.contact.Title === undefined) {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông báo')
                            .textContent('Chưa nhập Tiêu đề!')
                            .ok('Đóng')
                            .fullscreen(true)
                    ).finally(function () {
                        $scope.focusElement("tieude");
                    });
                    return;
                } else if ($scope.contact.FullName === '' || $scope.contact.FullName === undefined) {
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
                this.contact.TypeContactId = 1;
                break;
            case 2:
                if ($scope.contact.Email === '' || $scope.contact.Email === undefined) {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông báo')
                            .textContent('Chưa nhập Email hoặc Email đã nhập không chính xác!')
                            .ok('Đóng')
                            .fullscreen(true)
                    ).finally(function () {
                        $scope.focusElement("EmailFooter");
                    });
                    return;
                }
                this.contact.TypeContactId = 2;
                break;
            default:
                break;
        }

        $scope.disableBtn.btSendContact = true;
        cfpLoadingBar.start();
        var obj = angular.copy(this.contact);

        var post = $http({
            method: 'POST',
            url: '/web/contact/SendContact',
            data: obj,
            headers: {}
        });

        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btSendContact = false;
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                let text = cs === 1 ? "Gửi thông tin thành công!" : "Đăng ký nhận bản tin thành công!";
                $mdToast.show($mdToast.simple()
                    .theme("success-toast")
                    .textContent(text)
                    .position('fixed bottom right')
                    .hideDelay(5000));
                $scope.resetContact();
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

    $scope.resetContact = function () {
        $scope.contact = {};
        var arr = document.getElementsByClassName("contact-placeholder");
        if (arr.length > 0) {
            angular.forEach(arr, function (item, key) {
                item.style.display = "block";
            });
        }
    };

    $scope.focusElement = function (id) {
        document.getElementById(id).focus();
    };

    //get list branch
    $scope.loadListBranch = function () {
        //var query = "TypeOriginId=3";
        $http.get("/web/contact/getBranch", {
            headers: {}
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listBranch = data.data.data;

                $scope.mapOptions = {
                    zoom: 10,
                    center: new google.maps.LatLng($scope.listBranch[0].Lat, $scope.listBranch[0].Long),
                    mapTypeId: google.maps.MapTypeId.ROADMAP
                };

                $scope.infoWindow = new google.maps.InfoWindow();
                $scope.Latlngbounds = new google.maps.LatLngBounds();
                $scope.map = new google.maps.Map(document.getElementById('map'), $scope.mapOptions);

                for (i = 0; i < $scope.listBranch.length; i++) {
                    $scope.createMarker($scope.listBranch[i]);
                }

                $scope.map.setCenter($scope.Latlngbounds.getCenter());
                $scope.map.fitBounds($scope.Latlngbounds);
                //for (i = 0; i < cities.length; i++) {
                //    $scope.createMarker(cities[i]);
                //}

               // console.log($scope.markers);
            }
        });
    };

    //google map
    
    $scope.createMarker = function (info) {

        var marker = new google.maps.Marker({
            map: $scope.map,
            position: new google.maps.LatLng(info.Lat, info.Long),
            title: info.Name
        });
        marker.content = '<div class="infoWindowContent"> Địa chỉ: ' + info.Address
            + '</br> Hotline: ' + info.Phone
            + '</br> Email: ' + info.Email
            + '</div>';

        google.maps.event.addListener(marker, 'click', function () {
            $scope.infoWindow.setContent('<h2>' + marker.title + '</h2>' + marker.content);
            $scope.infoWindow.open($scope.map, marker);
        });
        //console.log(marker)
        //$scope.markers.push(marker);
        $scope.Latlngbounds.extend(marker.position);
    };

    $scope.openInfoWindow = function (e, selectedMarker) {
        e.preventDefault();
        google.maps.event.trigger(selectedMarker, 'click');
    };

}]);
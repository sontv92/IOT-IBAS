myApp.controller('AutionController', ['$scope', '$http', '$mdDialog', '$mdToast', 'config', 'cfpLoadingBar', 'md5', '$window', 'app', '$rootScope', '$timeout', '$firebaseArray', '$uibModal', function AutionController($scope, $http, $mdDialog, $mdToast, config, cfpLoadingBar, md5, $window, app, $rootScope, $timeout, $firebaseArray, $uibModal) {
    $scope.page = 1;
    $scope.page_size = 6;
    $scope.query = "1=1";
    $scope.orderby = "";
    $scope.item_count = 0;
    //
    $scope.pageAH = 1;
    $scope.page_sizeAH = 8;
    $scope.item_countAH = 0;
    //
    $scope.q = {};
    
    $scope.login = {};
    $scope.bid = {};
    $scope.nick = {};
    $scope.disableBtn = { bt: false, btLogin: false };

    $scope.init = function () {
        cfpLoadingBar.start();
        $scope.customerId = app.data.CustomerId;
        $scope.nickName = undefined;
        if ($scope.customerId !== -1) {
            $scope.nickName = $window.localStorage.getItem("nickName");
            $scope.checkNickName = $window.localStorage.getItem("checkNickName");
        }
        if ($scope.checkNickName === undefined)
            $scope.checkNickName = false;
        $scope.loadListSessionKoi();
        $scope.loadListFarm();
        $scope.loadListTypeKoi();
        $scope.loadListAgeKoi();
        
        console.log($scope.customerId);
    };

    $scope.initOne = function () {
        cfpLoadingBar.start();

        $scope.access_token = app.data.access_token;
        $scope.customerId = app.data.CustomerId;
        //console.log($scope.access_token);
        //console.log($scope.customerId);
        $scope.bidPrice = 0;
        $scope.nickName = undefined;

        $scope.sessionKoi = [];
        if (JSON.parse($window.localStorage.getItem("SessionKoi")) !== undefined)
            $scope.sessionKoi = JSON.parse($window.localStorage.getItem("SessionKoi"));

        $scope.loadListFollowProduct();
        
        if ($scope.customerId !== -1) {
            $scope.nickName = $window.localStorage.getItem("nickName");
            $scope.checkNickName = $window.localStorage.getItem("checkNickName");
            $scope.getNickName();
        }
        if ($scope.checkNickName === undefined)
            $scope.checkNickName = false;

        $scope.login = {};
        $scope.bid = {};
        $scope.bid.NickName = $scope.nickName;
        $scope.bid.CustomerId = $scope.customerId;
        $scope.bid.SessionAutionId = $scope.sessionId;
        $scope.bid.ProductId = $scope.productId;

        $scope.koiMe = false;
        $scope.checkWin = false;
        $scope.checkBid = false;
        $scope.countAH = 0;
        //console.log($scope.customerId);
        //console.log($scope.timeEnd);
        $scope.SecondsRemains = [parseInt($scope.timeEnd)];
        $scope.Timer = $scope.InitTimer($scope.SecondsRemains);
        cfpLoadingBar.complete();

        //database
        var ref = firebase.database().ref();

        //list action
        //var listAction = $firebaseArray(ref.child("Action").child(app.data.userId));
        //$scope.listAction = listAction;

        var listAction = $firebaseArray(ref.child("Aution").child($scope.sessionId).child($scope.productId).limitToLast(1));
        //console.log(listAction);
        listAction.$watch(function (event) {
            var listAct = $firebaseArray(ref.child("Aution").child($scope.sessionId).child($scope.productId).child(event.key));
            //console.log("1:"+listAct);
            listAct.$watch(function (eventAct) {
                //console.log("2:"+event.event);
                if (event.event === "child_added" && eventAct.key === "ActionId") {
                    //$rootScope.$broadcast('onshipCompleted');
                    if ($scope.sessionId !== undefined && $scope.productId !== undefined) {
                        $scope.getPriceLatest();
                    }
                }
            });
        });

        if ($scope.sessionId !== undefined && $scope.productId !== undefined) {
            $scope.getPriceLatest();
        }

    };

    $scope.loadListFollowProduct = function () {
        //Bỏ những con cá hết thời gian đấu giá
        if ($scope.sessionKoi !== null) {
            if ($scope.sessionKoi.length > 0) {

                $scope.listSessionKoi2 = {};
                $scope.listSessionKoi2.SessionAutionId = $scope.sessionId;
                $scope.listSessionKoi2.TimeEnd = $scope.timeEnd;
                $scope.listSessionKoi2.ListProductKoi = $scope.sessionKoi;

                $scope.sessionKoi = [];
                var post = $http({
                    method: 'POST',
                    url: '/web/aution/getSessionProduct',
                    data: $scope.listSessionKoi2,
                    headers: { 'Authorization': 'bearer ' + app.data.access_token }
                });

                post.success(function successCallback(data, status, headers, config) {
                    cfpLoadingBar.complete();
                    if (data.meta.error_code === 200) {
                        $scope.sessionKoi = data.data;

                        console.log($scope.SecondsRemains);

                        $scope.sessionKoi.forEach(function (product) {
                            $scope.SecondsRemains = $scope.SecondsRemains.concat(parseInt(product.TimeEnd));
                            $scope.Timer = $scope.InitTimer($scope.SecondsRemains);
                        });
                        //add session
                        $window.localStorage.setItem("SessionKoi", JSON.stringify($scope.sessionKoi));
                    }
                }).error(function (data, status, headers, config) {
                });
            }
        }
    };

    $scope.loadListFarm = function () {
        var query = "TypeOriginId=3";
        $http.get("/web/aution/listManufacturer?page=1&query=" + query + "&order_by=", {
            headers: {}
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listFarm = data.data.data;
            }
        });
    };

    $scope.loadListTypeKoi = function () {
        var query = "TypeOriginId=2";
        $http.get("/web/aution/listManufacturer?page=1&query=" + query + "&order_by=", {
            headers: {}
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listTypeKoi = data.data.data;
            }
        });
    };

    $scope.loadListAgeKoi = function () {
        var query = "TypeAttributeId=1072";
        $http.get("/web/aution/listAgeKoi?page=1&query=" + query + "&order_by=", {
            headers: {}
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listAgeKoi = data.data.data;
            }
        });
    };

    $scope.loadListSessionKoi = function () {
        $http.get("/web/aution/listSessionKoi/" + $scope.sessionId + "/" + $scope.customerId + "?page=" + $scope.page + "&page_size=" + $scope.page_size + "&query=" + $scope.query + "&order_by=" + $scope.orderby, {
            headers: {}
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.item_count = data.data.metadata;
                $scope.metadata = data.data.metadata;
                $scope.listKois = data.data.data;
                //console.log($scope.listKois);
            }
        });
    };

    $scope.loadListAutionHistory = function () {
        $http.get("/web/aution/listAutionHistory/" + $scope.sessionId + "/" + $scope.productId + "?page=" + $scope.pageAH + "&page_size=" + $scope.page_sizeAH + "&query=1=1&order_by=", {
            headers: {}
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.item_countAH = data.data.metadata;
                $scope.listAutionHistory = data.data.data;
            }
        });
    };

    $scope.loadListAutionTop = function () {
        $http.get("/web/aution/listAutionTop/" + $scope.sessionId + "/" + $scope.productId + "/" + $scope.typeSession, {
            headers: {}
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.item_countAH = data.data.metadata;
                $scope.listAutionTop = data.data.data;
            }
        });
    };

    $scope.getNickName = function () {
        $http.get("/web/aution/getNickName/" + $scope.sessionId + "/" + $scope.customerId + "/" + $scope.productId, {
            headers: {}
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $window.localStorage.setItem("nickName", $scope.nickName);
                $window.localStorage.setItem("checkNickName", true);
                $scope.checkNickName = true;
            }
            else {
                $scope.checkNickName = false;
            }
        });
    };

    $scope.FollowKoi = function (item) {
        var ProductId = item.ProductId;
        //console.log($scope.customerId);
        if ($scope.customerId === undefined || $scope.customerId === -1) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng đăng nhập để thực hiện chức năng này.')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;
        }
        if (item.Status === 10) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn đã theo dõi Cá Koi trên!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;
        }
        //$scope.disableBtn.btnFollow = true;
        cfpLoadingBar.start();

        var post = $http({
            method: 'POST',
            url: '/web/customer/FollowProduct/' + $scope.customerId + "/" + ProductId,
            data: undefined,
            headers: { 'Authorization': 'bearer ' + app.data.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            //$scope.disableBtn.btnFollow = false;
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                item.Status = 10;
                $scope.status = 10;
                $mdToast.show($mdToast.simple()
                    .theme("success-toast")
                    .textContent(data.meta.error_message)
                    .position('fixed bottom right')
                    .hideDelay(3500));
                //var ElementMbId = "ProductMb-" + ProductId;
                //var ElementDkId = "ProductDk-" + ProductId;
                //angular.element(document.querySelector('#' + ElementMbId)).css('display', 'none');
                //angular.element(document.querySelector('#' + ElementDkId)).css('display', 'none');
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
            //$scope.disableBtn.btnFollow = false;
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

    $scope.RemoveKoiFollow = function (ProductId) {
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
                $scope.loadListSessionKoi();
                $scope.status = 1;
                //console.log($scope.status);
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

    $scope.searchKoi = function () {
        var query = '1=1';

        //if ($scope.q.txtSearch != '-1' && $scope.q.txtSearch != undefined) {
        //    if (query != '')
        //        query += ' and Name.Contains("' + $scope.q.txtSearch + '")';
        //    else
        //        query += 'Name.Contains("' + $scope.q.txtSearch + '")';
        //}

        if ($scope.FarmKoi !== '-1' && $scope.FarmKoi !== undefined) {
            query += ' and ManufacturerId==' + $scope.FarmKoi;
        }

        if ($scope.TypeKoi !== '-1' && $scope.TypeKoi !== undefined) {
            query += ' and TrademarkId==' + $scope.TypeKoi;
        }

        if ($scope.WidthKoi !== '-1' && $scope.WidthKoi !== undefined) {
            if ($scope.WidthKoi === '1')
                query += ' and Width >= 0 and Width <= 20';
            else if ($scope.WidthKoi === '2')
                query += ' and Width >= 21 and Width <= 40';
            else if ($scope.WidthKoi === '3')
                query += ' and Width >= 41 and Width <= 60';
            else if ($scope.WidthKoi === '4')
                query += ' and Width >= 61 and Width <= 80';
            else if ($scope.WidthKoi === '5')
                query += ' and Width >= 81';
        }

        if ($scope.SexKoi !== '-1' && $scope.SexKoi !== undefined) {
            query += ' and ProductSex==' + $scope.SexKoi;
        }

        if ($scope.AgeKoi !== '-1' && $scope.AgeKoi !== undefined) {
            query += ' and ProductAge==' + $scope.AgeKoi;
        }

        $scope.query = query;

        $scope.loadListSessionKoi();
    };

    $scope.checkKoiMe = function () {
        console.log($scope.koiMe);
        //$scope.koiMe = !$scope.koiMe;
        if ($scope.koiMe) {
            $scope.listKoisNew = $scope.listKois;
            $scope.productK = [];
            $scope.listKoisNew2 = JSON.parse($window.localStorage.getItem("SessionKoi"));
            //console.log($scope.listKoisNew2);
            $scope.listKoisNew2.forEach(function (product) {
                //console.log(product);
                if (product.SessionAutionId === $scope.sessionId) {
                    $scope.productK = $scope.productK.concat(product);
                }
            });
            $scope.listKois = $scope.productK;
        }
        else if ($scope.listKoisNew !== undefined) {
            $scope.listKois = $scope.listKoisNew;
        }
    };

    $scope.selectAution = function (product) {
        //$window.localStorage.removeItem("SessionKoi");
        $scope.sessionKoi = [];
        $scope.sessionKoi = JSON.parse($window.localStorage.getItem("SessionKoi"));
        console.log(product);
        console.log($scope.sessionKoi);
        //Bỏ những con cá hết thời gian đấu giá
        if ($scope.sessionKoi !== undefined && $scope.sessionKoi !== null) {
            var loop = false;
            for (var i = 0; i < $scope.sessionKoi.length; i++) {
                if ($scope.sessionKoi[i].ProductId === product.ProductId && $scope.sessionKoi[i].SessionAutionId === $scope.sessionId) {
                    loop = true;
                    break;
                }
            }

            if (!loop) {
                $scope.sessionKoi = $scope.sessionKoi.concat(product);
                $scope.loadListFollowProduct();
                            $window.localStorage.setItem("SessionKoi", JSON.stringify($scope.sessionKoi));
            }
        }
        else {
            $scope.sessionKoi = [];
            $scope.sessionKoi = $scope.sessionKoi.concat(product);
            $scope.loadListFollowProduct();
                        $window.localStorage.setItem("SessionKoi", JSON.stringify($scope.sessionKoi));
        }

        //if ($scope.sessionKoi !== undefined && $scope.sessionKoi !== null ) {
        //    var loop = false;
        //    angular.forEach($scope.sessionKoi, function (item, key) {
        //        if (item.ProductId === product.ProductId) {
        //            loop = true;
        //        }
        //    });
        //    if (!loop) $scope.sessionKoi.push(product);
        //}
        //else {
        //    $scope.sessionKoi = [];
        //    $scope.sessionKoi.push(product);
        //}

        //$window.localStorage.setItem("SessionKoi", JSON.stringify($scope.sessionKoi));
    };

    $scope.deleteSessionKoi = function (ProductId) {
        //$scope.sessionKoi = [];
        if ($scope.sessionKoi !== undefined) {
            angular.forEach($scope.sessionKoi, function (item, key) {
                if (item.ProductId === ProductId) {
                    $scope.sessionKoi.splice(key, 1);
                }
            });

            if ($scope.sessionKoi.length === 0) $scope.sessionKoi = null;
        }

        $window.localStorage.setItem("SessionKoi", JSON.stringify($scope.sessionKoi));
    };

    $scope.setNickName = function () {
       
        if ($scope.nickName === undefined || $scope.nickName === '') {
            $scope.showMes = "Bạn chưa nhập Nickname!";
            return;
        }

        $scope.showMes = "";
        //Nếu phù hợp thì thêm vào db
        $scope.nick = {};
        $scope.nick.NickName = $scope.nickName;
        $scope.nick.CustomerId = $scope.customerId;
        $scope.nick.SessionAutionId = $scope.sessionId;
        $scope.nick.ProductId = $scope.productId;

        $scope.disableBtn.btnNew = true;

        var post = $http({
            method: 'POST',
            url: '/web/aution/addNickName',
            data: $scope.nick,
            headers: { 'Authorization': 'bearer ' + app.data.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            if (data.meta.error_code === 200) {
                $scope.disableBtn.btnNew = false;

                $window.localStorage.setItem("nickName", $scope.nickName);
                $window.localStorage.setItem("checkNickName", true);
                $scope.checkNickName = true;
            }
            else {
                $scope.disableBtn.btnNew = false;
                $scope.showMes = data.meta.error_message;
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btnNew = false;
            $scope.showMes = "Đã xảy ra lỗi! Xin vui lòng thử lại sau!";
        });
    };

    $scope.changePrice = function () {
        console.log($scope.bidPrice);
        if ($scope.bidPrice !== undefined && $scope.bidPrice !== '') {
            $scope.bidPrice = parseInt($scope.bidPrice) ? parseInt($scope.bidPrice.replace(/\,/g, '')) : $scope.bidPrice;
        }
        else {
            $scope.bidPrice = '';
        }
    };

    $scope.bidKoi = function () {
        //console.log($scope.bidPrice);
        //$scope.bidPriceNew = $scope.bidPrice * 1000;
        $scope.bidPriceNew = $scope.bidPrice;
        console.log("Giá đấu:" + $scope.bidPriceNew + "; Giá hiện tại: " + $scope.priceSpecial);
        if ($scope.bidPrice === undefined || $scope.bidPrice === '') {
            $scope.showMes = "Bạn chưa nhập giá đấu!";
            return;
        }
        else if ($scope.bidPrice <= 0) {
            $scope.showMes = "Giá đấu phải là một số dương!";
            return;
        }
        else if ($scope.bidPriceNew <= $scope.priceStart) {
            $scope.showMes = "Giá đấu phải lớn hơn giá hiện tại!";
            return;
        }
        else if ($scope.typeSession === 3) {
            if ($scope.priceLatest !== undefined)
                $scope.priceSpecial = $scope.priceLatest.PriceWin;
            else
                $scope.priceSpecial = $scope.priceStart;

            if ($scope.bidPriceNew <= $scope.priceSpecial) {
                $scope.showMes = "Giá đấu phải lớn hơn giá hiện tại!";
                return;
            }
        }

        $scope.showMes = "";
        //Nếu phù hợp thì thêm vào db
        $scope.bid = {};
        $scope.bid.NickName = $scope.nickName;
        $scope.bid.CustomerId = $scope.customerId;
        $scope.bid.SessionAutionId = $scope.sessionId;
        $scope.bid.ProductId = $scope.productId;
        $scope.bid.ProductCode = $scope.productCode;
        $scope.bid.PriceNew = $scope.bidPriceNew;
        $scope.bid.TypeBid = $scope.typeSession;

        $scope.disableBtn.btnNew = true;

        var post = $http({
            method: 'POST',
            url: '/web/aution/bidKoi',
            data: $scope.bid,
            headers: { 'Authorization': 'bearer ' + app.data.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            if (data.meta.error_code === 200) {
                $scope.disableBtn.btnNew = false;

                $scope.showMes = "Bạn đã trả giá thành công, vui lòng vào email để xác nhận đấu giá!";
                $scope.bidPriceNew = undefined;
                $scope.bidPrice = undefined;
            }
            else {
                $scope.disableBtn.btnNew = false;
                $scope.showMes = data.meta.error_message;
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btnNew = false;
            $scope.showMes = "Đã xả ra lỗi! Xin vui lòng thử lại sau!";
        });
    };

    $scope.bidKoiBT = function (price) {
        //console.log($scope.bidPrice);
        //$scope.bidPriceNew = $scope.bidPrice * 1000;
        //console.log("Giá đấu:" + $scope.bidPriceNew + "; Giá hiện tại: " + $scope.priceSpecial);
        //if ($scope.bidPrice === undefined || $scope.bidPrice === '') {
        //    $scope.showMes = "Bạn chưa nhập giá đấu!";
        //    return;
        //}
        //else if ($scope.bidPrice <= 0) {
        //    $scope.showMes = "Giá đấu phải là một số dương!";
        //    return;
        //}
        //else if ($scope.bidPriceNew <= $scope.priceStart) {
        //    $scope.showMes = "Giá đấu phải lớn hơn giá khởi điểm!";
        //    return;
        //}
        //else if ($scope.typeSession === 3) {
        //    if ($scope.priceLatest !== undefined)
        //        $scope.priceSpecial = $scope.priceLatest.PriceWin;
        //    else
        //        $scope.priceSpecial = $scope.priceStart;

        //    if ($scope.bidPriceNew <= $scope.priceSpecial) {
        //        $scope.showMes = "Giá đấu phải lớn hơn giá trả hiện tại!";
        //        return;
        //    }
        //}

        $scope.showMes = "";
        //Nếu phù hợp thì thêm vào db
        $scope.bid = {};
        $scope.bid.NickName = $scope.nickName;
        $scope.bid.CustomerId = $scope.customerId;
        $scope.bid.SessionAutionId = $scope.sessionId;
        $scope.bid.ProductId = $scope.productId;
        $scope.bid.ProductCode = $scope.productCode;
        $scope.bid.PriceNew = price;
        $scope.bid.TypeBid = $scope.typeSession;

        $scope.disableBtn.btnNew = true;

        var post = $http({
            method: 'POST',
            url: '/web/aution/bidKoi',
            data: $scope.bid,
            headers: { 'Authorization': 'bearer ' + app.data.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            if (data.meta.error_code === 200) {
                $scope.disableBtn.btnNew = false;

                $scope.showMes = "Bạn đã tham gia bốc thăm thành công, vui lòng vào email để xác nhận!";
                $scope.bidPriceNew = undefined;
                $scope.bidPrice = undefined;
            }
            else {
                $scope.disableBtn.btnNew = false;
                $scope.showMes = data.meta.error_message;
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btnNew = false;
            $scope.showMes = "Đã xả ra lỗi! Xin vui lòng thử lại sau!";
        });
    };

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
        if (email === '' || email === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông tin')
                    .textContent('Bạn chưa điền tài khoản !')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;
        }

        if (password === '' || password === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông tin')
                    .textContent('Bạn chưa điền mật khẩu !')
                    .ok('Đóng')
                    .fullscreen(true)
            );
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

            if (data.meta.error_code === 200) {
                $scope.login = {};

                $scope.access_token = data.data.access_token;
                $scope.customerId = data.data.CustomerId;
                //console.log(data.data.FullName);
                $scope.nickName = data.data.FullName !== undefined ? data.data.FullName : "";
                $scope.checkNickName = undefined;

                //$window.localStorage.setItem("CustomerId", data.data.CustomerId);
                //$window.localStorage.setItem("CustomerEmail", data.data.Email);
                //$window.localStorage.setItem("CustomerFullName", data.data.FullName);
                //$window.localStorage.setItem("CustomerAvata", data.data.Avata);
                //$window.localStorage.setItem("CustomerAddress", data.data.Address);
                //$window.localStorage.setItem("CustomerPassword", data.data.Password);
                //$window.localStorage.setItem("CustomerPhoneNumber", data.data.PhomeNumber);
                //$window.localStorage.setItem("Customer_access_token", data.data.access_token);
                //$window.localStorage.setItem("CustomerSex", data.data.Sex);

                app.updateData(data.data.CustomerId, data.data.Email, data.data.FullName, data.data.Avata, data.data.Address, data.data.Password, data.data.PhomeNumber, data.data.access_token, data.data.Sex);
                $rootScope.$emit("UpdateUserInit", {});

                $window.localStorage.setItem("nickName", data.data.FullName);
                $window.localStorage.removeItem("checkNickName");

                //get nickname
                $scope.getNickName();
                //set timeout
                var now = new Date().getTime();
                $window.localStorage.setItem("Timeout", now);

                $scope.closeAddLoginModal();
                //$scope.goHome();
                //$mdToast.show($mdToast.simple()
                //    .textContent(data.meta.error_message)
                //    .position('fixed bottom right')
                //    .hideDelay(5000));
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
            $scope.disableBtn.btLogin = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông tin')
                    .textContent('Đã xả ra lỗi! Xin vui lòng thử lại sau.')
                    .ok('Đóng')
                    .fullscreen(true)
            );
        });

    };

    $scope.logOut = function () {
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
                app.data.CustomerId ="-1";
                app.data.FullName ="";
                app.data.access_token ="";
                app.updateData(app.data.CustomerId, app.data.Email, data.data.FullName, data.data.Avata, data.data.Address, app.data.Password, data.data.PhomeNumber, app.data.access_token, data.data.Sex);
                $rootScope.$emit("UpdateUserInit", {});
            }
        });
        
        //$window.location.href = '/';
    };

    //aution history
    var modalAddHistory;
    $scope.openAddHistoryModal = function () {
        //$scope.listAutionHistory = new Array();
        $scope.loadListAutionHistory();
        modalAddHistory = $uibModal.open({
            templateUrl: '/popup/aution-history.html',
            windowClass: 'fade auction',
            backdrop: 'static',
            scope: $scope,
            size: 'lg'
        });
    };

    $scope.closeAddHistoryModal = function () {
        modalAddHistory.dismiss('cancel');
    };

    //aution top bidkoi
    var modalAddTop;
    $scope.openAddTopModal = function () {
        $scope.loadListAutionTop();
        modalAddTop = $uibModal.open({
            templateUrl: '/popup/aution-top.html',
            windowClass: 'fade auction',
            backdrop: 'static',
            scope: $scope,
            size: 'lg'
        });
    };

    $scope.closeAddTopModal = function () {
        modalAddTop.dismiss('cancel');
    };

    //Time countdown
    $scope.Timer = [{ RemainingTime: '00 00:00:00', RemainingTimeStr: "Hour" }];

    //Converts seconds to days hours minutes and seconds
    $scope.SecondsToStr = function (count) {
        //var day = Math.floor(count / 86400);
        var hour = Math.floor(count / 3600);
        var min = Math.floor(count % 3600 / 60);
        var sec = Math.floor(count % 3600 % 60);
        var secondsToStr = '' + ('00' + hour).substr(-2) + ':' + ('00' + min).substr(-2) + ':' + ('00' + sec).substr(-2);
        var remainingTimeStr = 'Hour';
        if (hour > 1)
            remainingTimeStr = 'Hours';
        //if (day > 0) {
        //    secondsToStr = '' + day;
        //    remainingTimeStr = 'Day';
        //    if (day > 1)
        //        remainingTimeStr = 'Days';
        //}
        return { RemainingTime: secondsToStr, RemainingTimeStr: remainingTimeStr };
    };

    $scope.UpdateTimer = function () {
        if ((!$scope.Timer))
            return true;
        for (var i = 0; i < $scope.Timer.length; i++) {
            var cDate = new Date();
            var diff = (cDate - $scope.Timer[i].InitTime) / 1000;
            if (diff < $scope.Timer[i].Seconds) {
                var secondToStr = $scope.SecondsToStr($scope.Timer[i].Seconds - diff);
                $scope.Timer[i].RemainingTime = secondToStr.RemainingTime;
                $scope.Timer[i].RemainingTimeStr = secondToStr.RemainingTimeStr;
                //return true;
            }
            else {
                //$mdDialog.show(
                //    $mdDialog.alert()
                //        .clickOutsideToClose(true)
                //        .title('Thông tin')
                //        .textContent('Hết phiên đấu giá!')
                //        .ok('Đóng')
                //        .fullscreen(true)
                //);
                if (diff >= $scope.Timer[0].Seconds) {
                    $scope.checkWin = true;
                }
                $scope.Timer[i].RemainingTime = '00:00:00';
                //return false;
            }
        }
        return true;
    };

    $scope.ReportTimeoutExpired = function () {
        //do something to show time is expired
        $scope.popTimeoutMessage = true;
    };

    //time out in a second
    $scope.Countdown = function () {
        $timeout(function () {
            if ($scope.UpdateTimer())
                $scope.Countdown();
            else
                $scope.ReportTimeoutExpired();
        }, 1000);
    };

    //Initialize Timer
    $scope.InitTimer = function (counts) {

        var timers = [];
        for (var i = 0; i < counts.length; i++) {
            var timer = {
                RemainingTime: "",
                RemainingTimeStr: "",
                InitTime: new Date(),
                Seconds: counts[i]
            };
            timers.push(timer);
        };
        $scope.Countdown();
        return timers;
    };

    // populate from serverside to get real time
    //$scope.SecondsRemains = [$scope.timeEnd, 5000, 40000];
    //$scope.Timer = $scope.InitTimer($scope.SecondsRemains);

    //Phân trang
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
            default:
                break;
        }

        $scope.loadListSessionKoi();
    };

    $scope.loadding = true;

    $scope.getPriceLatest = function () {
        //$scope.loadding = true;
        cfpLoadingBar.start();
        if ($scope.sessionId !== undefined && $scope.productId !== undefined) {
            //$scope.query = "Type=1";
            $http.get("/web/aution/priceLatest/" + $scope.sessionId + "/" + $scope.productId + "/" + $scope.typeSession, { headers: { "Authorization": "bearer " + app.data.access_token } }).success(function (data, status, headers) {
                cfpLoadingBar.complete();
                if (data.meta.error_code === 200) {
                    if ($scope.page === 1) {
                        $scope.priceLatest = data.data;
                        $scope.NickNameWin = $scope.priceLatest.NickName;
                        $scope.PriceLatestWin = $scope.priceLatest.PriceWin;
                        $scope.checkBid = true;
                        $scope.countAH = data.metadata;
                        console.log($scope.countAH);
                        //console.log($scope.NickNameWin);
                        //console.log($scope.PriceLatestWin);
                        //console.log($scope.nickName);
                    }
                    //$scope.loadding = false;
                    cfpLoadingBar.complete();
                }
            }).error(function (data, status, headers, config) {
            });
        }
    };

    $scope.getPriceLatest();

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
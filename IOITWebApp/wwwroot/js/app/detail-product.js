myApp.controller('DetailProductController', ['$scope', '$http', '$mdDialog', '$mdToast', 'config', 'cfpLoadingBar', 'md5', '$window', 'app', '$rootScope', function DetailProductController($scope, $http, $mdDialog, $mdToast, config, cfpLoadingBar, md5, $window, app, $rootScope) {
    $scope.page = 1;
    $scope.page_size = 5;
    $scope.query = "1=1";
    $scope.q = {};
    $scope.orderby = "";
    $scope.item_count = 0;
    $scope.ProductReview = {};
    $scope.ProductAdd = {};
    $scope.offBtn = {};
    $scope.listProductCart = [];
    $scope.regexEmail = config.regexEmail;

    $scope.init = function (model) {

   



        $scope.ProductReview.NumberStar = 5;
        $scope.ProductAdd.quantity = +1;
        $scope.model = model;
        $scope.CustomerId = app.data.CustomerId;
        cfpLoadingBar.start();
      //  $scope.star = 0;
        $scope.Status = model.Status;
       // console.log(model);
        //console.log($scope.Status);
        $scope.metadata = {};
        $scope.GetProductReviews();
    };

    $scope.AddProductOrder = function (cs) {
        var quantity = 0;
        console.log(cs);
        console.log($window.localStorage.getItem("Order"));
       
       quantity = angular.element(document.querySelector('#quantity')).val();
        // console.log($window.localStorage.getItem("Order"));
        if ($window.localStorage.getItem("Order") != null && $window.localStorage.getItem("Order") != undefined) {
            $scope.order = JSON.parse($window.localStorage.getItem("Order"));
        }
       
        $scope.model.quantity = parseInt(quantity);
        if ($scope.order !== null && $scope.order !== undefined) {
            var loop = false;
            angular.forEach($scope.order, function (item, key) {
                if (item.ProductId === $scope.model.ProductId) {
                    if ($scope.model.TypeProduct === 1) {
                        item.quantity = item.quantity + $scope.model.quantity;
                    }
                    loop = true;
                }
            });

            if (!loop) $scope.order.push($scope.model);
        }
        else {
            $scope.order = [];
            $scope.order.push($scope.model);
        }

       
        $window.localStorage.setItem("Order", JSON.stringify($scope.order));
        $rootScope.$emit("UpdateCountOrder", {});
        switch (cs) {
            case 1:
                $mdToast.show($mdToast.simple()
                    .theme("success-toast")
                    .textContent("Thêm sản phẩm vào giỏ hàng thành công!")
                    .position('fixed bottom right')
                    .hideDelay(3500));
                break;
            case 2:
                $window.localStorage.setItem("Order", JSON.stringify($scope.order));
                $rootScope.$emit("UpdateCountOrder", {});
                $window.location.href = '/gio-hang.html';
                break;
            default: break;

        }
        $window.localStorage.setItem("Order", JSON.stringify($scope.order));
        $rootScope.$emit("UpdateCountOrder", {});
    };

    $scope.FollowKoi = function (ProductId) {
        if ($scope.CustomerId === undefined || $scope.CustomerId === -1) {
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

        //$scope.disableBtn.btnFollow = true;
        cfpLoadingBar.start();

        var post = $http({
            method: 'POST',
            url: '/web/customer/FollowProduct/' + $scope.CustomerId + "/" + ProductId,
            data: undefined,
            headers: { 'Authorization': 'bearer ' + app.data.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            //$scope.disableBtn.btnFollow = false;
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                $mdToast.show($mdToast.simple()
                    .theme("success-toast")
                    .textContent(data.meta.error_message)
                    .position('fixed bottom right')
                    .hideDelay(3500));
                var ElementMbId = "ProductMb-" + ProductId;
                var ElementDkId = "ProductDk-" + ProductId;
                angular.element(document.querySelector('#' + ElementMbId)).css('display', 'none');
                angular.element(document.querySelector('#' + ElementDkId)).css('display', 'none');
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

    $scope.LoveProduct = function (ProductId,type) {
        if ($scope.CustomerId === undefined || $scope.CustomerId === -1) {
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
                    location.reload();
                }
                $mdToast.show($mdToast.simple()
                    .theme("success-toast")
                    .textContent(data.meta.error_message)
                    .position('fixed bottom right')
                    .hideDelay(3500));
                $scope.Status = 10;
                //var ElementId = "Product-" + ProductId;
                //angular.element(document.querySelector('#' + ElementId)).removeClass('fa-heart-o');
                //angular.element(document.querySelector('#' + ElementId)).addClass('fa-heart');
                //angular.element(document.querySelector('#' + ElementId)).css('color', 'red');
                //var ElementSpanId = "ProductSpan-" + ProductId;
                //angular.element(document.querySelector('#' + ElementSpanId)).text("Sản phẩm yêu thích");

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

    $scope.RemoveLikeProduct = function (ProductId,type) {
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

    $scope.GetProductReviews = function () {
        $http.get("/web/product/GetProductReviews/" + $scope.model.ProductId + "?page=" + $scope.page + "&page_size=" + $scope.page_size + "&query=" + $scope.query + "&order_by=" + $scope.orderby, {
            headers: {}
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.item_count = data.data.metadata.item_count;
                $scope.metadata = data.data.metadata;
                $scope.ProductReviewResult = data.data.data;
            }
        });
    };

    $scope.PostProductReviews = function (id) {
        $scope.idProduct = id;
        if ($scope.ProductReview.NumberStar === "" || $scope.ProductReview.NumberStar === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập Điểm đánh giá!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;
        }

        if ($scope.ProductReview.Contents === "" || $scope.ProductReview.Contents === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập Nội dung đánh giá!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                $scope.focusElement("ProductReviewContents");
            });
            return;
        }

        if ($scope.ProductReview.Name === "" || $scope.ProductReview.Name === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập Tên!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                $scope.focusElement("ProductReviewName");
            });
            return;
        }

        if ($scope.ProductReview.Email === "" || $scope.ProductReview.Email === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập Email hoặc Email bạn nhập không chính xác!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                $scope.focusElement("ProductReviewEmail");
            });
            return;
        }

        $scope.offBtn.btnSendReview = true;
        $scope.ProductReview.ProductId = id;
        var post = $http({
            method: 'POST',
            url: '/web/product/PostProductReviews/' + id,
            data: $scope.ProductReview,
            headers: {}
        });

        post.success(function successCallback(data, status, headers, config) {
            cfpLoadingBar.complete();
            $scope.offBtn.btnSendReview = false;
            if (data.meta.error_code === 200) {
                $scope.GetProductReviews();
                $scope.ProductReview.NumberStar = 5;
                $scope.ProductReview.Name = '';
                $scope.ProductReview.Email = '';
                $scope.ProductReview.Contents = '';
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Bình luận sản phẩm thành công !')
                        .ok('Close')
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
            $scope.offBtn.btnSendReview = false;
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
    $scope.DiemDanhGia = function (d) {
        $scope.ProductReview.NumberStar = '';
        $scope.ProductReview.NumberStar = d;
        console.log($scope.ProductReview.NumberStar);
    }

    //binh luan
    $scope.Comment = function (id) {
        console.log('o0000');

        $http.get("/web/Product/GetProductReviews/"+id+"?page=1&query=1=1&order_by=", {
            headers: {}
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listCodePhone = data.data.data;

            }
        });
    }
    //them vao gio hang
    $scope.AddtoCart = function (model1) {
        $scope.ProductAdd = {};
        $scope.quantityCart = 0;
        var x = document.getElementById("quantity").value;
        $scope.ProductAdd = model1;
        $scope.ProductAdd.quantity = +x;
        console.log($scope.ProductAdd);
        $scope.listProductCart.push($scope.ProductAdd);
        console.log($scope.listProductCart);
        for (let i = 0; i < $scope.listProductCart.length; i++) {
            $scope.quantityCart += $scope.listProductCart[i].quantity;
        }
      
    }
}]);
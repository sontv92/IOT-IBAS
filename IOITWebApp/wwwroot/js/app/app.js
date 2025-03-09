var myApp = angular.module('IOITWeb', ['angular-loading-bar', 'ngMaterial', 'ngMd5', 'ngCookies', 'ui.bootstrap', 'ui.carousel']);

myApp.value('config', {
    domain: 'http://localhost:5001/',
    domainPay: 'https://mtf.onepay.vn/paygate/vpcpay.op',
    //domainPay: 'https://onepay.vn/paygate/vpcpay.op',
    lang: 'vn',
    exchangeRate: '1',
    title: 'APC VIET NAM',
    againLink: 'tmdt.cnttvietnam.com.vn/gio-hang.html',
    paymentLink: 'thuc-hien-thanh-toan.html',
    resultfLink: 'ket-qua-don-hang.html',
    cardList: '970436',
    //opMerchant: 'OP_LTEVISA',
    //opAccessCode: 'E1101B05',
    opMerchant: 'TESTONEPAY',
    opAccessCode: '6BEB2546',
    keyCaptcha: '6LcPV8oUAAAAAJnnznu4E6jaNrXEWZdsrC3mRj6T',
    secretCaptcha: '6LcPV8oUAAAAAMT4e20qOOhLu4EeSWZv32-jDye6',
    regexEmail: /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/,
    regexPhone: /^(0[35789])[0-9]{8}$/
});

myApp.config(['cfpLoadingBarProvider', function (cfpLoadingBarProvider) {
    cfpLoadingBarProvider.includeSpinner = false;
}]);

myApp.config(function ($sceProvider) {
    $sceProvider.enabled(false);
});

myApp.factory('app', function () {
    return {
        data: {
            domain: 'http://localhost:5001/',
            CustomerId: -1,
            Email: '',
            FullName: '',
            Avata: '',
            Address: '',
            Password: '',
            PhomeNumber: '',
            access_token: '',
            Sex: ''
        },
        updateData: function (CustomerId, CustomerEmail, CustomerFullName, CustomerAvata, CustomerAddress, CustomerPassword, CustomerPhoneNumber, access_token, CustomerSex) {
            this.data.CustomerId = CustomerId;
            this.data.Email = CustomerEmail;
            this.data.FullName = CustomerFullName;
            this.data.Avata = CustomerAvata;
            this.data.Address = CustomerAddress;
            this.data.Password = CustomerPassword;
            this.data.PhomeNumber = CustomerPhoneNumber;
            this.data.access_token = access_token;
            this.data.Sex = CustomerSex;
        }
    };
});

//myApp.config(function () {

//    var config = {
//        apiKey: "AIzaSyBMiFAeztwaRURX1LW7JXnaKsp5gLsmc_M",
//        authDomain: "autionkoi.firebaseapp.com",
//        databaseURL: "https://autionkoi.firebaseio.com",
//        projectId: "autionkoi",
//        storageBucket: "",
//        messagingSenderId: "212371371238",
//        appId: "1:212371371238:web:7b1831199020752f"
//    };
//    firebase.initializeApp(config);

//});

myApp.filter('iif', function () {
    return function (input, trueValue, falseValue) {
        return input ? trueValue : falseValue;
    };
});

myApp.filter("formatPrice", function () {
    return function (price, digits, thoSeperator, decSeperator, bdisplayprice) {
        //console.log("displayprice: " + price);
        var i;
        if (price === null || price === '') {
            return '';
        }
        price = (typeof price === "undefined") ? 0 : price;
        digits = (typeof digits === "undefined") ? 3 : digits;
        bdisplayprice = (typeof bdisplayprice === "undefined") ? true : bdisplayprice;
        thoSeperator = (typeof thoSeperator === "undefined") ? "." : thoSeperator;
        decSeperator = (typeof decSeperator === "undefined") ? "," : decSeperator;
        price = (typeof price === undefined) ? "0" : price;

        if (price !== 0) {
            if (digits === 0)
                price = Math.round(price);
            //console.log(price);
            var prices = 0 - price;
            if (price > 0) {
                prices = price;
            }
            prices = prices + "";
            var _temp = prices.split('.');
            var dig = (typeof _temp[1] === "undefined") ? "00" : _temp[1];
            if (bdisplayprice && parseInt(dig, 10) === 0) {
                dig = "";
            } else {
                dig = dig + "";
                if (dig.length > digits) {
                    dig = (Math.round(parseFloat("0." + dig) * Math.pow(10, digits))) + "";
                }
                for (i = dig.length; i < digits; i++) {
                    dig += "0";
                }
            }
            var num = _temp[0];
            var s = "",
                ii = 0;
            for (i = num.length - 1; i > -1; i--) {
                s = ((ii++ % 3 === 2) ? ((i > 0) ? thoSeperator : "") : "") + num.substr(i, 1) + s;
            }
        }
        else {
            s = 0;
        }

        if (price < 0) {
            s = '- ' + s;
        }
        if (dig > 0) {
            return s + decSeperator + dig;
        }
        else {
            return s;
        }
    }
});

//Đinh dạng giá trong ô input
myApp.filter("displayprice", function () {
    return function (input) {
        //console.log("displayprice: " + input);
        input = (typeof input === 'undefined' || input === '') ? "" : input + "";
        if (parseInt(input) === 0) {
            input = 0 + "";
        }
        var comma = ",";
        var num = parseInt(input) ? parseInt(input.replace(/\./g, '')) : input;
        //var num = parseInt(input) ? parseInt(input.replace(/[^\d|\-+|\.+]/g, '')) : input;

        //console.log("displayprice2: " + input);
        var nums = 0;
        if (num >= 0) {
            nums = num;
        }
        else {
            nums = 0 - num;
        }
        nums = nums + "";

        var str = "";

        var k = (nums.length % 3);
        if (k > 0) {
            str += nums.substring(0, k) + comma;
        }

        while (k < nums.length) {

            str += nums.substring(k, k + 3) + comma;
            k = k + 3;
        }
        if (num >= 0) {
            str = str.substring(0, str.length - 1);
        }
        else {
            str = "-" + str.substring(0, str.length - 1);
        }
        return str;
    }
});

myApp.directive('format', ['$filter', function ($filter) {
    return {
        require: '?ngModel',
        link: function (scope, elem, attrs, ctrl) {
            if (!ctrl) return;

            ctrl.$formatters.unshift(function (a) {
                return $filter(attrs.format)(ctrl.$modelValue, "", 0)
            });

            elem.bind('blur', function (event) {
                var plainNumber = elem.val().replace(/[^\d|\-+|\.+]/g, '');
                elem.val($filter(attrs.format)(plainNumber, "", 0));
            });
        }
    };
}]);

myApp.directive('clickEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if (event.which === 13) {
                scope.$apply(function () {
                    scope.$eval(attrs.clickEnter);
                });

                event.preventDefault();
            }
        });
    };
});


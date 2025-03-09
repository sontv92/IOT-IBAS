"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var const_1 = require("./../../data/const");
var auth_service_1 = require("../../service/auth.service");
var ngx_cookie_service_1 = require("ngx-cookie-service");
var Model_1 = require("./../../data/Model");
var modal_1 = require("ngx-bootstrap/modal");
var http_1 = require("@angular/common/http");
var ngx_toastr_1 = require("ngx-toastr");
var md5_1 = require("ts-md5/dist/md5");
var model_1 = require("../../data/model");
var DefaultLayoutComponent = /** @class */ (function () {
    function DefaultLayoutComponent(auth, cookie, toastr, http) {
        var _this = this;
        this.auth = auth;
        this.cookie = cookie;
        this.toastr = toastr;
        this.http = http;
        this.navItem = [];
        this.sidebarMinimized = true;
        this.element = document.body;
        this.userChangePass = new Model_1.UserChangePass();
        this.domainImage = const_1.domainImage;
        setTimeout(function () {
            var json = JSON.parse(localStorage.getItem('menu'));
            console.log(json);
            _this.Item = new model_1.User();
            _this.navItem.push({
                icon: "fas fa-tachometer-alt",
                name: "Dashboard",
                url: "/dashboard"
            });
            for (var i = 0; i < json.length; i++) {
                _this.navItem.push(_this.createMenu(json[i], undefined));
            }
            console.log(_this.navItem);
        }, 1000);
        this.changes = new MutationObserver(function (mutations) {
            _this.sidebarMinimized = document.body.classList.contains('sidebar-minimized');
        });
        this.changes.observe(this.element, {
            attributes: true
        });
        this.myFuntion();
        this.userChangePass.UserId = parseInt(localStorage.getItem("userId"));
        this.userChangePass.UserName = localStorage.getItem("userName");
        this.userChangePass.Avatar = localStorage.getItem("avata");
        this.userChangePass.Logo = localStorage.getItem("logo");
        this.userChangePass.FullName = localStorage.getItem("fullName");
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    DefaultLayoutComponent.prototype.ngOnInit = function () {
        var json = JSON.parse(localStorage.getItem('roles'));
        this.companyId = parseInt(localStorage.getItem('companyId'));
        this.UserId = parseInt(localStorage.getItem('userId'));
        this.BranchId = localStorage.getItem('BranchId');
        this.userChangePass.Logo = localStorage.getItem("logo");
        console.log(this.userChangePass.Logo);
        if (json.length > 0) {
            for (var i = 0; i < json.length; i++) {
                this.role = json[i].RoleId;
            }
        }
    };
    DefaultLayoutComponent.prototype.createMenu = function (item, urlParent) {
        item["name"] = item["Name"];
        item["url"] = urlParent == undefined ? "/" + item["Url"] : urlParent + "/" + item["Url"];
        item["icon"] = item["Icon"];
        delete item["MenuId"];
        delete item["Code"];
        delete item["Name"];
        delete item["MenuParent"];
        delete item["Url"];
        delete item["Icon"];
        delete item["ActiveKey"];
        delete item["Status"];
        if (item["listMenus"].length > 0) {
            item["children"] = [];
            for (var i = 0; i < item["listMenus"].length; i++) {
                item["children"].push(item["listMenus"][i]);
                this.createMenu(item["children"][i], item["url"]);
            }
        }
        delete item["listMenus"];
        return item;
    };
    DefaultLayoutComponent.prototype.logout = function () {
        this.auth.logout();
    };
    DefaultLayoutComponent.prototype.myFuntion = function () {
        var _this = this;
        //
        setInterval(function () {
            if (_this.cookie.get("Expire") == '' || _this.cookie.get("Expire") == undefined || localStorage.getItem('isLoggedIn') != "true") {
                _this.auth.logout();
            }
        }, 10000);
    };
    DefaultLayoutComponent.prototype.OpenChangePasswordModal = function () {
        this.userChangePass.PasswordOldE = undefined;
        this.userChangePass.PasswordNewE = undefined;
        this.userChangePass.ConfirmPassword = undefined;
        this.changePasswordModal.show();
    };
    DefaultLayoutComponent.prototype.ChangePassword = function () {
        var _this = this;
        if (this.userChangePass.PasswordOldE == undefined || this.userChangePass.PasswordOldE == '') {
            this.toastWarning("Chưa nhập Mật khẩu hiện tại!");
            return;
        }
        else if (this.userChangePass.PasswordNewE == undefined || this.userChangePass.PasswordNewE == '') {
            this.toastWarning("Chưa nhập Mật khẩu mới!");
            return;
        }
        else if (this.userChangePass.ConfirmPassword == undefined || this.userChangePass.ConfirmPassword == '') {
            this.toastWarning("Chưa nhập Mật khẩu xác nhận!");
            return;
        }
        else if (this.userChangePass.ConfirmPassword != this.userChangePass.PasswordNewE) {
            this.toastWarning("Mật khẩu xác nhận không đúng!");
            return;
        }
        this.userChangePass.PasswordOld = md5_1.Md5.hashStr(this.userChangePass.PasswordOldE).toString();
        this.userChangePass.PasswordNew = md5_1.Md5.hashStr(this.userChangePass.PasswordNewE).toString();
        this.http.put('/api/user/changePass/' + this.userChangePass.UserId, this.userChangePass, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.changePasswordModal.hide();
                _this.toastSuccess("Đổi mật khẩu tài khoản thành công!");
            }
            else if (res["meta"]["error_code"] == 213) {
                _this.toastError("Mật khẩu hiện tại không đúng. Vui lòng thử lại!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
        });
    };
    //Thông báo
    DefaultLayoutComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    DefaultLayoutComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    DefaultLayoutComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    DefaultLayoutComponent.prototype.OpenModalInfo = function () {
        var _this = this;
        this.Item = new model_1.User();
        this.file.nativeElement.value = "";
        this.http.get('/api/user/infoUser/' + this.userChangePass.UserId, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.Item = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
        this.modalMyInfo.show();
    };
    DefaultLayoutComponent.prototype.upload = function (files) {
        var _this = this;
        if (files.length === 0)
            return;
        var formData = new FormData();
        for (var _i = 0, files_1 = files; _i < files_1.length; _i++) {
            var file = files_1[_i];
            formData.append(file.name, file);
        }
        var uploadReq = new http_1.HttpRequest('POST', 'api/upload/uploadImage/6', formData, {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            }),
            reportProgress: true,
        });
        this.http.request(uploadReq).subscribe(function (event) {
            if (event.type === http_1.HttpEventType.UploadProgress) {
            }
            else if (event.type === http_1.HttpEventType.Response) {
                _this.Item.Avata = event.body["data"].toString();
            }
        });
    };
    DefaultLayoutComponent.prototype.SaveInfo = function () {
        var _this = this;
        if (this.Item.FullName == undefined || this.Item.FullName == '') {
            this.toastWarning("Chưa nhập Tên người dùng!");
            return;
        }
        else if (this.Item.UserName == undefined || this.Item.UserName == '') {
            this.toastWarning("Chưa nhập Tên tài khoản!");
            return;
        }
        this.http.put('/api/user/changeInfoUser', this.Item, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.modalMyInfo.hide();
                _this.toastSuccess("Lưu thông tin thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    __decorate([
        core_1.ViewChild('ChangePasswordModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], DefaultLayoutComponent.prototype, "changePasswordModal", void 0);
    __decorate([
        core_1.ViewChild('modalMyInfo'),
        __metadata("design:type", modal_1.ModalDirective)
    ], DefaultLayoutComponent.prototype, "modalMyInfo", void 0);
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], DefaultLayoutComponent.prototype, "file", void 0);
    DefaultLayoutComponent = __decorate([
        core_1.Component({
            selector: 'app-dashboard',
            templateUrl: './default-layout.component.html',
            styleUrls: ['./default-layout.component.scss']
        }),
        __metadata("design:paramtypes", [auth_service_1.AuthService, ngx_cookie_service_1.CookieService, ngx_toastr_1.ToastrService, http_1.HttpClient])
    ], DefaultLayoutComponent);
    return DefaultLayoutComponent;
}());
exports.DefaultLayoutComponent = DefaultLayoutComponent;
//# sourceMappingURL=default-layout.component.js.map
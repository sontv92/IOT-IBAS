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
var forms_1 = require("@angular/forms");
var router_1 = require("@angular/router");
var auth_service_1 = require("../../service/auth.service");
var http_1 = require("@angular/common/http");
var md5_1 = require("ts-md5/dist/md5");
var ngx_cookie_service_1 = require("ngx-cookie-service");
var ngx_toastr_1 = require("ngx-toastr");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var auth_guard_1 = require("../../auth.guard");
var platform_browser_1 = require("@angular/platform-browser");
var httpOptions = {
    headers: new http_1.HttpHeaders({
        'Content-Type': 'application/json',
        'Access-Control-Allow-Credentials': 'true',
        'Access-Control-Allow-Origin': '*',
        'Access-Control-Allow-Methods': 'OPTIONS, GET, POST',
        'Access-Control-Allow-Headers': 'Origin, Content-Type, Accept, Access-Control-Allow-Origin'
    }),
    withCredentials: true
};
var md5 = new md5_1.Md5();
var formData = new FormData();
var LoginComponent = /** @class */ (function () {
    function LoginComponent(formBuilder, router, authService, http, cookieService, toastr, modalDialogService, viewRef, authGuard, title) {
        this.formBuilder = formBuilder;
        this.router = router;
        this.authService = authService;
        this.http = http;
        this.cookieService = cookieService;
        this.toastr = toastr;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.authGuard = authGuard;
        this.title = title;
    }
    LoginComponent.prototype.ngOnInit = function () {
        //this.cookieService.set('Expire', '');
        this.title.setTitle("Đăng nhập");
        this.submitted = false;
        this.loginForm = this.formBuilder.group({
            username: ['', forms_1.Validators.required],
            password: ['', forms_1.Validators.required]
        });
        this.returnUrl = '/dashboard';
        // if(this.authGuard.canActivate) {
        //   this.router.navigate([this.returnUrl]);
        // }
        //this.authService.logout();
    };
    Object.defineProperty(LoginComponent.prototype, "f", {
        get: function () { return this.loginForm.controls; },
        enumerable: true,
        configurable: true
    });
    ;
    LoginComponent.prototype.login = function () {
        var _this = this;
        this.submitted = false;
        if (this.loginForm.invalid) {
            this.submitted = true;
            return;
        }
        else {
            var email = this.f.username.value;
            var password = md5_1.Md5.hashStr(this.f.password.value);
            this.http.post('/api/user/login', JSON.stringify({ email: email, password: password }), httpOptions).subscribe(function (res) {
                var data = JSON.stringify(res);
                if (res["meta"]["error_code"] == 200) {
                    localStorage.setItem('isLoggedIn', "true");
                    localStorage.setItem('data', res.toString());
                    localStorage.setItem('access_token', res["data"]["access_token"].toString());
                    localStorage.setItem('access_key', res["data"]["access_key"].toString());
                    localStorage.setItem('userId', res["data"]["userId"].toString());
                    localStorage.setItem('userName', res["data"]["userName"].toString());
                    localStorage.setItem('avata', res["data"]["avata"] != undefined ? res["data"]["avata"].toString() : undefined);
                    localStorage.setItem('fullName', res["data"]["fullName"] != undefined ? res["data"]["fullName"].toString() : undefined);
                    localStorage.setItem('companyId', res["data"]["companyId"] != undefined ? res["data"]["companyId"].toString() : undefined);
                    localStorage.setItem('languageId', res["data"]["languageId"] ? res["data"]["languageId"].toString() : undefined);
                    localStorage.setItem('websiteId', res["data"]["websiteId"] != undefined ? res["data"]["websiteId"].toString() : undefined);
                    localStorage.setItem('BranchId', res["data"]["BranchId"] != undefined ? res["data"]["BranchId"].toString() : undefined);
                    localStorage.setItem('PMQLXe', res["data"]["PMQLXe"] != undefined ? res["data"]["PMQLXe"].toString() : undefined);
                    localStorage.setItem('menu', JSON.stringify(res["data"]["listMenus"]));
                    localStorage.setItem('roles', JSON.stringify(res["data"]["listRoles"]));
                    _this.cookieService.set('Expire', Date.now().toLocaleString(), 0.1);
                    _this.router.navigate([_this.returnUrl]);
                }
                else {
                    _this.submitted = true;
                    _this.message = "Tài khoản hoặc mật khẩu không đúng";
                    _this.router.navigate(['/login']);
                }
            }, function (err) {
                _this.showConfirm("Đăng nhập không thành công. Xin vui lòng thử lại sau!");
            });
        }
    };
    LoginComponent.prototype.toat = function () {
        this.toastr.info('Phần này tạm thời chưa có nhé', 'Thông báo');
    };
    LoginComponent.prototype.showConfirm = function (message) {
        this.modalDialogService.openDialog(this.viewRef, {
            title: 'Thông báo',
            childComponent: ngx_modal_dialog_1.SimpleModalComponent,
            data: {
                text: message
            },
            actionButtons: [
                {
                    text: 'Xác nhận',
                    buttonClass: 'btn btn-success'
                }
            ],
        });
    };
    LoginComponent = __decorate([
        core_1.Component({
            selector: 'app-dashboard',
            templateUrl: 'login.component.html',
            styleUrls: ['login.component.css']
        }),
        __metadata("design:paramtypes", [forms_1.FormBuilder,
            router_1.Router,
            auth_service_1.AuthService,
            http_1.HttpClient,
            ngx_cookie_service_1.CookieService,
            ngx_toastr_1.ToastrService,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            auth_guard_1.AuthGuard,
            platform_browser_1.Title])
    ], LoginComponent);
    return LoginComponent;
}());
exports.LoginComponent = LoginComponent;
//# sourceMappingURL=login.component.js.map
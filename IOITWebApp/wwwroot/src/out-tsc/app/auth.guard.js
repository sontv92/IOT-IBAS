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
var router_1 = require("@angular/router");
var ngx_cookie_service_1 = require("ngx-cookie-service");
var AuthGuard = /** @class */ (function () {
    function AuthGuard(router, cookie) {
        this.router = router;
        this.cookie = cookie;
    }
    AuthGuard.prototype.canActivate = function (route, state) {
        var url = state.url;
        return this.verifyLogin(url);
    };
    AuthGuard.prototype.verifyLogin = function (url) {
        console.log(url);
        console.log(this.isLoggedIn());
        if (!this.isLoggedIn()) {
            if (url != '/login') {
                this.router.navigate(['/login']);
                return false;
            }
            return true;
        }
        else {
            if (this.isLoggedIn()) {
                if (url == '/login') {
                    this.router.navigate(['/dashboard']);
                    return false;
                }
                var arr = new Array();
                var json = JSON.parse(localStorage.getItem('menu'));
                json.push({
                    ActiveKey: "111111111",
                    Code: "",
                    Icon: "fa fa-tachometer",
                    MenuId: undefined,
                    MenuParent: 0,
                    Name: "Dashboard",
                    Status: undefined,
                    Url: "dashboard",
                    listMenus: []
                });
                for (var i = 0; i < json.length; i++) {
                    this.checkUrlPermission(arr, undefined, json[i]);
                }
                if (arr.indexOf(url) == -1) {
                    this.router.navigate(['/404']);
                }
                else {
                    return true;
                }
                return true;
            }
        }
    };
    AuthGuard.prototype.isLoggedIn = function () {
        var status = false;
        if (localStorage.getItem('isLoggedIn') == "true" && this.cookie.get("Expire") != '') {
            status = true;
        }
        else {
            status = false;
        }
        return status;
    };
    AuthGuard.prototype.checkUrlPermission = function (arr, urlParent, item) {
        var url = urlParent == undefined ? "/" + item["Url"] : urlParent + "/" + item["Url"];
        arr.push(url);
        if (item["listMenus"].length > 0) {
            for (var i = 0; i < item["listMenus"].length; i++) {
                this.checkUrlPermission(arr, url, item["listMenus"][i]);
            }
        }
    };
    AuthGuard = __decorate([
        core_1.Injectable({
            providedIn: 'root'
        }),
        __metadata("design:paramtypes", [router_1.Router, ngx_cookie_service_1.CookieService])
    ], AuthGuard);
    return AuthGuard;
}());
exports.AuthGuard = AuthGuard;
//# sourceMappingURL=auth.guard.js.map
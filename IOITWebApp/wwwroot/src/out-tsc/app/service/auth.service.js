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
var AuthService = /** @class */ (function () {
    function AuthService(router, cookieService) {
        this.router = router;
        this.cookieService = cookieService;
    }
    AuthService.prototype.logout = function () {
        localStorage.setItem('isLoggedIn', "false");
        localStorage.removeItem('data');
        localStorage.removeItem('access_token');
        localStorage.removeItem('access_key');
        localStorage.removeItem('userId');
        localStorage.removeItem('userName');
        localStorage.removeItem('avata');
        localStorage.removeItem('fullName');
        localStorage.removeItem('companyId');
        localStorage.removeItem('languageId');
        localStorage.removeItem('menu');
        this.cookieService.delete("Expire");
        this.router.navigate(['/login']);
    };
    AuthService = __decorate([
        core_1.Injectable({
            providedIn: 'root'
        }),
        __metadata("design:paramtypes", [router_1.Router, ngx_cookie_service_1.CookieService])
    ], AuthService);
    return AuthService;
}());
exports.AuthService = AuthService;
//# sourceMappingURL=auth.service.js.map
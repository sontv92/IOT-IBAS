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
var operators_1 = require("rxjs/operators");
var platform_browser_1 = require("@angular/platform-browser");
var auth_service_1 = require("./service/auth.service");
var ngx_cookie_service_1 = require("ngx-cookie-service");
var AppComponent = /** @class */ (function () {
    function AppComponent(router, titleService, activatedRoute, cookie, auth) {
        this.router = router;
        this.titleService = titleService;
        this.activatedRoute = activatedRoute;
        this.cookie = cookie;
        this.auth = auth;
        this.titleService.setTitle('');
        //this.myFuntion();
    }
    AppComponent.prototype.ngOnInit = function () {
        // this.router.events.subscribe((evt) => {
        //   if (!(evt instanceof NavigationEnd)) {
        //     return;
        //   }
        //   window.scrollTo(0, 0);
        // });
        var _this = this;
        this.router.events.pipe(operators_1.filter(function (event) { return event instanceof router_1.NavigationEnd; }), operators_1.map(function () { return _this.activatedRoute; }), operators_1.map(function (route) {
            while (route.firstChild)
                route = route.firstChild;
            return route;
        }), operators_1.filter(function (route) { return route.outlet === 'primary'; }), operators_1.mergeMap(function (route) { return route.data; })).
            subscribe(function (event) { return _this.titleService.setTitle(event['title']); });
    };
    AppComponent.prototype.myFuntion = function () {
        var _this = this;
        setInterval(function () {
            if (_this.cookie.get("Expire") == '') {
                _this.auth.logout();
            }
        }, 10000);
    };
    AppComponent = __decorate([
        core_1.Component({
            // tslint:disable-next-line
            selector: 'body',
            template: '<router-outlet></router-outlet><ngx-loading-bar></ngx-loading-bar>',
            //selector: 'app-root',
            //templateUrl: './app.component.html',
            styleUrls: ['./app.component.css']
        }),
        __metadata("design:paramtypes", [router_1.Router, platform_browser_1.Title,
            router_1.ActivatedRoute, ngx_cookie_service_1.CookieService, auth_service_1.AuthService])
    ], AppComponent);
    return AppComponent;
}());
exports.AppComponent = AppComponent;
//# sourceMappingURL=app.component.js.map
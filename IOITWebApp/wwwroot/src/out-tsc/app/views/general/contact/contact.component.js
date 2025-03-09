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
var modal_1 = require("ngx-bootstrap/modal");
var http_1 = require("@angular/common/http");
var platform_browser_1 = require("@angular/platform-browser");
var ContactComponent = /** @class */ (function () {
    function ContactComponent(http, sanitizer) {
        this.http = http;
        this.sanitizer = sanitizer;
        this.listBranch = [];
        this.listCompany = [];
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    ContactComponent.prototype.ngOnInit = function () {
        this.userid = parseInt(localStorage.getItem('userId'));
        var json = JSON.parse(localStorage.getItem('roles'));
        this.companyId = parseInt(localStorage.getItem('companyId'));
        if (json.length > 0) {
            for (var i = 0; i < json.length; i++) {
                this.role = json[i].RoleId;
            }
        }
        if (this.role == 3) {
            this.companyselectsearch = this.companyId;
            this.GetListBranchSearch();
        }
        else if (this.role == 1) {
            this.GetListCompany();
        }
        else if (this.role == 4) {
            this.GetQLCamera();
        }
    };
    //GET
    ContactComponent.prototype.GetQLCamera = function () {
        var _this = this;
        this.http.get('/api/Bank/GetByPage?userid=' + this.userid, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listBranch = res["data"];
                _this.QLCamera = _this.listBranch[0]["QLCamera"];
                if (_this.QLCamera != '' && _this.QLCamera != undefined) {
                    _this.urlSafe = _this.sanitizer.bypassSecurityTrustResourceUrl(_this.QLCamera);
                }
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    ContactComponent.prototype.GetListCompany = function () {
        var _this = this;
        this.http.get('/api/userrole/GetByCompany?page=1&query=1=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listCompany = res["data"];
                _this.companyselectsearch = _this.listCompany[0]["CompanyId"];
                _this.GetListBranchSearch();
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    ContactComponent.prototype.GetListBranchSearch = function () {
        var _this = this;
        this.http.get('/api/branch/GetByPage?page=1&query=CompanyId = ' + this.companyselectsearch + ' &order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listBranch = res["data"];
                _this.QLCamera = _this.listBranch[0]["QLCamera"];
                if (_this.QLCamera != '' && _this.QLCamera != undefined) {
                    _this.urlSafe = _this.sanitizer.bypassSecurityTrustResourceUrl(_this.QLCamera);
                }
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    ContactComponent.prototype.CompanyChanged = function () {
        this.GetListBranchSearch();
    };
    ContactComponent.prototype.QueryChanged = function () {
        if (this.QLCamera != '' && this.QLCamera != undefined) {
            this.urlSafe = this.sanitizer.bypassSecurityTrustResourceUrl(this.QLCamera);
        }
    };
    __decorate([
        core_1.ViewChild('AddModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], ContactComponent.prototype, "addModal", void 0);
    __decorate([
        core_1.ViewChild('EditModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], ContactComponent.prototype, "editModal", void 0);
    ContactComponent = __decorate([
        core_1.Component({
            selector: 'app-contact',
            templateUrl: './contact.component.html',
            styleUrls: ['./contact.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            platform_browser_1.DomSanitizer])
    ], ContactComponent);
    return ContactComponent;
}());
exports.ContactComponent = ContactComponent;
//# sourceMappingURL=contact.component.js.map
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
var http_1 = require("@angular/common/http");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var model_1 = require("../../../data/model");
var ngx_toastr_1 = require("ngx-toastr");
var const_1 = require("../../../data/const");
var ConfigGeneralComponent = /** @class */ (function () {
    function ConfigGeneralComponent(http, modalDialogService, viewRef, toastr) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.listConfig = [];
        this.CompanyId = parseInt(localStorage.getItem("companyId"));
        this.typeUpload = const_1.TypeUpload;
        this.updateItem = new model_1.Config();
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    ConfigGeneralComponent.prototype.ngOnInit = function () {
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        this.GetListConfig(this.CompanyId);
    };
    ConfigGeneralComponent.prototype.GetListConfig = function (CompanyId) {
        var _this = this;
        this.http.get('/api/Config/' + CompanyId, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listConfig = res["data"];
                _this.updateItem = Object.assign(_this.updateItem, _this.listConfig);
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Thông báo
    ConfigGeneralComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    ConfigGeneralComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    ConfigGeneralComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    ConfigGeneralComponent.prototype.Update = function () {
        var _this = this;
        if (this.updateItem.EmailHost == undefined) {
            this.toastWarning("Chưa nhập Email Host!");
            return;
        }
        else if (this.updateItem.EmailHost.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập Email host!");
            return;
        }
        else if (this.updateItem.EmailSender == undefined) {
            this.toastWarning("Chưa nhập Email Sender!");
            return;
        }
        else if (this.updateItem.EmailSender.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập Email Sender!");
            return;
        }
        else if (this.updateItem.EmailEnableSsl == undefined) {
            this.toastWarning("Chưa nhập Email Enable SSl");
            return;
        }
        else if (this.updateItem.EmailUserName == undefined) {
            this.toastWarning("Chưa nhập Email User Name!");
            return;
        }
        else if (this.updateItem.EmailUserName.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập Email UserName!");
            return;
        }
        else if (this.updateItem.EmailPasswordHash == undefined) {
            this.toastWarning("Chưa nhập Email Password Hash");
            return;
        }
        else if (this.updateItem.EmailPasswordHash.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập EmailPasswordHash!");
            return;
        }
        else if (this.updateItem.EmailPort == undefined) {
            this.toastWarning("Chưa nhập Email Port!");
            return;
        }
        this.http.put('/api/Config/' + this.updateItem.ConfigId, this.updateItem, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.toastSuccess("Cập nhật thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    ConfigGeneralComponent = __decorate([
        core_1.Component({
            selector: 'app-config-general',
            templateUrl: './config-general.component.html',
            styleUrls: ['./config-general.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService])
    ], ConfigGeneralComponent);
    return ConfigGeneralComponent;
}());
exports.ConfigGeneralComponent = ConfigGeneralComponent;
//# sourceMappingURL=config-general.component.js.map
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
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var ngx_toastr_1 = require("ngx-toastr");
var const_1 = require("../../../data/const");
var model_1 = require("../../../data/model");
var dt_1 = require("../../../data/dt");
var WebsiteComponent = /** @class */ (function () {
    function WebsiteComponent(http, modalDialogService, viewRef, toastr) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.listLanguage = [];
        this.Image = [];
        this.listWebsite = [];
        this.listWebsiteParent = [];
        this.domainImage = const_1.domainImage;
        this.Item = new model_1.Website();
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "1=1";
        this.paging.order_by = "WebsiteId Desc";
        this.paging.item_count = 0;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    WebsiteComponent.prototype.ngOnInit = function () {
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        this.GetListWebsite();
    };
    //GET
    WebsiteComponent.prototype.GetListWebsite = function () {
        var _this = this;
        this.http.get('/api/website/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listWebsite = res["data"];
                _this.paging.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    WebsiteComponent.prototype.GetListWebsiteParent = function (Id) {
        var _this = this;
        console.log(Id);
        this.http.get('/api/website/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listWebsiteParent = res["data"];
                _this.listWebsiteParent.forEach(function (item) {
                    item.Space = "";
                    for (var i = 0; i < _this.listWebsiteParent.length; i++) {
                        item.Space += "&nbsp;";
                        if (item.WebsiteId == Id) {
                            item.disabled = true;
                        }
                    }
                });
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    // Get danh sách ngôn ngữ
    WebsiteComponent.prototype.GetListLanguage = function () {
        var _this = this;
        this.http.get('/api/Language/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listLanguage = res["data"];
                if (_this.listLanguage.length == 1 && (_this.Item.WebsiteId == undefined || (_this.Item.WebsiteId != undefined && _this.Item.LanguageId == undefined))) {
                    _this.Item.LanguageId = _this.listLanguage[0].LanguageId;
                }
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    WebsiteComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListWebsite();
    };
    //Thông báo
    WebsiteComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    WebsiteComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    WebsiteComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    WebsiteComponent.prototype.QueryChanged = function () {
        var query = '';
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            if (query != '') {
                query += ' and Name.Contains("' + this.q.txtSearch + '")';
            }
            else {
                query += 'Name.Contains("' + this.q.txtSearch + '")';
            }
        }
        if (query == '')
            this.paging.query = '1=1';
        else
            this.paging.query = query;
        this.GetListWebsite();
    };
    //Mở modal
    WebsiteComponent.prototype.OpenWebsiteModal = function (item) {
        this.Item = new model_1.Website();
        this.fileHeader.nativeElement.value = "";
        this.fileFooter.nativeElement.value = "";
        this.fileBanner.nativeElement.value = "";
        this.messageBanner = undefined;
        this.messageFooter = undefined;
        this.messageHeader = undefined;
        this.progressHeader = undefined;
        this.progressFooter = undefined;
        this.progressBanner = undefined;
        if (item == undefined) {
            this.GetListWebsiteParent(undefined);
        }
        else {
            this.Item = JSON.parse(JSON.stringify(item));
            this.GetListWebsiteParent(this.Item.WebsiteId);
        }
        this.GetListLanguage();
        this.WebsiteModal.show();
    };
    //Thêm mới
    WebsiteComponent.prototype.SaveWebsite = function () {
        var _this = this;
        if (this.Item.Name == undefined || this.Item.Name == '') {
            this.toastWarning("Chưa nhập Tên!");
            return;
        }
        else if (this.Item.Name.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên!");
            return;
        }
        else if (this.Item.Url == undefined || this.Item.Url == '') {
            this.toastWarning("Chưa nhập đường dẫn!");
            return;
        }
        else if (this.Item.Url.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập đường dẫn!");
            return;
        }
        else if (this.Item.LanguageId == undefined) {
            this.toastWarning("Chưa chọn ngôn ngữ!");
            return;
        }
        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
        if (this.Item.WebsiteId == undefined) {
            this.http.post('/api/Website', this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListWebsite();
                    _this.WebsiteModal.hide();
                    _this.toastSuccess("Thêm thành công!");
                }
                else {
                    _this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
            });
        }
        else {
            this.http.put('/api/Website/' + this.Item.WebsiteId, this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListWebsite();
                    _this.WebsiteModal.hide();
                    _this.toastSuccess("Cập nhật thành công!");
                }
                else {
                    _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
    };
    //Popup xác nhận xóa
    WebsiteComponent.prototype.ShowConfirmDelete = function (Id) {
        var _this = this;
        this.modalDialogService.openDialog(this.viewRef, {
            title: 'Xác nhận',
            childComponent: ngx_modal_dialog_1.SimpleModalComponent,
            data: {
                text: "Bạn có chắc chắn muốn xóa bản ghi này?"
            },
            actionButtons: [
                {
                    text: 'Đồng ý',
                    buttonClass: 'btn btn-success',
                    onAction: function () {
                        _this.Delete(Id);
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-default',
                }
            ],
        });
    };
    WebsiteComponent.prototype.Delete = function (Id) {
        var _this = this;
        this.http.delete('/api/Website/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListWebsite();
                _this.viewRef.clear();
                _this.toastSuccess("Xóa thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    WebsiteComponent.prototype.upload = function (files, key) {
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
                switch (key) {
                    case 1:
                        _this.progressHeader = Math.round(100 * event.loaded / event.total);
                        break;
                    case 2:
                        _this.progressFooter = Math.round(100 * event.loaded / event.total);
                        break;
                    case 3:
                        _this.progressBanner = Math.round(100 * event.loaded / event.total);
                        break;
                    default:
                        break;
                }
            }
            else if (event.type === http_1.HttpEventType.Response) {
                switch (key) {
                    case 1:
                        _this.messageHeader = event.body["data"].toString();
                        _this.Item.LogoHeader = _this.messageHeader;
                        break;
                    case 1:
                        _this.messageFooter = event.body["data"].toString();
                        _this.Item.LogoFooter = _this.messageFooter;
                        break;
                    case 1:
                        _this.messageBanner = event.body["data"].toString();
                        _this.Item.Banner = _this.messageBanner;
                        break;
                    default:
                        break;
                }
            }
        });
    };
    WebsiteComponent.prototype.findParent = function (item) {
        if (item == undefined) {
            return "";
        }
        else {
            return item.Name;
        }
    };
    WebsiteComponent.prototype.RemoveImage = function (key) {
        switch (key) {
            case 1:
                this.fileHeader.nativeElement.value = "";
                this.Item.LogoHeader = undefined;
                this.messageHeader = undefined;
                this.progressHeader = undefined;
                break;
            case 2:
                this.fileFooter.nativeElement.value = "";
                this.Item.LogoFooter = undefined;
                this.messageFooter = undefined;
                this.progressFooter = undefined;
                break;
            case 3:
                this.fileBanner.nativeElement.value = "";
                this.Item.Banner = undefined;
                this.messageBanner = undefined;
                this.progressBanner = undefined;
                break;
            default:
                break;
        }
    };
    WebsiteComponent.prototype.SortTable = function (str) {
        var First = "";
        var Last = "";
        if (this.paging.order_by != "") {
            First = this.paging.order_by.split(" ")[0];
            Last = this.paging.order_by.split(" ")[1];
        }
        if (First != str) {
            this.paging.order_by = str + " Desc";
        }
        else {
            Last = Last == "Asc" ? "Desc" : "Asc";
            this.paging.order_by = str + " " + Last;
        }
        this.GetListWebsite();
    };
    WebsiteComponent.prototype.GetClassSortTable = function (str) {
        if (this.paging.order_by != (str + " Desc") && this.paging.order_by != (str + " Asc")) {
            return "sorting";
        }
        else {
            if (this.paging.order_by == (str + " Desc"))
                return "sorting_desc";
            else
                return "sorting_asc";
        }
    };
    __decorate([
        core_1.ViewChild('WebsiteModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], WebsiteComponent.prototype, "WebsiteModal", void 0);
    __decorate([
        core_1.ViewChild('fileHeader'),
        __metadata("design:type", core_1.ElementRef)
    ], WebsiteComponent.prototype, "fileHeader", void 0);
    __decorate([
        core_1.ViewChild('fileFooter'),
        __metadata("design:type", core_1.ElementRef)
    ], WebsiteComponent.prototype, "fileFooter", void 0);
    __decorate([
        core_1.ViewChild('fileBanner'),
        __metadata("design:type", core_1.ElementRef)
    ], WebsiteComponent.prototype, "fileBanner", void 0);
    WebsiteComponent = __decorate([
        core_1.Component({
            selector: 'app-website',
            templateUrl: './website.component.html',
            styleUrls: ['./website.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService])
    ], WebsiteComponent);
    return WebsiteComponent;
}());
exports.WebsiteComponent = WebsiteComponent;
//# sourceMappingURL=website.component.js.map
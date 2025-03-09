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
var const_1 = require("../../data/const");
var model_1 = require("../../data/model");
var ngx_toastr_1 = require("ngx-toastr");
var common_1 = require("@angular/common");
var common_service_1 = require("../../service/common.service");
var dt_1 = require("../../data/dt");
var ng_pick_datetime_1 = require("ng-pick-datetime");
var ng_pick_datetime_moment_1 = require("ng-pick-datetime-moment");
exports.MY_CUSTOM_FORMATS = {
    parseInput: 'DD/MM/YYYY HH:mm',
    fullPickerInput: 'DD/MM/YYYY HH:mm',
    datePickerInput: 'DD/MM/YYYY',
    timePickerInput: ' HH:mm',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY'
};
var ServiceComponent = /** @class */ (function () {
    function ServiceComponent(http, modalDialogService, viewRef, toastr, datePipe, common) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.datePipe = datePipe;
        this.common = common;
        this.listNews = [];
        this.domainImage = const_1.domainImage;
        this.Item = new model_1.News();
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "IsService == true";
        this.paging.order_by = "NewsId Desc";
        this.paging.item_count = 0;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    ServiceComponent.prototype.ngOnInit = function () {
        this.GetListNews();
    };
    //Get danh sách danh bài viết
    ServiceComponent.prototype.GetListNews = function () {
        var _this = this;
        this.http.get('/api/news/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listNews = res["data"];
                _this.listNews.forEach(function (item) {
                    item.IsShow = item.Status == 1 ? true : false;
                });
                _this.paging.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    ServiceComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListNews();
    };
    //Thông báo
    ServiceComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    ServiceComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    ServiceComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    ServiceComponent.prototype.QueryChanged = function () {
        var query = 'IsService == true';
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            if (query != '') {
                query += ' and Title.Contains("' + this.q.txtSearch + '")';
            }
            else {
                query += 'Title.Contains("' + this.q.txtSearch + '")';
            }
        }
        if (query == '')
            this.paging.query = '1=1';
        else
            this.paging.query = query;
        this.GetListNews();
    };
    //Mở modal thêm mới
    ServiceComponent.prototype.OpenNewsModal = function (item) {
        this.Item = new model_1.News();
        this.file.nativeElement.value = "";
        this.message = undefined;
        this.progress = undefined;
        if (item != undefined) {
            this.Item = JSON.parse(JSON.stringify(item));
        }
        this.NewsModal.show();
    };
    //Thêm mới danh mục trang
    ServiceComponent.prototype.SaveNews = function () {
        var _this = this;
        if (this.Item.Title == undefined || this.Item.Title == '') {
            this.toastWarning("Chưa nhập Tiêu đề!");
            return;
        }
        else if (this.Item.Title.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tiêu đề!");
            return;
        }
        else if (this.Item.Url == undefined || this.Item.Url == '') {
            this.toastWarning("Chưa nhập Đường dẫn!");
            return;
        }
        else if (this.Item.Url.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập đường dẫn!");
            return;
        }
        else if (this.Item.Contents == undefined || this.Item.Contents == '') {
            this.toastWarning("Chưa nhập Nội dung!");
            return;
        }
        else if (this.Item.Contents.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập nội dung!");
            return;
        }
        this.Item.Status = 1;
        this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        if (this.Item.NewsId == undefined) {
            this.http.post('/api/news', this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListNews();
                    _this.NewsModal.hide();
                    _this.toastSuccess("Thêm mới thành công!");
                }
                else {
                    _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
        else {
            this.http.put('/api/news/' + this.Item.NewsId, this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListNews();
                    _this.NewsModal.hide();
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
    ServiceComponent.prototype.ChangeTitle = function (key) {
        switch (key) {
            case 1:
                this.Item.MetaTitle = this.Item.Title;
                this.Item.MetaKeyword = this.Item.Title;
                this.Item.Url = this.common.ConvertUrl(this.Item.Title);
                break;
            case 2:
                this.Item.MetaDescription = this.Item.Description;
                break;
            default:
                break;
        }
    };
    //Popup xác nhận xóa
    ServiceComponent.prototype.ShowConfirmDelete = function (Id) {
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
                        _this.DeleteNews(Id);
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-default',
                }
            ],
        });
    };
    ServiceComponent.prototype.DeleteNews = function (Id) {
        var _this = this;
        this.http.delete('/api/news/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListNews();
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
    ServiceComponent.prototype.upload = function (files) {
        var _this = this;
        if (files.length === 0)
            return;
        var formData = new FormData();
        for (var _i = 0, files_1 = files; _i < files_1.length; _i++) {
            var file = files_1[_i];
            formData.append(file.name, file);
        }
        var uploadReq = new http_1.HttpRequest('POST', 'api/upload/uploadImage/1', formData, {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            }),
            reportProgress: true,
        });
        this.http.request(uploadReq).subscribe(function (event) {
            if (event.type === http_1.HttpEventType.UploadProgress)
                _this.progress = Math.round(100 * event.loaded / event.total);
            else if (event.type === http_1.HttpEventType.Response) {
                _this.message = event.body["data"].toString();
                _this.Item.Image = _this.message;
            }
        });
    };
    ServiceComponent.prototype.RemoveImage = function () {
        this.Item.Image = undefined;
        this.file.nativeElement.value = "";
        this.message = undefined;
        this.progress = undefined;
    };
    ServiceComponent.prototype.SortTable = function (str) {
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
        this.GetListNews();
    };
    ServiceComponent.prototype.GetClassSortTable = function (str) {
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
        core_1.ViewChild('NewsModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], ServiceComponent.prototype, "NewsModal", void 0);
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], ServiceComponent.prototype, "file", void 0);
    ServiceComponent = __decorate([
        core_1.Component({
            selector: 'app-service',
            templateUrl: './service.component.html',
            styleUrls: ['./service.component.scss'],
            providers: [
                { provide: ng_pick_datetime_1.DateTimeAdapter, useClass: ng_pick_datetime_moment_1.MomentDateTimeAdapter, deps: [ng_pick_datetime_1.OWL_DATE_TIME_LOCALE] },
                { provide: ng_pick_datetime_1.OWL_DATE_TIME_FORMATS, useValue: exports.MY_CUSTOM_FORMATS }
            ]
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService,
            common_1.DatePipe,
            common_service_1.CommonService])
    ], ServiceComponent);
    return ServiceComponent;
}());
exports.ServiceComponent = ServiceComponent;
//# sourceMappingURL=service.component.js.map
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
var modal_1 = require("ngx-bootstrap/modal");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var ngx_toastr_1 = require("ngx-toastr");
var model_1 = require("../../../data/model");
var dt_1 = require("../../../data/dt");
var const_1 = require("../../../data/const");
var CompanyComponent = /** @class */ (function () {
    function CompanyComponent(http, modalDialogService, viewRef, toastr) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.listCompany = [];
        this.domainImage = const_1.domainImage;
        this.Item = new model_1.Company();
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "1=1";
        this.paging.order_by = "CompanyId Desc";
        this.paging.item_count = 0;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    CompanyComponent.prototype.ngOnInit = function () {
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        this.GetListCompany();
    };
    //GET
    CompanyComponent.prototype.GetListCompany = function () {
        var _this = this;
        this.http.get('/api/company/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listCompany = res["data"];
                _this.paging.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    CompanyComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListCompany();
    };
    //Thông báo
    CompanyComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    CompanyComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    CompanyComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    CompanyComponent.prototype.QueryChanged = function () {
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
        this.GetListCompany();
    };
    //Mở modal
    CompanyComponent.prototype.OpenCompanyModal = function (item) {
        this.Item = new model_1.Company();
        this.file.nativeElement.value = "";
        if (item != undefined) {
            this.Item = JSON.parse(JSON.stringify(item));
        }
        this.CompanyModal.show();
    };
    //Thêm mới
    CompanyComponent.prototype.SaveCompany = function () {
        var _this = this;
        if (this.Item.Code == undefined || this.Item.Code == '') {
            this.toastWarning("Chưa nhập Mã!");
            return;
        }
        else if (this.Item.Code.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập mã!");
            return;
        }
        else if (this.Item.Name == undefined || this.Item.Name == '') {
            this.toastWarning("Chưa nhập Tên!");
            return;
        }
        else if (this.Item.Name.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên!");
            return;
        }
        else if (this.Item.Email == undefined || this.Item.Email == '') {
            this.toastWarning("Chưa nhập Email!");
            return;
        }
        else if (this.Item.Email.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập Email!");
            return;
        }
        else if (this.Item.Phone == undefined || this.Item.Phone == '') {
            this.toastWarning("Chưa nhập số điện thoại!");
            return;
        }
        else if (this.Item.Phone.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập số điện thoại!");
            return;
        }
        else if (this.Item.Code.length < 2) {
            this.toastWarning("Mã ít nhất 2 ký tự!");
            return;
        }
        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        if (this.Item.CompanyId == undefined) {
            this.http.post('/api/Company', this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListCompany();
                    _this.CompanyModal.hide();
                    _this.toastSuccess("Thêm thành công!");
                }
                else if (res["meta"]["error_code"] == 212) {
                    _this.toastWarning("Mã đã tồn tại!");
                }
                else {
                    _this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
            });
        }
        else {
            this.http.put('/api/Company/' + this.Item.CompanyId, this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListCompany();
                    _this.CompanyModal.hide();
                    _this.toastSuccess("Cập nhật thành công!");
                }
                else if (res["meta"]["error_code"] == 212) {
                    _this.toastWarning("Mã đã tồn tại!");
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
    CompanyComponent.prototype.ShowConfirmDelete = function (Id) {
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
    CompanyComponent.prototype.upload = function (files) {
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
            if (event.type === http_1.HttpEventType.UploadProgress)
                _this.progress = Math.round(100 * event.loaded / event.total);
            else if (event.type === http_1.HttpEventType.Response) {
                _this.message = event.body["data"].toString();
                _this.Item.Logo = _this.message;
            }
        });
    };
    CompanyComponent.prototype.Delete = function (Id) {
        var _this = this;
        this.http.delete('/api/Company/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListCompany();
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
    CompanyComponent.prototype.SortTable = function (str) {
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
        this.GetListCompany();
    };
    CompanyComponent.prototype.GetClassSortTable = function (str) {
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
        core_1.ViewChild('CompanyModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], CompanyComponent.prototype, "CompanyModal", void 0);
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], CompanyComponent.prototype, "file", void 0);
    CompanyComponent = __decorate([
        core_1.Component({
            selector: 'app-company',
            templateUrl: './company.component.html',
            styleUrls: ['./company.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService])
    ], CompanyComponent);
    return CompanyComponent;
}());
exports.CompanyComponent = CompanyComponent;
//# sourceMappingURL=company.component.js.map
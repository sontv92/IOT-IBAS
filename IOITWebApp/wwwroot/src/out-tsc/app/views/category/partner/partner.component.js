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
var model_1 = require("../../../data/model");
var common_service_1 = require("../../../service/common.service");
var ngx_toastr_1 = require("ngx-toastr");
var const_1 = require("../../../data/const");
var dt_1 = require("../../../data/dt");
var PartnerComponent = /** @class */ (function () {
    function PartnerComponent(http, modalDialogService, viewRef, toastr, common) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.common = common;
        this.listManufacturer = [];
        this.listCompany = [];
        this.domainImage = const_1.domainImage;
        this.Item = new model_1.Manufacturer();
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "TypeOriginId=3";
        this.paging.order_by = "ManufacturerId Desc";
        this.paging.item_count = 0;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    PartnerComponent.prototype.ngOnInit = function () {
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        this.GetListManufacturer();
    };
    // get ds nhà sản xuất
    PartnerComponent.prototype.GetListManufacturer = function () {
        var _this = this;
        this.http.get('/api/manufacturer/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listManufacturer = res["data"];
                _this.listManufacturer.forEach(function (item) {
                    item.IsShow = item.Status == 1 ? true : false;
                });
                _this.paging.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    // get ds công ty
    PartnerComponent.prototype.GetListCompany = function () {
        var _this = this;
        this.http.get('/api/company/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listCompany = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    PartnerComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListManufacturer();
    };
    //Thông báo
    PartnerComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    PartnerComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    PartnerComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    PartnerComponent.prototype.QueryChanged = function () {
        var query = 'TypeOriginId=3';
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
        this.GetListManufacturer();
    };
    //Mở modal
    PartnerComponent.prototype.OpenModalManuFacturer = function (item) {
        this.Item = new model_1.Manufacturer();
        this.file.nativeElement.value = "";
        this.fileOwner.nativeElement.value = "";
        this.progress = undefined;
        this.progressOwner = undefined;
        if (item != undefined) {
            this.Item = Object.assign(this.Item, item);
        }
        this.ModalManuFacture.show();
    };
    PartnerComponent.prototype.ChangeTitle = function (key) {
        switch (key) {
            case 1:
                this.Item.MetaTitle = this.Item.Name;
                this.Item.MetaKeywords = this.Item.Name;
                this.Item.Url = this.common.ConvertUrl(this.Item.Name);
                break;
            case 2:
                this.Item.MetaDescription = this.Item.Description;
                break;
            default:
                break;
        }
    };
    // cập nhật 
    PartnerComponent.prototype.SaveManuFacture = function () {
        var _this = this;
        if (this.Item.Code == undefined || this.Item.Code == '') {
            this.toastWarning("Chưa nhập Mã trại cá!");
            return;
        }
        else if (this.Item.Code.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập mã trại cá !");
            return;
        }
        else if (this.Item.Name == undefined || this.Item.Name == '') {
            this.toastWarning("Chưa nhập Tên trại cá!");
            return;
        }
        else if (this.Item.Name.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên trại cá !");
            return;
        }
        else if (this.Item.Owner == undefined || this.Item.Owner == '') {
            this.toastWarning("Chưa nhập Tên chủ trại cá!");
            return;
        }
        else if (this.Item.NickName == undefined || this.Item.NickName == '') {
            this.toastWarning("Chưa nhập Biệt danh!");
            return;
        }
        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
        this.Item.TypeOriginId = 3;
        if (this.Item.ManufacturerId) {
            this.http.put('/api/Manufacturer/' + this.Item.ManufacturerId, this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListManufacturer();
                    _this.ModalManuFacture.hide();
                    _this.toastSuccess("Cập nhật thành công!");
                }
                else {
                    _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
        else {
            this.http.post('/api/Manufacturer', this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListManufacturer();
                    _this.ModalManuFacture.hide();
                    _this.toastSuccess("Thêm thành công!");
                }
                else {
                    _this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
            });
        }
    };
    //Popup xác nhận xóa
    PartnerComponent.prototype.ShowConfirmDelete = function (Id) {
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
                        console.log('OnAction');
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
    PartnerComponent.prototype.Delete = function (Id) {
        var _this = this;
        this.http.delete('/api/Manufacturer/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListManufacturer();
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
    //
    PartnerComponent.prototype.upload = function (files, cs) {
        var _this = this;
        if (files.length === 0)
            return;
        var formData = new FormData();
        for (var _i = 0, files_1 = files; _i < files_1.length; _i++) {
            var file = files_1[_i];
            formData.append(file.name, file);
        }
        var uploadReq = new http_1.HttpRequest('POST', 'api/upload/uploadImage/5', formData, {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            }),
            reportProgress: true,
        });
        this.http.request(uploadReq).subscribe(function (event) {
            if (event.type === http_1.HttpEventType.UploadProgress) {
                switch (cs) {
                    case 1:
                        _this.progress = Math.round(100 * event.loaded / event.total);
                        break;
                    case 2:
                        _this.progressOwner = Math.round(100 * event.loaded / event.total);
                        break;
                    default:
                        break;
                }
            }
            else if (event.type === http_1.HttpEventType.Response) {
                switch (cs) {
                    case 1:
                        _this.Item.Logo = event.body["data"].toString();
                        break;
                    case 2:
                        _this.Item.AvatarOwner = event.body["data"].toString();
                        break;
                    default:
                        break;
                }
            }
        });
    };
    PartnerComponent.prototype.RemoveImage = function (cs) {
        switch (cs) {
            case 1:
                this.Item.Logo = undefined;
                this.file.nativeElement.value = "";
                this.progress = undefined;
                break;
            case 2:
                this.Item.AvatarOwner = undefined;
                this.fileOwner.nativeElement.value = "";
                this.progressOwner = undefined;
                break;
            default:
                break;
        }
    };
    PartnerComponent.prototype.ShowHide = function (id, i) {
        var _this = this;
        var stt = this.listManufacturer[i].IsShow ? 1 : 10;
        this.http.put('/api/Manufacturer/ShowHide/' + id + "/" + stt, undefined, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.toastSuccess("Thay đổi trạng thái thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                _this.listManufacturer[i].IsShow = !_this.listManufacturer[i].IsShow;
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            _this.listManufacturer[i].IsShow = !_this.listManufacturer[i].IsShow;
        });
    };
    PartnerComponent.prototype.SortTable = function (str) {
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
        this.GetListManufacturer();
    };
    PartnerComponent.prototype.GetClassSortTable = function (str) {
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
        core_1.ViewChild('ModalManuFacture'),
        __metadata("design:type", modal_1.ModalDirective)
    ], PartnerComponent.prototype, "ModalManuFacture", void 0);
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], PartnerComponent.prototype, "file", void 0);
    __decorate([
        core_1.ViewChild('fileOwner'),
        __metadata("design:type", core_1.ElementRef)
    ], PartnerComponent.prototype, "fileOwner", void 0);
    PartnerComponent = __decorate([
        core_1.Component({
            selector: 'app-partner',
            templateUrl: './partner.component.html',
            styleUrls: ['./partner.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService,
            common_service_1.CommonService])
    ], PartnerComponent);
    return PartnerComponent;
}());
exports.PartnerComponent = PartnerComponent;
//# sourceMappingURL=partner.component.js.map
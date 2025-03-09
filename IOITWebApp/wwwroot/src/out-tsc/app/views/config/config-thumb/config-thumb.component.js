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
var ngx_toastr_1 = require("ngx-toastr");
var const_1 = require("../../../data/const");
var dt_1 = require("../../../data/dt");
var ConfigThumbComponent = /** @class */ (function () {
    function ConfigThumbComponent(http, modalDialogService, viewRef, toastr) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.listConfigThumb = [];
        this.typeUpload = const_1.TypeUpload;
        this.Item = new model_1.ConfigThumb();
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "1=1";
        this.paging.order_by = "ConfigThumbId Desc";
        this.paging.item_count = 0;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    ConfigThumbComponent.prototype.ngOnInit = function () {
        //this.ckeConfig = {
        //  allowedContent: false,
        //  extraPlugins: 'divarea',
        //  forcePasteAsPlainText: true
        //};
        this.GetListConfigThumb();
    };
    //GET danh sách ảnh thumb
    ConfigThumbComponent.prototype.GetListConfigThumb = function () {
        var _this = this;
        this.http.get('/api/ConfigThumb/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listConfigThumb = res["data"];
                _this.paging.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    ConfigThumbComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListConfigThumb();
    };
    //Thông báo
    ConfigThumbComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    ConfigThumbComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    ConfigThumbComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    ConfigThumbComponent.prototype.QueryChanged = function () {
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
        this.GetListConfigThumb();
    };
    //Mở modal
    ConfigThumbComponent.prototype.OpenConfigThumbModal = function (item) {
        this.Item = new model_1.ConfigThumb();
        if (item != undefined) {
            this.Item = JSON.parse(JSON.stringify(item));
        }
        this.ConfigThumbModal.show();
    };
    //Thêm mới
    ConfigThumbComponent.prototype.SaveConfigThumb = function () {
        var _this = this;
        if (this.Item.Width == undefined) {
            this.toastWarning("Chưa nhập chiều rộng!");
            return;
        }
        else if (this.Item.Height == undefined) {
            this.toastWarning("Chưa nhập chiều cao!");
            return;
        }
        else if (this.Item.Type == undefined) {
            this.toastWarning("Chưa nhập loại thumb");
            return;
        }
        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
        this.Item.WebsiteId = parseInt(localStorage.getItem("websiteId"));
        if (this.Item.ConfigThumbId == undefined) {
            this.http.post('/api/ConfigThumb', this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListConfigThumb();
                    _this.ConfigThumbModal.hide();
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
            this.http.put('/api/ConfigThumb/' + this.Item.ConfigThumbId, this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListConfigThumb();
                    _this.ConfigThumbModal.hide();
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
    ConfigThumbComponent.prototype.ShowConfirmDelete = function (Id) {
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
    ConfigThumbComponent.prototype.Delete = function (Id) {
        var _this = this;
        this.http.delete('/api/ConfigThumb/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListConfigThumb();
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
    //auto gen thumbs
    ConfigThumbComponent.prototype.ShowConfirmGenThumb = function (type, width) {
        var _this = this;
        this.modalDialogService.openDialog(this.viewRef, {
            title: 'Xác nhận',
            childComponent: ngx_modal_dialog_1.SimpleModalComponent,
            data: {
                text: "Bạn có chắc chắn muốn sinh thumb có kích thước " + width + "?"
            },
            actionButtons: [
                {
                    text: 'Đồng ý',
                    buttonClass: 'btn btn-success',
                    onAction: function () {
                        console.log('OnAction');
                        _this.AutoGenThumbs(type, width);
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-default',
                }
            ],
        });
    };
    ConfigThumbComponent.prototype.AutoGenThumbs = function (type, width) {
        var _this = this;
        this.http.post('/api/upload/autoGenThumbs/' + type + '/' + width, null, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.viewRef.clear();
                _this.toastSuccess("Sinh thumb thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    ConfigThumbComponent.prototype.SortTable = function (str) {
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
        this.GetListConfigThumb();
    };
    ConfigThumbComponent.prototype.GetClassSortTable = function (str) {
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
    ConfigThumbComponent.prototype.FindTypeThumb = function (id) {
        for (var i = 0; i < this.typeUpload.length; i++) {
            if (this.typeUpload[i].Id == id)
                return this.typeUpload[i].Name;
        }
    };
    __decorate([
        core_1.ViewChild('ConfigThumbModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], ConfigThumbComponent.prototype, "ConfigThumbModal", void 0);
    ConfigThumbComponent = __decorate([
        core_1.Component({
            selector: 'app-config-thumb',
            templateUrl: './config-thumb.component.html',
            styleUrls: ['./config-thumb.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService])
    ], ConfigThumbComponent);
    return ConfigThumbComponent;
}());
exports.ConfigThumbComponent = ConfigThumbComponent;
//# sourceMappingURL=config-thumb.component.js.map
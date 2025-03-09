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
var model_1 = require("../../../data/model");
var dt_1 = require("../../../data/dt");
var AttribuiteComponent = /** @class */ (function () {
    function AttribuiteComponent(http, modalDialogService, viewRef, toastr) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.listAttribuite = [];
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "1=1";
        this.paging.order_by = "AttribuiteId Desc";
        this.paging.item_count = 0;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        this.Item = new model_1.Attribuite();
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    AttribuiteComponent.prototype.ngOnInit = function () {
        this.GetListAttribuite();
    };
    //GET
    AttribuiteComponent.prototype.GetListAttribuite = function () {
        var _this = this;
        this.http.get('/api/attribuite/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listAttribuite = res["data"];
                _this.paging.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    AttribuiteComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListAttribuite();
    };
    //Thông báo
    AttribuiteComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    AttribuiteComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    AttribuiteComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    AttribuiteComponent.prototype.QueryChanged = function () {
        var query = '';
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            if (query != '') {
                query += ' and (Name.Contains("' + this.q.txtSearch + '") Or Name.Contains("' + this.q.txtSearch + '"))';
            }
            else {
                query += '(Name.Contains("' + this.q.txtSearch + '") or Name.Contains("' + this.q.txtSearch + '"))';
            }
        }
        if (query == '')
            this.paging.query = '1=1';
        else
            this.paging.query = query;
        this.GetListAttribuite();
    };
    AttribuiteComponent.prototype.SortTable = function (str) {
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
        this.GetListAttribuite();
    };
    AttribuiteComponent.prototype.GetClassSortTable = function (str) {
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
    //Mở modal thêm mới
    AttribuiteComponent.prototype.OpenAttribuiteModal = function (item) {
        this.Item = new model_1.Attribuite();
        if (item != undefined) {
            this.Item = JSON.parse(JSON.stringify(item));
        }
        this.AttribuiteModal.show();
    };
    //Thêm mới danh mục trang
    AttribuiteComponent.prototype.SaveAttribuite = function () {
        var _this = this;
        if (this.Item.Name == undefined || this.Item.Name == '') {
            this.toastWarning("Chưa nhập Tên thuộc tính!");
            return;
        }
        else if (this.Item.Name.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập Tên thuộc tính!");
            return;
        }
        else if (this.Item.Location == undefined) {
            this.toastWarning("Chưa nhập Thứ tự hiển thị!");
            return;
        }
        if (this.Item.AttribuiteId == undefined) {
            this.http.post('/api/attribuite', this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListAttribuite();
                    _this.AttribuiteModal.hide();
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
            this.http.put('/api/attribuite/' + this.Item.AttribuiteId, this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListAttribuite();
                    _this.AttribuiteModal.hide();
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
    AttribuiteComponent.prototype.ShowConfirmDelete = function (Id) {
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
                        _this.http.delete('/api/attribuite/' + Id, _this.httpOptions).subscribe(function (res) {
                            if (res["meta"]["error_code"] == 200) {
                                _this.GetListAttribuite();
                                _this.viewRef.clear();
                                _this.toastSuccess("Xóa thành công!");
                            }
                            else {
                                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                            }
                        }, function (err) {
                            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                        });
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-default',
                }
            ],
        });
    };
    __decorate([
        core_1.ViewChild('AttribuiteModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], AttribuiteComponent.prototype, "AttribuiteModal", void 0);
    AttribuiteComponent = __decorate([
        core_1.Component({
            selector: 'app-attribuite',
            templateUrl: './attribuite.component.html',
            styleUrls: ['./attribuite.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService])
    ], AttribuiteComponent);
    return AttribuiteComponent;
}());
exports.AttribuiteComponent = AttribuiteComponent;
//# sourceMappingURL=attribuite.component.js.map
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
var model_2 = require("../../../data/model");
var dt_1 = require("../../../data/dt");
var TypeAttributeComponent = /** @class */ (function () {
    function TypeAttributeComponent(http, modalDialogService, viewRef, toastr) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.listTypeAttribute = [];
        this.listTypeAttributeItem = [];
        this.Item = new model_1.TypeAttribute();
        this.newType = new model_2.TypeAttributeItem();
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "1=1";
        this.paging.order_by = "TypeAttributeId Desc";
        this.paging.item_count = 0;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    TypeAttributeComponent.prototype.ngOnInit = function () {
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        this.GetListTypeAttribute();
    };
    //Get danh sách loại hình
    TypeAttributeComponent.prototype.GetListTypeAttribute = function () {
        var _this = this;
        this.http.get('/api/typeattribute/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listTypeAttribute = res["data"];
                _this.paging.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    TypeAttributeComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListTypeAttribute();
    };
    //Thông báo
    TypeAttributeComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    TypeAttributeComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    TypeAttributeComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    TypeAttributeComponent.prototype.QueryChanged = function () {
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
        this.GetListTypeAttribute();
    };
    //Mở modal thêm mới loại hình
    TypeAttributeComponent.prototype.OpenTypeAttributeModal = function (item) {
        this.Item = new model_1.TypeAttribute();
        this.newType = new model_2.TypeAttributeItem();
        this.Item.listAttributeItem = [];
        if (item != undefined) {
            this.Item = JSON.parse(JSON.stringify(item));
        }
        this.TypeAttributeModal.show();
    };
    //Thêm mới loại hình
    TypeAttributeComponent.prototype.SaveTypeAttribute = function () {
        var _this = this;
        if (this.Item.Name == undefined || this.Item.Name == '') {
            this.toastWarning("Chưa nhập tên loại hình!");
            return;
        }
        else if (this.Item.Name.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên loại hình!");
            return;
        }
        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        if (this.Item.TypeAttributeId == undefined) {
            this.http.post('/api/TypeAttribute', this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListTypeAttribute();
                    _this.TypeAttributeModal.hide();
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
            this.http.put('/api/TypeAttribute/' + this.Item.TypeAttributeId, this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListTypeAttribute();
                    _this.TypeAttributeModal.hide();
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
    // // Mở modal cập nhật TypeAttribute
    // OpenEditModal(item) {
    //   this.editItem = new TypeAttribute();
    //   this.newType = new TypeAttributeItem();
    //   this.editItem = Object.assign(this.editItem, item);
    //   this.showItem = false;
    //   this.editModal.show();
    // }
    // // cập nhật typeAttribute
    // EditFunc() {
    //   if (this.editItem.Name == undefined || this.editItem.Name == '') {
    //     this.toastWarning("Chưa nhập tên!");
    //     return;
    //   }
    //   this.editItem.UserId = parseInt(localStorage.getItem("userId"));
    //   this.http.put('/api/TypeAttribute/' + this.editItem.TypeAttributeId, this.editItem, this.httpOptions).subscribe(
    //     (res) => {
    //       if (res["meta"]["error_code"] == 200) {
    //         this.GetListTypeAttribute();
    //         this.editModal.hide();
    //         this.toastSuccess("Cập nhật thành công!");
    //         console.log(this.editItem.listAttributeItem);
    //       }
    //       else {
    //         this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
    //         console.log("error");
    //       }
    //     },
    //     (err) => {
    //       this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
    //     }
    //   );
    // }
    //Mở modal thêm TypeAttributeItem
    TypeAttributeComponent.prototype.OpenTypeModalModal = function (i) {
        this.newType = new model_2.TypeAttributeItem();
        if (i != undefined) {
            this.newType = JSON.parse(JSON.stringify(this.Item.listAttributeItem[i]));
        }
        this.TypeModal.show();
    };
    //Thêm TypeAttributeItem
    TypeAttributeComponent.prototype.SaveTypeItem = function () {
        if (this.newType.Name == undefined || this.newType.Name == '') {
            this.toastWarning("Chưa nhập tên thuộc tính!");
            return;
        }
        if (this.newType.TypeAttributeItemId == undefined) {
            this.newType.Status = 1;
            this.Item.listAttributeItem.push(this.newType);
        }
        else {
            for (var i = 0; i < this.Item.listAttributeItem.length; i++) {
                if (this.newType.TypeAttributeItemId == this.Item.listAttributeItem[i].TypeAttributeItemId) {
                    this.Item.listAttributeItem[i] = JSON.parse(JSON.stringify(this.newType));
                }
            }
        }
        this.TypeModal.hide();
    };
    //Popup xác nhận xóa  
    TypeAttributeComponent.prototype.ShowConfirmDelete = function (Id) {
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
    //Xóa TypeAttribute
    TypeAttributeComponent.prototype.Delete = function (Id) {
        var _this = this;
        this.http.delete('/api/TypeAttribute/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListTypeAttribute();
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
    //xóa AttributeItem
    TypeAttributeComponent.prototype.DeleteItem = function (i) {
        if (this.Item.TypeAttributeId == undefined) {
            this.Item.listAttributeItem.splice(i, 1);
        }
        else {
            if (this.Item.listAttributeItem[i].TypeAttributeItemId == undefined) {
                this.Item.listAttributeItem.splice(i, 1);
            }
            else {
                this.Item.listAttributeItem[i].Status = 99;
            }
        }
    };
    TypeAttributeComponent.prototype.SortTable = function (str) {
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
        this.GetListTypeAttribute();
    };
    TypeAttributeComponent.prototype.GetClassSortTable = function (str) {
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
        core_1.ViewChild('TypeAttributeModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], TypeAttributeComponent.prototype, "TypeAttributeModal", void 0);
    __decorate([
        core_1.ViewChild('TypeModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], TypeAttributeComponent.prototype, "TypeModal", void 0);
    TypeAttributeComponent = __decorate([
        core_1.Component({
            selector: 'app-type-attribute',
            templateUrl: './type-attribute.component.html',
            styleUrls: ['./type-attribute.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService])
    ], TypeAttributeComponent);
    return TypeAttributeComponent;
}());
exports.TypeAttributeComponent = TypeAttributeComponent;
//# sourceMappingURL=type-attribute.component.js.map
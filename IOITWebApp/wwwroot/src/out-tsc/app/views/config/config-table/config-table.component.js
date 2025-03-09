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
var Model_1 = require("../../../data/Model");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var ngx_toastr_1 = require("ngx-toastr");
var ConfigTableComponent = /** @class */ (function () {
    function ConfigTableComponent(http, modalDialogService, viewRef, toastr) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.listConfigTable = [];
        this.showSort = [true, false];
        this.newItem = new Model_1.ConfigTable();
        this.editItem = new Model_1.ConfigTable();
        this.newType = new Model_1.ConfigTableItem();
        this.paging = {
            page: 1,
            page_size: 10,
            query: '1=1',
            order_by: '',
            item_count: 0
        };
        this.q = {
            txtSearch: ''
        };
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    ConfigTableComponent.prototype.ngOnInit = function () {
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        this.GetListConfigTable();
    };
    //
    ConfigTableComponent.prototype.Sort = function (IsK, s, i) {
        if (IsK) {
            this.paging.order_by = s + " asc";
        }
        else {
            this.paging.order_by = s + " desc";
        }
        this.GetListConfigTable();
        //this.showSort = !IsK;
        for (var j = 0; j < this.showSort.length; j++) {
            if (j == i) {
                this.showSort[j] = IsK;
            }
            else {
                this.showSort[j] = false;
            }
        }
    };
    //Get
    ConfigTableComponent.prototype.GetListConfigTable = function () {
        var _this = this;
        this.http.get('/api/ConfigTable/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listConfigTable = res["data"];
                _this.paging.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    ConfigTableComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListConfigTable();
    };
    //Thông báo
    ConfigTableComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    ConfigTableComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    ConfigTableComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    ConfigTableComponent.prototype.QueryChanged = function () {
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
        this.GetListConfigTable();
    };
    ConfigTableComponent.prototype.OpenAddModal = function () {
        this.newItem = new Model_1.ConfigTable();
        this.newType = new Model_1.ConfigTableItem();
        this.newItem.listConfigTableItem = [];
        this.showItem = true;
        this.addModal.show();
    };
    ConfigTableComponent.prototype.AddFunc = function () {
        var _this = this;
        if (this.newItem.Code == undefined || this.newItem.Code == '') {
            this.toastWarning("Chưa nhập mã!");
            return;
        }
        else if (this.newItem.Code.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập mã!");
            return;
        }
        else if (this.newItem.Name == undefined || this.newItem.Name == '') {
            this.toastWarning("Chưa nhập tên!");
            return;
        }
        else if (this.newItem.Name.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên!");
            return;
        }
        this.newItem.UserId = parseInt(localStorage.getItem("userId"));
        this.newItem.CompanyId = parseInt(localStorage.getItem("companyId"));
        this.http.post('/api/ConfigTable', this.newItem, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListConfigTable();
                _this.addModal.hide();
                _this.toastSuccess("Thêm mới thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    ConfigTableComponent.prototype.OpenEditModal = function (item) {
        this.editItem = new Model_1.ConfigTable();
        this.newType = new Model_1.ConfigTableItem();
        this.editItem = Object.assign(this.editItem, item);
        this.showItem = false;
        this.editModal.show();
    };
    ConfigTableComponent.prototype.EditFunc = function () {
        var _this = this;
        if (this.editItem.Code == undefined || this.editItem.Code == '') {
            this.toastWarning("Chưa nhập mã!");
            return;
        }
        else if (this.editItem.Code.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập mã!");
            return;
        }
        else if (this.editItem.Name == undefined || this.editItem.Name == '') {
            this.toastWarning("Chưa nhập tên!");
            return;
        }
        else if (this.editItem.Name.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên!");
            return;
        }
        this.editItem.UserId = parseInt(localStorage.getItem("userId"));
        this.editItem.CompanyId = parseInt(localStorage.getItem("companyId"));
        this.http.put('/api/ConfigTable/' + this.editItem.ConfigTableId, this.editItem, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListConfigTable();
                _this.editModal.hide();
                _this.toastSuccess("Cập nhật thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                console.log("error");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    // Config table item
    ConfigTableComponent.prototype.OpenAddItemModal = function () {
        this.newType = new Model_1.ConfigTableItem();
        this.addtypeModal.show();
    };
    ConfigTableComponent.prototype.AddTypeFunc = function (IsNew) {
        if (this.newType.Code == undefined || this.newType.Code == '') {
            this.toastWarning("Chưa nhập mã!");
            return;
        }
        else if (this.newItem.Code.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập mã!");
            return;
        }
        else if (this.newType.Name == undefined || this.newType.Name == '') {
            this.toastWarning("Chưa nhập tên thuộc tính!");
            return;
        }
        else if (this.newItem.Name.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên thuộc tính!");
            return;
        }
        if (IsNew) {
            this.newItem.listConfigTableItem.push(this.newType);
        }
        else {
            this.editItem.listConfigTableItem.push(this.newType);
        }
        this.addtypeModal.hide();
    };
    //xoa
    ConfigTableComponent.prototype.ConfirmDelete = function (Id) {
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
    ConfigTableComponent.prototype.Delete = function (Id) {
        var _this = this;
        this.http.delete('/api/ConfigTable/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListConfigTable();
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
    ConfigTableComponent.prototype.DeleteItem = function (i, IsNew) {
        if (IsNew) {
            this.newItem.listConfigTableItem.splice(i, 1);
        }
        else {
            console.log(this.editItem.listConfigTableItem[i]);
            if (this.editItem.listConfigTableItem[i].ConfigTableItemId != null) {
                this.editItem.listConfigTableItem.splice(i, 1);
            }
            else {
                this.editItem.listConfigTableItem.splice(i, 1);
            }
        }
    };
    __decorate([
        core_1.ViewChild('AddModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], ConfigTableComponent.prototype, "addModal", void 0);
    __decorate([
        core_1.ViewChild('EditModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], ConfigTableComponent.prototype, "editModal", void 0);
    __decorate([
        core_1.ViewChild('AddTypeModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], ConfigTableComponent.prototype, "addtypeModal", void 0);
    ConfigTableComponent = __decorate([
        core_1.Component({
            selector: 'app-config-table',
            templateUrl: './config-table.component.html',
            styleUrls: ['./config-table.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService])
    ], ConfigTableComponent);
    return ConfigTableComponent;
}());
exports.ConfigTableComponent = ConfigTableComponent;
//# sourceMappingURL=config-table.component.js.map
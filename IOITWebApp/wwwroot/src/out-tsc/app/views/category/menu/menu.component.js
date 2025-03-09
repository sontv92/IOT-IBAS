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
var $ = require("jquery");
var common_1 = require("@angular/common");
var dt_1 = require("../../../data/dt");
var MenuComponent = /** @class */ (function () {
    function MenuComponent(location, http, modalDialogService, viewRef, toastr) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.listMenu = [];
        this.listCate = [];
        this.key = 'Children';
        this.dataOutput = [{ CategoryId: 0, Name: "Root", Children: [] }];
        this.dataInput1 = [];
        this.PathLocation = location;
        this.Item = new model_1.Menu();
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "1=1";
        this.paging.order_by = "MenuId Desc";
        this.paging.item_count = 0;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    MenuComponent.prototype.ngOnInit = function () {
        this.GetListMenu();
    };
    MenuComponent.prototype.GetListMenu = function () {
        var _this = this;
        this.http.get('/api/menu/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listMenu = res["data"];
                _this.paging.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    MenuComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListMenu();
    };
    //Toast cảnh báo
    MenuComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    //Toast thành công
    MenuComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    //Toast thành công
    MenuComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    MenuComponent.prototype.QueryChanged = function () {
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
        this.GetListMenu();
    };
    MenuComponent.prototype.GetListCate = function () {
        var _this = this;
        this.http.get('/api/menu/GetCategoryMenu', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.dataInput1 = res["data"];
                _this.dataOutput = [{ CategoryId: 0, Name: "Root", Children: [] }];
                _this.MenuModal.show();
            }
        }, function (err) {
            console.log("Error: connect to API");
            _this.toastError("Đã xảy ra lỗi! Xin vui lòng thử lại sau.");
        });
    };
    //Mở modal thêm mới
    MenuComponent.prototype.OpenAddModal = function () {
        this.Item = new model_1.Menu();
        this.GetListCate();
        loadNestable();
    };
    MenuComponent.prototype.SaveMenuModal = function () {
        var _this = this;
        if (this.Item.Code == undefined || this.Item.Code == '') {
            this.toastWarning("Chưa nhập Mã Menu!");
            return;
        }
        else if (this.Item.Name == undefined || this.Item.Name == '') {
            this.toastWarning("Chưa nhập Tên Menu!");
            return;
        }
        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        this.Item.LanguageId = parseInt(localStorage.getItem("languageId"));
        this.Item.WebsiteId = parseInt(localStorage.getItem("websiteId"));
        this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
        var obj = this.Item;
        obj["listMenuItem"] = JSON.parse($('#nestable2-output').val())[0]["children"] != undefined ? JSON.parse($('#nestable2-output').val())[0]["children"] : [];
        if (this.Item.MenuId != undefined) {
            this.http.put('/api/menu/' + obj.MenuId, obj, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListMenu();
                    _this.MenuModal.hide();
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
            this.http.post('/api/menu', obj, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListMenu();
                    _this.MenuModal.hide();
                    _this.toastSuccess("Thêm mới thành công!");
                }
                else {
                    _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
    };
    MenuComponent.prototype.GetCategoryMenuLeft = function (id) {
        var _this = this;
        this.http.get('/api/menu/GetCategoryMenuLeft/' + id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.dataInput1 = res["data"];
                console.log(_this.dataInput1);
                loadNestable();
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    MenuComponent.prototype.GetCategoryMenuRight = function (id) {
        var _this = this;
        this.http.get('/api/menu/GetCategoryMenuRight/' + id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.dataOutput = res["data"];
                console.log(_this.dataOutput);
                loadNestable();
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    MenuComponent.prototype.OpenEditModal = function (item) {
        this.dataInput1 = [];
        this.dataOutput = [{ CategoryId: 0, Name: "Root", Children: [] }];
        this.GetCategoryMenuLeft(item.MenuId);
        this.GetCategoryMenuRight(item.MenuId);
        this.Item = Object.assign(this.Item, item);
        this.MenuModal.show();
    };
    MenuComponent.prototype.ShowConfirmDelete = function (Id) {
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
                        _this.DeleteMenu(Id);
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-default',
                }
            ],
        });
    };
    MenuComponent.prototype.DeleteMenu = function (Id) {
        var _this = this;
        this.http.delete('/api/menu/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListMenu();
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
    MenuComponent.prototype.SortTable = function (str) {
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
        this.GetListMenu();
    };
    MenuComponent.prototype.GetClassSortTable = function (str) {
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
        core_1.ViewChild('MenuModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], MenuComponent.prototype, "MenuModal", void 0);
    MenuComponent = __decorate([
        core_1.Component({
            selector: 'app-menu',
            providers: [common_1.Location, {
                    provide: common_1.LocationStrategy,
                    useClass: common_1.PathLocationStrategy
                }],
            templateUrl: './menu.component.html',
            styleUrls: ['./menu.component.scss']
        }),
        __metadata("design:paramtypes", [common_1.Location,
            http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService])
    ], MenuComponent);
    return MenuComponent;
}());
exports.MenuComponent = MenuComponent;
//# sourceMappingURL=menu.component.js.map
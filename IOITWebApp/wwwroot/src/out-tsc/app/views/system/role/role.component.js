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
var dt_1 = require("../../../data/dt");
var model_1 = require("../../../data/model");
var RoleComponent = /** @class */ (function () {
    function RoleComponent(http, modalDialogService, viewRef, toastr) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.listRole = [];
        this.listFunction = [];
        this.Item = new model_1.Role();
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "1=1";
        this.paging.order_by = "RoleId Desc";
        this.paging.item_count = 0;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        this.Action = {
            View: false,
            Create: false,
            Update: false,
            Delete: false,
            Import: false,
            Export: false,
            Print: false,
            Other: false,
            Menu: false
        };
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    RoleComponent.prototype.ngOnInit = function () {
        this.GetListRole();
    };
    //Get danh sách chức năng
    RoleComponent.prototype.GetListRole = function () {
        var _this = this;
        this.http.get('/api/functionrole/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listRole = res["data"];
                _this.paging.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    RoleComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListRole();
    };
    //Toast cảnh báo
    RoleComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    //Toast thành công
    RoleComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    //Toast thành công
    RoleComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    RoleComponent.prototype.QueryChanged = function () {
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
        this.GetListRole();
    };
    //Get danh sách chức năng cha
    RoleComponent.prototype.GetListFunction = function (IsNew) {
        var _this = this;
        this.http.get('/api/function/listFunction', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listFunction = res["data"];
                if (IsNew) {
                    _this.listFunction.forEach(function (item) {
                        item.Space = "";
                        item.View = false;
                        item.Create = false;
                        item.Update = false;
                        item.Delete = false;
                        item.Import = false;
                        item.Export = false;
                        item.Print = false;
                        item.Other = false;
                        item.Menu = false;
                        for (var i = 0; i < (item.Level) * 7; i++) {
                            item.Space += "&nbsp;";
                        }
                    });
                }
                else {
                    for (var i = 0; i < _this.listFunction.length; i++) {
                        for (var j = 0; j < _this.Item.listFunction.length; j++) {
                            if (_this.listFunction[i].FunctionId == _this.Item.listFunction[j].FunctionId) {
                                _this.listFunction[i].View = _this.Item.listFunction[j].ActiveKey[0] == "1" ? true : false;
                                _this.listFunction[i].Create = _this.Item.listFunction[j].ActiveKey[1] == "1" ? true : false;
                                _this.listFunction[i].Update = _this.Item.listFunction[j].ActiveKey[2] == "1" ? true : false;
                                _this.listFunction[i].Delete = _this.Item.listFunction[j].ActiveKey[3] == "1" ? true : false;
                                _this.listFunction[i].Import = _this.Item.listFunction[j].ActiveKey[4] == "1" ? true : false;
                                _this.listFunction[i].Export = _this.Item.listFunction[j].ActiveKey[5] == "1" ? true : false;
                                _this.listFunction[i].Print = _this.Item.listFunction[j].ActiveKey[6] == "1" ? true : false;
                                _this.listFunction[i].Other = _this.Item.listFunction[j].ActiveKey[7] == "1" ? true : false;
                                _this.listFunction[i].Menu = _this.Item.listFunction[j].ActiveKey[8] == "1" ? true : false;
                                break;
                            }
                        }
                        _this.listFunction[i].Space = "";
                        for (var idx = 0; idx < (_this.listFunction[i].Level) * 7; idx++) {
                            _this.listFunction[i].Space += "&nbsp;";
                        }
                    }
                    _this.changeCell();
                }
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    RoleComponent.prototype.changeAction = function (cs) {
        var _this = this;
        this.listFunction.forEach(function (item) {
            switch (cs) {
                case 1:
                    item.View = _this.Action.View;
                    break;
                case 2:
                    item.Create = _this.Action.Create;
                    break;
                case 3:
                    item.Update = _this.Action.Update;
                    break;
                case 4:
                    item.Delete = _this.Action.Delete;
                    break;
                case 5:
                    item.Import = _this.Action.Import;
                    break;
                case 6:
                    item.Export = _this.Action.Export;
                    break;
                case 7:
                    item.Print = _this.Action.Print;
                    break;
                case 8:
                    item.Other = _this.Action.Other;
                    break;
                case 9:
                    item.Menu = _this.Action.Menu;
                    break;
                default:
                    break;
            }
            if (item.View && item.Create && item.Update && item.Delete && item.Import && item.Export && item.Print && item.Other && item.Menu) {
                item.Full = true;
            }
            else {
                item.Full = false;
            }
        });
    };
    RoleComponent.prototype.changeFull = function (i) {
        if (i != undefined) {
            this.listFunction[i].View = this.listFunction[i].Full;
            this.listFunction[i].Create = this.listFunction[i].Full;
            this.listFunction[i].Update = this.listFunction[i].Full;
            this.listFunction[i].Delete = this.listFunction[i].Full;
            this.listFunction[i].Import = this.listFunction[i].Full;
            this.listFunction[i].Export = this.listFunction[i].Full;
            this.listFunction[i].Print = this.listFunction[i].Full;
            this.listFunction[i].Other = this.listFunction[i].Full;
            this.listFunction[i].Menu = this.listFunction[i].Full;
        }
        if (this.listFunction.filter(function (l) { return l.View == false || l.View == undefined; }).length > 0) {
            this.Action.View = false;
        }
        else {
            this.Action.View = true;
        }
        if (this.listFunction.filter(function (l) { return l.Create == false || l.Create == undefined; }).length > 0) {
            this.Action.Create = false;
        }
        else {
            this.Action.Create = true;
        }
        if (this.listFunction.filter(function (l) { return l.Update == false || l.Update == undefined; }).length > 0) {
            this.Action.Update = false;
        }
        else {
            this.Action.Update = true;
        }
        if (this.listFunction.filter(function (l) { return l.Delete == false || l.Delete == undefined; }).length > 0) {
            this.Action.Delete = false;
        }
        else {
            this.Action.Delete = true;
        }
        if (this.listFunction.filter(function (l) { return l.Import == false || l.Import == undefined; }).length > 0) {
            this.Action.Import = false;
        }
        else {
            this.Action.Import = true;
        }
        if (this.listFunction.filter(function (l) { return l.Export == false || l.Export == undefined; }).length > 0) {
            this.Action.Export = false;
        }
        else {
            this.Action.Export = true;
        }
        if (this.listFunction.filter(function (l) { return l.Print == false || l.Print == undefined; }).length > 0) {
            this.Action.Print = false;
        }
        else {
            this.Action.Print = true;
        }
        if (this.listFunction.filter(function (l) { return l.Other == false || l.Other == undefined; }).length > 0) {
            this.Action.Other = false;
        }
        else {
            this.Action.Other = true;
        }
        if (this.listFunction.filter(function (l) { return l.Menu == false || l.Menu == undefined; }).length > 0) {
            this.Action.Menu = false;
        }
        else {
            this.Action.Menu = true;
        }
    };
    RoleComponent.prototype.changeCell = function () {
        this.changeAction(10);
        this.changeFull(undefined);
    };
    RoleComponent.prototype.OpenModalRole = function (item) {
        this.Item = new model_1.Role();
        this.Item.listFunction = [];
        this.listFunction = [];
        this.Action = {
            View: false,
            Create: false,
            Update: false,
            Delete: false,
            Import: false,
            Export: false,
            Print: false,
            Other: false,
            Menu: false,
        };
        if (item == undefined) {
            this.GetListFunction(true);
        }
        else {
            this.Item = Object.assign(this.Item, item);
            this.GetListFunction(false);
        }
        this.modalRole.show();
    };
    RoleComponent.prototype.SaveRole = function () {
        var _this = this;
        if (this.Item.Code == undefined || this.Item.Code == '') {
            this.toastWarning("Chưa nhập Mã quyền!");
            return;
        }
        else if (this.Item.Name == undefined || this.Item.Name == '') {
            this.toastWarning("Chưa nhập Tên quyền!");
            return;
        }
        var listFunction = [];
        this.listFunction.forEach(function (item) {
            var newFunc = new model_1.FuncRole();
            newFunc.FunctionId = item.FunctionId;
            newFunc.ActiveKey = "";
            newFunc.ActiveKey += item.View == true ? 1 : 0;
            newFunc.ActiveKey += item.Create == true ? 1 : 0;
            newFunc.ActiveKey += item.Update == true ? 1 : 0;
            newFunc.ActiveKey += item.Delete == true ? 1 : 0;
            newFunc.ActiveKey += item.Import == true ? 1 : 0;
            newFunc.ActiveKey += item.Export == true ? 1 : 0;
            newFunc.ActiveKey += item.Print == true ? 1 : 0;
            newFunc.ActiveKey += item.Other == true ? 1 : 0;
            newFunc.ActiveKey += item.Menu == true ? 1 : 0;
            listFunction.push(newFunc);
        });
        this.Item.listFunction = listFunction;
        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        if (this.Item.RoleId == undefined) {
            this.http.post('/api/functionRole', this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListRole();
                    _this.modalRole.hide();
                    _this.toastSuccess("Thêm mới thành công!");
                }
                else if (res["meta"]["error_code"] == 211) {
                    _this.toastWarning("Mã quyền đã tồn tại. Xin vui lòng thử lại!");
                }
                else {
                    _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
        else {
            this.http.put('/api/functionRole/' + this.Item.RoleId, this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListRole();
                    _this.modalRole.hide();
                    _this.toastSuccess("Cập nhật thành công!");
                }
                else if (res["meta"]["error_code"] == 211) {
                    _this.toastWarning("Mã quyền đã tồn tại. Xin vui lòng thử lại!");
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
    RoleComponent.prototype.ShowConfirmDelete = function (Id) {
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
                    buttonClass: 'btn btn-default'
                }
            ],
        });
    };
    RoleComponent.prototype.Delete = function (Id) {
        var _this = this;
        this.http.delete('/api/functionrole/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListRole();
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
    RoleComponent.prototype.SortTable = function (str) {
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
        this.GetListRole();
    };
    RoleComponent.prototype.GetClassSortTable = function (str) {
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
        core_1.ViewChild('modalRole'),
        __metadata("design:type", modal_1.ModalDirective)
    ], RoleComponent.prototype, "modalRole", void 0);
    RoleComponent = __decorate([
        core_1.Component({
            selector: 'app-role',
            templateUrl: './role.component.html',
            styleUrls: ['./role.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient, ngx_modal_dialog_1.ModalDialogService, core_1.ViewContainerRef, ngx_toastr_1.ToastrService])
    ], RoleComponent);
    return RoleComponent;
}());
exports.RoleComponent = RoleComponent;
//# sourceMappingURL=role.component.js.map
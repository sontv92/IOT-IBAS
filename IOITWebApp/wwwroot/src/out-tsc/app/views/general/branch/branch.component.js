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
var const_1 = require("../../../data/const");
var model_1 = require("../../../data/model");
var ngx_toastr_1 = require("ngx-toastr");
var dt_1 = require("../../../data/dt");
var BranchComponent = /** @class */ (function () {
    function BranchComponent(http, modalDialogService, viewRef, toastr) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.listBranch = [];
        this.listCompany = [];
        this.domainImage = const_1.domainImage;
        this.Item = new model_1.Branch();
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "1=1";
        this.paging.order_by = "BranchId Desc";
        this.paging.item_count = 0;
        this.isdisabled = false;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        this.role = -1;
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    BranchComponent.prototype.ngOnInit = function () {
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        var json = JSON.parse(localStorage.getItem('roles'));
        this.companyId = parseInt(localStorage.getItem('companyId'));
        if (json.length > 0) {
            for (var i = 0; i < json.length; i++) {
                if (json[i].RoleId == 1) {
                    this.role = 1;
                }
            }
        }
        this.GetListBranch();
        if (this.role == 1) {
            this.GetListCompany();
        }
    };
    BranchComponent.prototype.GetListBranch = function () {
        var _this = this;
        if (this.role != 1) {
            this.paging.query += ' and CompanyId = ' + this.companyId;
        }
        this.http.get('/api/branch/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listBranch = res["data"];
                _this.paging.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    BranchComponent.prototype.GetListCompany = function () {
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
    BranchComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListBranch();
    };
    //Toast cảnh báo
    BranchComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    //Toast thành công
    BranchComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    //Toast thành công
    BranchComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //Search
    BranchComponent.prototype.QueryChanged = function () {
        var query = '';
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            if (query != '') {
                query += ' and Name.Contains("' + this.q.txtSearch + '")';
            }
            else {
                query += 'Name.Contains("' + this.q.txtSearch + '")';
            }
        }
        if (this.q.CompanyId != undefined && this.q.CompanyId != -1) {
            if (query != '') {
                query += ' and CompanyId = ' + this.q.CompanyId;
            }
            else {
                query += 'CompanyId = ' + this.q.CompanyId;
            }
        }
        if (this.role != 1) {
            if (query != '') {
                query += ' and CompanyId = ' + this.companyId;
            }
            else {
                query += 'CompanyId = ' + this.companyId;
            }
        }
        if (query == '')
            this.paging.query = '1=1';
        else
            this.paging.query = query;
        this.GetListBranch();
    };
    //Mở modal thêm mới
    BranchComponent.prototype.OpenBranchModal = function (item) {
        this.Item = new model_1.Branch();
        this.progress = undefined;
        if (item != undefined) {
            this.isdisabled = true;
            this.Item = JSON.parse(JSON.stringify(item));
        }
        else {
            this.isdisabled = false;
        }
        this.BranchModal.show();
    };
    //Thêm mới khách hàng
    BranchComponent.prototype.SaveBranch = function () {
        var _this = this;
        if (this.Item.Code == undefined || this.Item.Code == '') {
            this.toastWarning("Chưa nhập mã!");
            return;
        }
        else if (this.Item.Code.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập mã!");
            return;
        }
        else if (this.Item.Name == undefined || this.Item.Name == '') {
            this.toastWarning("Chưa nhập tên!");
            return;
        }
        else if (this.Item.Name.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên!");
            return;
        }
        else if (this.Item.Email == undefined || this.Item.Email == '') {
            this.toastWarning("Chưa nhập email!");
            return;
        }
        else if (this.Item.Email.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập email!");
            return;
        }
        else if (this.Item.Phone == undefined || this.Item.Phone == '') {
            this.toastWarning("Chưa nhập mật khẩu!");
            return;
        }
        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        if (this.Item.BranchId == undefined) {
            if (this.role != 1) {
                this.Item.CompanyId = this.companyId;
            }
            var obj = JSON.parse(JSON.stringify(this.Item));
            this.http.post('/api/Branch', obj, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListBranch();
                    _this.BranchModal.hide();
                    _this.toastSuccess("Thêm mới thành công!");
                }
                else if (res["meta"]["error_code"] == 212) {
                    _this.toastWarning("Mã chi nhánh đã tồn tại!");
                }
                else {
                    _this.toastError(res["meta"]["error_message"]);
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
        else {
            this.http.put('/api/Branch/' + this.Item.BranchId, this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListBranch();
                    _this.BranchModal.hide();
                    _this.toastSuccess(res["meta"]["error_message"]);
                }
                else if (res["meta"]["error_code"] == 212) {
                    _this.toastWarning("Mã chi nhánh đã tồn tại!");
                }
                else {
                    _this.toastError(res["meta"]["error_message"]);
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
    };
    //Popup xác nhận xóa
    BranchComponent.prototype.ShowConfirmDelete = function (Id) {
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
                        _this.DeleteBranch(Id);
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-default',
                }
            ],
        });
    };
    BranchComponent.prototype.DeleteBranch = function (Id) {
        var _this = this;
        this.http.delete('/api/Branch/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListBranch();
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
    BranchComponent.prototype.upload = function (files) {
        var _this = this;
        if (files.length === 0)
            return;
        var formData = new FormData();
        for (var _i = 0, files_1 = files; _i < files_1.length; _i++) {
            var file = files_1[_i];
            formData.append(file.name, file);
        }
        console.log(formData);
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
                _this.Item.Avatar = event.body["data"].toString();
            }
        });
    };
    //
    BranchComponent.prototype.RemoveImage = function () {
        this.Item.Avatar = undefined;
        this.progress = undefined;
    };
    BranchComponent.prototype.SortTable = function (str) {
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
        this.GetListBranch();
    };
    BranchComponent.prototype.GetClassSortTable = function (str) {
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
        core_1.ViewChild('BranchModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], BranchComponent.prototype, "BranchModal", void 0);
    BranchComponent = __decorate([
        core_1.Component({
            selector: 'app-branch',
            templateUrl: './branch.component.html',
            styleUrls: ['./branch.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService])
    ], BranchComponent);
    return BranchComponent;
}());
exports.BranchComponent = BranchComponent;
//# sourceMappingURL=branch.component.js.map
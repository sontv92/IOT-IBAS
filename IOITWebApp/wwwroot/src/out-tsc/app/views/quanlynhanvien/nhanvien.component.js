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
var model_1 = require("../../data/model");
var ngx_toastr_1 = require("ngx-toastr");
var common_1 = require("@angular/common");
var common_service_1 = require("../../service/common.service");
var const_1 = require("../../data/const");
var dt_1 = require("../../data/dt");
var router_1 = require("@angular/router");
var $ = require("jquery");
var NhanVienComponent = /** @class */ (function () {
    function NhanVienComponent(http, modalDialogService, viewRef, toastr, datePipe, common, router) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.datePipe = datePipe;
        this.common = common;
        this.router = router;
        this.listUser = [];
        this.listCompany = [];
        this.listBranch = [];
        this.listBranchId = [];
        this.listRole = [];
        this.listFunc = [];
        this.domainImage = const_1.domainImage;
        this.Item = new model_1.NHANVIEN();
        this.Item.GHICHU = '';
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "1=1";
        this.paging.order_by = "Ma Asc";
        this.paging.item_count = 0;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        this.role = -1;
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
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    NhanVienComponent.prototype.ngOnInit = function () {
        $('input[type="checkbox"]').on('change', function () {
            $('input[type="checkbox"]').not(this).prop('checked', false);
        });
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        this.edit = false;
        var json = JSON.parse(localStorage.getItem('roles'));
        this.companyId = parseInt(localStorage.getItem('companyId'));
        this.UserId = parseInt(localStorage.getItem('userId'));
        this.companyselect = -1;
        if (json.length > 0) {
            for (var i = 0; i < json.length; i++) {
                this.role = json[i].RoleId;
            }
        }
        if (this.role == 3) {
            this.companyselect = this.companyId;
            this.GetListBranch();
        }
        else if (this.role == 1) {
            this.GetListCompany();
        }
        // this.GetListKhachHang();
        ////this.GetListCompany();
        // if (this.role == 3) {
        //   this.companyselect = this.companyId;
        //   this.GetListBranch();
        // }
    };
    NhanVienComponent.prototype.ResetCurrentRouter = function () {
        this.router.routeReuseStrategy.shouldReuseRoute = function () {
            return false;
        };
        this.router.onSameUrlNavigation = 'reload';
        this.router.navigateByUrl(this.router.url);
    };
    //Get danh sách khach hang
    NhanVienComponent.prototype.GetListNhanVien = function () {
        var _this = this;
        // if (this.role != 1) {
        //   this.paging.query += ' and CompanyId = ' + this.companyId;
        // }
        this.http.get('/api/NhanVien/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by + '&companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listUser = res["data"];
                _this.paging.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    NhanVienComponent.prototype.GetListCompany = function () {
        var _this = this;
        this.http.get('/api/userrole/GetByCompany?page=1&query=1=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listCompany = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    NhanVienComponent.prototype.GetListBranch = function () {
        var _this = this;
        this.http.get('/api/branch/GetByPage?page=1&query=CompanyId = ' + this.companyselect + ' &order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listBranch = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    NhanVienComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListNhanVien();
    };
    //Thông báo
    NhanVienComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    NhanVienComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    NhanVienComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    NhanVienComponent.prototype.CompanyChanged = function () {
        //this.companyselect = this.Item.CompanyId;
        this.GetListBranch();
    };
    //
    NhanVienComponent.prototype.QueryChanged = function () {
        if (this.role == 1) {
            if (this.q.CompanyId == undefined || this.q.CompanyId == null) {
                this.toastr.warning('Vui lòng chọn công ty !', 'Cảnh báo');
                return;
            }
        }
        if (this.q.BranchId == undefined || this.q.BranchId == null) {
            this.toastr.warning('Vui lòng chọn trạm trộn !', 'Cảnh báo');
            return;
        }
        var query = '';
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            if (query != '') {
                query += " and TENNV LIKE  \N'%" + this.q.txtSearch + "%\'";
            }
            else {
                query += "  TENNV LIKE  \N'%" + this.q.txtSearch + "%\'";
            }
        }
        if (this.role == 3) {
            this.paging.CompanyId = this.companyId;
        }
        else {
            if (this.q.CompanyId != undefined) {
                this.paging.CompanyId = this.q.CompanyId;
            }
            else {
                this.paging.CompanyId = 0;
            }
        }
        // if (this.q.Branchlist != undefined) {
        //   this.paging.Branchid = '';
        //   this.q.Branchlist.forEach((item, index) => {
        //     if (item != '') {
        //       this.paging.Branchid = this.paging.Branchid + item + ',';
        //     }
        //   });
        // }
        // else {
        //   this.paging.Branchid = '';
        //   if (this.role != 1) {
        //     this.listBranch.forEach((item, index) => {
        //       if (item != '') {
        //         this.paging.Branchid = this.paging.Branchid + item["BranchId"] + ',';
        //       }
        //     });
        //   }
        // }
        if (this.q.BranchId != undefined) {
            this.paging.Branchid = this.q.BranchId;
        }
        if (query == '')
            this.paging.query = '1=1';
        else
            this.paging.query = query;
        this.GetListNhanVien();
    };
    //
    NhanVienComponent.prototype.QueryChangedCompany = function () {
        var query = '';
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            if (query != '') {
                query += ' and Name.Contains("' + this.q.txtSearch + '")';
            }
            else {
                query += 'Name.Contains("' + this.q.txtSearch + '")';
            }
        }
        if (this.q.CompanyId != undefined) {
            this.companyselect = this.q.CompanyId;
            this.listBranch = [];
            this.q.BranchId = null;
            this.GetListBranch();
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
        if (this.q.BranchId != undefined) {
            if (query != '') {
                query += ' and BranchId.Contains("' + this.q.BranchId + '")';
            }
            else {
                query += 'BranchId.Contains("' + this.q.BranchId + '")';
            }
        }
        if (query == '')
            this.paging.query = '1=1';
        else
            this.paging.query = query;
        this.GetListNhanVien();
    };
    //Mở modal thêm mới
    NhanVienComponent.prototype.OpenAddModal = function () {
        if (this.q.BranchId == undefined || this.q.BranchId == null) {
            this.toastWarning("Vui lòng chọn trạm trộn!");
            return;
        }
        this.edit = false;
        this.Item = new model_1.NHANVIEN();
        this.disable = true;
        this.message = undefined;
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
        this.userModal.show();
    };
    NhanVienComponent.prototype.changeFn = function (val) {
        this.Item.BranchId = val;
    };
    //Thêm mới danh mục trang
    NhanVienComponent.prototype.AddUserFunc = function () {
        var _this = this;
        if (this.Item.SDT == undefined || this.Item.SDT == '') {
            this.toastWarning("Chưa nhập Số điện thoại!");
            return;
        }
        else if (this.Item.SDT.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập Số điện thoại!");
            return;
        }
        if (this.Item.TENNV == undefined || this.Item.TENNV == '') {
            this.toastWarning("Chưa nhập Tên nhân viên!");
            return;
        }
        else if (this.Item.TENNV.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập Tên nhân viên!");
            return;
        }
        // if (this.q.BranchId != undefined) {
        //     this.Item.BranchId = this.q.BranchId;
        // }
        // else{
        //   this.toastWarning("Chưa chọn trạm trộn!");
        //   return;
        // }
        // if (this.listBranchId != undefined) {
        //   this.Item.BranchId = '';
        //   this.listBranchId.forEach((item, index) => {
        //     if (index == this.listBranchId.length - 1) {
        //       this.Item.BranchId += item;
        //     }
        //     else {
        //       this.Item.BranchId += item + ",";
        //     }
        //   });
        // }
        // else {
        //   this.Item.BranchId = '';
        // }
        if (this.Item.ID) {
            this.Item.BranchId = this.q.BranchId;
            this.http.put('/api/NhanVien/' + this.Item.ID, this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListNhanVien();
                    _this.userModal.hide();
                    _this.toastSuccess("Cập nhật thành công!");
                }
                else if (res["meta"]["error_code"] == 21111) {
                    _this.toastWarning("Đã quá số lượng tài khoản cho phép!");
                }
                else if (res["meta"]["error_code"] == 211) {
                    _this.toastWarning("Tên tài khoản đã tồn tại!");
                }
                else if (res["meta"]["error_code"] == 2111) {
                    _this.toastWarning("Email đã tồn tại!");
                }
                else {
                    _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
        else {
            // if (this.role != 1) {
            //   this.Item.CompanyId = this.companyId;
            // }
            this.Item.BranchId = this.q.BranchId;
            this.http.post('/api/NhanVien', this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListNhanVien();
                    _this.userModal.hide();
                    _this.toastSuccess("Thêm mới thành công!");
                }
                else if (res["meta"]["error_code"] == 21111) {
                    _this.toastWarning("Đã quá số lượng tài khoản cho phép!");
                }
                else if (res["meta"]["error_code"] == 211) {
                    _this.toastWarning("Tên tài khoản đã tồn tại!");
                }
                else if (res["meta"]["error_code"] == 2111) {
                    _this.toastWarning("Email đã tồn tại!");
                }
                else {
                    _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
    };
    NhanVienComponent.prototype.OpenEditModal = function (item) {
        //this.form.controls['name'].disable();
        this.disable = true;
        this.edit = true;
        this.Item = new model_1.NHANVIEN();
        this.GetListCompany();
        this.Item = Object.assign(this.Item, item);
        // if (item.CompanyId != '' && item.CompanyId != null) {
        //   this.companyselect = item.CompanyId;
        //   this.GetListBranch();
        // }
        this.GetListBranch();
        this.userModal.show();
    };
    //Popup xác nhận xóa
    NhanVienComponent.prototype.ShowConfirmDelete = function (Id, BranchId) {
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
                        _this.XoaKH(Id, BranchId);
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-default',
                }
            ],
        });
    };
    NhanVienComponent.prototype.ShowConfirmReset = function (Id) {
        var _this = this;
        this.modalDialogService.openDialog(this.viewRef, {
            title: 'Xác nhận',
            childComponent: ngx_modal_dialog_1.SimpleModalComponent,
            data: {
                text: "Bạn có chắc chắn muốn reset mật khẩu không?"
            },
            actionButtons: [
                {
                    text: 'Đồng ý',
                    buttonClass: 'btn btn-success',
                    onAction: function () {
                        _this.ResetUser(Id);
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-default',
                }
            ],
        });
    };
    NhanVienComponent.prototype.XoaKH = function (Id, BranchId) {
        var _this = this;
        var strID = Id + "_" + BranchId;
        this.http.delete('/api/NhanVien/' + strID, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListNhanVien();
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
    NhanVienComponent.prototype.ResetUser = function (Id) {
        var _this = this;
        this.http.get('/api/userrole/ResetPassUser?id=' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListNhanVien();
                _this.viewRef.clear();
                _this.toastSuccess("Reset thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    NhanVienComponent.prototype.upload = function (files) {
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
                //this.Item.Avata = this.message;
            }
        });
    };
    NhanVienComponent.prototype.GetListFunction = function (IsNew) {
        var _this = this;
        this.http.get('/api/function/listFunction', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listFunc = res["data"];
                if (IsNew) {
                    _this.listFunc.forEach(function (item) {
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
                    _this.changeCell();
                }
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    NhanVienComponent.prototype.changeAction = function (cs) {
        var _this = this;
        this.listFunc.forEach(function (item) {
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
    NhanVienComponent.prototype.changeFull = function (i) {
        if (i != undefined) {
            this.listFunc[i].View = this.listFunc[i].Full;
            this.listFunc[i].Create = this.listFunc[i].Full;
            this.listFunc[i].Update = this.listFunc[i].Full;
            this.listFunc[i].Delete = this.listFunc[i].Full;
            this.listFunc[i].Import = this.listFunc[i].Full;
            this.listFunc[i].Export = this.listFunc[i].Full;
            this.listFunc[i].Print = this.listFunc[i].Full;
            this.listFunc[i].Other = this.listFunc[i].Full;
            this.listFunc[i].Menu = this.listFunc[i].Full;
        }
        if (this.listFunc.filter(function (l) { return l.View == false; }).length > 0) {
            this.Action.View = false;
        }
        else {
            this.Action.View = true;
        }
        if (this.listFunc.filter(function (l) { return l.Create == false; }).length > 0) {
            this.Action.Create = false;
        }
        else {
            this.Action.Create = true;
        }
        if (this.listFunc.filter(function (l) { return l.Update == false; }).length > 0) {
            this.Action.Update = false;
        }
        else {
            this.Action.Update = true;
        }
        if (this.listFunc.filter(function (l) { return l.Delete == false; }).length > 0) {
            this.Action.Delete = false;
        }
        else {
            this.Action.Delete = true;
        }
        if (this.listFunc.filter(function (l) { return l.Import == false; }).length > 0) {
            this.Action.Import = false;
        }
        else {
            this.Action.Import = true;
        }
        if (this.listFunc.filter(function (l) { return l.Export == false; }).length > 0) {
            this.Action.Export = false;
        }
        else {
            this.Action.Export = true;
        }
        if (this.listFunc.filter(function (l) { return l.Print == false; }).length > 0) {
            this.Action.Print = false;
        }
        else {
            this.Action.Print = true;
        }
        if (this.listFunc.filter(function (l) { return l.Other == false; }).length > 0) {
            this.Action.Other = false;
        }
        else {
            this.Action.Other = true;
        }
        if (this.listFunc.filter(function (l) { return l.Menu == false; }).length > 0) {
            this.Action.Menu = false;
        }
        else {
            this.Action.Menu = true;
        }
    };
    NhanVienComponent.prototype.changeCell = function () {
        this.changeAction(10);
        this.changeFull(undefined);
    };
    NhanVienComponent.prototype.SortTable = function (str) {
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
        this.GetListNhanVien();
    };
    NhanVienComponent.prototype.GetClassSortTable = function (str) {
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
        core_1.ViewChild('UserModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], NhanVienComponent.prototype, "userModal", void 0);
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], NhanVienComponent.prototype, "file", void 0);
    NhanVienComponent = __decorate([
        core_1.Component({
            selector: 'app-nhanvien',
            templateUrl: './nhanvien.component.html'
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService,
            common_1.DatePipe,
            common_service_1.CommonService,
            router_1.Router])
    ], NhanVienComponent);
    return NhanVienComponent;
}());
exports.NhanVienComponent = NhanVienComponent;
//# sourceMappingURL=nhanvien.component.js.map
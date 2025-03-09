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
var const_1 = require("../../../data/const"); //có thể bỏ typeRank
var model_1 = require("../../../data/model");
var ngx_toastr_1 = require("ngx-toastr");
var RankComponent = /** @class */ (function () {
    function RankComponent(http, modalDialogService, viewRef, toastr) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.show = false;
        this.showSort = [true, false, false, false];
        this.listCateRank = [];
        this.listTypeRank = [];
        this.listWebsite = [];
        this.listLanguage = [];
        this.typeRank = const_1.typeRank;
        this.newItem = new model_1.CategoryRank();
        this.editItem = new model_1.CategoryRank();
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
    RankComponent.prototype.ngOnInit = function () {
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        this.GetListCateRank();
    };
    //
    RankComponent.prototype.Sort = function (IsK, s, i) {
        if (IsK) {
            this.paging.order_by = s + " asc";
        }
        else {
            this.paging.order_by = s + " desc";
        }
        this.GetListCateRank();
        //this.showSort = !IsK;
        for (var j = 0; j < this.showSort.length; j++) {
            if (j == i) {
                this.showSort[j] = IsK;
                console.log("if = " + this.showSort[j]);
            }
            else {
                this.showSort[j] = false;
                console.log("else = " + this.showSort[j]);
            }
        }
    };
    //Get danh sách rank
    RankComponent.prototype.GetListCateRank = function () {
        var _this = this;
        this.http.get('/api/categoryrank/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listCateRank = res["data"];
                _this.paging.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Get danh sách type rank
    RankComponent.prototype.GetListTypeRank = function () {
        var _this = this;
        this.http.get('/api/typeattributeitem/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listTypeRank = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    RankComponent.prototype.GetListLanguage = function () {
        var _this = this;
        this.http.get('/api/Language/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listLanguage = res["data"];
                if (_this.listLanguage.length == 1) {
                    _this.newItem.LanguageId = parseInt(localStorage.getItem("languageId"));
                    _this.editItem.LanguageId = parseInt(localStorage.getItem("languageId"));
                }
                else {
                    _this.show = true;
                }
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    RankComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListCateRank();
    };
    //Toast cảnh báo
    RankComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    //Toast thành công
    RankComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    //Toast thành công
    RankComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    RankComponent.prototype.QueryChanged = function () {
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
        this.GetListCateRank();
    };
    //Mở modal thêm mới
    RankComponent.prototype.OpenAddModal = function () {
        this.newItem = new model_1.CategoryRank();
        this.newItem.Description = "";
        this.GetListTypeRank();
        this.GetListLanguage();
        this.addModal.show();
    };
    //Thêm mới danh mục trang
    RankComponent.prototype.AddFunc = function () {
        var _this = this;
        if (this.newItem.Name == undefined || this.newItem.Name == '') {
            this.toastWarning("Chưa nhập Tên danh mục!");
            return;
        }
        else if (this.newItem.Name.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên danh mục !");
            return;
        }
        else if (this.newItem.RankStart == undefined) {
            this.toastWarning("Chưa nhập khoảng bắt đầu!");
            return;
        }
        else if (this.newItem.RankEnd == undefined) {
            this.toastWarning("Chưa nhập khoảng kết thúc!");
            return;
        }
        else if (this.newItem.LanguageId == undefined) {
            this.toastWarning("Chưa chọn ngôn ngữ!");
            return;
        }
        if (this.newItem.RankStart > this.newItem.RankEnd) {
            this.toastWarning("Khoảng bắt đầu phải nhỏ hơn khoảng kết thúc!");
            return;
        }
        this.newItem.CompanyId = parseInt(localStorage.getItem("companyId"));
        this.newItem.UserId = parseInt(localStorage.getItem("userId"));
        this.newItem.WebsiteId = parseInt(localStorage.getItem("websiteId"));
        this.http.post('/api/CategoryRank', this.newItem, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListCateRank();
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
    //Mở modal cập nhật
    RankComponent.prototype.OpenEditModal = function (item) {
        this.editItem = new model_1.CategoryRank();
        this.editItem = Object.assign(this.editItem, item);
        if (this.editItem.TypeRankId == 0)
            this.editItem.TypeRankId = undefined;
        this.GetListTypeRank();
        this.GetListLanguage();
        this.editModal.show();
    };
    //Cập nhật danh mục trang
    RankComponent.prototype.EditFunc = function () {
        var _this = this;
        if (this.editItem.Name == undefined || this.editItem.Name == '') {
            this.toastWarning("Chưa nhập Tên danh mục!");
            return;
        }
        else if (this.editItem.Name.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên danh mục");
            return;
        }
        else if (this.editItem.RankStart == undefined || this.editItem.RankStart == 0) {
            this.toastWarning("Chưa nhập khoảng bắt đầu!");
            return;
        }
        else if (this.editItem.RankEnd == undefined || this.editItem.RankEnd == 0) {
            this.toastWarning("Chưa nhập khoảng kết thúc!");
            return;
        }
        else if (this.editItem.LanguageId == undefined) {
            this.toastWarning("Chưa chọn ngôn ngữ!");
            return;
        }
        if (this.newItem.RankStart > this.newItem.RankEnd) {
            this.toastWarning("Khoảng bắt đầu phải nhỏ hơn khoảng kết thúc!");
            return;
        }
        this.newItem.CompanyId = parseInt(localStorage.getItem("companyId"));
        this.newItem.UserId = parseInt(localStorage.getItem("userId"));
        this.newItem.WebsiteId = parseInt(localStorage.getItem("websiteId"));
        this.http.put('/api/CategoryRank/' + this.editItem.CategoryRankId, this.editItem, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListCateRank();
                _this.editModal.hide();
                _this.toastSuccess("Cập nhật thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    //Popup xác nhận xóa
    RankComponent.prototype.ShowConfirmDelete = function (Id) {
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
    RankComponent.prototype.Delete = function (Id) {
        var _this = this;
        this.http.delete('/api/CategoryRank/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListCateRank();
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
    RankComponent.prototype.ShowHide = function (id, i) {
        var _this = this;
        var stt = this.listCateRank[i].IsShow ? 1 : 10;
        this.http.put('/api/CategoryRank/ShowHide/' + id + "/" + stt, undefined, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.toastSuccess("Thay đổi trạng thái thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    __decorate([
        core_1.ViewChild('AddModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], RankComponent.prototype, "addModal", void 0);
    __decorate([
        core_1.ViewChild('EditModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], RankComponent.prototype, "editModal", void 0);
    RankComponent = __decorate([
        core_1.Component({
            selector: 'app-rank',
            templateUrl: './rank.component.html',
            styleUrls: ['./rank.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService])
    ], RankComponent);
    return RankComponent;
}());
exports.RankComponent = RankComponent;
//# sourceMappingURL=rank.component.js.map
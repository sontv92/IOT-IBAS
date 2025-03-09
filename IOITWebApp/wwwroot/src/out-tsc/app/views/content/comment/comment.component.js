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
var common_service_1 = require("../../../service/common.service");
var CommentComponent = /** @class */ (function () {
    function CommentComponent(http, modalDialogService, viewRef, toastr, common) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.common = common;
        this.listComment = [];
        this.typeComment = [];
        this.listProduct = [];
        this.listNews = [];
        this.showSort = [true, false];
        this.item = new model_1.Comment();
        this.paging = {
            page: 1,
            page_size: 10,
            query: '1=1',
            order_by: '',
            item_count: 0
        };
        this.q = {
            txtSearch: '',
            TargetType: undefined,
            TargetProductId: undefined,
            TargetNewsId: undefined
        };
        this.typeComment = [
            { TargetType: 1, Name: "Sản phẩm" },
            { TargetType: 2, Name: "Tin tức" }
        ];
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    CommentComponent.prototype.ngOnInit = function () {
        this.GetListComment();
    };
    CommentComponent.prototype.Sort = function (IsK, s, i) {
        if (IsK) {
            this.paging.order_by = s + " asc";
        }
        else {
            this.paging.order_by = s + " desc";
        }
        this.GetListComment();
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
    CommentComponent.prototype.GetListComment = function () {
        var _this = this;
        this.http.get('/api/comment/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listComment = res["data"];
                _this.paging.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    CommentComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListComment();
    };
    //Thông báo
    CommentComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    CommentComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    CommentComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    CommentComponent.prototype.QueryChanged = function (flag) {
        var query = '';
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            if (query != '') {
                query += ' and Contents.Contains("' + this.q.txtSearch + '")';
            }
            else {
                query += 'Contents.Contains("' + this.q.txtSearch + '")';
            }
        }
        if (this.q.TargetType != undefined && this.q.TargetType != '') {
            if (this.q.TargetType == 1 && flag) {
                this.GetListProduct();
            }
            else if (this.q.TargetType == 2 && flag) {
                this.GetListNews();
            }
            if (query != '') {
                query += ' and TargetType = ' + this.q.TargetType;
            }
            else {
                query += 'TargetType = ' + this.q.TargetType;
            }
        }
        if (this.q.TargetProductId != undefined && this.q.TargetProductId != '' && this.q.TargetType == 1) {
            if (query != '') {
                query += ' and TargetId = ' + this.q.TargetProductId;
            }
            else {
                query += 'TargetId = ' + this.q.TargetProductId;
            }
        }
        if (this.q.TargetNewsId != undefined && this.q.TargetNewsId != '' && this.q.TargetType == 2) {
            if (query != '') {
                query += ' and TargetId = ' + this.q.TargetNewsId;
            }
            else {
                query += 'TargetId = ' + this.q.TargetNewsId;
            }
        }
        if (query == '')
            this.paging.query = '1=1';
        else
            this.paging.query = query;
        this.GetListComment();
    };
    CommentComponent.prototype.Expand = function (i) {
        this.listComment[i].Expand = this.listComment[i].Expand ? false : true;
    };
    //Popup xác nhận xóa
    CommentComponent.prototype.ShowConfirmDelete = function (Id) {
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
                        _this.DeleteComment(Id);
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-default',
                }
            ],
        });
    };
    CommentComponent.prototype.DeleteComment = function (Id) {
        var _this = this;
        this.http.delete('/api/comment/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListComment();
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
    CommentComponent.prototype.ShowHide = function (id, i, idx) {
        var _this = this;
        var stt = idx == undefined ? (this.listComment[i].IsShow ? 1 : 10) : (this.listComment[i].listChildComment[idx].IsShow ? 1 : 10);
        this.http.put('/api/comment/ShowHide/' + id + "/" + stt, undefined, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.toastSuccess("Thay đổi trạng thái bình luận thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                if (idx == undefined)
                    _this.listComment[i].IsShow = !_this.listComment[i].IsShow;
                else
                    _this.listComment[i].listChildComment[idx].IsShow = !_this.listComment[i].listChildComment[idx].IsShow;
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            if (idx == undefined)
                _this.listComment[i].IsShow = !_this.listComment[i].IsShow;
            else
                _this.listComment[i].listChildComment[idx].IsShow = !_this.listComment[i].listChildComment[idx].IsShow;
        });
    };
    CommentComponent.prototype.GetListProduct = function () {
        var _this = this;
        this.http.get('/api/product/GetByPage?page=1&query=1=1&order_by=&select=ProductId,Name', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listProduct = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    CommentComponent.prototype.GetListNews = function () {
        var _this = this;
        this.http.get('/api/news/GetByPage?page=1&query=1=1&order_by=&select=NewsId,Title', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listNews = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    __decorate([
        core_1.ViewChild('Modal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], CommentComponent.prototype, "modal", void 0);
    CommentComponent = __decorate([
        core_1.Component({
            selector: 'app-comment',
            templateUrl: './comment.component.html',
            styleUrls: ['./comment.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService,
            common_service_1.CommonService])
    ], CommentComponent);
    return CommentComponent;
}());
exports.CommentComponent = CommentComponent;
//# sourceMappingURL=comment.component.js.map
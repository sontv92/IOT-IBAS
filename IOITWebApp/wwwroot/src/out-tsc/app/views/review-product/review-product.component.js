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
var dt_1 = require("../../data/dt");
var const_1 = require("../../data/const");
var http_1 = require("@angular/common/http");
var ngx_toastr_1 = require("ngx-toastr");
var ReviewProductComponent = /** @class */ (function () {
    function ReviewProductComponent(http, toastr) {
        this.http = http;
        this.toastr = toastr;
        this.listProductReviews = [];
        this.ProductReviewStatus = const_1.ProductReviewStatus;
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "1=1";
        this.paging.order_by = "";
        this.paging.item_count = 0;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    ReviewProductComponent.prototype.ngOnInit = function () {
        this.GetListProductReviews();
    };
    ReviewProductComponent.prototype.GetListProductReviews = function () {
        var _this = this;
        this.http.get('/api/product/ProductReview/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listProductReviews = res["data"];
                _this.paging.item_count = res["metadata"].Sum;
                _this.total = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    ReviewProductComponent.prototype.SortTable = function (str) {
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
        this.GetListProductReviews();
    };
    ReviewProductComponent.prototype.GetClassSortTable = function (str) {
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
    ReviewProductComponent.prototype.ChangeStatusProductReview = function (ProductReviewId, Status) {
        var _this = this;
        this.http.put('/api/Product/ChangeStatusProductReview/' + ProductReviewId + "/" + Status, undefined, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.toastSuccess("Thay đổi trạng thái thành công!");
                _this.GetListProductReviews();
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                _this.GetListProductReviews();
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            _this.GetListProductReviews();
        });
    };
    //Chuyển trang
    ReviewProductComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListProductReviews();
    };
    //Toast cảnh báo
    ReviewProductComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    //Toast thành công
    ReviewProductComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    //Toast thành công
    ReviewProductComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    ReviewProductComponent.prototype.QueryChanged = function () {
        var query = '';
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            if (query != '') {
                query += ' and (Name.Contains("' + this.q.txtSearch + '") OR Email.Contains("' + this.q.txtSearch + '") OR ProductName.Contains("' + this.q.txtSearch + '"))';
            }
            else {
                query += '(Name.Contains("' + this.q.txtSearch + '") OR Email.Contains("' + this.q.txtSearch + '") OR ProductName.Contains("' + this.q.txtSearch + '"))';
            }
        }
        if (this.q["Type"] != undefined) {
            if (query != '') {
                query += ' and Status=' + this.q["Type"];
            }
            else {
                query += 'Status=' + this.q["Type"];
            }
        }
        if (query == '')
            this.paging.query = '1=1';
        else
            this.paging.query = query;
        this.GetListProductReviews();
    };
    ReviewProductComponent = __decorate([
        core_1.Component({
            selector: 'app-review-product',
            templateUrl: './review-product.component.html',
            styleUrls: ['./review-product.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient, ngx_toastr_1.ToastrService])
    ], ReviewProductComponent);
    return ReviewProductComponent;
}());
exports.ReviewProductComponent = ReviewProductComponent;
//# sourceMappingURL=review-product.component.js.map
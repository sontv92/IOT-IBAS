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
var const_1 = require("../../data/const");
var model_1 = require("../../data/model");
var ngx_toastr_1 = require("ngx-toastr");
var common_service_1 = require("../../service/common.service");
var common_1 = require("@angular/common");
var dt_1 = require("../../data/dt");
var ng_pick_datetime_1 = require("ng-pick-datetime");
var ng_pick_datetime_moment_1 = require("ng-pick-datetime-moment");
var tabs_1 = require("ngx-bootstrap/tabs");
exports.MY_CUSTOM_FORMATS = {
    parseInput: 'DD/MM/YYYY HH:mm',
    fullPickerInput: 'DD/MM/YYYY HH:mm',
    datePickerInput: 'DD/MM/YYYY',
    timePickerInput: ' HH:mm',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY'
};
var ProductComponent = /** @class */ (function () {
    function ProductComponent(http, modalDialogService, viewRef, toastr, common, datePipe) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.common = common;
        this.datePipe = datePipe;
        // public Tag: string;
        this.listProduct = [];
        this.listProductReview = [];
        this.listTrademark = [];
        this.listManufacture = [];
        this.listCateNews = [];
        this.listOrderByProduct = [];
        this.domainImage = const_1.domainImage;
        this.listSuggestProduct = [];
        this.ProductName = "";
        this.ProductReviewStatus = const_1.ProductReviewStatus;
        this.attribuites = [];
        this.PriceCurrencyMaskConfig = {
            align: "left",
            allowNegative: false,
            decimal: ".",
            precision: 0,
            prefix: "",
            suffix: " Vnđ",
            thousands: ","
        };
        this.StockQuantityMaskConfig = {
            align: "left",
            allowNegative: false,
            decimal: ".",
            precision: 0,
            prefix: "",
            suffix: "",
            thousands: ","
        };
        this.Item = new model_1.Product();
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "TypeProduct=1";
        this.paging.order_by = "ProductId Desc";
        this.paging.item_count = 0;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        this.pagingReview = new dt_1.Paging();
        this.pagingReview.page = 1;
        this.pagingReview.page_size = 10;
        this.pagingReview.query = "1=1";
        this.pagingReview.order_by = "";
        this.pagingReview.item_count = 0;
        this.qReview = new dt_1.QueryFilter();
        this.qReview.txtSearch = "";
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
        this.ItemAttribuite = new model_1.Attribute();
    }
    ProductComponent.prototype.ngOnInit = function () {
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        this.GetListProduct();
        this.GetListManufacture();
        this.GetListTrademark();
        this.GetAttribuites();
    };
    //Get danh sách sản phẩm
    ProductComponent.prototype.GetListProduct = function () {
        var _this = this;
        this.http.get('/api/product/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listProduct = res["data"];
                _this.listProduct.forEach(function (item) {
                    item.IsShow = item.Status == 1 ? true : false;
                });
                _this.paging.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Lấy toàn bộ danh sách sản phẩm
    ProductComponent.prototype.GetListAllProduct = function () {
        var _this = this;
        var query = "";
        if (this.Item.ProductId != undefined) {
            query = "TypeProduct=1 and ProductId !=" + this.Item.ProductId;
        }
        else {
            query = "TypeProduct=1";
        }
        this.http.get('/api/product/GetByPage?page=1&query=' + query + '&order_by=&select=ProductId,PriceSpecial,Name,Image', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listSuggestProduct = res["data"];
                _this.listSuggestProduct.forEach(function (item) {
                    item.Check = false;
                });
                if (_this.Item.ProductId != undefined) {
                    for (var i = 0; i < _this.listSuggestProduct.length; i++) {
                        for (var j = 0; j < _this.Item.listRelated.length; j++) {
                            if (_this.listSuggestProduct[i].ProductId == _this.Item.listRelated[j].TargetRelatedId) {
                                _this.listSuggestProduct[i].Check = true;
                                break;
                            }
                        }
                    }
                }
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Danh sách nhà sản xuất
    ProductComponent.prototype.GetListManufacture = function () {
        var _this = this;
        this.http.get('/api/manufacturer/GetByPage?page=1&query=TypeOriginId=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listManufacture = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Danh sách thương hiệu
    ProductComponent.prototype.GetListTrademark = function () {
        var _this = this;
        this.http.get('/api/manufacturer/GetByPage?page=1&query=TypeOriginId=2&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listTrademark = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    ProductComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListProduct();
    };
    //Toast cảnh báo
    ProductComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    //Toast thành công
    ProductComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    //Toast thành công
    ProductComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    ProductComponent.prototype.QueryChanged = function () {
        var query = 'TypeProduct=1';
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            if (query != '') {
                query += ' and Name.Contains("' + this.q.txtSearch + '")';
            }
            else {
                query += 'Name.Contains("' + this.q.txtSearch + '")';
            }
        }
        if (this.q["ManufacturerId"] != undefined) {
            if (query != '') {
                query += ' and ManufacturerId=' + this.q["ManufacturerId"];
            }
            else {
                query += 'ManufacturerId=' + this.q["ManufacturerId"];
            }
        }
        if (this.q["TrademarkId"] != undefined) {
            if (query != '') {
                query += ' and TrademarkId=' + this.q["TrademarkId"];
            }
            else {
                query += 'TrademarkId=' + this.q["TrademarkId"];
            }
        }
        if (query == '')
            this.paging.query = '1=1';
        else
            this.paging.query = query;
        this.GetListProduct();
    };
    //Mở modal thêm mới
    ProductComponent.prototype.OpenProductModal = function (item) {
        this.tabset.tabs[0].active = true;
        this.Item = new model_1.Product();
        this.file.nativeElement.value = "";
        this.progress = undefined;
        this.listCateNews = [];
        this.Item.listImage = [];
        if (item != undefined) {
            this.Item = JSON.parse(JSON.stringify(item));
            var listAttribute_1 = JSON.parse(JSON.stringify(item.listAttribute));
            var attribuites = JSON.parse(JSON.stringify(this.attribuites));
            attribuites.forEach(function (itemLoop) {
                for (var i = 0; i < listAttribute_1.length; i++) {
                    if (itemLoop.AttribuiteId == listAttribute_1[i].AttribuiteId) {
                        itemLoop.Value = listAttribute_1[i].Value;
                        break;
                    }
                }
            });
            this.Item.listAttribute = attribuites;
        }
        else {
            this.Item.listAttribute = JSON.parse(JSON.stringify(this.attribuites));
        }
        this.GetListAllProduct();
        this.GetListCateNews();
        this.ProductModal.show();
    };
    //Thêm mới danh mục trang
    ProductComponent.prototype.SaveProduct = function () {
        var _this = this;
        if (this.Item.Name == undefined || this.Item.Name == '') {
            this.toastWarning("Chưa nhập Tên sản phẩm!");
            return;
        }
        else if (this.Item.Name.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên sản phẩm!");
            return;
        }
        else if (this.Item.Url == undefined || this.Item.Url == '') {
            this.toastWarning("Chưa nhập Đường dẫn!");
            return;
        }
        else if (this.Item.Url.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập đường dẫn!");
            return;
        }
        else if (this.Item.StockQuantity == undefined) {
            this.toastWarning("Chưa nhập Số lượng sản phẩm!");
            return;
        }
        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
        this.Item.WebsiteId = parseInt(localStorage.getItem("websiteId"));
        if (typeof this.Item.DateStartActive === 'object' && this.Item.DateStartActive != undefined) {
            var DateStartActive = this.Item.DateStartActive.add(7, 'hours');
            this.Item.DateStartActive = DateStartActive.toISOString();
        }
        this.Item.listRelated = [];
        this.listSuggestProduct.forEach(function (item) {
            if (item.Check == true) {
                var obj = { TargetRelatedId: item.ProductId };
                _this.Item.listRelated.push(obj);
            }
        });
        if (this.Item.ProductId == undefined) {
            this.Item.listCategory = [];
            this.listCateNews.forEach(function (item) {
                if (item.Check) {
                    _this.Item.listCategory.push(item);
                }
            });
            this.Item.TypeProduct = 1;
            this.http.post('/api/Product', this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListProduct();
                    _this.ProductModal.hide();
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
            var arr_1 = [];
            this.Item.listCategory.forEach(function (item) {
                var flag = false;
                for (var i = 0; i < _this.listCateNews.length; i++) {
                    if (item.CategoryId == _this.listCateNews[i].CategoryId && _this.listCateNews[i].Check == true) {
                        flag = true;
                        break;
                    }
                }
                if (!flag) {
                    item.Check = false;
                    arr_1.push(item);
                }
            });
            this.Item.listCategory = arr_1.concat(this.listCateNews.filter(function (e) { return e.Check == true; }));
            this.http.put('/api/product/' + this.Item.ProductId, this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListProduct();
                    _this.ProductModal.hide();
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
    //Popup xác nhận xóa
    ProductComponent.prototype.ShowConfirmDelete = function (Id) {
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
    ProductComponent.prototype.Delete = function (Id) {
        var _this = this;
        this.http.delete('/api/Product/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListProduct();
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
    // check chữ
    ProductComponent.prototype.ChangeNameProduct = function (key) {
        switch (key) {
            case 1:
                this.Item.MetaTitle = this.Item.Name;
                this.Item.MetaKeyword = this.Item.Name;
                this.Item.Url = this.common.ConvertUrl(this.Item.Name);
                break;
            case 2:
                this.Item.MetaDescription = this.Item.Description;
                break;
            default:
                break;
        }
    };
    ProductComponent.prototype.GetListCateNews = function () {
        var _this = this;
        this.http.get('/api/category/GetByTree?arr=11', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listCateNews = res["data"];
                if (_this.Item.ProductId != undefined) {
                    for (var i = 0; i < _this.listCateNews.length; i++) {
                        for (var j = 0; j < _this.Item.listCategory.length; j++) {
                            if (_this.listCateNews[i].CategoryId == _this.Item.listCategory[j].CategoryId) {
                                _this.listCateNews[i].Check = true;
                                break;
                            }
                        }
                    }
                }
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
        // this.http.get('/api/category/GetByPage?page=1&query=TypeCategoryId=11', this.httpOptions).subscribe(
        // 	(res) => {
        // 		if (res["meta"]["error_code"] == 200) {
        // 			this.listCateNews = [];
        // 			if (res["data"].length > 0) {
        // 				res["data"].forEach(cate => {
        // 					this.listCateNews.push({ CategoryId: cate.CategoryId, Name: cate.Name, Check: false });
        // 				});
        // 				if (this.Item.ProductId != undefined) {
        // 					for (var i = 0; i < this.listCateNews.length; i++) {
        // 						for (var j = 0; j < this.Item.listCategory.length; j++) {
        // 							if (this.listCateNews[i].CategoryId == this.Item.listCategory[j].CategoryId) {
        // 								this.listCateNews[i].Check = true;
        // 								break;
        // 							}
        // 						}
        // 					}
        // 				}
        // 			}
        // 		}
        // 	},
        // 	(err) => {
        // 		console.log("Error: connect to API");
        // 	}
        // );
    };
    ProductComponent.prototype.upload = function (files) {
        var _this = this;
        if (files.length === 0)
            return;
        var formData = new FormData();
        for (var _i = 0, files_1 = files; _i < files_1.length; _i++) {
            var file = files_1[_i];
            formData.append(file.name, file);
        }
        var uploadReq = new http_1.HttpRequest('POST', 'api/upload/uploadImage/2', formData, {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            }),
            reportProgress: true,
        });
        this.http.request(uploadReq).subscribe(function (event) {
            if (event.type === http_1.HttpEventType.UploadProgress)
                _this.progress = Math.round(100 * event.loaded / event.total);
            else if (event.type === http_1.HttpEventType.Response) {
                event.body["data"].forEach(function (item) {
                    _this.ImageProduct = new model_1.ImageProduct();
                    _this.ImageProduct.Image = item;
                    _this.ImageProduct.IsImageMain = false;
                    _this.ImageProduct.Status = 1;
                    _this.Item.listImage.push(_this.ImageProduct);
                });
            }
        });
    };
    ProductComponent.prototype.RemoveImage = function (idx) {
        if (this.Item.listImage[idx].ProductImageId == undefined) {
            this.Item.listImage.splice(idx, 1);
        }
        else {
            this.Item.listImage[idx].Status = 99;
        }
    };
    ProductComponent.prototype.findTrademark = function (item) {
        if (item == undefined) {
            return "";
        }
        else {
            return item.Name;
        }
    };
    ProductComponent.prototype.ShowHide = function (id, i) {
        var _this = this;
        var stt = this.listProduct[i].IsShow ? 1 : 10;
        this.http.put('/api/Product/ShowHide/' + id + "/" + stt, undefined, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.toastSuccess("Thay đổi trạng thái thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                _this.listProduct[i].IsShow = !_this.listProduct[i].IsShow;
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            _this.listProduct[i].IsShow = !_this.listProduct[i].IsShow;
        });
    };
    ProductComponent.prototype.SortTable = function (str) {
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
        this.GetListProduct();
    };
    ProductComponent.prototype.GetClassSortTable = function (str) {
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
    ProductComponent.prototype.SetIsMain = function (idx) {
        for (var i = 0; i < this.Item.listImage.length; i++) {
            this.Item.listImage[i].IsImageMain = false;
            if (idx == i) {
                this.Item.listImage[i].IsImageMain = true;
            }
        }
    };
    ProductComponent.prototype.GetListOrderBy = function () {
        var _this = this;
        this.http.get('/api/orderby/GetOrderBy/10', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listOrderByProduct = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    ProductComponent.prototype.OpenOrderByModal = function () {
        this.listOrderByProduct = [];
        this.GetListOrderBy();
        this.OrderByModal.show();
    };
    ProductComponent.prototype.DeleteOrderBy = function (item) {
        for (var i = 0; i < this.listOrderByProduct.length; i++) {
            if (this.listOrderByProduct[i].CategoryMappingId == item.CategoryMappingId) {
                this.listOrderByProduct[i].Status = 99;
                break;
            }
        }
    };
    ProductComponent.prototype.SaveOrderBy = function () {
        var _this = this;
        this.http.post('/api/orderby', this.listOrderByProduct, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListProduct();
                _this.OrderByModal.hide();
                _this.toastSuccess("Lưu thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    //Product Review
    ProductComponent.prototype.GetListProductReviews = function () {
        var _this = this;
        this.http.get('/api/product/ProductReview/GetByPage?page=' + this.pagingReview.page + '&page_size=' + this.pagingReview.page_size + '&query=' + this.pagingReview.query + '&order_by=' + this.pagingReview.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listProductReview = res["data"];
                _this.pagingReview.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    ProductComponent.prototype.ProductReviewsModal = function (ProductId, Name) {
        this.ProductName = Name;
        this.ProductId = ProductId;
        this.pagingReview = new dt_1.Paging();
        this.pagingReview.page = 1;
        this.pagingReview.page_size = 10;
        this.pagingReview.query = "ProductId=" + ProductId;
        this.pagingReview.order_by = "";
        this.pagingReview.item_count = 0;
        this.qReview = new dt_1.QueryFilter();
        this.qReview.txtSearch = "";
        this.qReview.Type = undefined;
        this.GetListProductReviews();
        this.ProductReviewModal.show();
    };
    ProductComponent.prototype.PageChangedReview = function (event) {
        this.pagingReview.page = event.page;
        this.GetListProductReviews();
    };
    ProductComponent.prototype.QueryReviewChanged = function () {
        var query = 'ProductId=' + this.ProductId;
        if (this.qReview["Type"] != undefined) {
            if (query != '') {
                query += ' and Status=' + this.qReview["Type"];
            }
            else {
                query += 'Status=' + this.qReview["Type"];
            }
        }
        if (query == '')
            this.pagingReview.query = '1=1';
        else
            this.pagingReview.query = query;
        this.GetListProductReviews();
    };
    ProductComponent.prototype.ChangeStatusProductReview = function (ProductReviewId, Status) {
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
    ProductComponent.prototype.CheckCategory = function (CategoryId, curItem) {
        var Check = curItem["Check"];
        var CategoryParentId = curItem["CategoryParentId"];
        var CheckParent = false;
        this.listCateNews.forEach(function (item) {
            if (Check) {
                if (item.Genealogy.indexOf(CategoryId.toString()) != -1) {
                    item.Check = !Check;
                }
            }
            if (Check == false) {
                CheckParent = true;
            }
            else {
                if (item.CategoryParentId == CategoryParentId) {
                    if (item.Check == true) {
                        CheckParent = true;
                    }
                }
            }
        });
        if (CheckParent) {
            this.listCateNews.forEach(function (item) {
                if (item.CategoryId == CategoryParentId) {
                    item.Check = true;
                }
            });
        }
    };
    //Lấy ra danh sách thuộc tính
    ProductComponent.prototype.GetAttribuites = function () {
        var _this = this;
        this.http.get('/api/attribuite/GetByPage?page=1&query=1=1&order_by=Location Asc&select=AttribuiteId,Name,Location', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.attribuites = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    ProductComponent.prototype.OpenAttribuiteModal = function () {
        this.ItemAttribuite = new model_1.Attribute();
        this.ItemAttribuite.Status = 1;
        this.AttribuiteModal.show();
    };
    ProductComponent.prototype.SaveAttribuite = function () {
        if (this.ItemAttribuite.AttribuiteId == undefined) {
            this.toastWarning("Chưa chọn Thuộc tính!");
            return;
        }
        else if (this.ItemAttribuite.Value == undefined || this.ItemAttribuite.Value == '') {
            this.toastWarning("Chưa nhập Giá trị thuộc tính!");
            return;
        }
        else if (this.ItemAttribuite.Value.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập Giá trị thuộc tính!");
            return;
        }
        else if (this.ItemAttribuite.Location == undefined) {
            this.toastWarning("Chưa nhập Thứ tự hiển thị!");
            return;
        }
        if (this.Item.listAttribute == undefined) {
            this.Item.listAttribute = [];
        }
        this.Item.listAttribute.push(this.ItemAttribuite);
        this.ItemAttribuite = new model_1.Attribute();
        this.AttribuiteModal.hide();
    };
    ProductComponent.prototype.ShowConfirmDeleteAttribuite = function (i) {
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
                        _this.Item.listAttribute[i].Status = 99;
                        _this.viewRef.clear();
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
        core_1.ViewChild('ProductModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], ProductComponent.prototype, "ProductModal", void 0);
    __decorate([
        core_1.ViewChild('OrderByModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], ProductComponent.prototype, "OrderByModal", void 0);
    __decorate([
        core_1.ViewChild('ProductReviewModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], ProductComponent.prototype, "ProductReviewModal", void 0);
    __decorate([
        core_1.ViewChild('AttribuiteModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], ProductComponent.prototype, "AttribuiteModal", void 0);
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], ProductComponent.prototype, "file", void 0);
    __decorate([
        core_1.ViewChild('tabset'),
        __metadata("design:type", tabs_1.TabsetComponent)
    ], ProductComponent.prototype, "tabset", void 0);
    ProductComponent = __decorate([
        core_1.Component({
            selector: 'app-product',
            templateUrl: './product.component.html',
            styleUrls: ['./product.component.scss'],
            providers: [
                { provide: ng_pick_datetime_1.DateTimeAdapter, useClass: ng_pick_datetime_moment_1.MomentDateTimeAdapter, deps: [ng_pick_datetime_1.OWL_DATE_TIME_LOCALE] },
                { provide: ng_pick_datetime_1.OWL_DATE_TIME_FORMATS, useValue: exports.MY_CUSTOM_FORMATS }
            ]
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService,
            common_service_1.CommonService,
            common_1.DatePipe])
    ], ProductComponent);
    return ProductComponent;
}());
exports.ProductComponent = ProductComponent;
//# sourceMappingURL=product.component.js.map
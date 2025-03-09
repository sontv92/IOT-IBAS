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
var ngx_toastr_1 = require("ngx-toastr");
var model_1 = require("../../../data/model");
var common_service_1 = require("../../../service/common.service");
var call_category_function_service_1 = require("../../../service/call-category-function.service");
var router_1 = require("@angular/router");
var NewsComponent = /** @class */ (function () {
    function NewsComponent(http, modalDialogService, viewRef, toastr, common, callCategoryFunctionService, elm, router) {
        var _this = this;
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.common = common;
        this.callCategoryFunctionService = callCategoryFunctionService;
        this.elm = elm;
        this.router = router;
        this.listCateNews = [];
        this.listCateParent = [];
        this.listLanguage = [];
        this.listOrderByCat = [];
        this.typeCategoryNews = const_1.typeCategoryNews;
        this.domainImage = const_1.domainImage;
        this.query = "arr=1&arr=2&arr=3&arr=4&arr=5";
        this.key = 'categorySorts';
        this.Item = new model_1.Category();
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
        this.subscription = this.callCategoryFunctionService.getAction().subscribe(function (action) {
            if (action.TypeAction == 1) {
                _this.OpenCateNewsModal(undefined, action.CategoryId);
            }
            else if (action.TypeAction == 2) {
                _this.OpenCateNewsModal(action.CategoryId, undefined);
            }
            else if (action.TypeAction == 3) {
                _this.ShowConfirmDelete(action.CategoryId);
            }
        });
    }
    NewsComponent.prototype.ngOnInit = function () {
        this.GetListCateNews();
    };
    NewsComponent.prototype.ngOnDestroy = function () {
        this.subscription.unsubscribe();
        this.router.onSameUrlNavigation = 'ignore';
    };
    //Get danh sách tin
    NewsComponent.prototype.GetListCateNews = function () {
        var _this = this;
        this.listCateNews = [];
        this.http.get('/api/category/GetCategorySort?' + this.query, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listCateNews = res["data"];
                _this.total_item = res["metadata"];
                loadNestable();
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    NewsComponent.prototype.QueryChanged = function () {
        var query = "arr=1&arr=2&arr=3&arr=4&arr=5";
        if (this.txtSearch != undefined && this.txtSearch != "") {
            this.query = query + "&txtSearch=" + this.txtSearch;
        }
        else {
            this.query = query;
        }
        this.GetListCateNews();
    };
    // Get danh sách ngôn ngữ
    NewsComponent.prototype.GetListLanguage = function () {
        var _this = this;
        this.http.get('/api/Language/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listLanguage = res["data"];
                if (_this.listLanguage.length == 1 && (_this.Item.CategoryId == undefined || (_this.Item.CategoryId != undefined && _this.Item.LanguageId == undefined))) {
                    _this.Item.LanguageId = _this.listLanguage[0].LanguageId;
                }
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    NewsComponent.prototype.GetListOrderByCat = function () {
        var _this = this;
        this.http.get('api/category/listNews/' + this.Item.CategoryId, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listOrderByCat = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Open sắp xếp tin
    NewsComponent.prototype.OpenSortNewsModal = function (item) {
        this.Item = JSON.parse(JSON.stringify(item));
        this.listOrderByCat = [];
        this.GetListOrderByCat();
        this.SortNewsModal.show();
    };
    NewsComponent.prototype.SaveSortNews = function () {
        var _this = this;
        for (var i = this.listOrderByCat.length; i > 0; i--) {
            this.listOrderByCat[i - 1].Location = (this.listOrderByCat.length - i) + 1;
        }
        this.http.put('/api/category/sortCategoryMapping/' + this.Item.CategoryId, this.listOrderByCat, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.SortNewsModal.hide();
                _this.toastSuccess("Lưu thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    //Get danh sách danh mục cha
    NewsComponent.prototype.GetListCateParent = function (Id) {
        var _this = this;
        console.log(Id);
        this.http.get('/api/category/GetByTree?arr=1&arr=2&arr=3&arr=4&arr=5', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listCateParent = res["data"];
                _this.listCateParent.forEach(function (item) {
                    if (item.CategoryId == Id || item.Genealogy.indexOf(Id) != -1)
                        item.disabled = true;
                    item.Space = "";
                    for (var i = 0; i < (item.Level - 1) * 7; i++) {
                        item.Space += "&nbsp;";
                    }
                });
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Thông báo
    NewsComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    NewsComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    NewsComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //Mở modal thêm mới
    NewsComponent.prototype.OpenCateNewsModal = function (CategoryId, CategoryParentId) {
        var _this = this;
        this.Item = new model_1.Category();
        this.Item.CategoryParentId = CategoryParentId;
        this.file.nativeElement.value = "";
        this.fileIcon.nativeElement.value = "";
        this.message = undefined;
        this.messageIcon = undefined;
        this.progress = undefined;
        this.progressIcon = undefined;
        if (CategoryId != undefined) {
            this.http.get('/api/category/' + CategoryId, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.Item = Object.assign(_this.Item, res["data"]);
                    if (_this.Item.CategoryParentId == 0)
                        _this.Item.CategoryParentId = undefined;
                    _this.GetListCateParent(_this.Item.CategoryId);
                    _this.CateNewsModal.show();
                }
                else {
                    _this.toastError("Không tìm thấy danh mục trên hệ thống!");
                    return;
                }
            }, function (err) {
                _this.toastError("Không tìm thấy danh mục trên hệ thống!");
                return;
            });
        }
        else {
            this.GetListCateParent(undefined);
            this.CateNewsModal.show();
        }
        this.GetListLanguage();
    };
    //Thêm mới danh mục trang
    NewsComponent.prototype.SaveCateNews = function () {
        var _this = this;
        if (this.Item.Code == undefined || this.Item.Code == '') {
            this.toastWarning("Chưa nhập Mã danh mục!");
            return;
        }
        else if (this.Item.Code.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập mã danh mục");
            return;
        }
        else if (this.Item.Name == undefined || this.Item.Name == '') {
            this.toastWarning("Chưa nhập Tên danh mục!");
            return;
        }
        else if (this.Item.Name.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên danh mục");
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
        else if (this.Item.TypeCategoryId == undefined || this.Item.TypeCategoryId == 0) {
            this.toastWarning("Chưa chọn Loại danh mục!");
            return;
        }
        else if (this.Item.LanguageId == undefined) {
            this.toastWarning("Chưa chọn ngôn ngữ!");
            return;
        }
        this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        this.Item.WebsiteId = parseInt(localStorage.getItem("websiteId"));
        if (!this.Item.LanguageId) {
            this.Item.LanguageId = parseInt(localStorage.getItem("languageId"));
        }
        if (this.Item.CategoryId) {
            this.http.put('/api/Category/' + this.Item.CategoryId, this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.ResetCurrentRouter();
                    _this.CateNewsModal.hide();
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
            this.http.post('/api/Category', this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.ResetCurrentRouter();
                    _this.CateNewsModal.hide();
                    _this.toastSuccess("Thêm mới thành công!");
                }
                else if (res["meta"]["error_code"] == 213) {
                    _this.toastWarning("Tên đã tồn tại!");
                }
                else {
                    _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
    };
    NewsComponent.prototype.ChangeTitle = function (key) {
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
    //Popup xác nhận xóa
    NewsComponent.prototype.ShowConfirmDelete = function (Id) {
        var _this = this;
        this.modalDialogService.openDialog(this.viewRef, {
            title: 'Xác nhận',
            childComponent: ngx_modal_dialog_1.SimpleModalComponent,
            data: {
                text: "Bạn có chắc chắn muốn xóa danh mục này và các danh mục con của nó?"
            },
            actionButtons: [
                {
                    text: 'Đồng ý',
                    buttonClass: 'btn btn-success',
                    onAction: function () {
                        console.log('OnAction');
                        _this.DeleteCateNews(Id);
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-default',
                }
            ],
        });
    };
    NewsComponent.prototype.DeleteCateNews = function (Id) {
        var _this = this;
        this.http.delete('/api/Category/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.ResetCurrentRouter();
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
    NewsComponent.prototype.findParent = function (item) {
        if (item == undefined) {
            return "";
        }
        else {
            return item.Name;
        }
    };
    NewsComponent.prototype.upload = function (files, Type) {
        var _this = this;
        if (files.length === 0)
            return;
        var formData = new FormData();
        for (var _i = 0, files_1 = files; _i < files_1.length; _i++) {
            var file = files_1[_i];
            formData.append(file.name, file);
        }
        console.log(formData);
        var uploadReq = new http_1.HttpRequest('POST', 'api/upload/uploadImage/' + Type, formData, {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            }),
            reportProgress: true,
        });
        this.http.request(uploadReq).subscribe(function (event) {
            if (event.type === http_1.HttpEventType.UploadProgress) {
                if (Type == 5) {
                    _this.progress = Math.round(100 * event.loaded / event.total);
                }
                else {
                    _this.progressIcon = Math.round(100 * event.loaded / event.total);
                }
            }
            else if (event.type === http_1.HttpEventType.Response) {
                if (Type == 5) {
                    _this.message = event.body["data"].toString();
                    _this.Item.Image = _this.message;
                }
                else {
                    _this.messageIcon = event.body["data"].toString();
                    _this.Item.Icon = _this.messageIcon;
                }
            }
        });
    };
    NewsComponent.prototype.RemoveImage = function (Type) {
        if (Type == 5) {
            this.file.nativeElement.value = "";
            this.Item.Image = undefined;
            this.message = undefined;
            this.progress = undefined;
        }
        else {
            this.fileIcon.nativeElement.value = "";
            this.Item.Icon = undefined;
            this.messageIcon = undefined;
            this.progressIcon = undefined;
        }
    };
    NewsComponent.prototype.ShowHide = function (id, i) {
        var _this = this;
        var stt = this.listCateNews[i].IsShow ? 1 : 10;
        this.http.put('/api/Category/ShowHide/' + id + "/" + stt, undefined, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.toastSuccess("Thay đổi trạng thái thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                _this.listCateNews[i].IsShow = !_this.listCateNews[i].IsShow;
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            _this.listCateNews[i].IsShow = !_this.listCateNews[i].IsShow;
        });
    };
    NewsComponent.prototype.SaveSortCategory = function () {
        var _this = this;
        var attribute = document.getElementById("nestable");
        var Arr = [];
        this.common.ConvertHtmlToJson(Arr, attribute, "#nestable", 0, 1);
        this.http.post('/api/Category/SaveCategorySort', Arr, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.ResetCurrentRouter();
                _this.CateNewsModal.hide();
                _this.toastSuccess("Lưu thông tin sắp xếp thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    NewsComponent.prototype.ResetCurrentRouter = function () {
        this.router.routeReuseStrategy.shouldReuseRoute = function () {
            return false;
        };
        this.router.onSameUrlNavigation = 'reload';
        this.router.navigateByUrl(this.router.url);
    };
    __decorate([
        core_1.ViewChild('CateNewsModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], NewsComponent.prototype, "CateNewsModal", void 0);
    __decorate([
        core_1.ViewChild('SortNewsModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], NewsComponent.prototype, "SortNewsModal", void 0);
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], NewsComponent.prototype, "file", void 0);
    __decorate([
        core_1.ViewChild('fileIcon'),
        __metadata("design:type", core_1.ElementRef)
    ], NewsComponent.prototype, "fileIcon", void 0);
    NewsComponent = __decorate([
        core_1.Component({
            selector: 'app-news',
            templateUrl: './news.component.html',
            styleUrls: ['./news.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService,
            common_service_1.CommonService,
            call_category_function_service_1.CallCategoryFunctionService,
            core_1.ElementRef,
            router_1.Router])
    ], NewsComponent);
    return NewsComponent;
}());
exports.NewsComponent = NewsComponent;
//# sourceMappingURL=news.component.js.map
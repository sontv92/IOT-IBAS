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
var common_1 = require("@angular/common");
var common_service_1 = require("../../../service/common.service");
var dt_1 = require("../../../data/dt");
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
var NewsTextComponent = /** @class */ (function () {
    function NewsTextComponent(http, modalDialogService, viewRef, toastr, datePipe, common) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.datePipe = datePipe;
        this.common = common;
        this.listNews = [];
        this.listOrderByProduct = [];
        this.listNewsT = [];
        this.listCateNews = [];
        this.listSuggestNews = [];
        this.listTypeNews = const_1.typeCategoryNews;
        this.domainImage = const_1.domainImage;
        this.Item = new model_1.News();
        this.paging = new dt_1.Paging();
        this.website = new model_1.Website();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "IsService != true";
        this.paging.order_by = "NewsId Desc";
        this.paging.item_count = 0;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        // this.q = {
        //   cate: -1,
        //   type: -1,
        //   txtSearch: ''
        // }
        this.IsAll = false;
        this.CheckConfirmNews = this.common.CheckAccessKey(localStorage.getItem("access_key"), "DBV");
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    NewsTextComponent.prototype.ngOnInit = function () {
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        this.GetListNews();
        this.GetListCateNews();
    };
    //Lấy toàn bộ danh sách tin văn bản
    NewsTextComponent.prototype.GetListAllNews = function () {
        var _this = this;
        var query = "";
        if (this.Item.NewsId != undefined) {
            query = "IsService != true and TypeNewsId == 1 and NewsId !=" + this.Item.NewsId;
        }
        else {
            query = "IsService != true and TypeNewsId == 1";
        }
        this.http.get('/api/news/GetByPage?page=1&query=' + query + '&order_by=&select=NewsId,Title,Url,Image', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listSuggestNews = res["data"];
                _this.listSuggestNews.forEach(function (item) {
                    item.Check = false;
                });
                if (_this.Item.NewsId != undefined) {
                    for (var i = 0; i < _this.listSuggestNews.length; i++) {
                        for (var j = 0; j < _this.Item.listRelated.length; j++) {
                            if (_this.listSuggestNews[i].NewsId == _this.Item.listRelated[j].TargetRelatedId) {
                                _this.listSuggestNews[i].Check = true;
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
    //Get danh sách danh bài viết
    NewsTextComponent.prototype.GetListNews = function () {
        var _this = this;
        this.http.get('/api/news/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listNews = res["data"];
                _this.listNews.forEach(function (item) {
                    item.IsShow = item.Status == 1 ? true : false;
                });
                _this.paging.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    NewsTextComponent.prototype.GetListCateNews = function () {
        var _this = this;
        // this.Item.listCategory = [];
        console.log(this.Item.listCategory);
        var query = 'TypeCategoryId=1 OR TypeCategoryId=2 OR TypeCategoryId=3 OR TypeCategoryId=4 OR TypeCategoryId=5';
        if (!this.IsAll) {
            query = this.Item.TypeNewsId != undefined ? "TypeCategoryId=" + this.Item.TypeNewsId : "TypeCategoryId=-1";
        }
        this.http.get('/api/category/GetByPage?page=1&query=' + query, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listCateNews = [];
                if (res["data"].length > 0) {
                    res["data"].forEach(function (cate) {
                        _this.listCateNews.push({ CategoryId: cate.CategoryId, Name: cate.Name, Check: false });
                    });
                    if (_this.Item.NewsId != undefined) {
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
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    NewsTextComponent.prototype.GetListOrderBy = function () {
        var _this = this;
        this.http.get('/api/orderby/GetOrderBy/11', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listOrderByProduct = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    NewsTextComponent.prototype.OpenChooseHighlightsNews = function () {
        this.listOrderByProduct = [];
        this.GetListOrderBy();
        this.HighlightNewsModal.show();
    };
    NewsTextComponent.prototype.SaveHighlightNews = function () {
        var _this = this;
        this.http.post('/api/orderby', this.listOrderByProduct, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.HighlightNewsModal.hide();
                _this.toastSuccess("Lưu thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    NewsTextComponent.prototype.DeleteOrderBy = function (item) {
        for (var i = 0; i < this.listOrderByProduct.length; i++) {
            if (this.listOrderByProduct[i].CategoryMappingId == item.CategoryMappingId) {
                this.listOrderByProduct[i].Status = 99;
                break;
            }
        }
    };
    //Chuyển trang
    NewsTextComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListNews();
    };
    //Thông báo
    NewsTextComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    NewsTextComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    NewsTextComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    NewsTextComponent.prototype.QueryChanged = function () {
        var query = 'IsService != true';
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            if (query != '') {
                query += ' and Title.Contains("' + this.q.txtSearch + '")';
            }
            else {
                query += 'Title.Contains("' + this.q.txtSearch + '")';
            }
        }
        if (this.q["Type"] != undefined) {
            if (query != '') {
                query += ' and TypeNewsId=' + this.q["Type"];
            }
            else {
                query += 'TypeNewsId=' + this.q["Type"];
            }
        }
        if (query == '')
            this.paging.query = '1=1';
        else
            this.paging.query = query;
        this.GetListNews();
    };
    //Mở modal thêm mới
    NewsTextComponent.prototype.OpenNewsModal = function (item) {
        //this.tabset.tabs[0].active = true;
        this.Item = new model_1.News();
        this.Item.listCategory = [];
        this.Item.listTag = [];
        this.Item.listAttachment = [];
        this.Tag = "";
        this.IsAll = true;
        if (this.file)
            this.file.nativeElement.value = "";
        this.message = undefined;
        this.progress = undefined;
        this.progressAttachment = undefined;
        this.GetListAllNews();
        this.GetListCateNews();
        this.Item.TypeNewsId = 1;
        this.CheckBoxStatus = true;
        if (item != undefined) {
            // this.Item = Object.assign(this.Item, item);
            this.Item = JSON.parse(JSON.stringify(item));
            this.CheckBoxStatus = this.Item.Status == 1 ? true : false;
        }
        this.NewsModal.show();
    };
    //Thêm mới danh mục trang
    NewsTextComponent.prototype.SaveNews = function () {
        var _this = this;
        if (this.Item.TypeNewsId == undefined) {
            this.toastWarning("Chưa chọn Loại tin!");
            return;
        }
        else if (this.Item.Title == undefined || this.Item.Title == '') {
            this.toastWarning("Chưa nhập Tiêu đề!");
            return;
        }
        else if (this.Item.Title.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tiêu đề!");
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
        else if (this.Item.TypeNewsId != 3 && this.Item.TypeNewsId != 4 && (this.Item.Contents == undefined || this.Item.Contents == '')) {
            this.toastWarning("Chưa nhập Nội dung!");
            return;
        }
        this.Item.Status = this.CheckBoxStatus ? 1 : 10;
        this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        if (typeof this.Item.DateStartActive === 'object' && this.Item.DateStartActive != undefined) {
            var DateStartActive = this.Item.DateStartActive.add(7, 'hours');
            this.Item.DateStartActive = DateStartActive.toISOString();
        }
        if (typeof this.Item.DateStartOn === 'object' && this.Item.DateStartOn != undefined) {
            var DateStartOn = this.Item.DateStartOn.add(7, 'hours');
            this.Item.DateStartOn = DateStartOn.toISOString();
        }
        if (typeof this.Item.DateEndOn === 'object' && this.Item.DateEndOn != undefined) {
            var DateEndOn = this.Item.DateEndOn.add(7, 'hours');
            this.Item.DateEndOn = DateEndOn.toISOString();
        }
        var obj = Object.assign({}, this.Item);
        obj.listRelated = [];
        this.listSuggestNews.forEach(function (item) {
            if (item.Check == true) {
                var it_1 = { TargetRelatedId: item.NewsId };
                obj.listRelated.push(it_1);
            }
        });
        if (this.Item.NewsId == undefined) {
            //this.Item.listCategory = [];
            //this.listCateNews.forEach(item => {
            //  if (item.Check) {
            //    this.Item.listCategory.push(item);
            //  }
            //});
            obj.listCategory = this.listNewsT;
            this.http.post('/api/news', obj, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListNews();
                    _this.NewsModal.hide();
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
            obj.listCategory.forEach(function (item) {
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
            obj.listCategory = arr_1.concat(this.listCateNews.filter(function (e) { return e.Check == true; }));
            this.http.put('/api/news/' + obj.NewsId, obj, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListNews();
                    _this.NewsModal.hide();
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
    NewsTextComponent.prototype.ToggleCateToList = function (id) {
        console.log(this.listCateNews);
        this.listNewsT = [];
        //if (this.Item.listCategory.includes(id)) {
        //  let index = this.Item.listCategory.indexOf(id, 0);
        //  this.Item.listCategory.splice(index, 1);
        //}
        //else
        //  this.Item.listCategory.push(id);
        for (var i = 0; i < this.listCateNews.length; i++) {
            if (this.listCateNews[i].Check == true) {
                this.objNew = this.listCateNews[i];
                this.listNewsT.push(this.objNew);
            }
        }
        console.log('---');
        //console.log(this.listNewsT);
    };
    NewsTextComponent.prototype.AddTag = function () {
        if (this.Tag != undefined && this.Tag != '') {
            this.Item.listTag.push({ TagId: null, Name: this.Tag, Check: false });
            this.Tag = '';
        }
    };
    NewsTextComponent.prototype.RemoveTag = function (i) {
        if (this.Item.NewsId == undefined) {
            this.Item.listTag.splice(i, 1);
        }
        else {
            if (this.Item.listTag[i].TagId != null) {
                this.Item.listTag[i].Check = false;
            }
            else {
                this.Item.listTag.splice(i, 1);
            }
        }
    };
    NewsTextComponent.prototype.ChangeTitle = function (key) {
        switch (key) {
            case 1:
                this.Item.MetaTitle = this.Item.Title;
                this.Item.MetaKeyword = this.Item.Title;
                this.Item.Url = this.common.ConvertUrl(this.Item.Title);
                break;
            case 2:
                this.Item.MetaDescription = this.Item.Description;
                break;
            default:
                break;
        }
    };
    //Popup xác nhận xóa
    NewsTextComponent.prototype.ShowConfirmDelete = function (Id) {
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
                        _this.DeleteNews(Id);
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-default',
                }
            ],
        });
    };
    NewsTextComponent.prototype.DeleteNews = function (Id) {
        var _this = this;
        this.http.delete('/api/news/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListNews();
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
    NewsTextComponent.prototype.upload = function (files, cs) {
        var _this = this;
        if (files.length === 0)
            return;
        var formData = new FormData();
        for (var _i = 0, files_1 = files; _i < files_1.length; _i++) {
            var file = files_1[_i];
            formData.append(file.name, file);
        }
        var uploadReq = new http_1.HttpRequest('POST', 'api/upload/uploadImage/1', formData, {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            }),
            reportProgress: true,
        });
        this.http.request(uploadReq).subscribe(function (event) {
            if (event.type === http_1.HttpEventType.UploadProgress)
                switch (cs) {
                    case 1:
                        _this.progress = Math.round(100 * event.loaded / event.total);
                        break;
                    case 2:
                        _this.progressAttachment = Math.round(100 * event.loaded / event.total);
                        _this.attachment.nativeElement.value = "";
                        break;
                    default:
                        break;
                }
            else if (event.type === http_1.HttpEventType.Response) {
                switch (cs) {
                    case 1:
                        _this.message = event.body["data"].toString();
                        _this.Item.Image = _this.message;
                        break;
                    case 2:
                        _this.attachment.nativeElement.value = "";
                        event.body["data"].forEach(function (item) {
                            var attachment = new model_1.Attactment();
                            attachment.Url = item;
                            attachment.IsImageMain = false;
                            attachment.Status = 1;
                            _this.Item.listAttachment.push(attachment);
                        });
                        break;
                    default:
                        break;
                }
            }
        });
    };
    NewsTextComponent.prototype.findAuthor = function (item) {
        if (item == undefined) {
            return "";
        }
        else {
            return item.FullName;
        }
    };
    NewsTextComponent.prototype.RemoveImage = function () {
        this.Item.Image = undefined;
        this.file.nativeElement.value = "";
        this.message = undefined;
        this.progress = undefined;
    };
    NewsTextComponent.prototype.ShowHide = function (id, i) {
        var _this = this;
        var stt = this.listNews[i].IsShow ? 1 : 10;
        this.http.put('/api/news/ShowHide/' + id + "/" + stt, undefined, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.toastSuccess("Thay đổi trạng thái thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                _this.listNews[i].IsShow = !_this.listNews[i].IsShow;
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            _this.listNews[i].IsShow = !_this.listNews[i].IsShow;
        });
    };
    NewsTextComponent.prototype.SortTable = function (str) {
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
        this.GetListNews();
    };
    NewsTextComponent.prototype.GetClassSortTable = function (str) {
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
    NewsTextComponent.prototype.RemoveAttachment = function (idx) {
        if (this.Item.listAttachment[idx].AttactmentId == undefined) {
            this.Item.listAttachment.splice(idx, 1);
        }
        else {
            this.Item.listAttachment[idx].Status = 99;
        }
    };
    NewsTextComponent.prototype.SetIsMain = function (idx) {
        for (var i = 0; i < this.Item.listAttachment.length; i++) {
            this.Item.listAttachment[i].IsImageMain = false;
            if (idx == i) {
                this.Item.listAttachment[i].IsImageMain = true;
            }
        }
    };
    __decorate([
        core_1.ViewChild('NewsModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], NewsTextComponent.prototype, "NewsModal", void 0);
    __decorate([
        core_1.ViewChild('HighlightNewsModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], NewsTextComponent.prototype, "HighlightNewsModal", void 0);
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], NewsTextComponent.prototype, "file", void 0);
    __decorate([
        core_1.ViewChild('attachment'),
        __metadata("design:type", core_1.ElementRef)
    ], NewsTextComponent.prototype, "attachment", void 0);
    __decorate([
        core_1.ViewChild('tabset'),
        __metadata("design:type", tabs_1.TabsetComponent)
    ], NewsTextComponent.prototype, "tabset", void 0);
    NewsTextComponent = __decorate([
        core_1.Component({
            selector: 'app-news-text',
            templateUrl: './news-text.component.html',
            styleUrls: ['./news-text.component.scss'],
            providers: [
                { provide: ng_pick_datetime_1.DateTimeAdapter, useClass: ng_pick_datetime_moment_1.MomentDateTimeAdapter, deps: [ng_pick_datetime_1.OWL_DATE_TIME_LOCALE] },
                { provide: ng_pick_datetime_1.OWL_DATE_TIME_FORMATS, useValue: exports.MY_CUSTOM_FORMATS }
            ]
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService,
            common_1.DatePipe,
            common_service_1.CommonService])
    ], NewsTextComponent);
    return NewsTextComponent;
}());
exports.NewsTextComponent = NewsTextComponent;
//# sourceMappingURL=news-text.component.js.map
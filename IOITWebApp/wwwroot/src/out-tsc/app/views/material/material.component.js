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
var MaterialComponent = /** @class */ (function () {
    function MaterialComponent(http, modalDialogService, viewRef, toastr, common) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.common = common;
        this.showSort = [true, false, false];
        this.showRemove = false;
        this.listMaterial = [];
        this.listCategorys = [];
        this.listProvince = [];
        this.listYear = [];
        this.listMonth = [];
        this.listTypeNews = const_1.typeCategoryNews;
        this.domainImage = const_1.domainImage;
        this.newItem = new model_1.Material();
        this.editItem = new model_1.Material();
        this.paging = {
            page: 1,
            page_size: 10,
            query: '1=1',
            order_by: '',
            item_count: 0
        };
        this.q = {
            cate: -1,
            type: -1,
            txtSearch: ''
        };
        this.IsAll = true;
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    MaterialComponent.prototype.ngOnInit = function () {
        this.GetListMaterial();
        this.GetListProvince();
        this.GetListYear();
        this.GetListMonth();
    };
    //Get danh sách danh mục vật liệu
    MaterialComponent.prototype.GetListMaterial = function () {
        var _this = this;
        this.http.get('/api/material/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listMaterial = res["data"];
                _this.paging.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    MaterialComponent.prototype.GetListProvince = function () {
        var _this = this;
        this.http.get('/api/province/GetByPage?page=1&page_size=100&query=1=1&order_by=ProvinceId asc', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listProvince = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    MaterialComponent.prototype.GetListYear = function () {
        var dateNow = new Date();
        for (var i = dateNow.getFullYear(); i >= dateNow.getFullYear() - 10; i--) {
            this.listYear.push({ YearId: i, Name: i });
        }
    };
    MaterialComponent.prototype.GetListMonth = function () {
        var dateNow = new Date();
        for (var i = 1; i <= 12; i++) {
            this.listMonth.push({ MonthId: i, Name: 'Thàng ' + i });
        }
    };
    //Chuyển trang
    MaterialComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListMaterial();
    };
    //Thông báo
    MaterialComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    MaterialComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    MaterialComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    MaterialComponent.prototype.QueryChanged = function () {
        var query = '';
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            if (query != '') {
                query += ' and Title.Contains("' + this.q.txtSearch + '")';
            }
            else {
                query += 'Title.Contains("' + this.q.txtSearch + '")';
            }
        }
        if (query == '')
            this.paging.query = '1=1';
        else
            this.paging.query = query;
        this.GetListMaterial();
    };
    MaterialComponent.prototype.Filter = function () {
        var query = "1=1";
        if (query == 'TypeNewsId=null' || query == 'CategoryId=null')
            this.paging.query = '1=1';
        else
            this.paging.query = query;
        this.GetListMaterial();
    };
    //Mở modal thêm mới
    MaterialComponent.prototype.OpenAddModal = function () {
        this.newItem = new model_1.Material();
        this.newItem.listMaterialImportExcelChild = [];
        this.file.nativeElement.value = "";
        //this.message = undefined;
        this.newItem.Note = "";
        this.addModal.show();
    };
    //Thêm mới danh mục vật liệu
    MaterialComponent.prototype.AddNew = function () {
        var _this = this;
        if (this.newItem.ProvinceId == undefined) {
            this.toastWarning("Chưa chọn Tỉnh/Thành phố!");
            return;
        }
        if (this.newItem.Year == undefined) {
            this.toastWarning("Chưa chọn năm!");
            return;
        }
        if (this.newItem.Month == undefined) {
            this.toastWarning("Chưa nhập tháng!");
            return;
        }
        if (this.newItem.listMaterialImportExcelChild.length <= 0) {
            this.toastWarning("Chưa nhập danh sách danh mục vật liệu xây dựng!");
            return;
        }
        this.newItem.Status = 1;
        this.newItem.UserId = parseInt(localStorage.getItem("userId"));
        this.http.post('/api/material', this.newItem, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListMaterial();
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
    //ToggleCateToList(id) {
    //  //console.log(this.listCateNews);
    //  //return;
    //  //if (this.newItem.listCategory.includes(id)) {
    //  //  let index = this.newItem.listCategory.indexOf(id, 0);
    //  //  this.newItem.listCategory.splice(index, 1);
    //  //}
    //  //else
    //  //  this.newItem.listCategory.push(id);
    //}
    //AddTag(IsNew) {
    //  if (this.Tag != undefined && this.Tag != '') {
    //    if (IsNew) {
    //      this.newItem.listTag.push({ TagId: null, Name: this.Tag, Check: false });
    //    }
    //    else {
    //      this.editItem.listTag.push({ TagId: null, Name: this.Tag, Check: false });
    //    }
    //    this.Tag = '';
    //  }
    //}
    //RemoveTag(i, IsNew) {
    //  //let index = this.newItem.listTag.indexOf(tag, 0);
    //  if (IsNew) {
    //    this.newItem.listTag.splice(i, 1);
    //  }
    //  else {
    //    if (this.editItem.listTag[i].TagId != null) {
    //      this.editItem.listTag[i].Check = false;
    //    }
    //    else {
    //      this.editItem.listTag.splice(i, 1);
    //    }
    //  }
    //}
    //ChangeTitle(IsNew) {
    //  if (IsNew) {
    //    this.newItem.MetaTitle = this.newItem.Title;
    //    this.newItem.MetaKeyword = this.newItem.Title;
    //    this.newItem.MetaDescription = this.newItem.Description;
    //    //let str = this.newItem.Title;
    //    this.newItem.Url = this.common.ConvertUrl(this.newItem.Title);
    //  }
    //  else {
    //    this.editItem.MetaTitle = this.editItem.Title;
    //    this.editItem.MetaKeyword = this.editItem.Title;
    //    this.editItem.MetaDescription = this.editItem.Description;
    //    this.editItem.Url = this.common.ConvertUrl(this.editItem.Title);
    //  }
    //}
    //OpenEditModal(item) {
    //  this.editItem = new News();
    //  this.IsAll = true;
    //  this.Tag = '';
    //  this.editItem = Object.assign(this.editItem, item);
    //  this.editItem.Contents = item.Contents;
    //  this.fileE.nativeElement.value = "";
    //  this.editItem.DateStartActive = this.datePipe.transform(item.DateStartActive, "yyyy-MM-dd");
    //  this.editItem.DateStartOn = this.datePipe.transform(item.DateStartOn, "yyyy-MM-dd");
    //  this.editItem.DateEndOn = this.datePipe.transform(item.DateEndOn, "yyyy-MM-dd");
    //  this.CheckBoxStatus = this.editItem.Status == 1 ? true : false;
    //  this.message = this.editItem.Image;
    //  if (this.editItem.Image != undefined) { this.showRemove = true }
    //  this.GetListCateNews(false);
    //  this.editModal.show();
    //}
    //EditNewsFunc() {
    //  if (this.editItem.Title == undefined || this.editItem.Title == '') {
    //    this.toastWarning("Chưa nhập Tiêu đề!");
    //    return;
    //  } else if (this.editItem.Url == undefined || this.editItem.Url == '') {
    //    this.toastWarning("Chưa nhập Đường dẫn!");
    //    return;
    //  } else if (this.editItem.Contents == undefined || this.editItem.Contents == '') {
    //    this.toastWarning("Chưa nhập Nội dung!");
    //    return;
    //  } else if (this.editItem.TypeNewsId == undefined) {
    //    this.toastWarning("Chưa chọn Loại tin!");
    //    return;
    //  }
    //  this.editItem.UserId = userInfo.userId;
    //  this.editItem.Status = this.CheckBoxStatus ? 1 : 10;
    //  let arr = [];
    //  this.editItem.listCategory.forEach(item => {
    //    var flag = false;
    //    for (var i = 0; i < this.listCateNews.length; i++) {
    //      if (item.CategoryId == this.listCateNews[i].CategoryId && this.listCateNews[i].Check == true) {
    //        flag = true;
    //        break;
    //      }
    //    }
    //    if (!flag) {
    //      item.Check = false;
    //      arr.push(item);
    //    }
    //  });
    //  this.editItem.listCategory = arr.concat(this.listCateNews.filter(e => e.Check == true));
    //  this.http.put('/api/news/' + this.editItem.NewsId, this.editItem, httpOptions).subscribe(
    //    (res) => {
    //      if (res["meta"]["error_code"] == 200) {
    //        this.GetListNews();
    //        this.editModal.hide();
    //        this.toastSuccess("Cập nhật thành công!");
    //      }
    //      else {
    //        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
    //      }
    //    },
    //    (err) => {
    //      this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
    //    }
    //  );
    //}
    ////Popup xác nhận xóa
    //ShowConfirmDelete(Id) {
    //  this.modalDialogService.openDialog(this.viewRef, {
    //    title: 'Xác nhận',
    //    childComponent: SimpleModalComponent,
    //    data: {
    //      text: "Bạn có chắc chắn muốn xóa bản ghi này?"
    //    },
    //    actionButtons: [
    //      {
    //        text: 'Đồng ý',
    //        buttonClass: 'btn btn-success',
    //        onAction: () => {
    //          this.DeleteNews(Id);
    //        }
    //      },
    //      {
    //        text: 'Đóng',
    //        buttonClass: 'btn btn-default',
    //      }
    //    ],
    //  });
    //}
    //DeleteNews(Id) {
    //  this.http.delete('/api/news/' + Id, httpOptions).subscribe(
    //    (res) => {
    //      if (res["meta"]["error_code"] == 200) {
    //        this.GetListNews();
    //        this.viewRef.clear();
    //        this.toastSuccess("Xóa thành công!");
    //      }
    //      else {
    //        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
    //      }
    //    },
    //    (err) => {
    //      this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
    //    }
    //  );
    //}
    MaterialComponent.prototype.upload = function (files, IsNew) {
        var _this = this;
        if (files.length === 0)
            return;
        var formData = new FormData();
        for (var _i = 0, files_1 = files; _i < files_1.length; _i++) {
            var file = files_1[_i];
            formData.append(file.name, file);
        }
        var uploadReq = new http_1.HttpRequest('POST', 'api/material/importMaterialExcel', formData, {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            }),
            reportProgress: true,
        });
        this.http.request(uploadReq).subscribe(function (event) {
            if (event.type === http_1.HttpEventType.UploadProgress)
                _this.progress = Math.round(100 * event.loaded / event.total);
            else if (event.type === http_1.HttpEventType.Response) {
                //this.message = event.body["data"].toString();
                if (IsNew) {
                    _this.newItem.listMaterialImportExcelChild = event.body["data"];
                    //this.showRemove = true;
                }
                //else {
                //  this.editItem.Image = this.message;
                //  this.showRemove = true;
                //}
            }
        });
    };
    MaterialComponent.prototype.findName = function (item) {
        if (item == undefined) {
            return "";
        }
        else {
            return item.Name;
        }
    };
    //RemoveImage(IsNew) {
    //  if (IsNew) {
    //    this.file.nativeElement.value = "";
    //    this.message = undefined;
    //    this.showRemove = false;
    //  }
    //  else {
    //    this.fileE.nativeElement.value = "";
    //    this.message = undefined;
    //    this.showRemove = false;
    //  }
    //}
    MaterialComponent.prototype.Sort = function (Is, Target, Index) {
    };
    __decorate([
        core_1.ViewChild('AddModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], MaterialComponent.prototype, "addModal", void 0);
    __decorate([
        core_1.ViewChild('EditModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], MaterialComponent.prototype, "editModal", void 0);
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], MaterialComponent.prototype, "file", void 0);
    __decorate([
        core_1.ViewChild('fileE'),
        __metadata("design:type", core_1.ElementRef)
    ], MaterialComponent.prototype, "fileE", void 0);
    MaterialComponent = __decorate([
        core_1.Component({
            selector: 'app-material',
            templateUrl: './material.component.html',
            styleUrls: ['./material.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService,
            common_service_1.CommonService])
    ], MaterialComponent);
    return MaterialComponent;
}());
exports.MaterialComponent = MaterialComponent;
//# sourceMappingURL=material.component.js.map
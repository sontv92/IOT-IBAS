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
var dt_1 = require("../../../data/dt");
var ngx_toastr_1 = require("ngx-toastr");
var BlockComponent = /** @class */ (function () {
    function BlockComponent(http, modalDialogService, viewRef, toastr) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.listBlock = [];
        this.listLanguage = [];
        this.domainImage = const_1.domainImage;
        this.Item = new model_1.Block();
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "1=1";
        this.paging.order_by = "BlockId Desc";
        this.paging.item_count = 0;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    BlockComponent.prototype.ngOnInit = function () {
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        this.GetListBlock();
    };
    //Get block
    BlockComponent.prototype.GetListBlock = function () {
        var _this = this;
        this.http.get('/api/block/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listBlock = res["data"];
                _this.paging.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    // Get danh sách ngôn ngữ
    BlockComponent.prototype.GetListLanguage = function () {
        var _this = this;
        this.http.get('/api/Language/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listLanguage = res["data"];
                if (_this.listLanguage.length == 1 && (_this.Item.BlockId == undefined || (_this.Item.BlockId != undefined && _this.Item.LanguageId == undefined))) {
                    _this.Item.LanguageId = _this.listLanguage[0].LanguageId;
                }
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    BlockComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListBlock();
    };
    //Toast cảnh báo
    BlockComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    //Toast thành công
    BlockComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    //Toast thành công
    BlockComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    BlockComponent.prototype.QueryChanged = function () {
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
        this.GetListBlock();
    };
    //Mở modal thêm mới
    BlockComponent.prototype.OpenBlockModal = function (item) {
        this.Item = new model_1.Block();
        this.message = undefined;
        this.progress = undefined;
        this.file.nativeElement.value = "";
        if (item != undefined) {
            this.Item = Object.assign(this.Item, item);
        }
        this.GetListLanguage();
        this.BlockModal.show();
    };
    //Thêm mới danh mục trang
    BlockComponent.prototype.SaveBlock = function () {
        var _this = this;
        if (this.Item.Code == undefined || this.Item.Code == '') {
            this.toastWarning("Chưa nhập Mã!");
            return;
        }
        else if (this.Item.Code.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập mã");
            return;
        }
        else if (this.Item.Name == undefined || this.Item.Name == '') {
            this.toastWarning("Chưa nhập Tên!");
            return;
        }
        else if (this.Item.Name.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên!");
            return;
        }
        else if (this.Item.LanguageId == undefined) {
            this.toastWarning("Chưa chọn ngôn ngữ!");
            return;
        }
        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
        this.Item.WebsiteId = parseInt(localStorage.getItem("websiteId"));
        if (!this.Item.LanguageId) {
            this.Item.LanguageId = parseInt(localStorage.getItem("languageId"));
        }
        if (this.Item.BlockId != undefined) {
            this.http.put('/api/Block/' + this.Item.BlockId, this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListBlock();
                    _this.BlockModal.hide();
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
            this.http.post('/api/Block', this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListBlock();
                    _this.BlockModal.hide();
                    _this.toastSuccess("Thêm mới thành công!");
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
    BlockComponent.prototype.ShowConfirmDelete = function (Id) {
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
                        console.log('OnAction');
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
    BlockComponent.prototype.Delete = function (Id) {
        var _this = this;
        this.http.delete('/api/Block/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListBlock();
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
    BlockComponent.prototype.upload = function (files) {
        var _this = this;
        if (files.length === 0)
            return;
        var formData = new FormData();
        for (var _i = 0, files_1 = files; _i < files_1.length; _i++) {
            var file = files_1[_i];
            formData.append(file.name, file);
        }
        var uploadReq = new http_1.HttpRequest('POST', 'api/upload/uploadImage/4', formData, {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            }),
            reportProgress: true,
        });
        this.http.request(uploadReq).subscribe(function (event) {
            if (event.type === http_1.HttpEventType.UploadProgress) {
                _this.progress = Math.round(100 * event.loaded / event.total);
            }
            else if (event.type === http_1.HttpEventType.Response) {
                _this.message = event.body["data"].toString();
                _this.Item.Icon = _this.message;
            }
        });
    };
    BlockComponent.prototype.findAuthor = function (item) {
        if (item == undefined) {
            return "";
        }
        else {
            return item.FullName;
        }
    };
    BlockComponent.prototype.RemoveImage = function () {
        this.Item.Icon = undefined;
        this.file.nativeElement.value = "";
        this.message = undefined;
        this.progress = undefined;
    };
    BlockComponent.prototype.SortTable = function (str) {
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
        this.GetListBlock();
    };
    BlockComponent.prototype.GetClassSortTable = function (str) {
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
        core_1.ViewChild('BlockModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], BlockComponent.prototype, "BlockModal", void 0);
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], BlockComponent.prototype, "file", void 0);
    BlockComponent = __decorate([
        core_1.Component({
            selector: 'app-block',
            templateUrl: './block.component.html',
            styleUrls: ['./block.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService])
    ], BlockComponent);
    return BlockComponent;
}());
exports.BlockComponent = BlockComponent;
//# sourceMappingURL=block.component.js.map
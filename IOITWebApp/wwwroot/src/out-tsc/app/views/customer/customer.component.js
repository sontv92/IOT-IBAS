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
var ngx_toastr_1 = require("ngx-toastr");
var common_1 = require("@angular/common");
var dt_1 = require("../../data/dt");
var CustomerComponent = /** @class */ (function () {
    function CustomerComponent(http, modalDialogService, viewRef, toastr, datePipe) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.datePipe = datePipe;
        this.listCustomer = [];
        this.headerCustomer = [];
        this.listCompany = [];
        this.listBranchSearch = [];
        this.tablesilde = [];
        this.ListVatLieu = [];
        this.paging = {
            page: 1,
            page_size: 10,
            query: '1=1',
            order_by: 'name Desc',
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
    CustomerComponent.prototype.ngOnInit = function () {
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        var json = JSON.parse(localStorage.getItem('roles'));
        this.companyId = parseInt(localStorage.getItem('companyId'));
        this.UserId = parseInt(localStorage.getItem('userId'));
        this.BranchId = localStorage.getItem('BranchId');
        this.companyselect = -1;
        if (json.length > 0) {
            for (var i = 0; i < json.length; i++) {
                this.role = json[i].RoleId;
            }
        }
        if (this.role == 3) {
            this.paging.CompanyId = this.companyId;
        }
        else {
            this.paging.CompanyId = 0;
        }
        this.paging.Branchid = '';
        if (this.role == 3) {
            this.GetListBranchSearchStart();
        }
        if (this.role == 4) {
            this.GetListBranchCTCon();
            this.paging.Branchid = this.BranchId;
        }
        if (this.role == 1) {
            this.GetListCompany();
        }
        //Tạo table
        var obj = new dt_1.tablesilde();
        obj.name = 'MAMACBETONG';
        obj.nametable = 'Mác BT';
        obj.nameget = 'item.MAMACBETONG';
        this.tablesilde.push(obj);
        //
        var obj1 = new dt_1.tablesilde();
        obj1.name = 'TENMACBETONG';
        obj1.nametable = 'Mác BT';
        obj1.nameget = 'item.TENMACBETONG';
        this.tablesilde.push(obj1);
        this.GetListVatLieu();
        this.GetListCustomer();
    };
    //Get danh sach khach hang
    CustomerComponent.prototype.GetListCustomer = function () {
        var _this = this;
        this.http.get('/api/customer/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by + '&companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listCustomer = res["data"];
                _this.headerCustomer = res["data1"];
                _this.paging.item_count = res["metadata"];
                _this.querytable = '';
                _this.querytable += '<table class="table table-bordered text-nowrap" id="DataTables_Table_0" role="grid" aria-describedby="DataTables_Table_0_info">';
                _this.querytable += '<thead><tr>';
                for (var i = 0; i < _this.headerCustomer.length; i++) {
                    _this.querytable += '<th class="th-custom">';
                    _this.querytable += _this.headerCustomer[i];
                    _this.querytable += '</th>';
                }
                _this.querytable += '</tr></thead><tbody>';
                for (var i = 0; i < _this.listCustomer.length; i++) {
                    var item = _this.listCustomer[i];
                    _this.querytable += '<tr role="row">';
                    for (var j = 0; j < _this.listCustomer[i].length; j++) {
                        _this.querytable += '<td>';
                        _this.querytable += item[j];
                        _this.querytable += '</td>';
                    }
                    _this.querytable += '</tr>';
                }
                _this.querytable += '</tbody></table>';
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    CustomerComponent.prototype.ExportExcel = function () {
        var _this = this;
        if (this.paging.CompanyId == undefined) {
            this.paging.CompanyId = 0;
        }
        fetch('/api/Customer/GetReport?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by + '&companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid, {
            method: 'GET',
            headers: new Headers({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            }),
        })
            .then(function (response) {
            var blob = response.blob();
            return blob;
        }).then(function (blob) {
            var DateObj = new Date();
            var date = ('0' + DateObj.getDate()).slice(-2) + '_' + ('0' + (DateObj.getMonth() + 1)).slice(-2) + '_' + DateObj.getFullYear();
            _this.DownloadFile(blob, "quan_ly_cap_phoi_" + date + ".xlsx", 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
        });
    };
    CustomerComponent.prototype.DownloadFile = function (data, filename, mime) {
        var blob = new Blob([data], { type: mime || 'application/octet-stream' });
        if (typeof window.navigator.msSaveBlob !== 'undefined') {
            // IE workaround for "HTML7007: One or more blob URLs were 
            // revoked by closing the blob for which they were created. 
            // These URLs will no longer resolve as the data backing 
            // the URL has been freed."
            window.navigator.msSaveBlob(blob, filename);
        }
        else {
            var blobURL = window.URL.createObjectURL(blob);
            var tempLink = document.createElement('a');
            tempLink.href = blobURL;
            tempLink.setAttribute('download', filename);
            tempLink.setAttribute('target', '_blank');
            document.body.appendChild(tempLink);
            tempLink.click();
            document.body.removeChild(tempLink);
        }
    };
    CustomerComponent.prototype.GetListCompany = function () {
        var _this = this;
        this.http.get('/api/company/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listCompany = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    CustomerComponent.prototype.GetListBranchSearchStart = function () {
        var _this = this;
        this.http.get('/api/branch/GetByPage?page=1&query=CompanyId = ' + this.companyId + ' &order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listBranchSearch = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    CustomerComponent.prototype.GetListBranchCTCon = function () {
        var _this = this;
        this.http.get('/api/Order/GetByUser?id=' + this.UserId, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listBranchSearch = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    CustomerComponent.prototype.GetListBranchSearch = function () {
        var _this = this;
        this.http.get('/api/branch/GetByPage?page=1&query=CompanyId = ' + this.companyselectsearch + ' &order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listBranchSearch = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    CustomerComponent.prototype.QueryChanged = function () {
        var _this = this;
        var query = '';
        //if (this.q.fromdate != undefined && this.q.todate != undefined && this.q.fromdate != '' && this.q.todate != '') {
        //  if (query != '') {
        //    query += " AND ( nv.NGAYDATHANG >= convert(datetime ,'" + this.q.fromdate + " 00:00:00.000') AND nv.NGAYDATHANG <= convert(datetime ,'" + this.q.todate + " 23:59:59.999') )";
        //  }
        //  else {
        //    query += "( nv.NGAYDATHANG >= convert(datetime ,'" + this.q.fromdate + " 00:00:00.000') AND nv.NGAYDATHANG <= convert(datetime ,'" + this.q.todate + " 23:59:59.999') )";
        //  }
        //}
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
        if (this.q.Branchlist != undefined) {
            this.paging.Branchid = this.q.Branchlist;
            //this.q.Branchlist.forEach((item, index) => {
            //  if (item != '') {
            //    this.paging.Branchid = this.paging.Branchid + item + ',';
            //  }
            //});
        }
        else {
            this.paging.Branchid = '';
            if (this.role != 1) {
                this.listBranchSearch.forEach(function (item, index) {
                    if (item != '') {
                        _this.paging.Branchid = _this.paging.Branchid + item["BranchId"] + ',';
                    }
                });
            }
        }
        if (query == '')
            this.paging.query = '1=1';
        else
            this.paging.query = query;
        this.GetListCustomer();
    };
    //Chuyển trang
    CustomerComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListCustomer();
    };
    //Toast cảnh báo
    CustomerComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    //Toast thành công
    CustomerComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    //Toast thành công
    CustomerComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    CustomerComponent.prototype.GetBranch = function () {
        this.companyselectsearch = this.q.CompanyId;
        this.listBranchSearch = [];
        this.q.BranchId = null;
        this.GetListBranchSearch();
        this.paging.CompanyId = this.q.CompanyId;
    };
    CustomerComponent.prototype.SortTable = function (str) {
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
        this.GetListCustomer();
    };
    CustomerComponent.prototype.GetClassSortTable = function (str) {
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
    CustomerComponent.prototype.GetListVatLieu = function () {
        var _this = this;
        this.http.get('/api/Customer/GetVatLieu', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.ListVatLieu = res["data"];
                if (_this.ListVatLieu.length > 0) {
                    for (var i = 0; i < _this.ListVatLieu.length; i++) {
                        var obj = new dt_1.tablesilde();
                        obj.name = _this.ListVatLieu[i].TENCUAVL;
                        obj.nametable = _this.ListVatLieu[i].TENCUAVL;
                        obj.nameget = 'item.VL_' + _this.ListVatLieu[i].MACUAVL + '';
                        _this.tablesilde.push(obj);
                    }
                }
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    __decorate([
        core_1.ViewChild('CustomerModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], CustomerComponent.prototype, "CustomerModal", void 0);
    __decorate([
        core_1.ViewChild('ResetPasswordModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], CustomerComponent.prototype, "ResetPasswordModal", void 0);
    __decorate([
        core_1.ViewChild('OrdersModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], CustomerComponent.prototype, "OrdersModal", void 0);
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], CustomerComponent.prototype, "file", void 0);
    CustomerComponent = __decorate([
        core_1.Component({
            selector: 'app-customer',
            templateUrl: './customer.component.html',
            styleUrls: ['./customer.component.scss'],
            styles: ['.th-custom { position: sticky !important; top: -1px !important; z-index: 99 !important; color: white !important; background: #3c8dbc !important; }'],
            encapsulation: core_1.ViewEncapsulation.None
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService,
            common_1.DatePipe])
    ], CustomerComponent);
    return CustomerComponent;
}());
exports.CustomerComponent = CustomerComponent;
//# sourceMappingURL=customer.component.js.map
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
var model_1 = require("../../data/model");
var const_1 = require("../../data/const");
var router_1 = require("@angular/router");
var OrderComponent = /** @class */ (function () {
    function OrderComponent(http, modalDialogService, viewRef, toastr, activatedRoute) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.activatedRoute = activatedRoute;
        this.listOrder = [];
        this.listOrderBranch = [];
        this.listCompany = [];
        this.listBranchSearch = [];
        this.listNV = [];
        this.listOrderStatus = const_1.OrderStatus;
        this.listPaymentOrderStatus = const_1.PaymentOrderStatus;
        this.PriceCurrencyMaskConfig = {
            align: "left",
            allowNegative: false,
            decimal: ".",
            precision: 0,
            prefix: "",
            suffix: " Vnđ",
            thousands: ","
        };
        this.Item = new model_1.Order();
        this.paging = {
            page: 1,
            page_size: 10,
            query: '1=1',
            order_by: 'NGAYDATHANG Desc',
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
    OrderComponent.prototype.ngOnInit = function () {
        this.totalMETKHOIDATHANG = 0;
        this.totalMETKHOITICHLUY = 0;
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
        this.paging.query = "DATEPART(dy,GETDATE()) = DATEPART(dy,NGAYDATHANG) AND DATEPART(yy,GETDATE()) = DATEPART(yy,NGAYDATHANG)";
        var todaynew = new Date().toISOString().substr(0, 10);
        this.q.fromdate = todaynew;
        this.q.todate = todaynew;
        if (localStorage.getItem('getlinkd') == "1") {
            if (localStorage.getItem('thoigiand') == '1') {
                var today_1 = new Date().toISOString().substr(0, 10);
                this.q.fromdate = today_1;
                this.q.todate = today_1;
                this.paging.query = "DATEPART(dy,GETDATE()) = DATEPART(dy,NGAYDATHANG) AND DATEPART(yy,GETDATE()) = DATEPART(yy,NGAYDATHANG)";
            }
            if (localStorage.getItem('thoigiand') == '2') {
                var today_2 = new Date();
                this.q.fromdate = new Date(today_2.setDate(today_2.getDate() - today_2.getDay() + 1)).toISOString().substr(0, 10);
                this.q.todate = new Date(today_2.setDate(today_2.getDate() - today_2.getDay() + 7)).toISOString().substr(0, 10);
                this.paging.query = "DATEPART(wk,GETDATE()) = DATEPART(wk,NGAYDATHANG) AND DATEPART(yy,GETDATE()) = DATEPART(yy,NGAYDATHANG)";
            }
            if (localStorage.getItem('thoigiand') == '3') {
                var today_3 = new Date();
                this.q.fromdate = new Date(today_3.setDate(today_3.getDate() - today_3.getDay() + 1 - 7)).toISOString().substr(0, 10);
                var today1_1 = new Date();
                this.q.todate = new Date(today1_1.setDate(today1_1.getDate() - today1_1.getDay())).toISOString().substr(0, 10);
                this.paging.query = "(DATEPART(wk,GETDATE()) -1) = DATEPART(wk,NGAYDATHANG) AND DATEPART(yy,GETDATE()) = DATEPART(yy,NGAYDATHANG)";
            }
            if (localStorage.getItem('thoigiand') == '4') {
                var today = new Date();
                var lastDayOfMonth = new Date(today.getFullYear(), today.getMonth(), 2);
                this.q.fromdate = lastDayOfMonth.toISOString().substr(0, 10);
                var today1 = new Date();
                var lastDayOfMonth1 = new Date(today1.getFullYear(), today1.getMonth() + 1, 1);
                this.q.todate = lastDayOfMonth1.toISOString().substr(0, 10);
                this.paging.query = "DATEPART(mm,GETDATE()) = DATEPART(mm,NGAYDATHANG) AND DATEPART(yy,GETDATE()) = DATEPART(yy,NGAYDATHANG)";
            }
        }
        this.GetlistKH();
        this.GetListOrder();
        this.GetListOrderBarch();
    };
    //Get danh sách danh mục đơn hàng
    OrderComponent.prototype.GetListOrder = function () {
        var _this = this;
        this.http.get('/api/order/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by + '&companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listOrder = res["data"];
                _this.paging.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    OrderComponent.prototype.ExportExcel = function () {
        var _this = this;
        if (this.paging.CompanyId == undefined) {
            this.paging.CompanyId = 0;
        }
        fetch('/api/order/GetReport?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&tungay=' + this.q.fromdate + '&denngay=' + this.q.todate + '&order_by=' + this.paging.order_by + '&companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid, {
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
            _this.DownloadFile(blob, "don_hang_" + date + ".xlsx", 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
        });
    };
    OrderComponent.prototype.DownloadFile = function (data, filename, mime) {
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
    //Get danh sách danh mục đơn hàng
    OrderComponent.prototype.GetListOrderBarch = function () {
        var _this = this;
        this.http.get('/api/Order/GetByBranch?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listOrderBranch = res["data"];
                _this.paging.item_count = res["metadata"];
                _this.totalMETKHOIDATHANG = 0;
                _this.totalMETKHOITICHLUY = 0;
                for (var index in _this.listOrderBranch) {
                    _this.totalMETKHOIDATHANG = _this.totalMETKHOIDATHANG + _this.listOrderBranch[index].METKHOIDATHANG;
                    _this.totalMETKHOITICHLUY = _this.totalMETKHOITICHLUY + _this.listOrderBranch[index].METKHOITICHLUY;
                }
                ;
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    // Get danh sách khách hàng
    OrderComponent.prototype.GetlistKH = function () {
        var _this = this;
        this.http.get('/api/Order/GetNV?companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listNV = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    OrderComponent.prototype.GetListCompany = function () {
        var _this = this;
        this.http.get('/api/company/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listCompany = res["data"];
                if (localStorage.getItem('congtyd') != 'undefined') {
                    var index = _this.listCompany.find(function (x) { return x.CompanyId == parseInt(localStorage.getItem('congtyd')); });
                    _this.q.CompanyId = index.CompanyId;
                    _this.paging.CompanyId = parseInt(localStorage.getItem('congtyd'));
                    _this.companyId = parseInt(localStorage.getItem('congtyd'));
                    _this.GetListBranchSearchStart();
                    _this.GetListOrder();
                    _this.GetListOrderBarch();
                }
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    OrderComponent.prototype.GetListBranchSearchStart = function () {
        var _this = this;
        this.http.get('/api/branch/GetByPage?page=1&query=CompanyId = ' + this.companyId + ' &order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listBranchSearch = res["data"];
                if (localStorage.getItem('tramtrond') != 'undefined') {
                    _this.q.Branchlist = [];
                    var lstnhombenh = localStorage.getItem('tramtrond').split(',');
                    lstnhombenh.forEach(function (element) {
                        _this.q.Branchlist.push(parseInt(element));
                    });
                }
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    OrderComponent.prototype.GetListBranchCTCon = function () {
        var _this = this;
        this.http.get('/api/Order/GetByUser?id=' + this.UserId, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listBranchSearch = res["data"];
                if (localStorage.getItem('tramtrond') != 'undefined') {
                    _this.q.Branchlist = [];
                    var lstnhombenh = localStorage.getItem('tramtrond').split(',');
                    lstnhombenh.forEach(function (element) {
                        _this.q.Branchlist.push(parseInt(element));
                    });
                }
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    OrderComponent.prototype.GetListBranchSearch = function () {
        var _this = this;
        this.http.get('/api/branch/GetByPage?page=1&query=CompanyId = ' + this.companyselectsearch + ' &order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listBranchSearch = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    OrderComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListOrder();
    };
    //Toast cảnh báo
    OrderComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    //Toast thành công
    OrderComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    //Toast thành công
    OrderComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    OrderComponent.prototype.QueryChanged = function () {
        var _this = this;
        var query = '';
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            if (query != '') {
                query += " and 1=1";
            }
            else {
                query += "1=1";
            }
        }
        if (this.q.fromdate != undefined && this.q.todate != undefined && this.q.fromdate != '' && this.q.todate != '') {
            if (query != '') {
                query += " AND ( nv.NGAYDATHANG >= convert(datetime ,'" + this.q.fromdate + " 00:00:00.000') AND nv.NGAYDATHANG <= convert(datetime ,'" + this.q.todate + " 23:59:59.999') )";
            }
            else {
                query += "( nv.NGAYDATHANG >= convert(datetime ,'" + this.q.fromdate + " 00:00:00.000') AND nv.NGAYDATHANG <= convert(datetime ,'" + this.q.todate + " 23:59:59.999') )";
            }
        }
        if (this.q.TENNV != undefined && this.q.TENNV != '') {
            if (query != '') {
                query += " AND nv.TENNV = N'" + this.q.TENNV + "'";
            }
            else {
                query += "nv.TENNV = N'" + this.q.TENNV + "'";
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
        this.GetListOrder();
        this.GetListOrderBarch();
    };
    //
    OrderComponent.prototype.GetBranch = function () {
        this.companyselectsearch = this.q.CompanyId;
        this.listBranchSearch = [];
        this.q.BranchId = null;
        this.GetListBranchSearch();
        this.paging.CompanyId = this.q.CompanyId;
    };
    //Open modal view
    OrderComponent.prototype.OpenViewModal = function (item) {
        this.Item = new model_1.Order();
        this.Item = Object.assign({}, item);
        this.viewModal.show();
    };
    OrderComponent.prototype.SortTable = function (str) {
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
        this.GetListOrder();
    };
    OrderComponent.prototype.GetClassSortTable = function (str) {
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
        core_1.ViewChild('ViewModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], OrderComponent.prototype, "viewModal", void 0);
    OrderComponent = __decorate([
        core_1.Component({
            selector: 'app-order',
            templateUrl: './order.component.html',
            styleUrls: ['./order.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService,
            router_1.ActivatedRoute])
    ], OrderComponent);
    return OrderComponent;
}());
exports.OrderComponent = OrderComponent;
//# sourceMappingURL=order.component.js.map
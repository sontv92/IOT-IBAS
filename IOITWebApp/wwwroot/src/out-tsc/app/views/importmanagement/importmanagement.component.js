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
var ImportManagement = /** @class */ (function () {
    function ImportManagement(http, modalDialogService, viewRef, toastr, activatedRoute) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.activatedRoute = activatedRoute;
        this.listOrder = [];
        this.dataSum = {};
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
        this.Tonkho = new model_1.NhapKho();
        this.cuavatlieu1 = true;
        this.cuavatlieu2 = true;
        this.cuavatlieu3 = true;
        this.cuavatlieu4 = true;
        this.cuavatlieu5 = true;
        this.cuavatlieu6 = true;
        this.cuavatlieu7 = true;
        this.cuavatlieu8 = true;
        this.cuavatlieu9 = true;
        this.cuavatlieu10 = true;
        this.cuavatlieu11 = true;
        this.cuavatlieu12 = true;
        this.kieudulieu1 = true;
        this.kieudulieu2 = true;
        // this.Tonkho = new 
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
    ImportManagement.prototype.getQueryPar = function () {
        var str = '0';
        var str1 = '0';
        if (this.cuavatlieu1 == true) {
            str += ',1';
        }
        if (this.cuavatlieu2 == true) {
            str += ',2';
        }
        if (this.cuavatlieu3 == true) {
            str += ',3';
        }
        if (this.cuavatlieu4 == true) {
            str += ',4';
        }
        if (this.cuavatlieu5 == true) {
            str += ',5';
        }
        if (this.cuavatlieu6 == true) {
            str += ',6';
        }
        if (this.cuavatlieu7 == true) {
            str += ',7';
        }
        if (this.cuavatlieu8 == true) {
            str += ',8';
        }
        if (this.cuavatlieu9 == true) {
            str += ',9';
        }
        if (this.cuavatlieu10 == true) {
            str += ',10';
        }
        if (this.cuavatlieu11 == true) {
            str += ',11';
        }
        if (this.cuavatlieu12 == true) {
            str += ',12';
        }
        if (this.kieudulieu1 == true) {
            str1 += ',1';
        }
        if (this.kieudulieu2 == true) {
            str1 += ',2';
        }
        this.cuavatlieu = str;
        this.kieudulieu = str1;
    };
    ImportManagement.prototype.ngOnInit = function () {
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
        this.q.ckbNhapKho = this.kieudulieu1;
        this.q.ckTinhKho = this.kieudulieu2;
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
        //   this.GetListOrder();
        this.GetListOrderBarch();
    };
    //test
    ImportManagement.prototype.TestChange = function (i) {
        if (i == 1) {
            this.cuavatlieu1 = !this.cuavatlieu1;
        }
        else if (i == 2) {
            this.cuavatlieu2 = !this.cuavatlieu2;
        }
        else if (i == 3) {
            this.cuavatlieu3 = !this.cuavatlieu3;
        }
        else if (i == 4) {
            this.cuavatlieu4 = !this.cuavatlieu4;
        }
        else if (i == 5) {
            this.cuavatlieu5 = !this.cuavatlieu5;
        }
        else if (i == 6) {
            this.cuavatlieu6 = !this.cuavatlieu6;
        }
        else if (i == 7) {
            this.cuavatlieu7 = !this.cuavatlieu7;
        }
        else if (i == 8) {
            this.cuavatlieu8 = !this.cuavatlieu8;
        }
        else if (i == 9) {
            this.cuavatlieu9 = !this.cuavatlieu9;
        }
        else if (i == 10) {
            this.cuavatlieu10 = !this.cuavatlieu10;
        }
        else if (i == 11) {
            this.cuavatlieu11 = !this.cuavatlieu11;
        }
        else if (i == 12) {
            this.cuavatlieu12 = !this.cuavatlieu12;
        }
    };
    //Get danh sách danh mục đơn hàng
    ImportManagement.prototype.GetListOrder = function () {
        var _this = this;
        console.log("Error: connect to API");
        this.http.get('/api/ImportManagement/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by + '&companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid + '&KDL=' + this.kieudulieu + '&CVL=' + this.cuavatlieu, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listOrder = res["data"];
                _this.Tonkho = res["data1"];
                _this.dataSum = res["dataSUM"];
                _this.paging.item_count = res["metadata"];
                console.log("dataSum: " + _this.dataSum);
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    ImportManagement.prototype.ExportExcel = function () {
        var _this = this;
        if (this.paging.CompanyId == undefined) {
            this.paging.CompanyId = 0;
        }
        if (this.q.Branchlist == undefined) {
            this.toastWarning("Bạn chưa chọn trạm trộn!");
        }
        else {
            this.getQueryPar();
            fetch('/api/ImportManagement/GetReport?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by + '&companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid + '&KDL=' + this.kieudulieu + '&CVL=' + this.cuavatlieu, {
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
        }
    };
    ImportManagement.prototype.DownloadFile = function (data, filename, mime) {
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
    ImportManagement.prototype.GetListOrderBarch = function () {
        var _this = this;
        this.http.get('/api/ImportManagement/GetByBranch?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid, this.httpOptions).subscribe(function (res) {
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
    ImportManagement.prototype.GetlistKH = function () {
        var _this = this;
        this.http.get('/api/ImportManagement/GetNV?companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listNV = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    ImportManagement.prototype.GetListCompany = function () {
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
    ImportManagement.prototype.GetListBranchSearchStart = function () {
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
    ImportManagement.prototype.GetListBranchCTCon = function () {
        var _this = this;
        this.http.get('/api/ImportManagement/GetByUser?id=' + this.UserId, this.httpOptions).subscribe(function (res) {
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
    ImportManagement.prototype.GetListBranchSearch = function () {
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
    ImportManagement.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListOrder();
    };
    //Toast cảnh báo
    ImportManagement.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    //Toast thành công
    ImportManagement.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    //Toast thành công
    ImportManagement.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    ImportManagement.prototype.QueryChanged = function () {
        var _this = this;
        var query = '';
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            if (query != '') {
                query += " and 1=1";
            }
            else {
                query += " 1=1 ";
            }
        }
        if (this.q.fromdate != undefined && this.q.todate != undefined && this.q.fromdate != '' && this.q.todate != '') {
            if (query != '') {
                query += " AND ( NGAY >= convert(datetime ,'" + this.q.fromdate + " 00:00:00.000') AND NGAY <= convert(datetime ,'" + this.q.todate + " 23:59:59.999') )";
            }
            else {
                query += " AND ( NGAY >= convert(datetime ,'" + this.q.fromdate + " 00:00:00.000') AND NGAY <= convert(datetime ,'" + this.q.todate + " 23:59:59.999') )";
            }
        }
        this.getQueryPar();
        //if (this.q.TENNV != undefined && this.q.TENNV != '') {
        //  if (query != '') {
        //    query += " AND nv.TENNV = N'" + this.q.TENNV + "'";
        //  }
        //  else {
        //    query += "nv.TENNV = N'" + this.q.TENNV + "'";
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
            //this.paging.Branchid = '';
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
        if (this.q.Branchlist == undefined) {
            this.toastWarning("Bạn chưa chọn trạm trộn!");
        }
        else {
            this.GetListOrder();
        }
        // this.GetListOrderBarch();
    };
    //
    ImportManagement.prototype.GetBranch = function () {
        this.companyselectsearch = this.q.CompanyId;
        this.listBranchSearch = [];
        this.q.BranchId = null;
        this.GetListBranchSearch();
        this.paging.CompanyId = this.q.CompanyId;
    };
    //Open modal view
    ImportManagement.prototype.OpenViewModal = function (item) {
        this.Item = new model_1.Order();
        this.Item = Object.assign({}, item);
        this.viewModal.show();
    };
    ImportManagement.prototype.SortTable = function (str) {
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
    ImportManagement.prototype.GetClassSortTable = function (str) {
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
    ], ImportManagement.prototype, "viewModal", void 0);
    ImportManagement = __decorate([
        core_1.Component({
            selector: 'app-importmanagement',
            templateUrl: './importmanagement.component.html',
            styleUrls: ['./importmanagement.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService,
            router_1.ActivatedRoute])
    ], ImportManagement);
    return ImportManagement;
}());
exports.ImportManagement = ImportManagement;
//# sourceMappingURL=importmanagement.component.js.map
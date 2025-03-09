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
var common_1 = require("@angular/common");
var http_1 = require("@angular/common/http");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var ngx_toastr_1 = require("ngx-toastr");
var common_2 = require("@angular/common");
var DashboardComponent = /** @class */ (function () {
    function DashboardComponent(http, modalDialogService, toastr, datepipe) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.toastr = toastr;
        this.datepipe = datepipe;
        this.listOrder = [];
        this.listOrderCharH = [
            { NGAYDATHANG: '0H', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '1H', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '2H', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '3H', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '4H', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '5H', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '6H', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '7H', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '8H', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '9H', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '10H', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '11H', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '12H', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '13H', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '14H', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '15H', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '16H', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '17H', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '18H', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '19H', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '20H', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '21H', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '22H', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '23H', METKHOITICHLUY: 0 }
        ];
        this.listOrderChartW = [
            { NGAYDATHANG: 'Thứ 2', METKHOITICHLUY: 0 },
            { NGAYDATHANG: 'Thứ 3', METKHOITICHLUY: 0 },
            { NGAYDATHANG: 'Thứ 4', METKHOITICHLUY: 0 },
            { NGAYDATHANG: 'Thứ 5', METKHOITICHLUY: 0 },
            { NGAYDATHANG: 'Thứ 6', METKHOITICHLUY: 0 },
            { NGAYDATHANG: 'Thứ 7', METKHOITICHLUY: 0 },
            { NGAYDATHANG: 'Chủ nhật', METKHOITICHLUY: 0 }
        ];
        this.listOrderChartD = [
            { NGAYDATHANG: '1', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '2', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '3', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '4', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '5', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '6', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '7', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '8', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '9', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '10', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '11', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '12', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '13', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '14', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '15', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '16', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '17', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '18', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '19', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '20', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '21', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '22', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '23', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '24', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '25', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '26', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '27', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '28', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '29', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '30', METKHOITICHLUY: 0 },
            { NGAYDATHANG: '31', METKHOITICHLUY: 0 },
        ];
        this.listCompany = [];
        this.listBranchSearch = [];
        this.barChartLabels = [];
        this.totalName = [];
        this.totalMETKHOITICHLUY = [];
        this.barChartOptions = {
            scaleShowVerticalLines: false,
            responsive: true,
            legend: {
                labels: {
                    // This more specific font property overrides the global property
                    fontColor: 'black',
                    fontSize: 18
                }
            }
        };
        this.barChartType = 'line';
        this.barChartLegend = true;
        this.lineChartColors = [
            {
                backgroundColor: '#FFAE00',
                borderColor: 'rgb(243, 167, 5)',
                pointBackgroundColor: 'rgb(243, 167, 2)',
                pointBorderColor: '#fff',
                pointHoverBackgroundColor: '#fff',
                pointHoverBorderColor: 'rgba(148,159,177,0.8)'
            },
            {
                backgroundColor: '#FFAE00',
                borderColor: 'red',
                pointBackgroundColor: 'rgba(148,159,177,1)',
                pointBorderColor: '#fff',
                pointHoverBackgroundColor: '#fff',
                pointHoverBorderColor: 'rgba(148,159,177,0.8)'
            }
        ];
        this.barChartData = [
            { data: [], label: 'Tổng' }
        ];
        this.paging = {
            page: 1,
            page_size: 10,
            query: '1=1',
            querychar: '1=1',
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
    DashboardComponent.prototype.chartColors = function () {
        return [{
                borderColor: 'rgba(225,10,24,0.2)',
                pointBackgroundColor: 'rgba(225,10,24,0.2)',
                pointBorderColor: '#fff',
                pointHoverBackgroundColor: '#fff',
                pointHoverBorderColor: 'rgba(225,10,24,0.2)'
            }];
    };
    DashboardComponent.prototype.ngOnInit = function () {
        localStorage.setItem('thoigiand', undefined);
        localStorage.setItem('congtyd', undefined);
        localStorage.setItem('tramtrond', undefined);
        localStorage.setItem('getlinkd', "0");
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        var json = JSON.parse(localStorage.getItem('roles'));
        this.companyId = parseInt(localStorage.getItem('companyId'));
        this.UserId = parseInt(localStorage.getItem('userId'));
        this.BranchId = localStorage.getItem('BranchId');
        if (json.length > 0) {
            for (var i = 0; i < json.length; i++) {
                this.role = json[i].RoleId;
            }
        }
        this.q.status = "1";
        this.paging.Branchid = '';
        if (this.role == 3) {
            this.paging.CompanyId = this.companyId;
            this.GetListBranchSearchStart();
        }
        else {
            this.paging.CompanyId = 0;
        }
        if (this.role == 4) {
            this.GetListBranchCTCon();
            this.paging.Branchid = this.BranchId;
        }
        //  let today = new Date().toLocaleDateString().substr(0, 10);
        var today = new Date();
        this.q.tungay = this.datepipe.transform(today, 'yyyy-MM-dd');
        this.q.denngay = this.datepipe.transform(today, 'yyyy-MM-dd');
        this.paging.query = "( sa.NGAYDATHANG >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND sa.NGAYDATHANG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
        this.paging.querychar = "( tr.GIOXONG >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND tr.GIOXONG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
        this.GetListOrder();
        this.GetListCompany();
        this.GetListOrderChart();
        this.GetListOrderDH();
    };
    //Get danh sách danh mục đơn hàng
    DashboardComponent.prototype.GetListOrder = function () {
        var _this = this;
        this.http.get('/api/Dashboard/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.querychar + '&order_by=' + this.paging.order_by + '&companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                console.log('data' + res["data"]);
                _this.listOrder = res["data"];
                _this.paging.item_count = res["metadata"];
                _this.totalMETKHOITICHLUY = [];
                _this.totalName = [];
                var _loop_1 = function (index) {
                    if (_this.totalMETKHOITICHLUY.find(function (item) { return item == _this.listOrder[index].Name; }) == undefined) {
                        _this.totalMETKHOITICHLUY.push(_this.listOrder[index].Name);
                    }
                    if (_this.totalName.find(function (item) { return item == _this.listOrder[index].TENNV; }) == undefined) {
                        _this.totalName.push(_this.listOrder[index].TENNV);
                    }
                };
                for (var index in _this.listOrder) {
                    _loop_1(index);
                }
                ;
                console.log('totalMETKHOITICHLUY' + _this.totalMETKHOITICHLUY.length);
                _this.totalmac = _this.totalMETKHOITICHLUY.length;
                _this.totalxe = _this.totalName.length;
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    DashboardComponent.prototype.GetListOrderDH = function () {
        var _this = this;
        this.http.get('/api/Dashboard/GetByPageDH?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by + '&companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listOrder = res["data"];
                _this.paging.item_count = res["metadata"];
                _this.totaldonhang = 0;
                _this.totalTENNV = 0;
                _this.tongdonhang = 0;
                for (var index in _this.listOrder) {
                    _this.totaldonhang = _this.totaldonhang + _this.listOrder[index].donhang;
                    _this.tongdonhang = _this.tongdonhang + _this.listOrder[index].tongdonhang;
                    _this.totalTENNV = _this.totalTENNV + 1;
                }
                ;
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    DashboardComponent.prototype.GetListOrderChart = function () {
        var _this = this;
        this.http.get('/api/Dashboard/GetByPageChart?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.querychar + '&order_by=' + this.paging.order_by + '&sort=' + this.q.status + '&companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                var data = res["data"];
                //debugger;
                var sum_1 = 0;
                if (_this.q.status == "1") {
                    _this.listOrderCharH.forEach(function (value) {
                        value.METKHOITICHLUY = 0;
                    });
                    if (data != null) {
                        var that1_1 = _this;
                        data.forEach(function (value) {
                            that1_1.listOrderCharH.find(function (item) { return item.NGAYDATHANG == value.NGAYDATHANG; }).METKHOITICHLUY = value.METKHOITICHLUY;
                            sum_1 = sum_1 + value.METKHOITICHLUY;
                        });
                    }
                    _this.barChartData = [
                        { data: _this.listOrderCharH.map(function (x) { return x.METKHOITICHLUY; }), label: 'Tổng khối lượng: ' + sum_1.toFixed(1).toString() + 'm3' }
                    ];
                    if (_this.barChartLabels.length !== 0) {
                        _this.barChartLabels.splice(0, 99);
                    }
                    var that_1 = _this;
                    _this.listOrderCharH.map(function (x) { return x.NGAYDATHANG; }).forEach(function (snapshot) {
                        that_1.barChartLabels.push(snapshot);
                    });
                }
                if (_this.q.status == "2") {
                    _this.listOrderChartW.forEach(function (value) {
                        value.METKHOITICHLUY = 0;
                    });
                    if (data != null) {
                        var that1_2 = _this;
                        data.forEach(function (value) {
                            that1_2.listOrderChartW.find(function (item) { return item.NGAYDATHANG == value.NGAYDATHANG; }).METKHOITICHLUY = value.METKHOITICHLUY;
                            sum_1 += value.METKHOITICHLUY;
                        });
                    }
                    _this.barChartData = [
                        { data: _this.listOrderChartW.map(function (x) { return x.METKHOITICHLUY; }), label: 'Tổng khối lượng: ' + sum_1.toFixed(1).toString() + 'm3' }
                    ];
                    if (_this.barChartLabels.length !== 0) {
                        _this.barChartLabels.splice(0, 99);
                    }
                    var that_2 = _this;
                    _this.listOrderChartW.map(function (x) { return x.NGAYDATHANG; }).forEach(function (snapshot) {
                        that_2.barChartLabels.push(snapshot);
                    });
                }
                if (_this.q.status == "3") {
                    _this.listOrderChartW.forEach(function (value) {
                        value.METKHOITICHLUY = 0;
                    });
                    if (data != null) {
                        var that1_3 = _this;
                        data.forEach(function (value) {
                            that1_3.listOrderChartW.find(function (item) { return item.NGAYDATHANG == value.NGAYDATHANG; }).METKHOITICHLUY = value.METKHOITICHLUY;
                            sum_1 += value.METKHOITICHLUY;
                        });
                    }
                    _this.barChartData = [
                        { data: _this.listOrderChartW.map(function (x) { return x.METKHOITICHLUY; }), label: 'Tổng khối lượng: ' + sum_1.toFixed(1).toString() + 'm3' }
                    ];
                    if (_this.barChartLabels.length !== 0) {
                        _this.barChartLabels.splice(0, 99);
                    }
                    var that_3 = _this;
                    _this.listOrderChartW.map(function (x) { return x.NGAYDATHANG; }).forEach(function (snapshot) {
                        that_3.barChartLabels.push(snapshot);
                    });
                }
                if (_this.q.status == "4") {
                    _this.listOrderChartD.forEach(function (value) {
                        value.METKHOITICHLUY = 0;
                    });
                    if (data != null) {
                        var that1_4 = _this;
                        data.forEach(function (value) {
                            that1_4.listOrderChartD.find(function (item) { return item.NGAYDATHANG == value.NGAYDATHANG; }).METKHOITICHLUY = value.METKHOITICHLUY;
                            sum_1 += value.METKHOITICHLUY;
                        });
                    }
                    _this.barChartData = [
                        { data: _this.listOrderChartD.map(function (x) { return x.METKHOITICHLUY; }), label: 'Tổng khối lượng: ' + sum_1.toFixed(1).toString() + 'm3' }
                    ];
                    if (_this.barChartLabels.length !== 0) {
                        _this.barChartLabels.splice(0, 99);
                    }
                    var that_4 = _this;
                    _this.listOrderChartD.map(function (x) { return x.NGAYDATHANG; }).forEach(function (snapshot) {
                        that_4.barChartLabels.push(snapshot);
                    });
                }
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    DashboardComponent.prototype.GetListCompany = function () {
        var _this = this;
        this.http.get('/api/company/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listCompany = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    DashboardComponent.prototype.GetListBranchSearchStart = function () {
        var _this = this;
        this.http.get('/api/branch/GetByPage?page=1&query=CompanyId = ' + this.companyId + ' &order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listBranchSearch = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    DashboardComponent.prototype.GetListBranchCTCon = function () {
        var _this = this;
        this.http.get('/api/Order/GetByUser?id=' + this.UserId, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listBranchSearch = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    DashboardComponent.prototype.GetListBranchSearch = function () {
        var _this = this;
        this.http.get('/api/branch/GetByPage?page=1&query=CompanyId = ' + this.companyselectsearch + ' &order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listBranchSearch = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    DashboardComponent.prototype.clickd = function () {
        localStorage.setItem('getlinkd', "1");
    };
    //
    DashboardComponent.prototype.QueryChanged = function () {
        var _this = this;
        var query = '';
        var querychar = '';
        if (this.q.status != undefined && this.q.status != '') {
            if (this.q.status == '1') {
                var today = new Date();
                this.q.tungay = this.datepipe.transform(today, 'yyyy-MM-dd');
                this.q.denngay = this.datepipe.transform(today, 'yyyy-MM-dd');
                // let today = new Date().toISOString().substr(0, 10);
                query += "( sa.NGAYDATHANG >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND sa.NGAYDATHANG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
                querychar += "( tr.GIOXONG >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND tr.GIOXONG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
            }
            if (this.q.status == '2') {
                var today = new Date();
                this.q.tungay = this.datepipe.transform(new Date(today.setDate(today.getDate() - today.getDay() + 1)), 'yyyy-MM-dd');
                this.q.denngay = this.datepipe.transform(new Date(today.setDate(today.getDate() - today.getDay() + 7)), 'yyyy-MM-dd');
                ;
                query += "( sa.NGAYDATHANG >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND sa.NGAYDATHANG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
                querychar += "( tr.GIOXONG >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND tr.GIOXONG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
            }
            if (this.q.status == '3') {
                var today = new Date();
                this.q.tungay = this.datepipe.transform(new Date(today.setDate(today.getDate() - today.getDay() + 1 - 7)), 'yyyy-MM-dd');
                var today1 = new Date();
                this.q.denngay = this.datepipe.transform(new Date(today1.setDate(today1.getDate() - today1.getDay())), 'yyyy-MM-dd');
                query += "( sa.NGAYDATHANG >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND sa.NGAYDATHANG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
                querychar += "( tr.GIOXONG >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND tr.GIOXONG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
            }
            if (this.q.status == '4') {
                var today2 = new Date();
                var lastDayOfMonth = new Date(today2.getFullYear(), today2.getMonth(), 2);
                this.q.tungay = this.datepipe.transform(lastDayOfMonth, 'yyyy-MM-dd');
                var today3 = new Date();
                var lastDayOfMonth1 = new Date(today3.getFullYear(), today3.getMonth() + 1, 1);
                this.q.denngay = this.datepipe.transform(lastDayOfMonth1, 'yyyy-MM-dd');
                query += "( sa.NGAYDATHANG >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND sa.NGAYDATHANG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
                querychar += "( tr.GIOXONG >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND tr.GIOXONG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
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
            this.paging.Branchid = '';
            this.q.Branchlist.forEach(function (item, index) {
                if (item != '') {
                    _this.paging.Branchid = _this.paging.Branchid + item + ',';
                }
            });
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
        if (query == '') {
            this.paging.query = '1=1';
        }
        else {
            this.paging.query = query;
        }
        if (querychar == '') {
            this.paging.querychar = '1=1';
        }
        else {
            this.paging.querychar = querychar;
        }
        localStorage.setItem('thoigiand', this.q.status);
        localStorage.setItem('congtyd', this.q.CompanyId);
        localStorage.setItem('tramtrond', this.q.Branchlist);
        this.GetListOrder();
        //this.GetListOrderDH();
        //this.GetListOrderChart();
    };
    //
    DashboardComponent.prototype.QueryChangedCompany = function () {
        var _this = this;
        var query = '';
        var querychar = '';
        if (this.q.status != undefined && this.q.status != '') {
            if (this.q.status == '1') {
                var today = this.datepipe.transform(new Date(), 'yyyy-MM-dd');
                ;
                this.q.tungay = today;
                this.q.denngay = today;
                query += "( sa.NGAYDATHANG >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND sa.NGAYDATHANG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
                querychar += "( tr.GIOXONG >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND tr.GIOXONG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
            }
            if (this.q.status == '2') {
                var today = new Date();
                this.q.tungay = this.datepipe.transform(new Date(today.setDate(today.getDate() - today.getDay() + 1)), 'yyyy-MM-dd');
                this.q.denngay = this.datepipe.transform(new Date(today.setDate(today.getDate() - today.getDay() + 7)), 'yyyy-MM-dd');
                ;
                query += "( sa.NGAYDATHANG >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND sa.NGAYDATHANG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
                querychar += "( tr.GIOXONG >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND tr.GIOXONG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
            }
            if (this.q.status == '3') {
                var today = new Date();
                this.q.tungay = this.datepipe.transform(new Date(today.setDate(today.getDate() - today.getDay() + 1 - 7)), 'yyyy-MM-dd');
                var today1 = new Date();
                this.q.denngay = this.datepipe.transform(new Date(today1.setDate(today1.getDate() - today1.getDay())), 'yyyy-MM-dd');
                query += "( sa.NGAYDATHANG >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND sa.NGAYDATHANG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
                querychar += "( tr.GIOXONG >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND tr.GIOXONG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
            }
            if (this.q.status == '4') {
                var today2 = new Date();
                var lastDayOfMonth = new Date(today2.getFullYear(), today2.getMonth(), 2);
                this.q.tungay = this.datepipe.transform(lastDayOfMonth, 'yyyy-MM-dd');
                var today3 = new Date();
                var lastDayOfMonth1 = new Date(today3.getFullYear(), today3.getMonth() + 1, 1);
                this.q.denngay = this.datepipe.transform(lastDayOfMonth1, 'yyyy-MM-dd');
                query += "( sa.NGAYDATHANG >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND sa.NGAYDATHANG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
                querychar += "( tr.GIOXONG >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND tr.GIOXONG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
            }
        }
        if (this.q.CompanyId != undefined) {
            this.companyselectsearch = this.q.CompanyId;
            this.listBranchSearch = [];
            this.q.BranchId = null;
            this.GetListBranchSearch();
            this.paging.CompanyId = this.q.CompanyId;
        }
        else {
            this.paging.CompanyId = 0;
        }
        if (this.q.Branchlist != undefined) {
            this.paging.Branchlist = this.q.Branchlist;
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
        if (query == '') {
            this.paging.query = '1=1';
        }
        else {
            this.paging.query = query;
        }
        if (querychar == '') {
            this.paging.querychar = '1=1';
        }
        else {
            this.paging.querychar = querychar;
        }
        localStorage.setItem('thoigiand', this.q.status);
        localStorage.setItem('congtyd', this.q.CompanyId);
        localStorage.setItem('tramtrond', this.q.Branchlist);
        this.GetListOrder();
        this.GetListOrderDH();
        this.GetListOrderChart();
    };
    DashboardComponent = __decorate([
        core_1.Component({
            providers: [common_1.Location, {
                    provide: common_1.LocationStrategy,
                    useClass: common_1.PathLocationStrategy
                }],
            templateUrl: 'dashboard.component.html'
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            ngx_toastr_1.ToastrService, common_2.DatePipe])
    ], DashboardComponent);
    return DashboardComponent;
}());
exports.DashboardComponent = DashboardComponent;
//# sourceMappingURL=dashboard.component.js.map
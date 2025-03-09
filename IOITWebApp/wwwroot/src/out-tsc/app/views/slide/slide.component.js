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
var const_1 = require("../../data/const");
var dt_1 = require("../../data/dt");
var common_service_1 = require("../../service/common.service");
var SlideComponent = /** @class */ (function () {
    function SlideComponent(http, modalDialogService, viewRef, toastr, commonService) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.commonService = commonService;
        this.listSlide = [];
        this.listSlideTong = [];
        this.listTenMacBeTong = [];
        this.listKH = [];
        this.listBienSo = [];
        this.listCompany = [];
        this.listBranchSearch = [];
        this.ListVatLieu = [];
        this.ListNV = [];
        this.tablesilde = [];
        this.domainImage = const_1.domainImage;
        this.paging = {
            page: 1,
            page_size: 10,
            query: '(1=1)',
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
    SlideComponent.prototype.ngOnInit = function () {
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        var today = new Date().toISOString().substr(0, 10);
        this.q.tungay = today;
        this.q.denngay = today;
        this.paging.query = '(1=1)';
        if (this.q.tungay != undefined && this.q.tungay != undefined && this.q.denngay != '' && this.q.denngay != '') {
            this.paging.query += "AND ( [t2].GIOBATDAU >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND [t2].GIOXONG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
        }
        //
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
        this.q.ckbKhachHang = true;
        this.q.ckbXeTron = true;
        this.q.ckbMacBeTong = true;
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
        if (localStorage.getItem('getlinkd') == "1") {
            if (localStorage.getItem('thoigiand') == '1') {
                var today_1 = new Date().toISOString().substr(0, 10);
                this.q.tungay = today_1;
                this.q.denngay = today_1;
                this.paging.query = "( [t2].GIOBATDAU >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND [t2].GIOXONG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
            }
            if (localStorage.getItem('thoigiand') == '2') {
                var today_2 = new Date();
                this.q.tungay = new Date(today_2.setDate(today_2.getDate() - today_2.getDay() + 1)).toISOString().substr(0, 10);
                this.q.denngay = new Date(today_2.setDate(today_2.getDate() - today_2.getDay() + 7)).toISOString().substr(0, 10);
                this.paging.query = "( [t2].GIOBATDAU >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND [t2].GIOXONG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
            }
            if (localStorage.getItem('thoigiand') == '3') {
                var today_3 = new Date();
                this.q.tungay = new Date(today_3.setDate(today_3.getDate() - today_3.getDay() + 1 - 7)).toISOString().substr(0, 10);
                var today1 = new Date();
                this.q.denngay = new Date(today1.setDate(today1.getDate() - today1.getDay())).toISOString().substr(0, 10);
                this.paging.query = "( [t2].GIOBATDAU >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND [t2].GIOXONG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
            }
            if (localStorage.getItem('thoigiand') == '4') {
                var today2 = new Date();
                var lastDayOfMonth = new Date(today2.getFullYear(), today2.getMonth(), 2);
                this.q.tungay = lastDayOfMonth.toISOString().substr(0, 10);
                var today3 = new Date();
                var lastDayOfMonth1 = new Date(today3.getFullYear(), today3.getMonth() + 1, 1);
                this.q.denngay = lastDayOfMonth1.toISOString().substr(0, 10);
                this.paging.query = "( [t2].GIOBATDAU >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND [t2].GIOXONG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
            }
        }
        //this.GetListSlide();
        this.GetlistKH();
        this.GetlistBienSo();
        this.GetlistTenMacBeTong();
        this.GetlistNV();
    };
    SlideComponent.prototype.TaoTable = function () {
        this.tablesilde = [];
        this.GetListVatLieu();
    };
    // Get danh sách slide
    SlideComponent.prototype.GetListSlide = function () {
        var _this = this;
        this.listSlide = [];
        this.querytable = '';
        this.TaoTable();
        if (this.paging.CompanyId == undefined) {
            this.paging.CompanyId = 0;
        }
        this.http.get('/api/slide/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&order_by=' + this.paging.order_by + '&tungay=' + this.q.tungay + '&denngay=' + this.q.denngay + '&TENKHACHHANG=' + this.q.TENKHACHHANG + '&BIENSO=' + this.q.BIENSO + '&TENMACBETONG=' + this.q.TENMACBETONG + '&TENNV=' + this.q.TENNV + '&companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid + '&query=' + this.paging.query + '&status=' + this.q.status + '&ckbKhachHang=' + this.q.ckbKhachHang + '&ckbXeTron=' + this.q.ckbXeTron + '&ckbMacBeTong=' + this.q.ckbMacBeTong, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listSlide = res["data"];
                _this.listSlideTong = res["data1"];
                _this.tongKg = res["tongKg"];
                _this.tongm3 = res["tongm3"];
                _this.paging.item_count = res["metadata"];
                _this.querytable = '';
                if (_this.listSlide.length > 0) {
                    _this.querytable += '<table class="table table-bordered text-nowrap" id="DataTables_Table_0" role="grid" aria-describedby="DataTables_Table_0_info">';
                    _this.querytable += '<thead><tr>';
                    for (var _i = 0, _a = _this.tablesilde; _i < _a.length; _i++) {
                        var child = _a[_i];
                        //this.querytable += '<th style="position: sticky; top: 0px; z-index: 99; color: white; background: #3c8dbc;">'
                        _this.querytable += '<th class="th-custom">';
                        _this.querytable += child.nametable;
                        _this.querytable += '</th>';
                    }
                    _this.querytable += '</tr></thead><tbody>';
                    for (var i = 0; i < _this.listSlide.length; i++) {
                        var item = _this.listSlide[i].ItemArray;
                        _this.querytable += '<tr role="row">';
                        for (var j = 0; j < _this.listSlide[i].ItemArray.length; j++) {
                            _this.querytable += '<td>';
                            _this.querytable += item[j];
                            _this.querytable += '</td>';
                        }
                        _this.querytable += '</tr>';
                    }
                    _this.querytable += '</tbody></table>';
                }
            }
            else {
                _this.querytable = '';
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    SlideComponent.prototype.ExportExcel = function () {
        var _this = this;
        if (this.paging.CompanyId == undefined) {
            this.paging.CompanyId = 0;
        }
        var tenchinhanh = this.listBranchSearch.find(function (x) { return x.BranchId == _this.paging.Branchid; });
        //  debugger;
        fetch('/api/slide/GetReport?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&order_by=' + this.paging.order_by + '&tungay=' + this.q.tungay + '&denngay=' + this.q.denngay + '&TENKHACHHANG=' + this.q.TENKHACHHANG + '&BIENSO=' + this.q.BIENSO + '&TENMACBETONG=' + this.q.TENMACBETONG + '&TENNV=' + this.q.TENNV + '&companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid + '&query=' + this.paging.query + '&status=' + this.q.status + '&ckbKhachHang=' + this.q.ckbKhachHang + '&ckbXeTron=' + this.q.ckbXeTron + '&ckbMacBeTong=' + this.q.ckbMacBeTong, {
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
            _this.DownloadFile(blob, _this.commonService.ConvertUrl(tenchinhanh.Name) + "_thong_ke_dn_hang_ngay_" + date + ".xlsx", 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
            //   this.DownloadFile(blob,   "_thong_ke_dn_hang" + date + ".xlsx", 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
        });
    };
    SlideComponent.prototype.DownloadFile = function (data, filename, mime) {
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
    // Get danh sách khách hàng
    SlideComponent.prototype.GetlistKH = function () {
        var _this = this;
        this.http.get('/api/slide/GetKH', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listKH = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    // Get danh sách nhân viên kinh doanh
    SlideComponent.prototype.GetlistNV = function () {
        var _this = this;
        this.http.get('/api/slide/GetNV', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.ListNV = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Get danh sách biển số
    SlideComponent.prototype.GetlistBienSo = function () {
        var _this = this;
        this.http.get('/api/slide/GetBienSo', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listBienSo = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Get danh sách typeAttributeItem
    SlideComponent.prototype.GetlistTenMacBeTong = function () {
        var _this = this;
        this.http.get('/api/slide/GetTenMacBeTong', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listTenMacBeTong = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    SlideComponent.prototype.GetListCompany = function () {
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
                    _this.GetListSlide();
                    _this.GetlistKH();
                    _this.GetlistBienSo();
                    _this.GetlistTenMacBeTong();
                    _this.GetlistNV();
                }
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    SlideComponent.prototype.GetListBranchSearchStart = function () {
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
                    _this.GetListSlide();
                    _this.GetlistKH();
                    _this.GetlistBienSo();
                    _this.GetlistTenMacBeTong();
                    _this.GetlistNV();
                }
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    SlideComponent.prototype.GetListBranchCTCon = function () {
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
    SlideComponent.prototype.GetListBranchSearch = function () {
        var _this = this;
        this.http.get('/api/branch/GetByPage?page=1&query=CompanyId = ' + this.companyselectsearch + ' &order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listBranchSearch = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    SlideComponent.prototype.GetListVatLieu = function () {
        var _this = this;
        this.http.get('/api/slide/GetVatLieu?companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.ListVatLieu = res["data"];
                //Tạo table
                var obj = new dt_1.tablesilde();
                obj.name = 'NGAYTRON';
                obj.nametable = 'NGÀY';
                obj.nameget = 'item.NGAYTRON';
                _this.tablesilde.push(obj);
                //
                var obj1 = new dt_1.tablesilde();
                obj1.name = 'GIOBATDAU';
                obj1.nametable = 'BẮT ĐẦU';
                obj1.nameget = 'item.GIOBATDAU';
                _this.tablesilde.push(obj1);
                //
                var obj2 = new dt_1.tablesilde();
                obj2.name = 'GIOXONG';
                obj2.nametable = 'KẾT THÚC';
                obj2.nameget = 'item.GIOXONG';
                _this.tablesilde.push(obj2);
                //
                var obj3 = new dt_1.tablesilde();
                obj3.name = 'TENKHACHHANG';
                obj3.nametable = 'TÊN KHÁCH HÀNG';
                obj3.nameget = 'item.TENKHACHHANG';
                _this.tablesilde.push(obj3);
                //
                var obj4 = new dt_1.tablesilde();
                obj4.name = 'BIENSO';
                obj4.nametable = 'BIỂN XE';
                obj4.nameget = 'item.BIENSO';
                _this.tablesilde.push(obj4);
                //
                var obj5 = new dt_1.tablesilde();
                obj5.name = 'TENMACBETONG';
                obj5.nametable = 'MÁC BÊ TÔNG';
                obj5.nameget = 'item.TENMACBETONG';
                _this.tablesilde.push(obj5);
                //
                var obj7 = new dt_1.tablesilde();
                obj7.name = 'TENNV';
                obj7.nametable = 'NV KINH DOANH';
                obj7.nameget = 'item.TENNV';
                _this.tablesilde.push(obj7);
                //
                var obj6 = new dt_1.tablesilde();
                obj6.name = 'M3METRON';
                obj6.nametable = 'THỂ TÍCH';
                obj6.nameget = 'item.M3METRON';
                _this.tablesilde.push(obj6);
                //
                if (_this.ListVatLieu.length > 0) {
                    for (var i = 0; i < _this.ListVatLieu.length; i++) {
                        var obj_1 = new dt_1.tablesilde();
                        obj_1.name = _this.ListVatLieu[i].TENCUAVL;
                        obj_1.nametable = _this.ListVatLieu[i].TENCUAVL;
                        obj_1.nameget = 'item.' + _this.ListVatLieu[i].TENCUAVL + '';
                        _this.tablesilde.push(obj_1);
                        if (_this.ListVatLieu[i].COPHAIPHUGIA != 1) {
                            var obj1_1 = new dt_1.tablesilde();
                            obj1_1.name = "T." + _this.ListVatLieu[i].TENCUAVL;
                            obj1_1.nametable = "T." + _this.ListVatLieu[i].TENCUAVL;
                            obj1_1.nameget = 'item.T.' + _this.ListVatLieu[i].TENCUAVL + '';
                            _this.tablesilde.push(obj1_1);
                        }
                    }
                }
                if (_this.q.status == "1") {
                    var obj_2 = new dt_1.tablesilde();
                    obj_2.name = 'TENPHUGIA';
                    obj_2.nametable = 'TÊN PHỤ GIA';
                    obj_2.nameget = 'item.TENPHUGIA';
                    _this.tablesilde.push(obj_2);
                    var obj5_1 = new dt_1.tablesilde();
                    obj5_1.name = 'name';
                    obj5_1.nametable = 'TRẠM TRỘN';
                    obj5_1.nameget = 'item.name';
                    _this.tablesilde.push(obj5_1);
                }
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    SlideComponent.prototype.GetBranch = function () {
        this.companyselectsearch = this.q.CompanyId;
        this.listBranchSearch = [];
        this.q.BranchId = null;
        this.GetListBranchSearch();
        this.paging.CompanyId = this.q.CompanyId;
    };
    //Chuyển trang
    SlideComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListSlide();
    };
    //Toast cảnh báo
    SlideComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    //Toast thành công
    SlideComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    //Toast thành công
    SlideComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    SlideComponent.prototype.changeStatusckbKhachHang = function (event) {
        this.q.ckbKhachHang = event;
    };
    //
    SlideComponent.prototype.QueryChanged = function () {
        if (this.q.Branchlist == undefined || this.q.Branchlist == '') {
            this.toastr.warning('Chưa chọn trạm trộn !', 'Cảnh báo');
            return;
        }
        var query = '';
        if (this.q.tungay != undefined && this.q.tungay != undefined && this.q.denngay != '' && this.q.denngay != '') {
            if (query != '') {
                query += " AND ( [t2].GIOBATDAU >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND [t2].GIOXONG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
            }
            else {
                query += "( [t2].GIOBATDAU >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND [t2].GIOXONG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
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
        }
        if (query == '')
            this.paging.query = '(1=1)';
        else
            this.paging.query = query;
        this.GetListSlide();
    };
    SlideComponent.prototype.SortTable = function (str) {
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
        this.GetListSlide();
    };
    SlideComponent.prototype.GetClassSortTable = function (str) {
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
        core_1.ViewChild('SlideModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], SlideComponent.prototype, "SlideModal", void 0);
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], SlideComponent.prototype, "file", void 0);
    SlideComponent = __decorate([
        core_1.Component({
            selector: 'app-slide',
            templateUrl: './slide.component.html',
            styleUrls: ['./slide.component.scss'],
            styles: ['.th-custom { position: sticky !important; top: -1px !important; z-index: 99 !important; color: white !important; background: #3c8dbc !important; }'],
            encapsulation: core_1.ViewEncapsulation.None
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService,
            common_service_1.CommonService])
    ], SlideComponent);
    return SlideComponent;
}());
exports.SlideComponent = SlideComponent;
//# sourceMappingURL=slide.component.js.map
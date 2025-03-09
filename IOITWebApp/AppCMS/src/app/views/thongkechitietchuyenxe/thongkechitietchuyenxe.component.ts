import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef, ViewEncapsulation } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { ToastrService } from 'ngx-toastr';
import { domainImage } from '../../data/const';
import { Slide } from '../../data/Model';
import { Paging, QueryFilter, tablesilde } from '../../data/dt';
import { forEach } from '@angular/router/src/utils/collection';
import { debug } from 'util';
import { CommonService } from '../../service/common.service';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { Injectable } from '@angular/core'; Router

import { DatePipe } from '@angular/common'

@Component({
  selector: 'app-thongkechitietchuyenxe',
  templateUrl: './thongkechitietchuyenxe.component.html',
  styles: ['.th-custom { position: sticky !important; top: -1px !important; z-index: 99 !important; color: white !important; background: #3c8dbc !important; }'],
  encapsulation: ViewEncapsulation.None
})
export class ThongKeChiTietChuyenXeComponent implements OnInit {
  @ViewChild('SlideModal') public SlideModal: ModalDirective;
  @ViewChild('file') file: ElementRef;
  subscription: Subscription;
  public paging: any;
  public q: any;
  public listSlide = [];
  public listSlideTong = [];
  public listCheDo = [];
  public listCompany = [];
  public listBranchSearch = [];
  public ListVatLieu = [];
  public tableMockData = [];
  public tableMockDataTong = [];
  public ckeConfig: any;
  public tablesilde = [];
  public listChiTietChuyenXeDetail = [];
  public listChiTietChuyenXeTong = [];
  public role: number;
  public disable: boolean = true;
  public loading: boolean;
  public listBienSo = [];
  companyId: number;
  BranchId: string;
  tongKg: string;
  tongm3: string;
  querytable: string;
  UserId: number;
  actionView: boolean;
  actionCreate: boolean;
  actionUpdate: boolean;
  actionDelete: boolean;
  actionImport: boolean;
  actionExport: boolean;
  actionPrint: boolean;
  access_key: string;
  functionRole: string; 
  public companyselect: number;
  public companyselectsearch: number;


  public progress: number;
  public message: string;
  public domainImage = domainImage;
  public httpOptions: any;

  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
    public commonService: CommonService,
    public datepipe: DatePipe,
    private SpinnerService: Ng4LoadingSpinnerService,
    public router: Router
  ) {
    this.paging = {
      page: 1,
      page_size: 10,
      query: '(1=1)',
      order_by: 'GIOKETTHUC Desc',
      item_count: 0
    };

    this.q = {
      txtSearch: ''
    }

    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      })
    }
  }

  ngOnInit() {
    this.ckeConfig = {
      allowedContent: false,
      extraPlugins: 'divarea',
      forcePasteAsPlainText: true
    };
    var dateNow = new Date();
    let today = this.datepipe.transform(dateNow, 'yyyy-MM-dd');
    var hour = dateNow.getHours(),
      min = dateNow.getMinutes();

    this.q.tungay = today;
    this.q.denngay = today;
    this.q.timetungay = "00:00";
    this.q.timedenngay = ("0" + hour).slice(-2) + ":" + ("0" + min).slice(-2);

    var json = JSON.parse(localStorage.getItem('roles'));
    this.companyId = parseInt(localStorage.getItem('companyId'));

    this.UserId = parseInt(localStorage.getItem('userId'));
    this.BranchId = localStorage.getItem('BranchId');
    this.access_key = localStorage.getItem('access_key');
    this.checkRoleByCode();
    if (json.length > 0) {
      for (var i = 0; i < json.length; i++) {
        this.role = json[i].RoleId
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
    // if (this.role == 3) {

    // }
    // if (this.role == 4) {

    // }
    if (this.role == 1) {
      this.GetListCompany();

    }
    else {
      if (this.role != 1 && this.role != 3) {
        this.paging.Branchid = this.BranchId;
      }
      this.GetListBranchSearchStart();
      this.GetListBranchCTCon();
      //this.paging.Branchid = this.BranchId;

    }
    if (localStorage.getItem('getlinkd') == "1") {
      if (localStorage.getItem('thoigiand') == '1') {
        let today = this.datepipe.transform(new Date(), 'yyyy-MM-dd');
        this.q.tungay = today;
        this.q.denngay = today;
        this.paging.query = "( [D].GIOBATDAU >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND [D].GIOXONG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
      }
      if (localStorage.getItem('thoigiand') == '2') {
        let today = new Date();
        this.q.tungay = new Date(today.setDate(today.getDate() - today.getDay() + 1)).toISOString().substr(0, 10);
        this.q.denngay = new Date(today.setDate(today.getDate() - today.getDay() + 7)).toISOString().substr(0, 10);
        this.paging.query = "( [D].GIOBATDAU >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND [D].GIOXONG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
      }
      if (localStorage.getItem('thoigiand') == '3') {
        let today = new Date();
        this.q.tungay = new Date(today.setDate(today.getDate() - today.getDay() + 1 - 7)).toISOString().substr(0, 10);
        let today1 = new Date();
        this.q.denngay = new Date(today1.setDate(today1.getDate() - today1.getDay())).toISOString().substr(0, 10);
        this.paging.query = "( [D].GIOBATDAU >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND [D].GIOXONG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
      }
      if (localStorage.getItem('thoigiand') == '4') {
        var today2 = new Date();
        var lastDayOfMonth = new Date(today2.getFullYear(), today2.getMonth(), 2);
        this.q.tungay = lastDayOfMonth.toISOString().substr(0, 10);

        var today3 = new Date();
        var lastDayOfMonth1 = new Date(today3.getFullYear(), today3.getMonth() + 1, 1);
        this.q.denngay = lastDayOfMonth1.toISOString().substr(0, 10);
        this.paging.query = "( [D].GIOBATDAU >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND [D].GIOXONG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
      }
    }
    //this.GetListSlide();


  }

  checkRoleByCode(){
    var functionRole = this.access_key.split('-');
    functionRole.forEach(item => {
      var code = item.split(':')[0];
      if(code == 'TKCTCX') {
        this.functionRole = item.split(':')[1];
      }
    });
    this.actionExport = this.functionRole.charAt(5) == "1" ? true: false;
  }

  ResetCurrentRouter() {
    this.router.routeReuseStrategy.shouldReuseRoute = function () {
      return false;
    };
    this.router.onSameUrlNavigation = 'reload';
    this.router.navigateByUrl(this.router.url);
  }
  changeFn(val) {
    this.disable = false;
    this.GetlistBienSo(val);
    this.GetlistCD();

  }
  // Get danh sách slide
  GetListSlide() {
    this.SpinnerService.show();

    this.listChiTietChuyenXeTong = [];
    this.listChiTietChuyenXeDetail = [];

    if (this.paging.CompanyId == undefined) {
      this.paging.CompanyId = 0;
    }
    this.http.get('/api/thongkechitietchuyenxe/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&order_by=' + this.paging.order_by 
    + '&tungay=' + this.q.tungay 
    + '&denngay=' + this.q.denngay
    + '&timetungay=' + this.q.timetungay
    + '&timedenngay=' + this.q.timedenngay 
    + '&BIENSO=' + this.q.BIENSO + '&CHEDO=' + this.q.CHEDO + '&companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listChiTietChuyenXeDetail = res["data"];
          this.listChiTietChuyenXeTong = res["data1"];
          this.paging.item_count = res["metadata"];

          this.tableMockData = this.listChiTietChuyenXeDetail;
          this.tableMockDataTong = this.listChiTietChuyenXeTong;

          this.SpinnerService.hide();
        }
      },
      (err) => {
        this.SpinnerService.hide();
        console.log("Error: connect to API");
      }
    );
  }
  ExportExcel() {
    if (this.role == 1) {
      if (this.q.CompanyId == undefined || this.q.CompanyId == null) {
        this.toastr.warning('Vui lòng chọn công ty !', 'Cảnh báo');
        return;
      }
    }

    if (this.q.Branchlist == undefined || this.q.Branchlist == '') {
      this.toastr.warning('Vui lòng chọn trạm trộn !', 'Cảnh báo');
      return;
    }
    let query = '';
    if (this.paging.CompanyId == undefined) {
      this.paging.CompanyId = 0;
    }
    if (this.q.Branchlist != undefined) {
      this.paging.Branchid = this.q.Branchlist;
    }


    if (this.q.tungay != undefined && this.q.tungay != undefined && this.q.denngay != '' && this.q.denngay != '') {
      var _tungay = this.datepipe.transform(this.q.tungay, 'yyyy-MM-dd');
      var _denngay = this.datepipe.transform(this.q.denngay, 'yyyy-MM-dd');
      this.paging.query += "AND  [D].GIOBATDAU > '" + _tungay + " 00:00:00.000' AND [D].GIOXONG < '" + _denngay + " 23:59:59.999' ";
    }

    if (this.q.BIENSO != undefined && this.q.BIENSO != '') {
      this.paging.query += " AND [D].BIENSO='" + this.q.BIENSO + "'";
    }
    if (this.q.CHEDO != undefined && this.q.CHEDO != '') {
      this.paging.query += " AND [E].CHEDO='" + this.q.CHEDO + "'";
    }

    let tenchinhanh = this.listBranchSearch.find(x => x.BranchId == this.paging.Branchid);
    //  debugger;
    fetch('/api/thongkechitietchuyenxe/GetReportChiTiet?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&order_by=' + this.paging.order_by 
    + '&tungay=' + this.q.tungay 
    + '&denngay=' + this.q.denngay
    + '&timetungay=' + this.q.timetungay
    + '&timedenngay=' + this.q.timedenngay 
    + '&BIENSO=' + this.q.BIENSO + '&CHEDO=' + this.q.CHEDO + '&companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid, {
      method: 'GET',
      headers: new Headers({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      }),
    })
      .then(function (response) {
        var blob = response.blob();
        return blob;
      }).then(blob => {
        var DateObj = new Date();
        var date = ('0' + DateObj.getDate()).slice(-2) + '_' + ('0' + (DateObj.getMonth() + 1)).slice(-2) + '_' + DateObj.getFullYear();
        this.DownloadFile(blob, this.commonService.ConvertUrl(tenchinhanh.Name) + "_thong_ke_chi_tiet_chuyen-xe_" + date + ".xlsx", 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
        //   this.DownloadFile(blob,   "_thong_ke_dn_hang" + date + ".xlsx", 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
        this.SpinnerService.hide();
      });
  }
  DownloadFile(data: Blob, filename: string, mime: string): void {

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

  }

  GetlistCD() {
    this.listCheDo = [
      { Name: 'All', value: '' },
      { Name: 'Normal', value: 'NORMAL' },
      { Name: 'Simulation', value: 'SIM' }
    ];
  }
  GetListCompany() {
    this.http.get('/api/company/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listCompany = res["data"];
          if (localStorage.getItem('congtyd') != 'undefined') {
            var index = this.listCompany.find(x => x.CompanyId == parseInt(localStorage.getItem('congtyd')));
            this.q.CompanyId = index.CompanyId;
            this.paging.CompanyId = parseInt(localStorage.getItem('congtyd'));
            this.companyId = parseInt(localStorage.getItem('congtyd'));
            this.GetListBranchSearchStart();
            // this.GetListSlide();
          }
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  GetListBranchSearchStart() {
    this.http.get('/api/branch/GetByPage?page=1&query=CompanyId = ' + this.companyId + '&Branchlist=' + this.paging.Branchid + ' &order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listBranchSearch = res["data"];
          if (localStorage.getItem('tramtrond') != 'undefined') {
            this.q.Branchlist = [];
            let lstnhombenh = localStorage.getItem('tramtrond').split(',');
            lstnhombenh.forEach(element => {
              this.q.Branchlist.push(parseInt(element));
            });
            this.GetListSlide();

          }
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  GetListBranchCTCon() {
    this.http.get('/api/Order/GetByUser?id=' + this.UserId, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listBranchSearch = res["data"];
          if (localStorage.getItem('tramtrond') != 'undefined') {
            this.q.Branchlist = [];
            let lstnhombenh = localStorage.getItem('tramtrond').split(',');
            lstnhombenh.forEach(element => {
              this.q.Branchlist.push(parseInt(element));
            });
          }
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  GetListBranchSearch() {
    this.http.get('/api/branch/GetByPage?page=1&query=CompanyId = ' + this.companyselectsearch + ' &order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listBranchSearch = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  //Get danh sách biển số
  GetlistBienSo(val) {
    this.http.get('/api/thongkedonhangchitiet/GetBienSo/' + val, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listBienSo = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  GetBranch() {
    this.companyselectsearch = this.q.CompanyId;
    this.listBranchSearch = [];
    this.q.BranchId = null;
    this.GetListBranchSearch();
    this.paging.CompanyId = this.q.CompanyId;
  }
  //Chuyển trang
  PageChanged(event) {
    this.paging.page = event.page;
    this.GetListSlide();
  }

  //Toast cảnh báo
  toastWarning(msg): void {
    this.toastr.warning(msg, 'Cảnh báo');
  }

  //Toast thành công
  toastSuccess(msg): void {
    this.toastr.success(msg, 'Hoàn thành');
  }

  //Toast thành công
  toastError(msg): void {
    this.toastr.error(msg, 'Lỗi');
  }
  //
  changeStatusckbKhachHang(event) {

    this.q.ckbKhachHang = event;
  }
  //
  QueryChanged() {

    if (this.role == 1) {
      if (this.q.CompanyId == undefined || this.q.CompanyId == null) {
        this.toastr.warning('Vui lòng chọn công ty !', 'Cảnh báo');
        return;
      }
    }

    if (this.q.Branchlist == undefined || this.q.Branchlist == '') {
      this.toastr.warning('Vui lòng chọn trạm trộn !', 'Cảnh báo');
      return;
    }
    let query = '';
    if (this.paging.CompanyId == undefined) {
      this.paging.CompanyId = 0;
    }
    if (this.q.Branchlist != undefined) {
      this.paging.Branchid = this.q.Branchlist;
    }


    if (this.q.tungay != undefined && this.q.tungay != undefined && this.q.denngay != '' && this.q.denngay != '') {
      var _tungay = this.datepipe.transform(this.q.tungay, 'yyyy-MM-dd');
      var _denngay = this.datepipe.transform(this.q.denngay, 'yyyy-MM-dd');
      this.paging.query += "AND  [D].GIOBATDAU > '" + _tungay + " 00:00:00.000' AND [D].GIOXONG < '" + _denngay + " 23:59:59.999' ";
    }

    if (this.q.BIENSO != undefined && this.q.BIENSO != '') {
      this.paging.query += " AND [D].BIENSO='" + this.q.BIENSO + "'";
    }
    if (this.q.CHEDO != undefined && this.q.CHEDO != '') {
      this.paging.query += " AND [E].CHEDO='" + this.q.CHEDO + "'";
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

    this.GetListSlide();
  }

  RoundData(val) {

    if (!isNaN(Number(val))) {
      if (this.isInt(val)) {
        return val;
      }
      else {
        return Number(val).toFixed(2);
      }
    } else {
      return val;
    }
  }

  isInt(n) {
    return n % 1 === 0;
  }

  SortTable(str) {
    let First = "";
    let Last = "";
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
  }

  GetClassSortTable(str) {
    if (this.paging.order_by != (str + " Desc") && this.paging.order_by != (str + " Asc")) {
      return "sorting";
    }
    else {
      if (this.paging.order_by == (str + " Desc")) return "sorting_desc";
      else return "sorting_asc";
    }
  }
}

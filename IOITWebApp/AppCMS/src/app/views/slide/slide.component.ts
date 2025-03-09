import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef, ViewEncapsulation } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { ToastrService } from 'ngx-toastr';
import { domainImage } from '../../data/const';
import { Slide } from '../../data/model';
import { Paging, QueryFilter, tablesilde } from '../../data/dt';
import { forEach } from '@angular/router/src/utils/collection';
import { debug } from 'util';
import { CommonService } from '../../service/common.service';

@Component({
  selector: 'app-slide',
  templateUrl: './slide.component.html',
  styleUrls: ['./slide.component.scss'],
  styles: ['.th-custom { position: sticky !important; top: -1px !important; z-index: 99 !important; color: white !important; background: #3c8dbc !important; }'],
  encapsulation: ViewEncapsulation.None
})
export class SlideComponent implements OnInit {
  @ViewChild('SlideModal') public SlideModal: ModalDirective;
  @ViewChild('file') file: ElementRef;

  public paging: any;
  public q: any;
  public listSlide = [];
  public listSlideTong = [];
  public listTenMacBeTong = [];
  public listKH = [];
  public listBienSo = [];
  public listCompany = [];
  public listBranchSearch = [];
  public ListVatLieu = [];
  public ListNV = [];
  public ckeConfig: any;
  public tablesilde = [];

  public role: number;
  companyId: number;
  BranchId: string;
  tongKg: string;
  tongm3: string;
  querytable: string;
  UserId: number;
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
    public commonService: CommonService
  ) {
    this.paging = {
      page: 1,
      page_size: 10,
      query: '(1=1)',
      order_by: 'NGAYDATHANG Desc',
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
    let today = new Date().toISOString().substr(0, 10);
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
        let today = new Date().toISOString().substr(0, 10);
        this.q.tungay = today;
        this.q.denngay = today;
        this.paging.query = "( [t2].GIOBATDAU >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND [t2].GIOXONG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
      }
      if (localStorage.getItem('thoigiand') == '2') {
        let today = new Date();
        this.q.tungay = new Date(today.setDate(today.getDate() - today.getDay() + 1)).toISOString().substr(0, 10);
        this.q.denngay = new Date(today.setDate(today.getDate() - today.getDay() + 7)).toISOString().substr(0, 10);
        this.paging.query = "( [t2].GIOBATDAU >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND [t2].GIOXONG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
      }
      if (localStorage.getItem('thoigiand') == '3') {
        let today = new Date();
        this.q.tungay = new Date(today.setDate(today.getDate() - today.getDay() + 1 - 7)).toISOString().substr(0, 10);
        let today1 = new Date();
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
  }
  TaoTable() {
    this.tablesilde = [];

    this.GetListVatLieu();
  }
  // Get danh sách slide
  GetListSlide() {
    this.listSlide = [];
    this.querytable = '';
    this.TaoTable();
    if (this.paging.CompanyId == undefined) {
      this.paging.CompanyId = 0;
    }
    this.http.get('/api/slide/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&order_by=' + this.paging.order_by + '&tungay=' + this.q.tungay + '&denngay=' + this.q.denngay + '&TENKHACHHANG=' + this.q.TENKHACHHANG + '&BIENSO=' + this.q.BIENSO + '&TENMACBETONG=' + this.q.TENMACBETONG + '&TENNV=' + this.q.TENNV + '&companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid + '&query=' + this.paging.query + '&status=' + this.q.status + '&ckbKhachHang=' + this.q.ckbKhachHang + '&ckbXeTron=' + this.q.ckbXeTron + '&ckbMacBeTong=' + this.q.ckbMacBeTong, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listSlide = res["data"];
          this.listSlideTong = res["data1"];
          this.tongKg = res["tongKg"];
          this.tongm3 = res["tongm3"];
          this.paging.item_count = res["metadata"];
          this.querytable = '';
          if (this.listSlide.length > 0) {
            this.querytable += '<table class="table table-bordered text-nowrap" id="DataTables_Table_0" role="grid" aria-describedby="DataTables_Table_0_info">';
            this.querytable += '<thead><tr>';
            for (let child of this.tablesilde) {
              //this.querytable += '<th style="position: sticky; top: 0px; z-index: 99; color: white; background: #3c8dbc;">'
              this.querytable += '<th class="th-custom">'
              this.querytable += child.nametable
              this.querytable += '</th>'
            }
            this.querytable += '</tr></thead><tbody>';
            for (var i = 0; i < this.listSlide.length; i++) {
              let item = this.listSlide[i].ItemArray;
              this.querytable += '<tr role="row">'
              for (var j = 0; j < this.listSlide[i].ItemArray.length; j++) {
                this.querytable += '<td>'
                this.querytable += item[j]
                this.querytable += '</td>'
              }
              this.querytable += '</tr>'
            }
            this.querytable += '</tbody></table>'
          }

        } else {
          this.querytable = '';

        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  ExportExcel() {
    if (this.paging.CompanyId == undefined) {
      this.paging.CompanyId = 0;
    }
    
    let tenchinhanh = this.listBranchSearch.find(x => x.BranchId == this.paging.Branchid);
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
      }).then(blob => {
        var DateObj = new Date();
        var date = ('0' + DateObj.getDate()).slice(-2) + '_' + ('0' + (DateObj.getMonth() + 1)).slice(-2) + '_' + DateObj.getFullYear();
        this.DownloadFile(blob, this.commonService.ConvertUrl(tenchinhanh.Name) + "_thong_ke_dn_hang_ngay_" + date + ".xlsx", 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
     //   this.DownloadFile(blob,   "_thong_ke_dn_hang" + date + ".xlsx", 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
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
  // Get danh sách khách hàng
  GetlistKH() {
    this.http.get('/api/slide/GetKH', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listKH = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  // Get danh sách nhân viên kinh doanh
  GetlistNV() {
    this.http.get('/api/slide/GetNV', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.ListNV = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  //Get danh sách biển số
  GetlistBienSo() {
    this.http.get('/api/slide/GetBienSo', this.httpOptions).subscribe(
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

  //Get danh sách typeAttributeItem
  GetlistTenMacBeTong() {
    this.http.get('/api/slide/GetTenMacBeTong', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listTenMacBeTong = res["data"];

        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
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
            this.GetListSlide();
            this.GetlistKH();
            this.GetlistBienSo();
            this.GetlistTenMacBeTong();
            this.GetlistNV();
          }
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  GetListBranchSearchStart() {
    this.http.get('/api/branch/GetByPage?page=1&query=CompanyId = ' + this.companyId + ' &order_by=', this.httpOptions).subscribe(
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
            this.GetlistKH();
            this.GetlistBienSo();
            this.GetlistTenMacBeTong();
            this.GetlistNV();
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
  GetListVatLieu() {
    this.http.get('/api/slide/GetVatLieu?companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.ListVatLieu = res["data"];
        
          //Tạo table
          let obj = new tablesilde();
          obj.name = 'NGAYTRON';
          obj.nametable = 'NGÀY';
          obj.nameget = 'item.NGAYTRON';
          this.tablesilde.push(obj);
          //
          let obj1 = new tablesilde();
          obj1.name = 'GIOBATDAU';
          obj1.nametable = 'BẮT ĐẦU';
          obj1.nameget = 'item.GIOBATDAU'
          this.tablesilde.push(obj1);
          //
          let obj2 = new tablesilde();
          obj2.name = 'GIOXONG';
          obj2.nametable = 'KẾT THÚC';
          obj2.nameget = 'item.GIOXONG'
          this.tablesilde.push(obj2);
          //
          let obj3 = new tablesilde();
          obj3.name = 'TENKHACHHANG';
          obj3.nametable = 'TÊN KHÁCH HÀNG';
          obj3.nameget = 'item.TENKHACHHANG'
          this.tablesilde.push(obj3);
          //
          let obj4 = new tablesilde();
          obj4.name = 'BIENSO';
          obj4.nametable = 'BIỂN XE';
          obj4.nameget = 'item.BIENSO'
          this.tablesilde.push(obj4);
          //
          let obj5 = new tablesilde();
          obj5.name = 'TENMACBETONG';
          obj5.nametable = 'MÁC BÊ TÔNG';
          obj5.nameget = 'item.TENMACBETONG'
          this.tablesilde.push(obj5);
          //
          let obj7 = new tablesilde();
          obj7.name = 'TENNV';
          obj7.nametable = 'NV KINH DOANH';
          obj7.nameget = 'item.TENNV'
          this.tablesilde.push(obj7);
          //
          let obj6 = new tablesilde();
          obj6.name = 'M3METRON';
          obj6.nametable = 'THỂ TÍCH';
          obj6.nameget = 'item.M3METRON'
          this.tablesilde.push(obj6);
          //
          if (this.ListVatLieu.length > 0) {

            for (var i = 0; i < this.ListVatLieu.length; i++) {
              let obj = new tablesilde();
              obj.name = this.ListVatLieu[i].TENCUAVL;
              obj.nametable = this.ListVatLieu[i].TENCUAVL;
              obj.nameget = 'item.' + this.ListVatLieu[i].TENCUAVL + ''
              this.tablesilde.push(obj);
              if (this.ListVatLieu[i].COPHAIPHUGIA != 1) {
                let obj1 = new tablesilde();
                obj1.name = "T." + this.ListVatLieu[i].TENCUAVL;
                obj1.nametable = "T." + this.ListVatLieu[i].TENCUAVL;
                obj1.nameget = 'item.T.' + this.ListVatLieu[i].TENCUAVL + ''
                this.tablesilde.push(obj1);
              }
            }
          }
          if (this.q.status == "1") {
            let obj = new tablesilde();
            obj.name = 'TENPHUGIA';
            obj.nametable = 'TÊN PHỤ GIA';
            obj.nameget = 'item.TENPHUGIA'
            this.tablesilde.push(obj);
            let obj5 = new tablesilde();
            obj5.name = 'name';
            obj5.nametable = 'TRẠM TRỘN';
            obj5.nameget = 'item.name'
            this.tablesilde.push(obj5);
          }
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
    if (this.q.Branchlist == undefined || this.q.Branchlist == '') {
      this.toastr.warning('Chưa chọn trạm trộn !', 'Cảnh báo');
      return;
    }
    let query = '';
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

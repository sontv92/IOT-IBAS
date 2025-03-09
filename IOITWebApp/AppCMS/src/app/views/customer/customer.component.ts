import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef, ViewEncapsulation } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { domainImage } from '../../data/const';
import { Customer, ResetPasswordCustomerDTO } from '../../data/model';
import { ToastrService } from 'ngx-toastr';
import { DatePipe } from '@angular/common';
import { Md5 } from 'ts-md5/dist/md5';
import { Paging, QueryFilter, tablesilde } from '../../data/dt';
import { DateTimeAdapter, OWL_DATE_TIME_FORMATS, OWL_DATE_TIME_LOCALE } from 'ng-pick-datetime';
import { MomentDateTimeAdapter } from 'ng-pick-datetime-moment';

@Component({
  selector: 'app-customer',
  templateUrl: './customer.component.html',
  styleUrls: ['./customer.component.scss'],
  styles: ['.th-custom { position: sticky !important; top: -1px !important; z-index: 99 !important; color: white !important; background: #3c8dbc !important; }'],
  encapsulation: ViewEncapsulation.None
})


export class CustomerComponent implements OnInit {
  @ViewChild('CustomerModal') public CustomerModal: ModalDirective;
  @ViewChild('ResetPasswordModal') public ResetPasswordModal: ModalDirective;
  @ViewChild('OrdersModal') public OrdersModal: ModalDirective;

  @ViewChild('file') file: ElementRef;

  public paging: any;
  public pagingItem: any;
  public q: any;

  public listCustomer = [];
  public headerCustomer = [];
  public listCompany = [];
  public listBranchSearch = [];
  public ckeConfig: any;
  public httpOptions: any;
  public role: number;
  companyId: number;
  BranchId: string;
  UserId: number;
  public companyselect: number;
  public companyselectsearch: number;
  public tablesilde = [];
  public ListVatLieu = [];
  querytable: string;

  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
    public datePipe: DatePipe
  ) {

    this.paging = {
      page: 1,
      page_size: 10,
      query: '1=1',
      order_by: 'name Desc',
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

    var json = JSON.parse(localStorage.getItem('roles'));
    this.companyId = parseInt(localStorage.getItem('companyId'));
    this.UserId = parseInt(localStorage.getItem('userId'));
    this.BranchId = localStorage.getItem('BranchId');
    this.companyselect = -1;
    if (json.length > 0) {
      for (var i = 0; i < json.length; i++) {
        this.role = json[i].RoleId
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
      this.paging.Branchid = this.BranchId;
      this.GetListBranchSearchStart();
    }
    if (this.role != 1 && this.role != 3) {
      this.GetListBranchCTCon();
      this.paging.Branchid = this.BranchId;
    }
    if (this.role == 1) {
      this.GetListCompany();
    }
    //Tạo table
    let obj = new tablesilde();
    obj.name = 'MAMACBETONG';
    obj.nametable = 'Mác BT';
    obj.nameget = 'item.MAMACBETONG';
    this.tablesilde.push(obj);
    //
    let obj1 = new tablesilde();
    obj1.name = 'TENMACBETONG';
    obj1.nametable = 'Mác BT';
    obj1.nameget = 'item.TENMACBETONG'
    this.tablesilde.push(obj1);
    this.GetListVatLieu();
    this.GetListCustomer();
  }

  //Get danh sach khach hang
  GetListCustomer() {
    this.http.get('/api/customer/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by + '&companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listCustomer = res["data"];
          this.headerCustomer = res["data1"];
          this.paging.item_count = res["metadata"];
          this.querytable = '';
          this.querytable += '<table class="table table-bordered text-nowrap" id="DataTables_Table_0" role="grid" aria-describedby="DataTables_Table_0_info">';
          this.querytable += '<thead><tr>';
          for (var i = 0; i < this.headerCustomer.length; i++) {
            this.querytable += '<th class="th-custom">'
            this.querytable += this.headerCustomer[i];
            this.querytable += '</th>'
          }
          this.querytable += '</tr></thead><tbody>';
          for (var i = 0; i < this.listCustomer.length; i++) {
            let item = this.listCustomer[i];
            this.querytable += '<tr role="row">'
            for (var j = 0; j < this.listCustomer[i].length; j++) {
              this.querytable += '<td>'
              this.querytable += item[j]
              this.querytable += '</td>'
            }
            this.querytable += '</tr>'
          }
          this.querytable += '</tbody></table>'
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
    fetch('/api/Customer/GetReport?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by + '&companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid, {
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
        this.DownloadFile(blob, "quan_ly_cap_phoi_" + date + ".xlsx", 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
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
  GetListCompany() {
    this.http.get('/api/company/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listCompany = res["data"];
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
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  GetListBranchSearch() {
    this.http.get('/api/branch/GetByPage?page=1&query=CompanyId = ' + this.companyselectsearch + '&Branchlist=' + this.paging.Branchid + ' &order_by=', this.httpOptions).subscribe(
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

  QueryChanged() {

    let query = '';
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
        this.listBranchSearch.forEach((item, index) => {
          if (item != '') {
            this.paging.Branchid = this.paging.Branchid + item["BranchId"] + ',';
          }
        });
      }
    }
    if (query == '')
      this.paging.query = '1=1';
    else
      this.paging.query = query;

    this.GetListCustomer();
  }
  //Chuyển trang
  PageChanged(event) {
    this.paging.page = event.page;
    this.GetListCustomer();
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
  GetBranch() {
    this.companyselectsearch = this.q.CompanyId;
    this.listBranchSearch = [];
    this.q.BranchId = null;
    this.GetListBranchSearch();
    this.paging.CompanyId = this.q.CompanyId;
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

    this.GetListCustomer();
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

  GetListVatLieu() {
    this.http.get('/api/Customer/GetVatLieu', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.ListVatLieu = res["data"];
          if (this.ListVatLieu.length > 0) {

            for (var i = 0; i < this.ListVatLieu.length; i++) {
              let obj = new tablesilde();
              obj.name = this.ListVatLieu[i].TENCUAVL;
              obj.nametable = this.ListVatLieu[i].TENCUAVL;
              obj.nameget = 'item.VL_' + this.ListVatLieu[i].MACUAVL + ''
              this.tablesilde.push(obj);
            }
          }
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
}

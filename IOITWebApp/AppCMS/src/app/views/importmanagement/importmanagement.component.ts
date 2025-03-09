import { Component, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { ToastrService } from 'ngx-toastr';
import { Order, OrderItem, NhapKho } from '../../data/model';
import { OrderStatus, PaymentOrderStatus } from '../../data/const';
import { debug } from 'util';
import { forEach } from '@angular/router/src/utils/collection';
import { ActivatedRoute } from '@angular/router';
import { DatePipe } from '@angular/common';



@Component({
  selector: 'app-importmanagement',
  templateUrl: './importmanagement.component.html',
  styleUrls: ['./importmanagement.component.scss']
})
export class ImportManagement implements OnInit {
  @ViewChild('ViewModal') public viewModal: ModalDirective;

  public paging: any;
  public pagingItem: any;
  public q: any;

  public listOrder = [];
  public Tonkho: NhapKho;
  public dataSum = {};
  public listOrderBranch = [];
  public listCompany = [];
  public listBranchSearch = [];
  public listNV = [];
  public ckeConfig: any;
  public cuavatlieu1: boolean;
  public cuavatlieu2: boolean;
  public cuavatlieu3: boolean;
  public cuavatlieu4: boolean;
  public cuavatlieu5: boolean;
  public cuavatlieu6: boolean;
  public cuavatlieu7: boolean;
  public cuavatlieu8: boolean;
  public cuavatlieu9: boolean;
  public cuavatlieu10: boolean;
  public cuavatlieu11: boolean;
  public cuavatlieu12: boolean;
  public kieudulieu1: boolean;
  public kieudulieu2: boolean;
  public kieudulieu: string;
  public cuavatlieu: string;


  public Item: Order;
 
  public httpOptions: any;
  public listOrderStatus = OrderStatus;
  public listPaymentOrderStatus = PaymentOrderStatus;
  public role: number;
  companyId: number;
  BranchId: string;
  UserId: number;
  public companyselect: number;
  public companyselectsearch: number;
  public totalMETKHOIDATHANG: number;
  public totalMETKHOITICHLUY: number;

  PriceCurrencyMaskConfig = {
    align: "left",
    allowNegative: false,
    decimal: ".",
    precision: 0,
    prefix: "",
    suffix: " Vnđ",
    thousands: ","
  };

  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
    public activatedRoute: ActivatedRoute,
    public datePipe: DatePipe
  ) {
    this.Item = new Order();
    this.Tonkho = new NhapKho();
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
    }

    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      })
    }
  }

  getQueryPar() {
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
  }
  ngOnInit() {

  

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
    let todaynew = this.datePipe.transform(new Date(), 'yyyy-MM-dd');
    this.q.fromdate = todaynew;
    this.q.todate = todaynew;
    this.q.ckbNhapKho = this.kieudulieu1;
    this.q.ckTinhKho = this.kieudulieu2;
    
    if (localStorage.getItem('getlinkd') == "1") {
      if (localStorage.getItem('thoigiand') == '1') {
        let today = this.datePipe.transform(new Date(), 'yyyy-MM-dd');
        this.q.fromdate = today;
        this.q.todate = today;
        this.paging.query = "DATEPART(dy,GETDATE()) = DATEPART(dy,NGAYDATHANG) AND DATEPART(yy,GETDATE()) = DATEPART(yy,NGAYDATHANG)";
      }
      if (localStorage.getItem('thoigiand') == '2') {
        let today = new Date();
        this.q.fromdate = new Date(today.setDate(today.getDate() - today.getDay() + 1)).toISOString().substr(0, 10);
        this.q.todate = new Date(today.setDate(today.getDate() - today.getDay() + 7)).toISOString().substr(0, 10);
        this.paging.query = "DATEPART(wk,GETDATE()) = DATEPART(wk,NGAYDATHANG) AND DATEPART(yy,GETDATE()) = DATEPART(yy,NGAYDATHANG)";
      }
      if (localStorage.getItem('thoigiand') == '3') {
        let today = new Date();
        this.q.fromdate = new Date(today.setDate(today.getDate() - today.getDay() + 1 - 7)).toISOString().substr(0, 10);
        let today1 = new Date();
        this.q.todate = new Date(today1.setDate(today1.getDate() - today1.getDay())).toISOString().substr(0, 10);
        this.paging.query = "(DATEPART(wk,GETDATE()) -1) = DATEPART(wk,NGAYDATHANG) AND DATEPART(yy,GETDATE()) = DATEPART(yy,NGAYDATHANG)";
      }
      if (localStorage.getItem('thoigiand') == '4') {
        var today = new Date();
        var lastDayOfMonth = new Date(today.getFullYear(), today.getMonth(),2);
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

  }

  //test
  TestChange(i) {
    if (i == 1) {
      this.cuavatlieu1 = !this.cuavatlieu1;
    } else if (i == 2) {
      this.cuavatlieu2 = !this.cuavatlieu2;
    } else if (i == 3) {
      this.cuavatlieu3 = !this.cuavatlieu3;
    } else if (i == 4) {
      this.cuavatlieu4 = !this.cuavatlieu4;
    } else if (i == 5) {
      this.cuavatlieu5 = !this.cuavatlieu5;
    } else if (i == 6) {
      this.cuavatlieu6 = !this.cuavatlieu6;
    } else if (i == 7) {
      this.cuavatlieu7 = !this.cuavatlieu7;
    } else if (i == 8) {
      this.cuavatlieu8 = !this.cuavatlieu8;
    } else if (i == 9) {
      this.cuavatlieu9 = !this.cuavatlieu9;
    } else if (i == 10) {
      this.cuavatlieu10 = !this.cuavatlieu10;
    } else if (i == 11) {
      this.cuavatlieu11 = !this.cuavatlieu11;
    } else if (i == 12) {
      this.cuavatlieu12 = !this.cuavatlieu12;
    }
  }
  //Get danh sách danh mục đơn hàng
  GetListOrder() {
    console.log("Error: connect to API");
   
    this.http.get('/api/ImportManagement/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by + '&companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid + '&KDL=' + this.kieudulieu + '&CVL=' + this.cuavatlieu, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listOrder = res["data"];
          this.Tonkho = res["data1"];
          this.dataSum = res["dataSUM"];
          this.paging.item_count = res["metadata"];
          console.log("dataSum: " + this.dataSum);
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
    if (this.q.Branchlist == undefined) {
      this.toastWarning("Bạn chưa chọn trạm trộn!");
    } else {
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
        }).then(blob => {
          var DateObj = new Date();
          var date = ('0' + DateObj.getDate()).slice(-2) + '_' + ('0' + (DateObj.getMonth() + 1)).slice(-2) + '_' + DateObj.getFullYear();
          this.DownloadFile(blob, "don_hang_" + date + ".xlsx", 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
        });
    }
    
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
  //Get danh sách danh mục đơn hàng
  GetListOrderBarch() {
    this.http.get('/api/ImportManagement/GetByBranch?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listOrderBranch = res["data"];
          this.paging.item_count = res["metadata"];
          this.totalMETKHOIDATHANG = 0;
          this.totalMETKHOITICHLUY = 0;
          for (let index in this.listOrderBranch) {
            this.totalMETKHOIDATHANG = this.totalMETKHOIDATHANG + this.listOrderBranch[index].METKHOIDATHANG;
            this.totalMETKHOITICHLUY = this.totalMETKHOITICHLUY + this.listOrderBranch[index].METKHOITICHLUY;
          };
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  // Get danh sách khách hàng
  GetlistKH() {
    this.http.get('/api/ImportManagement/GetNV?companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listNV = res["data"];
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
            this.GetListOrder();
            this.GetListOrderBarch();
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
          }
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  GetListBranchCTCon() {
    this.http.get('/api/ImportManagement/GetByUser?id=' + this.UserId, this.httpOptions).subscribe(
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

  //Chuyển trang
  PageChanged(event) {
    this.paging.page = event.page;
    this.GetListOrder();
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
  QueryChanged() {
    
    let query = '';
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
      this.paging.Branchid  =  this.q.Branchlist;
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
    if (this.q.Branchlist == undefined) {
      this.toastWarning("Bạn chưa chọn trạm trộn!");
    } else {
      this.GetListOrder();
    }
    
   // this.GetListOrderBarch();
  }
  //
  GetBranch() {
    this.companyselectsearch = this.q.CompanyId;
    this.listBranchSearch = [];
    this.q.BranchId = null;
    this.GetListBranchSearch();
    this.paging.CompanyId = this.q.CompanyId;
  }
  //Open modal view
  OpenViewModal(item) {
    this.Item = new Order();
    this.Item = Object.assign({}, item);
    this.viewModal.show();
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

    this.GetListOrder();
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

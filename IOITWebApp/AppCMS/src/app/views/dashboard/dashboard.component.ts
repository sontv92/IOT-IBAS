import { AfterViewInit, Component, OnInit } from '@angular/core';
import { Location, LocationStrategy, PathLocationStrategy } from '@angular/common';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { ToastrService } from 'ngx-toastr';
import { ChartType, ChartOptions,ChartDataSets } from 'chart.js';
import { SingleDataSet, Label, Color } from 'ng2-charts';
import { debug } from 'util';
import * as pluginLabels from 'chartjs-plugin-labels';
 import * as pluginDataLabels from 'chartjs-plugin-datalabels';
 import { DatePipe } from '@angular/common'
 import * as CanvasJS from '@canvasjs/charts';

@Component({
  providers: [Location, {
    provide: LocationStrategy,
    useClass: PathLocationStrategy
  }],
  templateUrl: 'dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements AfterViewInit, OnInit {
  public paging: any;
  public pagingItem: any;
  public q: any;
  public chartPlugins = [pluginDataLabels];

  pieChartOptions: ChartOptions;
  pieChartLabels: Label[];
  pieChartData: any[];
  pieChartType: ChartType;
  pieChartLegend: boolean;
  pieChartPlugins = [];
  dataPo = [];
  cuaVLs = [];

  public tableHopDongTrongNgay =[];
  public listHopDongTrongNgay = [];
  public soPHUTHOANTHANH: string[] = [];

  public barChartLabels: Label[];
  public barChartType: ChartType = 'bar';
  public barChartLegend = false;
  public barChartPlugins = [];

  public barChartData: ChartDataSets[];
  public totalTheTich: 0;
  public time = "";


  optionsGraficoDespesa: ChartOptions = {
    responsive: true,
    title: {
      text: "Vật liệu tiêu hao trong ngày",
      display: true
    },
    tooltips: {
      enabled: false,
      callbacks: {
        label: function (tooltipItem, data) {
          let label = data.labels[tooltipItem.index];
          let count: any = data
            .datasets[tooltipItem.datasetIndex]
            .data[tooltipItem.index];
          return label + ": " + new Intl.NumberFormat("es").format(count);
        },
      },
    },
    plugins: {
      datalabels: {
        formatter: (value, ctx) => {
          let sum = 0;
          let dataArr: any[] = ctx.chart.data.datasets[0].data;
          dataArr.map((data: number) => {
            sum += data;
          });
          let percentage = (value * 100 / sum).toFixed(2) + "%";
          return percentage;
        },
        color: '#ffff',
       // backgroundColor: '#ffff'
      }
    },
    legend: {
      position: 'right',
      display: false,
    }
  };
  
  public barChartOptions: ChartOptions = {
    responsive: true,
    defaultColor: 'red',
    legend : {
      labels : {
        fontColor : '#ffffff'  
      }
  },
    tooltips: {
      callbacks: {
        title: function (tooltipItem, data) {
          return "Ngày: " + tooltipItem[0].label;
        },
        label: function (tooltipItem, data) {
          let label = data.labels[tooltipItem.index];
          let count: any = data
            .datasets[tooltipItem.datasetIndex]
            .data[tooltipItem.index];
          return new Intl.NumberFormat("es").format(count) + " khối";
        }
      }
    },
    hover: {mode: null},
    scales: {
      yAxes: [{
        stacked: true,            // this also..
        ticks: {
          stepSize:20,
          beginAtZero: true,
          callback: function (value, index, values) {
            return  value;
          },
        }
      }],
      // xAxes:[
      //   {
      //     stacked: true,
      //     ticks:{
      //       callback: function(value, index, data) 
      //       {
      //         console.log(value, index, data);
      //         var index = String(value).indexOf(",");
      //         return String(value).substring(0, index);
      //       }

      //     }
      //   }
      // ]
    }

  };

  
 

  public listOrder = [];
  public listOrderCharH = [
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
  public listOrderChartW = [
    { NGAYDATHANG: 'Thứ 2', METKHOITICHLUY: 0 },
    { NGAYDATHANG: 'Thứ 3', METKHOITICHLUY: 0 },
    { NGAYDATHANG: 'Thứ 4', METKHOITICHLUY: 0 },
    { NGAYDATHANG: 'Thứ 5', METKHOITICHLUY: 0 },
    { NGAYDATHANG: 'Thứ 6', METKHOITICHLUY: 0 },
    { NGAYDATHANG: 'Thứ 7', METKHOITICHLUY: 0 },
    { NGAYDATHANG: 'Chủ nhật', METKHOITICHLUY: 0 }
  ];
  public listOrderChartD = [];
  public listCompany = [];
  public listBranchSearch = [];
  public ckeConfig: any;
  

  public httpOptions: any;
  public role: number;
  companyId: number;
  BranchId: string;
  UserId: number;
  public totalName: string[] = [];
  public totaldonhang: number;
  public tongdonhang: number;
  public totalTENNV: number;


  public tongBeTongTronThang: number;
  public tongBeTongTronNgay: number;
  public tongDonHangTrongNgay: number;
  public tongDonHangHoanThanh: number;

  public totalmac: number;
  public totalxe: number;
  public totalMETKHOITICHLUY: string[] = [];
  
  public companyselectsearch: number;
  constructor(public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public datepipe: DatePipe,
    public toastr: ToastrService) {
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
    }

    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      })
    }
  }
 

  public chartColors() {
    return [{
      borderColor: 'rgba(225,10,24,0.2)',
      pointBackgroundColor: 'rgba(225,10,24,0.2)',
      pointBorderColor: '#fff',
      pointHoverBackgroundColor: '#fff',
      pointHoverBorderColor: 'rgba(225,10,24,0.2)'
  }]
}
 // events on slice click
 public chartClicked(e:any):void {
  console.log(e);
}

// event on pie chart slice hover
public chartHovered(e:any):void {
  console.log(e);
}
public lineChartColors: Color[] = [
  { // grey
    backgroundColor: '#FFAE00',
    borderColor: 'rgb(243, 167, 5)',
    pointBackgroundColor: 'rgb(243, 167, 2)',
    pointBorderColor: '#fff',
    pointHoverBackgroundColor: '#fff',
    pointHoverBorderColor: 'rgba(148,159,177,0.8)'
  },
  { // red
    backgroundColor: '#FFAE00',
    borderColor: 'red',
    pointBackgroundColor: 'rgba(148,159,177,1)',
    pointBorderColor: '#fff',
    pointHoverBackgroundColor: '#fff',
    pointHoverBorderColor: 'rgba(148,159,177,0.8)'
  }];

  
  ngOnInit(): void {
    console.log('sdsfdsfsfsdfdsfdsfsd')

   
    this.pieChartType = 'doughnut';
    this.pieChartLegend = true;
    this.pieChartPlugins = [pluginLabels];
    this.pieChartLabels = ['Không có dữ liệu','Không có dữ liệu','Không có dữ liệu','Không có dữ liệu','Không có dữ liệu','Không có dữ liệu','Không có dữ liệu','Không có dữ liệu','Không có dữ liệu','Không có dữ liệu','Không có dữ liệu','Không có dữ liệu'];
    this.pieChartData = [0,0,0,0,0,0,0,0,0,0,0,0];

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
        this.role = json[i].RoleId
      }
    }
    this.q.status = "1";
    this.paging.Branchid = '';
    if (this.role == 3) {
      this.paging.CompanyId = this.companyId;
      
    }
    else {
      this.paging.CompanyId = 0;
    }
    if (this.role != 1 && this.role != 3) {
      this.GetListBranchCTCon();
      this.paging.Branchid = this.BranchId;
    }
    let today = this.datepipe.transform(new Date(), 'yyyy-MM-dd');
    this.q.tungay = today;
    this.q.denngay = today;
    this.paging.query = "( sa.NGAYDATHANG >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND sa.NGAYDATHANG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
    this.paging.querychar = "( tr.GIOXONG >= convert(datetime ,'" + this.q.tungay + " 00:00:00.000') AND tr.GIOXONG <= convert(datetime ,'" + this.q.denngay + " 23:59:59.999') )";
    this.GetListBranchSearchStart();
    // this.GetListOrder();
     this.GetListCompany();
    // this.GetListOrderChart();
    // this.GetListOrderDH();
    this.GetTyLeDaTron();
    
    
  }

  ngAfterViewInit(): void {
    this.pieChartOptions = this.createOptions();
  }
  
  
  public createOptions(): ChartOptions {
    return {
      responsive: true,
          maintainAspectRatio: true,
          plugins: {
              labels: {
                render: 'percentage',
                fontColor: ['red', 'red', 'red'],
                precision: 2
              }
          },
    };
  }
  //Get chưa trộn/ đã trộn
  GetListOrder() {
    this.http.get('/api/Dashboard/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.querychar + '&order_by=' + this.paging.order_by + '&companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {

          this.listOrder = res["data"];
          this.paging.item_count = res["metadata"];
          this.totalMETKHOITICHLUY = [];
          this.totalName = [];
          for (let index in this.listOrder) {
            if (this.totalMETKHOITICHLUY.find(item => item == this.listOrder[index].Name) == undefined) {
              this.totalMETKHOITICHLUY.push(this.listOrder[index].Name);
            }
            if (this.totalName.find(item => item == this.listOrder[index].TENNV) == undefined) {
              this.totalName.push(this.listOrder[index].TENNV);
            }
          };
          this.totalmac = this.totalMETKHOITICHLUY.length;
          this.totalxe = this.totalName.length;

        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  GetListOrderDH() {
    this.http.get('/api/Dashboard/GetByPageDH?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by + '&companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listOrder = res["data"];
          this.paging.item_count = res["metadata"];
          this.totaldonhang = 0;
          this.tongdonhang = 0;
          this.totalTENNV = 0;
          for (let index in this.listOrder) {
            this.totaldonhang = this.totaldonhang + this.listOrder[index].donhang;
            this.tongdonhang = this.tongdonhang + this.listOrder[index].tongdonhang;
            this.totalTENNV = this.totalTENNV + 1;
          };
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  GetListOrderChart() {
    this.http.get('/api/Dashboard/GetByPageChart?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.querychar + '&order_by=' + this.paging.order_by + '&sort=' + this.q.status + '&companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          let data = res["data"];

          let sum = 0;
          if (this.q.status == "1") {
            this.listOrderCharH.forEach(function (value) {
              value.METKHOITICHLUY = 0;
            });

            if (data != null) {
              let that1 = this;
              data.forEach(function (value) {
                that1.listOrderCharH.find(item => item.NGAYDATHANG == value.NGAYDATHANG).METKHOITICHLUY = value.METKHOITICHLUY;
                sum = sum + value.METKHOITICHLUY;
              });
            }
            
            
          }
          if (this.q.status == "2") {
            this.listOrderChartW.forEach(function (value) {
              value.METKHOITICHLUY = 0;
            });
            if (data != null) {
              let that1 = this;
              data.forEach(function (value) {
                that1.listOrderChartW.find(item => item.NGAYDATHANG == value.NGAYDATHANG).METKHOITICHLUY = value.METKHOITICHLUY;
                sum += value.METKHOITICHLUY;
              });
            }
            
          }
          if (this.q.status == "3") {
            this.listOrderChartW.forEach(function (value) {
              value.METKHOITICHLUY = 0;
            });
            if (data != null) {
              let that1 = this;
              data.forEach(function (value) {
                that1.listOrderChartW.find(item => item.NGAYDATHANG == value.NGAYDATHANG).METKHOITICHLUY = value.METKHOITICHLUY;
                sum += value.METKHOITICHLUY;
              });
            }
            
            
          }
          if (this.q.status == "4") {
            this.listOrderChartD.forEach(function (value) {
              //value.METKHOITICHLUY = 0;
            });
            if (data != null) {
              let that1 = this;
              data.forEach(function (value) {
               // that1.listOrderChartD.find(item => item.NGAYDATHANG == value.NGAYDATHANG).METKHOITICHLUY = value.METKHOITICHLUY;
                sum += value.METKHOITICHLUY;
              });
            }
            
            
          }
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
        console.log('resssssssssssssssssssssss'+res["meta"]["error_code"]);
        if (res["meta"]["error_code"] == 200) {
          console.log('resssssssssssssssssssssss'+res["data"]);
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
  clickd() {
    localStorage.setItem('getlinkd', "1");
  }
  //
  QueryChanged() {
    let query = '';
    let querychar = '';

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
    if(this.q.Branchlist){
      this.paging.Branchid = this.q.Branchlist;

      // console.log(this.q.Branchlist);
      // if (this.q.Branchlist != undefined) {
      //   this.paging.Branchid = '';
      //   this.q.Branchlist.forEach((item, index) => {
      //     if (item != '') {
      //       this.paging.Branchid = this.paging.Branchid + item + ',';
      //     }
      //   });
      // }
      // else {
      //   this.paging.Branchid = '';
      //   if (this.role != 1) {
      //     this.listBranchSearch.forEach((item, index) => {
      //       if (item != '') {
      //         this.paging.Branchid = this.paging.Branchid + item["BranchId"] + ',';
      //       }
      //     });
      //   }
      // }
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

      // this.GetListOrder();
      // this.GetListOrderDH();
      // this.GetListOrderChart();
      this.GetTyLeDaTron();
    }

  }
  //lấy dữ liệu tỷ lệ đã trộn/ chưa trộn
  GetTyLeDaTron(){
    if (this.q.tungay != undefined  && this.q.tungay != '' ) {
      var _tungay = this.datepipe.transform(this.q.tungay, 'yyyy-MM-dd');
      
    }
    this.barChartLabels= [];
    this.barChartData = [];
    this.cuaVLs = [];
    this.tableHopDongTrongNgay = [];
    
    this.http.get('/api/Dashboard/GetTyLeDaTron?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by + '&companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid+ '&tungay=' + _tungay, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          var vatLieuTheoNgay = res["vatLieuTheoNgay"];
          if(vatLieuTheoNgay) {
            this.pieChartLabels = vatLieuTheoNgay.LOAICUAVL;
            this.pieChartData = vatLieuTheoNgay.SUMSOLUONG;

            var chart = new CanvasJS.Chart("chartContainer", {
              animationEnabled: true,
              responsive: true,
              legend: {
                itemWidth: 200,       // Comment itemWidth to see the difference
                fontStyle: "normal",
                fontWeight: "bold"
              },
              data: [{
                type: "pie",
                showInLegend: "true",
                toolTipContent: "<b>{x}</b>: {y} KG",
                legendText: "{x}: {y} KG",
                dataPoints:  vatLieuTheoNgay.dataPoints
              }]
            });
            chart.render();

            this.cuaVLs = vatLieuTheoNgay.VatLieuTheoNgayDetails;
          }
         // this.pieChartData = res["data"];

          //this.pieChartData = [200, 400, 100, 300, 400, 250, 450, 500];
          var data1 = res["data1"];
          if(data1)
          {
           this.tableHopDongTrongNgay = data1;
          }
          this.tongBeTongTronThang = res["tongBeTongThang"];
          this.tongBeTongTronNgay = res["tongBeTongNgay"];
          this.tongDonHangTrongNgay = res["tongDonHangNgay"];
          this.tongDonHangHoanThanh = res["tongDonHangHoanThanh"];
          var tongTheTichTheoNgayTron = res["tongTheTichTheoNgayTron"];
          this.barChartData = [{ data: [] }];
          if(tongTheTichTheoNgayTron)
          {
            this.time = tongTheTichTheoNgayTron.Time;
            tongTheTichTheoNgayTron.Day.forEach(item => {
              item = item.replace("\"", "\'")
            });
            this.barChartLabels = tongTheTichTheoNgayTron.Day;
            this.barChartData = [{ data: tongTheTichTheoNgayTron.SumM3, backgroundColor: "#7FFC7F", pointBackgroundColor: '#fff', borderColor: '#000'}];
            this.totalTheTich = tongTheTichTheoNgayTron.Total;

          }
          this.soPHUTHOANTHANH = res["listSoPhutHoanThanh"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  QueryChangedCompany() {
    let query = '';
    let querychar = '';
    


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
        this.listBranchSearch.forEach((item, index) => {
          if (item != '') {
            this.paging.Branchid = this.paging.Branchid + item["BranchId"] + ',';
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
    // this.GetListOrder();
    // this.GetListOrderDH();
    // this.GetListOrderChart();
    //this.GetTyLeDaTron();
  }

}

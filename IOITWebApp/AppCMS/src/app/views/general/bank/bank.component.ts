import { Component, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';



@Component({
  selector: 'app-bank',
  templateUrl: './bank.component.html',
  styleUrls: ['./bank.component.scss']
})
export class BankComponent implements OnInit {
  @ViewChild('AddModal') public addModal: ModalDirective;
  @ViewChild('EditModal') public editModal: ModalDirective;
  urlSafe: SafeResourceUrl;
  PMQLXe: string;
  userid: number;
  public paging: any;
  public listBranch = [];
  public listCompany = [];
  public companyselectsearch: number;
  public role: number;
  companyId: number;
  BranchId: string;
  public httpOptions: any;
  constructor(
    public http: HttpClient,
    public sanitizer: DomSanitizer
  ) {
    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      })
    }
  }
  ngOnInit() {
    this.userid = parseInt(localStorage.getItem('userId'));
    var json = JSON.parse(localStorage.getItem('roles'));
    this.companyId = parseInt(localStorage.getItem('companyId'));
    this.BranchId = localStorage.getItem('BranchId');
    if (json.length > 0) {
      for (var i = 0; i < json.length; i++) {
        this.role = json[i].RoleId
      }
    }
    if (this.role == 3) {
      this.companyselectsearch = this.companyId;
      this.paging.Branchid = this.BranchId;
      this.GetListBranchSearch();
    }
    else if (this.role == 1) {
      this.GetListCompany();
    }
    else if (this.role == 4) {
      this.GetPMQLXe();
    }
    
  }
  //GET
  GetPMQLXe() {
    this.http.get('/api/Bank/GetByPage?userid=' + this.userid, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listBranch = res["data"];
          this.PMQLXe = this.listBranch[0]["PMQLXe"];
          if (this.PMQLXe != '' && this.PMQLXe != undefined) {
            this.urlSafe = this.sanitizer.bypassSecurityTrustResourceUrl(this.PMQLXe);
            this.TargetBlank();
          }
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  GetListCompany() {
    this.http.get('/api/userrole/GetByCompany?page=1&query=1=1&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listCompany = res["data"];
          this.companyselectsearch = this.listCompany[0]["CompanyId"];
          this.GetListBranchSearch();
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
          this.listBranch = res["data"];
          this.PMQLXe = this.listBranch[0]["PMQLXe"];
          if (this.PMQLXe != '' && this.PMQLXe != undefined) {
            this.urlSafe = this.sanitizer.bypassSecurityTrustResourceUrl(this.PMQLXe);
            this.TargetBlank();
          }
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  CompanyChanged() {
    this.GetListBranchSearch();
  }
  QueryChanged() {
    if (this.PMQLXe != '' && this.PMQLXe != undefined) {
      this.urlSafe = this.sanitizer.bypassSecurityTrustResourceUrl(this.PMQLXe);
      this.TargetBlank();
    }
  }

  TargetBlank() {
    //alert(this.urlSafe);
    window.open(this.PMQLXe.toString(), "_blank");
  }
}

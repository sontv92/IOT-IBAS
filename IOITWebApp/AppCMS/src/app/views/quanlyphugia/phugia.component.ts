import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpParams, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { User, PHUGIA } from '../../data/Model';
import { ToastrService } from 'ngx-toastr';
import { DatePipe } from '@angular/common';
import { debug } from 'util';
import { CommonService } from '../../service/common.service';
import { domainImage } from '../../data/const';
import { Paging, QueryFilter, UserChangePass } from '../../data/dt';
import * as $ from 'jquery';
@Component({
  selector: 'app-phugia',
  templateUrl: './phugia.component.html'
})
export class PhuGiaComponent implements OnInit {
  @ViewChild('UserModal') public userModal: ModalDirective;
  @ViewChild('file') file: ElementRef;

  public paging: any;
  public q: QueryFilter;

  public listUser = [];
  public listCompany = [];
  public listBranch = [];
  public listBranchId = [];
  public listRole = [];
  public listFunc = [];
  public ckeConfig: any;
  public Action: any;
  public disable:boolean;
  public Item: PHUGIA;

  public progress: number;
  public message: string;
  public domainImage = domainImage;

  public httpOptions: any;
  public role: number;
  companyId: number;
  BranchId: string;
  UserId: number;
  BranchIduser: string;
  edit: boolean;
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
  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
    public datePipe: DatePipe,
    public common: CommonService
  ) {
    this.Item = new PHUGIA();
    this.paging = new Paging();
    this.paging.page = 1;
    this.paging.page_size = 10;
    this.paging.query = "1=1";
    this.paging.order_by = "Ma Asc";
    this.paging.item_count = 0;

    this.q = new QueryFilter();
    this.q.txtSearch = "";
    this.role = -1
    this.Action = {
      View: false,
      Create: false,
      Update: false,
      Delete: false,
      Import: false,
      Export: false,
      Print: false,
      Other: false,
      Menu: false,
    };
    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      })
    }

  }
  ngOnInit() {
    $('input[type="checkbox"]').on('change', function () {

      $('input[type="checkbox"]').not(this).prop('checked', false);
    });
    this.ckeConfig = {
      allowedContent: false,
      extraPlugins: 'divarea',
      forcePasteAsPlainText: true
    };
    this.edit = false;
    var json = JSON.parse(localStorage.getItem('roles'));
    this.companyId = parseInt(localStorage.getItem('companyId'));
    this.UserId = parseInt(localStorage.getItem('userId'));
    this.BranchId = localStorage.getItem('BranchId');
    this.access_key = localStorage.getItem('access_key');
    this.checkRoleByCode();
    this.companyselect = -1;
    if (json.length > 0) {
      for (var i = 0; i < json.length; i++) {
        this.role = json[i].RoleId
      }
    }
    if (this.role == 1) {
      this.GetListCompany();
    }
    else{
      if (this.role != 1 && this.role != 3) {
        this.paging.Branchid = this.BranchId;
      }
      this.companyselect = this.companyId;
      this.GetListBranch();
    }
   // this.GetListKhachHang();
    ////this.GetListCompany();

    // if (this.role == 3) {
    //   this.companyselect = this.companyId;
    //   this.GetListBranch();
    // }

  }

  checkRoleByCode(){
    var functionRole = this.access_key.split('-');
    functionRole.forEach(item => {
      var code = item.split(':')[0];
      if(code == 'QLDMPG') {
        this.functionRole = item.split(':')[1];
      }
    });
    this.actionView = this.functionRole.charAt(0) == "1" ? true: false;
    this.actionCreate = this.functionRole.charAt(1) == "1" ? true: false;
    this.actionUpdate = this.functionRole.charAt(2) == "1" ? true: false;
    this.actionDelete = this.functionRole.charAt(3) == "1" ? true: false;
  }

  //Get danh sách khach hang
  GetListPhuGia() {
    // if (this.role != 1) {
    //   this.paging.query += ' and CompanyId = ' + this.companyId;
    // }
    this.http.get('/api/PhuGia/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by+ '&companyid=' + this.paging.CompanyId + '&Branchlist=' + this.paging.Branchid, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listUser = res["data"];
          this.paging.item_count = res["metadata"];
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
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  GetListBranch() {
    this.http.get('/api/branch/GetByPage?page=1&query=CompanyId = ' + this.companyselect + '&Branchlist=' + this.paging.Branchid + ' &order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listBranch = res["data"];

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
    this.GetListPhuGia();
  }

  //Thông báo
  toastWarning(msg): void {
    this.toastr.warning(msg, 'Cảnh báo');
  }

  toastSuccess(msg): void {
    this.toastr.success(msg, 'Hoàn thành');
  }

  toastError(msg): void {
    this.toastr.error(msg, 'Lỗi');
  }
  CompanyChanged() {
    //this.companyselect = this.Item.CompanyId;
    this.GetListBranch();
  }
  //
  QueryChanged() {
    if (this.role == 1){
      if (this.q.CompanyId == undefined || this.q.CompanyId == null) {
        this.toastr.warning('Vui lòng chọn công ty !', 'Cảnh báo');
        return;
      }
    }
    if (this.q.BranchId == undefined || this.q.BranchId == null) {
      this.toastr.warning('Vui lòng chọn trạm trộn !', 'Cảnh báo');
      return;
    }
    let query = '';

    if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
      if (query != '') {

        query += " and TENPG LIKE  \N'%"+ this.q.txtSearch +"%\'";
      }
      else {
        query += "  TENPG LIKE  \N'%"+ this.q.txtSearch +"%\'";
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
    //     this.listBranch.forEach((item, index) => {
    //       if (item != '') {
    //         this.paging.Branchid = this.paging.Branchid + item["BranchId"] + ',';
    //       }
    //     });
    //   }
    // }
    if (this.q.BranchId !=undefined)
    {
      this.paging.Branchid = this.q.BranchId;
    }
    if (query == '')
      this.paging.query = '1=1';
    else
      this.paging.query = query;

    this.GetListPhuGia();
  }

  //
  QueryChangedCompany() {
    let query = '';
    if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
      if (query != '') {
        query += ' and Name.Contains("' + this.q.txtSearch + '")';
      }
      else {
        query += 'Name.Contains("' + this.q.txtSearch + '")';
      }
    }

    if (this.q.CompanyId != undefined) {
      this.companyselect = this.q.CompanyId;
      this.listBranch = [];
      this.q.BranchId = null;

      this.GetListBranch();
      if (query != '') {
        query += ' and CompanyId = ' + this.q.CompanyId;
      }
      else {
        query += 'CompanyId = ' + this.q.CompanyId;
      }
    }
    if (this.role != 1) {
      if (query != '') {
        query += ' and CompanyId = ' + this.companyId;
      }
      else {
        query += 'CompanyId = ' + this.companyId;
      }
    }
    if (this.q.BranchId != undefined) {
      if (query != '') {
        query += ' and BranchId.Contains("' + this.q.BranchId + '")';
      }
      else {
        query += 'BranchId.Contains("' + this.q.BranchId + '")';
      }
    }
    if (query == '')
      this.paging.query = '1=1';
    else
      this.paging.query = query;

    this.GetListPhuGia();
  }
  //Mở modal thêm mới
  OpenAddModal() {
    if (this.q.BranchId == undefined || this.q.BranchId == null) {
      this.toastWarning("Vui lòng chọn trạm trộn!");
      return;
    }
    this.edit = false;
    this.Item = new PHUGIA();
    this.disable = true;
    this.message = undefined;
    this.Action = {
      View: false,
      Create: false,
      Update: false,
      Delete: false,
      Import: false,
      Export: false,
      Print: false,
      Other: false,
      Menu: false,
    };
    this.userModal.show();
  }
  changeFn(val){
    this.Item.BranchId = val;
  }
  //Thêm mới danh mục trang
  AddUserFunc() {

    if (this.Item.TENPG == undefined || this.Item.TENPG == '') {
      this.toastWarning("Chưa nhập Tên phụ gia!");
      return;
    } else if (this.Item.TENPG.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập Tên phụ gia!");
      return;
    }
    // if (this.q.BranchId != undefined) {
    //     this.Item.BranchId = this.q.BranchId;
    // }
    // else{
    //   this.toastWarning("Chưa chọn trạm trộn!");
    //   return;
    // }

    // if (this.listBranchId != undefined) {
    //   this.Item.BranchId = '';
    //   this.listBranchId.forEach((item, index) => {
    //     if (index == this.listBranchId.length - 1) {
    //       this.Item.BranchId += item;
    //     }
    //     else {
    //       this.Item.BranchId += item + ",";
    //     }

    //   });
    // }
    // else {
    //   this.Item.BranchId = '';
    // }
    if (this.Item.ID) {
      this.Item.BranchId = this.q.BranchId;
      this.http.put('/api/PhuGia/' + this.Item.ID, this.Item, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListPhuGia();
            this.userModal.hide();
            this.toastSuccess("Cập nhật thành công!");
          }
          else if (res["meta"]["error_code"] == 21111) {
            this.toastWarning("Đã quá số lượng tài khoản cho phép!");
          }
          else if (res["meta"]["error_code"] == 211) {
            this.toastWarning("Tên tài khoản đã tồn tại!");
          }
          else if (res["meta"]["error_code"] == 2111) {
            this.toastWarning("Email đã tồn tại!");
          } else if (res["meta"]["error_code"] == 222) {
            this.toastError("Bạn không có quyền thực hiện chức năng này!");
            }
          else {
            this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
          }
        },
        (err) => {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        }
      );
    }
    else {
      // if (this.role != 1) {
      //   this.Item.CompanyId = this.companyId;
      // }
      this.Item.BranchId = this.q.BranchId;
      this.http.post('/api/PhuGia', this.Item, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListPhuGia();
            this.userModal.hide();
            this.toastSuccess("Thêm mới thành công!");
          }
          else if (res["meta"]["error_code"] == 21111) {
            this.toastWarning("Đã quá số lượng tài khoản cho phép!");
          }
          else if (res["meta"]["error_code"] == 211) {
            this.toastWarning("Tên tài khoản đã tồn tại!");
          }
          else if (res["meta"]["error_code"] == 2111) {
            this.toastWarning("Email đã tồn tại!");
          } else if (res["meta"]["error_code"] == 222) {
            this.toastError("Bạn không có quyền thực hiện chức năng này!");
            }
          else {
            this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
          }
        },
        (err) => {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        }
      );
    }
  }

  OpenEditModal(item) {
    //this.form.controls['name'].disable();

    this.disable = true;
    this.edit = true;
    this.Item = new PHUGIA();
    this.GetListCompany();
    this.Item = Object.assign(this.Item, item);
    // if (item.CompanyId != '' && item.CompanyId != null) {
    //   this.companyselect = item.CompanyId;
    //   this.GetListBranch();
    // }
    this.GetListBranch();


    this.userModal.show();
  }

  //Popup xác nhận xóa
  ShowConfirmDelete(Id, BranchId) {
    this.modalDialogService.openDialog(this.viewRef, {
      title: 'Xác nhận',
      childComponent: SimpleModalComponent,
      data: {
        text: "Bạn có chắc chắn muốn xóa bản ghi này?"
      },
      actionButtons: [
        {
          text: 'Đồng ý',
          buttonClass: 'btn btn-success',
          onAction: () => {
            this.XoaKH(Id, BranchId);
          }
        },
        {
          text: 'Đóng',
          buttonClass: 'btn btn-default',

        }
      ],
    });
  }

  ShowConfirmReset(Id) {
    this.modalDialogService.openDialog(this.viewRef, {
      title: 'Xác nhận',
      childComponent: SimpleModalComponent,
      data: {
        text: "Bạn có chắc chắn muốn reset mật khẩu không?"
      },
      actionButtons: [
        {
          text: 'Đồng ý',
          buttonClass: 'btn btn-success',
          onAction: () => {
            this.ResetUser(Id);
          }
        },
        {
          text: 'Đóng',
          buttonClass: 'btn btn-default',

        }
      ],
    });
  }
  XoaKH(Id,BranchId) {
    var strID = Id+"_"+BranchId;

    this.http.delete('/api/PhuGia/' + strID, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListPhuGia();
          this.viewRef.clear();
          this.toastSuccess("Xóa thành công!");
        } else if (res["meta"]["error_code"] == 222) {
          this.toastError("Bạn không có quyền thực hiện chức năng này!");
          }
        else {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        }
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
      }
    );
  }

  ResetUser(Id) {
    this.http.get('/api/userrole/ResetPassUser?id=' + Id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListPhuGia();
          this.viewRef.clear();
          this.toastSuccess("Reset thành công!");
        } else if (res["meta"]["error_code"] == 222) {
          this.toastError("Bạn không có quyền thực hiện chức năng này!");
          }
        else {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        }
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
      }
    );
  }

  upload(files) {
    if (files.length === 0)
      return;

    const formData = new FormData();

    for (let file of files)
      formData.append(file.name, file);

    const uploadReq = new HttpRequest('POST', 'api/upload/uploadImage/6', formData, {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      }),
      reportProgress: true,
    });

    this.http.request(uploadReq).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress)
        this.progress = Math.round(100 * event.loaded / event.total);
      else if (event.type === HttpEventType.Response) {
        this.message = event.body["data"].toString();
        //this.Item.Avata = this.message;
      }
    });
  }

  GetListFunction(IsNew) {
    this.http.get('/api/function/listFunction', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listFunc = res["data"];

          if (IsNew) {
            this.listFunc.forEach(item => {
              item.Space = "";
              item.View = false;
              item.Create = false;
              item.Update = false;
              item.Delete = false;
              item.Import = false;
              item.Export = false;
              item.Print = false;
              item.Other = false;
              item.Menu = false;
              for (var i = 0; i < (item.Level) * 7; i++) {
                item.Space += "&nbsp;";
              }
            })
          }
          else {
            this.changeCell();
          }
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  changeAction(cs) {
    this.listFunc.forEach(item => {
      switch (cs) {
        case 1:
          item.View = this.Action.View;
          break;
        case 2:
          item.Create = this.Action.Create;
          break;
        case 3:
          item.Update = this.Action.Update;
          break;
        case 4:
          item.Delete = this.Action.Delete;
          break;
        case 5:
          item.Import = this.Action.Import;
          break;
        case 6:
          item.Export = this.Action.Export;
          break;
        case 7:
          item.Print = this.Action.Print;
          break;
        case 8:
          item.Other = this.Action.Other;
          break;
        case 9:
          item.Menu = this.Action.Menu;
          break;
        default:
          break;
      }

      if (item.View && item.Create && item.Update && item.Delete && item.Import && item.Export && item.Print && item.Other && item.Menu) {
        item.Full = true;
      }
      else {
        item.Full = false;
      }

    });
  }

  changeFull(i) {
    if (i != undefined) {
      this.listFunc[i].View = this.listFunc[i].Full;
      this.listFunc[i].Create = this.listFunc[i].Full;
      this.listFunc[i].Update = this.listFunc[i].Full;
      this.listFunc[i].Delete = this.listFunc[i].Full;
      this.listFunc[i].Import = this.listFunc[i].Full;
      this.listFunc[i].Export = this.listFunc[i].Full;
      this.listFunc[i].Print = this.listFunc[i].Full;
      this.listFunc[i].Other = this.listFunc[i].Full;
      this.listFunc[i].Menu = this.listFunc[i].Full;
    }

    if (this.listFunc.filter(l => l.View == false).length > 0) {
      this.Action.View = false;
    }
    else {
      this.Action.View = true;
    }

    if (this.listFunc.filter(l => l.Create == false).length > 0) {
      this.Action.Create = false;
    }
    else {
      this.Action.Create = true;
    }

    if (this.listFunc.filter(l => l.Update == false).length > 0) {
      this.Action.Update = false;
    }
    else {
      this.Action.Update = true;
    }

    if (this.listFunc.filter(l => l.Delete == false).length > 0) {
      this.Action.Delete = false;
    }
    else {
      this.Action.Delete = true;
    }

    if (this.listFunc.filter(l => l.Import == false).length > 0) {
      this.Action.Import = false;
    }
    else {
      this.Action.Import = true;
    }

    if (this.listFunc.filter(l => l.Export == false).length > 0) {
      this.Action.Export = false;
    }
    else {
      this.Action.Export = true;
    }

    if (this.listFunc.filter(l => l.Print == false).length > 0) {
      this.Action.Print = false;
    }
    else {
      this.Action.Print = true;
    }

    if (this.listFunc.filter(l => l.Other == false).length > 0) {
      this.Action.Other = false;
    }
    else {
      this.Action.Other = true;
    }

    if (this.listFunc.filter(l => l.Menu == false).length > 0) {
      this.Action.Menu = false;
    }
    else {
      this.Action.Menu = true;
    }

  }

  changeCell() {
    this.changeAction(10);
    this.changeFull(undefined);
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

    this.GetListPhuGia();
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

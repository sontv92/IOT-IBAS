
import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpParams, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { User, ResetUser } from '../../data/model';
import { ToastrService } from 'ngx-toastr';
import { DatePipe } from '@angular/common';
import { debug } from 'util';
import { CommonService } from '../../service/common.service';
import { domainImage } from '../../data/const';
import { Paging, QueryFilter, UserChangePass } from '../../data/dt';
import * as $ from 'jquery';
@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})
export class UserComponent implements OnInit {
  @ViewChild('UserModal') public userModal: ModalDirective;
  @ViewChild('file') file: ElementRef;

  public paging: Paging;
  public q: QueryFilter;

  public listUser = [];
  public listCompany = [];
  public listBranch = [];
  public listBranchId = [];
  public listRole = [];
  public listFunc = [];
  public ckeConfig: any;
  public Action: any;

  public Item: User;

  public progress: number;
  public message: string;
  public domainImage = domainImage;

  public httpOptions: any;
  public role: number;
  companyId: number;
  BranchId: number;
  UserId: number;
  BranchIduser: string;
  edit: boolean;
  public companyselect: number;
  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
    public datePipe: DatePipe,
    public common: CommonService
  ) {
    this.Item = new User();
    this.paging = new Paging();
    this.paging.page = 1;
    this.paging.page_size = 10;
    this.paging.query = "1=1";
    this.paging.order_by = "UserId Desc";
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
    this.companyselect = -1;
    if (json.length > 0) {
      for (var i = 0; i < json.length; i++) {
        this.role = json[i].RoleId
      }
    }
    this.GetListUser();
    this.GetListCompany();
    if (this.role == 3) {
      this.companyselect = this.companyId;
      this.GetListBranch();
    }

  }

  //Get danh sách danh bài viết
  GetListUser() {
    if (this.role != 1) {
      this.paging.query += ' and CompanyId = ' + this.companyId;
    }
    this.http.get('/api/userrole/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
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
    this.http.get('/api/branch/GetByPage?page=1&query=CompanyId = ' + this.companyselect + ' &order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listBranch = res["data"];
          if (this.Item.BranchId != '' && this.Item.BranchId != null && this.edit == true) {
            this.listBranchId = [];
            let lstnhombenh = this.Item.BranchId.split(',');
            lstnhombenh.forEach(element => {
              this.listBranchId.push(parseInt(element));
            });
          }
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  GetListRole() {
    let arr = [];
    if (this.Item.UserId) {
      arr = Object.assign(this.Item["listRole"]);
    }

    this.http.get('/api/role/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          if (this.Item.UserId == undefined) {
            this.listRole = res["data"];
          }
          else {
            this.listRole = res["data"];
            for (let i = 0; i < this.listRole.length; i++) {
              for (let j = 0; j < arr.length; j++) {
                if (this.listRole[i].RoleId == arr[j].RoleId) {
                  this.Item.Roleid = this.listRole[i].RoleId;
                  break;
                }
              }
            }

          }
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
    this.GetListUser();
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
    this.companyselect = this.Item.CompanyId;
    this.GetListBranch();
  }
  //
  QueryChanged() {
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

    this.GetListUser();
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

    this.GetListUser();
  }
  //Mở modal thêm mới
  OpenAddModal() {
    this.edit = false;
    this.Item = new User();
    this.Item.IsRoleGroup = true;
    this.GetListRole();
    this.GetListFunction(true);
    this.file.nativeElement.value = "";
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
  //Thêm mới danh mục trang
  AddUserFunc() {

    if (this.Item.FullName == undefined || this.Item.FullName == '') {
      this.toastWarning("Chưa nhập Tên người dùng!");
      return;
    } else if (this.Item.FullName.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên người dùng!");
      return;
    } else if (this.Item.UserName == undefined || this.Item.UserName == '') {
      this.toastWarning("Chưa nhập Tên tài khoản!");
      return;
    } else if (this.Item.UserName.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên tài khoản!");
      return;
    }
    else if (this.Item.UserId == undefined && (this.Item.Password == undefined || this.Item.Password == '')) {
      this.toastWarning("Chưa nhập Mật khẩu!");
      return;
    }
    else if (this.Item.UserId == undefined && (this.Item["ConfirmPassword"] != this.Item.Password)) {
      this.toastWarning("Mật khẩu xác nhận không chính xác!");
      return;
    }
    else if (this.Item.Email == undefined || this.Item.Email == '') {
      this.toastWarning("Chưa nhập Email!");
      return;
    } else if (this.Item.Email.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập Email!");
      return;
    }
    this.Item.IsRoleGroup = true;
    this.Item["listRole"] = [];
    this.Item["listFunction"] = [];
    this.listRole.forEach(item => {
      if (item.Check) {
        this.Item["listRole"].push({ RoleId: item.RoleId, RoleName: item.RoleName });
      }
    });
    if (this.listBranchId != undefined) {
      this.Item.BranchId = '';
      this.listBranchId.forEach((item, index) => {
        if (index == this.listBranchId.length - 1) {
          this.Item.BranchId += item;
        }
        else {
          this.Item.BranchId += item + ",";
        }

      });
    }
    else {
      this.Item.BranchId = '';
    }
    if (this.Item.UserId) {
      this.http.put('/api/userrole/' + this.Item.UserId, this.Item, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListUser();
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
      if (this.role != 1) {
        this.Item.CompanyId = this.companyId;
      }
      this.http.post('/api/userrole', this.Item, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListUser();
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
    this.edit = true;
    this.Item = new User();
    this.GetListCompany();
    this.Item = Object.assign(this.Item, item);
    if (item.CompanyId != '' && item.CompanyId != null) {
      this.companyselect = item.CompanyId;
      this.GetListBranch();
    }
    this.file.nativeElement.value = "";
    this.GetListRole();
    this.userModal.show();
  }

  //Popup xác nhận xóa
  ShowConfirmDelete(Id) {
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
            this.DeleteUser(Id);
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
  DeleteUser(Id) {
    this.http.delete('/api/userrole/' + Id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListUser();
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
          this.GetListUser();
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
        this.Item.Avata = this.message;
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

    this.GetListUser();
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

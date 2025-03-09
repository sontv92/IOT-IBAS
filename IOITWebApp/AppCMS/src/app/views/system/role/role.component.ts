import { Component, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { ToastrService } from 'ngx-toastr';
import { CommonService } from '../../../service/common.service';
import { Paging, QueryFilter } from '../../../data/dt';
import { Role, FuncRole } from '../../../data/model';



@Component({
  selector: 'app-role',
  templateUrl: './role.component.html',
  styleUrls: ['./role.component.scss']
})
export class RoleComponent implements OnInit {
  @ViewChild('modalRole') public modalRole: ModalDirective;

  public paging: Paging;
  public q: QueryFilter;

  public listRole = [];
  public listFunction = [];

  public Action: any;
  public Item: Role;

  public httpOptions: any;

  constructor(public http: HttpClient, public modalDialogService: ModalDialogService, public viewRef: ViewContainerRef, public toastr: ToastrService) {
    this.Item = new Role();

    this.paging = new Paging();
    this.paging.page = 1;
    this.paging.page_size = 10;
    this.paging.query = "1=1";
    this.paging.order_by = "RoleId Desc";
    this.paging.item_count = 0;

    this.q = new QueryFilter();
    this.q.txtSearch = "";

    this.Action = {
      View: false,
      Create: false,
      Update: false,
      Delete: false,
      Import: false,
      Export: false,
      Print: false,
      Other: false,
      Menu: false
    };

    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      })
    }

  }

  ngOnInit() {
    this.GetListRole();
  }

  //Get danh sách chức năng
  GetListRole() {
    this.http.get('/api/functionrole/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listRole = res["data"];
          this.paging.item_count = res["metadata"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      });
  }

  //Chuyển trang
  PageChanged(event) {
    this.paging.page = event.page;
    this.GetListRole();
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
        query += ' and Name.Contains("' + this.q.txtSearch + '")';
      }
      else {
        query += 'Name.Contains("' + this.q.txtSearch + '")';
      }
    }

    if (query == '')
      this.paging.query = '1=1';
    else
      this.paging.query = query;

    this.GetListRole();
  }


  //Get danh sách chức năng cha
  GetListFunction(IsNew) {
    this.http.get('/api/function/listFunction', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listFunction = res["data"];

          if (IsNew) {
            this.listFunction.forEach(item => {
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
            for (let i = 0; i < this.listFunction.length; i++) {
              for (let j = 0; j < this.Item.listFunction.length; j++) {
                if (this.listFunction[i].FunctionId == this.Item.listFunction[j].FunctionId) {
                  this.listFunction[i].View = this.Item.listFunction[j].ActiveKey[0] == "1" ? true : false;
                  this.listFunction[i].Create = this.Item.listFunction[j].ActiveKey[1] == "1" ? true : false;
                  this.listFunction[i].Update = this.Item.listFunction[j].ActiveKey[2] == "1" ? true : false;
                  this.listFunction[i].Delete = this.Item.listFunction[j].ActiveKey[3] == "1" ? true : false;
                  this.listFunction[i].Import = this.Item.listFunction[j].ActiveKey[4] == "1" ? true : false;
                  this.listFunction[i].Export = this.Item.listFunction[j].ActiveKey[5] == "1" ? true : false;
                  this.listFunction[i].Print = this.Item.listFunction[j].ActiveKey[6] == "1" ? true : false;
                  this.listFunction[i].Other = this.Item.listFunction[j].ActiveKey[7] == "1" ? true : false;
                  this.listFunction[i].Menu = this.Item.listFunction[j].ActiveKey[8] == "1" ? true : false;
                  break;
                }
              }

              this.listFunction[i].Space = "";
              for (let idx = 0; idx < (this.listFunction[i].Level) * 7; idx++) {
                this.listFunction[i].Space += "&nbsp;";
              }
            }

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
    this.listFunction.forEach(item => {
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
      this.listFunction[i].View = this.listFunction[i].Full;
      this.listFunction[i].Create = this.listFunction[i].Full;
      this.listFunction[i].Update = this.listFunction[i].Full;
      this.listFunction[i].Delete = this.listFunction[i].Full;
      this.listFunction[i].Import = this.listFunction[i].Full;
      this.listFunction[i].Export = this.listFunction[i].Full;
      this.listFunction[i].Print = this.listFunction[i].Full;
      this.listFunction[i].Other = this.listFunction[i].Full;
      this.listFunction[i].Menu = this.listFunction[i].Full;
    }

    if (this.listFunction.filter(l => l.View == false || l.View == undefined).length > 0) {
      this.Action.View = false;
    }
    else {
      this.Action.View = true;
    }

    if (this.listFunction.filter(l => l.Create == false || l.Create == undefined).length > 0) {
      this.Action.Create = false;
    }
    else {
      this.Action.Create = true;
    }

    if (this.listFunction.filter(l => l.Update == false || l.Update == undefined).length > 0) {
      this.Action.Update = false;
    }
    else {
      this.Action.Update = true;
    }

    if (this.listFunction.filter(l => l.Delete == false || l.Delete == undefined).length > 0) {
      this.Action.Delete = false;
    }
    else {
      this.Action.Delete = true;
    }

    if (this.listFunction.filter(l => l.Import == false || l.Import == undefined).length > 0) {
      this.Action.Import = false;
    }
    else {
      this.Action.Import = true;
    }

    if (this.listFunction.filter(l => l.Export == false || l.Export == undefined).length > 0) {
      this.Action.Export = false;
    }
    else {
      this.Action.Export = true;
    }

    if (this.listFunction.filter(l => l.Print == false || l.Print == undefined).length > 0) {
      this.Action.Print = false;
    }
    else {
      this.Action.Print = true;
    }

    if (this.listFunction.filter(l => l.Other == false || l.Other == undefined).length > 0) {
      this.Action.Other = false;
    }
    else {
      this.Action.Other = true;
    }

    if (this.listFunction.filter(l => l.Menu == false || l.Menu == undefined).length > 0) {
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

  OpenModalRole(item) {
    this.Item = new Role();
    this.Item.listFunction = [];
    this.listFunction = [];
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

    if (item == undefined) {
      this.GetListFunction(true);
    }
    else {
      this.Item = Object.assign(this.Item, item);
      this.GetListFunction(false);
    }
    this.modalRole.show();
  }


  SaveRole() {
    if (this.Item.Code == undefined || this.Item.Code == '') {
      this.toastWarning("Chưa nhập Mã quyền!");
      return;
    } else if (this.Item.Name == undefined || this.Item.Name == '') {
      this.toastWarning("Chưa nhập Tên quyền!");
      return;
    }

    let listFunction = [];
    this.listFunction.forEach(item => {
      let newFunc = new FuncRole();
      newFunc.FunctionId = item.FunctionId;
      newFunc.ActiveKey = "";
      newFunc.ActiveKey += item.View == true ? 1 : 0;
      newFunc.ActiveKey += item.Create == true ? 1 : 0;
      newFunc.ActiveKey += item.Update == true ? 1 : 0;
      newFunc.ActiveKey += item.Delete == true ? 1 : 0;
      newFunc.ActiveKey += item.Import == true ? 1 : 0;
      newFunc.ActiveKey += item.Export == true ? 1 : 0;
      newFunc.ActiveKey += item.Print == true ? 1 : 0;
      newFunc.ActiveKey += item.Other == true ? 1 : 0;
      newFunc.ActiveKey += item.Menu == true ? 1 : 0;

      listFunction.push(newFunc);
    });
    this.Item.listFunction = listFunction;
    this.Item.UserId = parseInt(localStorage.getItem("userId"));

    if (this.Item.RoleId == undefined) {
      this.http.post('/api/functionRole', this.Item, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListRole();
            this.modalRole.hide();
            this.toastSuccess("Thêm mới thành công!");
          }
          else if (res["meta"]["error_code"] == 211) {
            this.toastWarning("Mã quyền đã tồn tại. Xin vui lòng thử lại!");
          } else if (res["meta"]["error_code"] == 222) {
						this.toastError("Bạn không có quyền thực hiện chức năng này!");
						}
          else {
            this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
          }
        },
        (err) => {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    }
    else {
      this.http.put('/api/functionRole/' + this.Item.RoleId, this.Item, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListRole();
            this.modalRole.hide();
            this.toastSuccess("Cập nhật thành công!");
          }
          else if (res["meta"]["error_code"] == 211) {
              this.toastWarning("Mã quyền đã tồn tại. Xin vui lòng thử lại!");
          } else if (res["meta"]["error_code"] == 222) {
						this.toastError("Bạn không có quyền thực hiện chức năng này!");
						}
          else {
            this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
          }
        },
        (err) => {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    }
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
            this.Delete(Id);
          }
        },
        {
          text: 'Đóng',
          buttonClass: 'btn btn-default'
        }
      ],
    });
  }

  Delete(Id) {
    this.http.delete('/api/functionrole/' + Id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListRole();
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

    this.GetListRole();
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

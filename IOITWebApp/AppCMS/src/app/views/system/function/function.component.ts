import { Component, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { ToastrService } from 'ngx-toastr';
import { CommonService } from '../../../service/common.service';
import { Paging, QueryFilter } from '../../../data/dt';
import { Function } from '../../../data/model';



@Component({
  selector: 'app-function',
  templateUrl: './function.component.html',
  styleUrls: ['./function.component.scss']
})
export class FunctionComponent implements OnInit {
  @ViewChild('modalFunction') public modalFunction: ModalDirective;

  public paging: Paging;
  public q: QueryFilter;

  public listFunction = [];
  public listFunctionParent = [];

  public Item: Function;

  public httpOptions: any;

  constructor(public http: HttpClient, public modalDialogService: ModalDialogService, public viewRef: ViewContainerRef, public toastr: ToastrService) {
    this.Item = new Function();

    this.paging = new Paging();
    this.paging.page = 1;
    this.paging.page_size = 10;
    this.paging.query = "1=1";
    this.paging.order_by = "FunctionId Desc";
    this.paging.item_count = 0;

    this.q = new QueryFilter();
    this.q.txtSearch = "";

    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      })
    }

  }

  ngOnInit() {
    this.GetListFunction();
  }

  //Get danh sách chức năng
  GetListFunction() {
    this.http.get('/api/function/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listFunction = res["data"];
          this.paging.item_count = res["metadata"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      });
  }

  GetListFunctionParent(Id) {
    this.http.get('/api/function/listFunction', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listFunctionParent = res["data"];
          this.listFunctionParent.forEach(item => {
            if (item.FunctionId == Id) {
              item.disabled = true;
            }

            item.Space = "";
            for (var i = 0; i < (item.Level) * 7; i++) {
              item.Space += "&nbsp;";
            }
          })
        }
      },
      (err) => {
        console.log("Error: connect to API");
      });
  }

  //Chuyển trang
  PageChanged(event) {
    this.paging.page = event.page;
    this.GetListFunction();
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
        query += ' and (Name.Contains("' + this.q.txtSearch + '") OR Code.Contains("' + this.q.txtSearch + '"))';
      }
      else {
        query += '(Name.Contains("' + this.q.txtSearch + '") OR Code.Contains("' + this.q.txtSearch + '"))';
      }
    }

    if (query == '')
      this.paging.query = '1=1';
    else
      this.paging.query = query;

    this.GetListFunction();
  }


  OpenModalFunction(item) {
    this.Item = new Function();
    if (item == undefined) {
      this.GetListFunctionParent(undefined);
    }
    else {
      this.Item = Object.assign(this.Item, item);
      if (this.Item.FunctionParentId == 0) this.Item.FunctionParentId = undefined;
      this.GetListFunctionParent(item.FunctionId);
    }

    this.modalFunction.show();
  }

  SaveFunc() {
    if (this.Item.Code == undefined || this.Item.Code == '') {
      this.toastWarning("Chưa nhập Mã chức năng!");
      return;
    } else if (this.Item.Code.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập mã chức năng!");
      return;
    } else if (this.Item.Name == undefined || this.Item.Name == '') {
      this.toastWarning("Chưa nhập Tên chức năng!");
      return;
    } else if (this.Item.Name.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên chức năng!");
      return;
    } else if (this.Item.Url == undefined || this.Item.Url == '') {
      this.toastWarning("Chưa nhập Đường dẫn!");
      return;
    } else if (this.Item.Url.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập đường dẫ!");
      return;
    } else if (this.Item.Icon == undefined || this.Item.Icon == '') {
      this.toastWarning("Chưa nhập Icon!");
      return;
    } else if (this.Item.Icon.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập Icon!");
      return;
    }

    let obj = JSON.parse(JSON.stringify(this.Item));
    if (this.Item.FunctionId == undefined) {
      this.http.post('/api/function', this.Item, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListFunction();
            this.modalFunction.hide();
            this.toastSuccess("Thêm mới thành công!");
          }
          else if (res["meta"]["error_code"] == 211) {
              this.toastWarning("Mã chức năng đã tồn tại. Xin vui lòng thử lại!");
          }
          else if (res["meta"]["error_code"] == 215) {
              this.toastWarning("Chức năng cha không hợp lệ. Xin vui lòng thử lại!");
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
      if (obj.FunctionParentId == null) obj.FunctionParentId = 0;

      this.http.put('/api/function/' + obj.FunctionId, obj, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListFunction();
            this.modalFunction.hide();
            this.toastSuccess("Cập nhật thành công!");
          }
          else if (res["meta"]["error_code"] == 211) {
              this.toastWarning("Mã chức năng đã tồn tại. Xin vui lòng thử lại!");
          }
          else if (res["meta"]["error_code"] == 215) {
              this.toastWarning("Chức năng cha không hợp lệ. Xin vui lòng thử lại!");
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
          buttonClass: 'btn btn-default',

        }
      ],
    });
  }

  Delete(Id) {
    this.http.delete('/api/function/' + Id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListFunction();
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

    this.GetListFunction();
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

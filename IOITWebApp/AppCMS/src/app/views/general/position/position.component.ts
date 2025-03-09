import { Component, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Position } from '../../../data/model';



@Component({
  selector: 'app-position',
  templateUrl: './position.component.html',
  styleUrls: ['./position.component.scss']
})
export class PositionComponent implements OnInit {
  @ViewChild('AddModal') public addModal: ModalDirective;
  @ViewChild('EditModal') public editModal: ModalDirective;

  public paging: any;
  public q: any;

  public listPosition = [];
  public listCompany = [];
  public showSort: boolean[] = [true, false, false];

  public ckeConfig: any;

  public newItem: Position;
  public editItem: Position;

  public httpOptions: any;

  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
  ) {
    this.newItem = new Position();
    this.editItem = new Position();

    this.paging = {
      page: 1,
      page_size: 10,
      query: '1=1',
      order_by: '',
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
    this.GetListPosition();
  }

  Sort(IsK, s, i) {
    if (IsK) {
      this.paging.order_by = s + " asc";
    }
    else {
      this.paging.order_by = s + " desc";
    }
    this.GetListPosition();
    //this.showSort = !IsK;
    for (var j = 0; j < this.showSort.length; j++) {
      if (j == i) {
        this.showSort[j] = IsK;
      }
      else {
        this.showSort[j] = false;
      }
    }
  }

  GetListPosition() {
    this.http.get('/api/position/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listPosition = res["data"];
          this.paging.item_count = res["metadata"];
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
    this.GetListPosition();
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

    this.GetListPosition();
  }

  //Mở modal
  OpenAddModal() {
    this.newItem = new Position();
    this.addModal.show();
  }
  //
  //Thêm mới
  AddFunc() {
    if (this.newItem.Name == undefined || this.newItem.Name == '') {
      this.toastWarning("Chưa nhập Tên!");
      return;
    } else if (this.newItem.Name.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên!");
      return;
    } else if (this.newItem.Code == undefined || this.newItem.Code == '') {
      this.toastWarning("Chưa nhập mã!");
      return;
    } else if (this.newItem.Code.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập mã!");
      return;
    } else if (this.newItem.LevelId == undefined) {
      this.toastWarning("Chưa chọn cấp độ!");
      return;
    }
    if (this.newItem.Code.length < 2) {
      this.toastWarning("Mã ít nhất 2 ký tự!");
      return;
    }
    this.newItem.CompanyId = parseInt(localStorage.getItem("companyId"));
    this.http.post('/api/Position', this.newItem, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListPosition();
          this.addModal.hide();
          this.toastSuccess("Thêm thành công!");
        } else if (res["meta"]["error_code"] == 213) {
          this.toastWarning("Mã đã tồn tại!");
        } else if (res["meta"]["error_code"] == 222) {
          this.toastError("Bạn không có quyền thực hiện chức năng này!");
        }
        else {
          this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
        }
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
      }
    );
  }

  //Mở edit Modal
  OpenEditModal(item) {
    this.editItem = new Position();
    this.editItem = Object.assign(this.editItem, item);
    this.editModal.show();
  }

  // cập nhật
  EditFunc() {
    if (this.editItem.Name == undefined || this.editItem.Name == '') {
      this.toastWarning("Chưa nhập Tên!");
      return;
    } else if (this.editItem.Name.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên!");
      return;
    } else if (this.editItem.Code == undefined || this.editItem.Code == '') {
      this.toastWarning("Chưa nhập mã!");
      return;
    } else if (this.editItem.Code.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập mã!");
      return;
    } else if (this.editItem.LevelId == undefined) {
      this.toastWarning("Chưa chọn cấp độ!");
      return;
    } else if (this.editItem.Code.length < 2) {
      this.toastWarning("Mã ít nhất 2 ký tự!");
      return;
    }
    this.http.put('/api/Position/' + this.editItem.PositionId, this.editItem, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListPosition();
          this.editModal.hide();
          this.toastSuccess("Cập nhật thành công!");
        } else if (res["meta"]["error_code"] == 213) {
          this.toastWarning("Mã đã tồn tại!");
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
            console.log('OnAction');
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
    this.http.delete('/api/Position/' + Id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListPosition();
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
  //
}

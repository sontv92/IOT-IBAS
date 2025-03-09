import { Component, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Department } from '../../../data/model';



@Component({
  selector: 'app-department',
  templateUrl: './department.component.html',
  styleUrls: ['./department.component.scss']
})
export class DepartmentComponent implements OnInit {
  @ViewChild('AddModal') public addModal: ModalDirective;
  @ViewChild('EditModal') public editModal: ModalDirective;

  public paging: any;
  public q: any;

  public listDepartment = [];
  public listCompany = [];
  public showSort: boolean[] = [true, false];

  public ckeConfig: any;

  public newItem: Department;
  public editItem: Department;

  public httpOptions: any;

  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
  ) {
    this.newItem = new Department();
    this.editItem = new Department();
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
    this.GetListDepartment();
  }
  //
  Sort(IsK, s, i) {
    if (IsK) {
      this.paging.order_by = s + " asc";
    }
    else {
      this.paging.order_by = s + " desc";
    }
    this.GetListDepartment();
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
  //
  GetListDepartment() {
    this.http.get('/api/department/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listDepartment = res["data"];
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
    this.GetListDepartment();
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

    this.GetListDepartment();
  }

  //Mở modal
  OpenAddModal() {
    this.newItem = new Department();
    this.addModal.show();
  }
  //
  //Thêm mới
  AddFunc() {
    if (this.newItem.Code == undefined || this.newItem.Code == '') {
      this.toastWarning("Chưa nhập mã!");
      return;
    } else if (this.newItem.Code.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập mã!");
      return;
    } else if (this.newItem.Name == undefined || this.newItem.Name == '') {
      this.toastWarning("Chưa nhập Tên!");
      return;
    } else if (this.newItem.Code.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên!");
      return;
    } else if (this.newItem.Code.length < 2) {
      this.toastWarning("Mã ít nhất 2 ký tự!");
      return;
    }

    this.newItem.UserId = parseInt(localStorage.getItem("userId"));
    this.newItem.CompanyId = parseInt(localStorage.getItem("companyId"));

    this.http.post('/api/Department', this.newItem, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListDepartment();
          this.addModal.hide();
          this.toastSuccess("Thêm thành công!");
        } else if (res["meta"]["error_code"] == 211) {
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
    this.editItem = new Department();
    this.editItem = Object.assign(this.editItem, item);
    this.editModal.show();
  }

  // cập nhật
  EditFunc() {
    if (this.editItem.Code == undefined || this.editItem.Code == '') {
      this.toastWarning("Chưa nhập mã!");
      return;
    } else if (this.editItem.Code.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập mã!");
      return;
    } else if (this.editItem.Name == undefined || this.editItem.Name == '') {
      this.toastWarning("Chưa nhập Tên!");
      return;
    } else if (this.editItem.Name.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên!");
      return;
    } else if (this.editItem.Code.length < 2) {
      this.toastWarning("Mã ít nhất 2 ký tự!");
      return;
    }
    this.editItem.UserId = parseInt(localStorage.getItem("userId"));
    this.http.put('/api/Department/' + this.editItem.DepartmentId, this.editItem, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListDepartment();
          this.editModal.hide();
          this.toastSuccess("Cập nhật thành công!");
        } else if (res["meta"]["error_code"] == 211) {
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
    this.http.delete('/api/Department/' + Id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListDepartment();
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
  onlyNumber(num) {
    console.log(num);
    var e = num;
    var charCode = e.which || e.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57))
      return false;
    return true;
  }
}

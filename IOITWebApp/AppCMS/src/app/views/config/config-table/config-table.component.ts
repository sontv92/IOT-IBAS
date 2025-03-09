import { Component, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { ConfigTable, ConfigTableItem } from '../../../data/Model';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { ToastrService } from 'ngx-toastr';



@Component({
  selector: 'app-config-table',
  templateUrl: './config-table.component.html',
  styleUrls: ['./config-table.component.scss']
})
export class ConfigTableComponent implements OnInit {
  @ViewChild('AddModal') public addModal: ModalDirective;
  @ViewChild('EditModal') public editModal: ModalDirective;
  @ViewChild('AddTypeModal') public addtypeModal: ModalDirective;

  public paging: any;
  public q: any;

  public showItem: boolean;

  public listConfigTable = [];
  public showSort: boolean[] = [true, false];

  public ckeConfig: any;

  public newItem: ConfigTable;
  public editItem: ConfigTable;
  public newType: ConfigTableItem;

  public httpOptions: any;


  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
  ) {
    this.newItem = new ConfigTable();
    this.editItem = new ConfigTable();
    this.newType = new ConfigTableItem();

    this.paging = {
      page: 1,
      page_size: 10,
      query: '1=1',
      order_by: '',
      item_count: 0
    };
    this.q = {
      txtSearch:''
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
    this.GetListConfigTable();
  }

  //
  Sort(IsK, s, i) {
    if (IsK) {
      this.paging.order_by = s + " asc";
    }
    else {
      this.paging.order_by = s + " desc";
    }
    this.GetListConfigTable();
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

  //Get
  GetListConfigTable() {
    this.http.get('/api/ConfigTable/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listConfigTable = res["data"];
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
    this.GetListConfigTable();
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

    this.GetListConfigTable();
  }

  OpenAddModal() {
    this.newItem = new ConfigTable();
    this.newType = new ConfigTableItem();
    this.newItem.listConfigTableItem = [];
    this.showItem = true;
    this.addModal.show();
  }

  AddFunc() {
    if (this.newItem.Code == undefined || this.newItem.Code == '') {
      this.toastWarning("Chưa nhập mã!");
      return;
    } else if (this.newItem.Code.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập mã!");
      return;
    }
    else if (this.newItem.Name == undefined || this.newItem.Name == '') {
      this.toastWarning("Chưa nhập tên!");
      return;
    } else if (this.newItem.Name.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên!");
      return;
    }
    this.newItem.UserId = parseInt(localStorage.getItem("userId"));
    this.newItem.CompanyId = parseInt(localStorage.getItem("companyId"));
    this.http.post('/api/ConfigTable', this.newItem, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListConfigTable();
          this.addModal.hide();
          this.toastSuccess("Thêm mới thành công!");
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

  OpenEditModal(item) {
    this.editItem = new ConfigTable();
    this.newType = new ConfigTableItem();
    this.editItem = Object.assign(this.editItem, item);
    this.showItem = false;
    this.editModal.show();
  }

  EditFunc() {
    if (this.editItem.Code == undefined || this.editItem.Code == '') {
      this.toastWarning("Chưa nhập mã!");
      return;
    } else if (this.editItem.Code.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập mã!");
      return;
    }
    else if (this.editItem.Name == undefined || this.editItem.Name == '') {
      this.toastWarning("Chưa nhập tên!");
      return;
    } else if (this.editItem.Name.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên!");
      return;
    }
    this.editItem.UserId = parseInt(localStorage.getItem("userId"));
    this.editItem.CompanyId = parseInt(localStorage.getItem("companyId"));
    this.http.put('/api/ConfigTable/' + this.editItem.ConfigTableId, this.editItem, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListConfigTable();
          this.editModal.hide();
          this.toastSuccess("Cập nhật thành công!");
        } else if (res["meta"]["error_code"] == 222) {
          this.toastError("Bạn không có quyền thực hiện chức năng này!");
          }
        else {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
          console.log("error");
        }
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
      }
    );
  }

  // Config table item
  OpenAddItemModal() {
    this.newType = new ConfigTableItem();
    this.addtypeModal.show()
  }

  AddTypeFunc(IsNew) {
    if (this.newType.Code == undefined || this.newType.Code == '') {
      this.toastWarning("Chưa nhập mã!");
      return;
    } else if (this.newItem.Code.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập mã!");
      return;
    }
    else if (this.newType.Name == undefined || this.newType.Name == '') {
      this.toastWarning("Chưa nhập tên thuộc tính!");
      return;
    } else if (this.newItem.Name.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên thuộc tính!");
      return;
    }
    if (IsNew) {
      this.newItem.listConfigTableItem.push(this.newType);
    }
    else {
      this.editItem.listConfigTableItem.push(this.newType);
    }
    this.addtypeModal.hide()
  }

  //xoa
  ConfirmDelete(Id) {
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
    this.http.delete('/api/ConfigTable/' + Id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListConfigTable();
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


  //xóa AttributeItem
  DeleteItem(i, IsNew) {
    if (IsNew) {
      this.newItem.listConfigTableItem.splice(i, 1);
    }
    else {
      console.log(this.editItem.listConfigTableItem[i]);
      if (this.editItem.listConfigTableItem[i].ConfigTableItemId != null) {
        this.editItem.listConfigTableItem.splice(i, 1);
      }
      else {
        this.editItem.listConfigTableItem.splice(i, 1);
      }
    }
  }
  //End
}

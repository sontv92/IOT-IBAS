import { Component, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { ToastrService } from 'ngx-toastr';
import { TypeAttribute } from '../../../data/model';
import { TypeAttributeItem } from '../../../data/model';
import { Paging, QueryFilter } from '../../../data/dt';




@Component({
  selector: 'app-type-attribute',
  templateUrl: './type-attribute.component.html',
  styleUrls: ['./type-attribute.component.scss']
})
export class TypeAttributeComponent implements OnInit {
  @ViewChild('TypeAttributeModal') public TypeAttributeModal: ModalDirective;
  @ViewChild('TypeModal') public TypeModal: ModalDirective;

  public paging: Paging;
  public q: QueryFilter;

  public listTypeAttribute = [];
  public listTypeAttributeItem = [];
  public ckeConfig: any;
  public Item: TypeAttribute;
  public newType: TypeAttributeItem;
  public httpOptions: any;

  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
  ) {
    this.Item = new TypeAttribute();
    this.newType = new TypeAttributeItem();

    this.paging = new Paging();
    this.paging.page = 1;
    this.paging.page_size = 10;
    this.paging.query = "1=1";
    this.paging.order_by = "TypeAttributeId Desc";
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
    this.ckeConfig = {
      allowedContent: false,
      extraPlugins: 'divarea',
      forcePasteAsPlainText: true
    };

    this.GetListTypeAttribute();
  }

  //Get danh sách loại hình
  GetListTypeAttribute() {
    this.http.get('/api/typeattribute/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listTypeAttribute = res["data"];
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
    this.GetListTypeAttribute();
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

    this.GetListTypeAttribute();
  }

  //Mở modal thêm mới loại hình
  OpenTypeAttributeModal(item) {
    this.Item = new TypeAttribute();
    this.newType = new TypeAttributeItem();
    this.Item.listAttributeItem = [];
    if (item != undefined) {
      this.Item = JSON.parse(JSON.stringify(item));
    }
    this.TypeAttributeModal.show();
  }

  //Thêm mới loại hình
  SaveTypeAttribute() {
    if (this.Item.Name == undefined || this.Item.Name == '') {
      this.toastWarning("Chưa nhập tên loại hình!");
      return;
    } else if (this.Item.Name.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên loại hình!");
      return;
    }
    this.Item.UserId = parseInt(localStorage.getItem("userId"));
    if (this.Item.TypeAttributeId == undefined) {
      this.http.post('/api/TypeAttribute', this.Item, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListTypeAttribute();
            this.TypeAttributeModal.hide();
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
    else {
      this.http.put('/api/TypeAttribute/' + this.Item.TypeAttributeId, this.Item, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListTypeAttribute();
            this.TypeAttributeModal.hide();
            this.toastSuccess("Cập nhật thành công!");
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

  // // Mở modal cập nhật TypeAttribute
  // OpenEditModal(item) {
  //   this.editItem = new TypeAttribute();
  //   this.newType = new TypeAttributeItem();
  //   this.editItem = Object.assign(this.editItem, item);
  //   this.showItem = false;
  //   this.editModal.show();
  // }

  // // cập nhật typeAttribute
  // EditFunc() {
  //   if (this.editItem.Name == undefined || this.editItem.Name == '') {
  //     this.toastWarning("Chưa nhập tên!");
  //     return;
  //   }
  //   this.editItem.UserId = parseInt(localStorage.getItem("userId"));


  //   this.http.put('/api/TypeAttribute/' + this.editItem.TypeAttributeId, this.editItem, this.httpOptions).subscribe(
  //     (res) => {
  //       if (res["meta"]["error_code"] == 200) {
  //         this.GetListTypeAttribute();
  //         this.editModal.hide();
  //         this.toastSuccess("Cập nhật thành công!");
  //         console.log(this.editItem.listAttributeItem);
  //       }
  //       else {
  //         this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
  //         console.log("error");
  //       }
  //     },
  //     (err) => {
  //       this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
  //     }
  //   );
  // }


  //Mở modal thêm TypeAttributeItem
  OpenTypeModalModal(i) {
    this.newType = new TypeAttributeItem();
    if (i != undefined) {
      this.newType = JSON.parse(JSON.stringify(this.Item.listAttributeItem[i]));
    }
    this.TypeModal.show();
  }

  //Thêm TypeAttributeItem
  SaveTypeItem() {
    if (this.newType.Name == undefined || this.newType.Name == '') {
      this.toastWarning("Chưa nhập tên thuộc tính!");
      return;
    }


    if (this.newType.TypeAttributeItemId == undefined) {
      this.newType.Status = 1;
      this.Item.listAttributeItem.push(this.newType);
    }
    else {
      for (let i = 0; i < this.Item.listAttributeItem.length; i++) {
        if (this.newType.TypeAttributeItemId == this.Item.listAttributeItem[i].TypeAttributeItemId) {
          this.Item.listAttributeItem[i] = JSON.parse(JSON.stringify(this.newType));
        }
      }
    }
    this.TypeModal.hide()
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

  //Xóa TypeAttribute
  Delete(Id) {
    this.http.delete('/api/TypeAttribute/' + Id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListTypeAttribute();
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
  DeleteItem(i) {
    if (this.Item.TypeAttributeId == undefined) {
      this.Item.listAttributeItem.splice(i, 1);
    }
    else {
      if (this.Item.listAttributeItem[i].TypeAttributeItemId == undefined) {
        this.Item.listAttributeItem.splice(i, 1);
      }
      else {
        this.Item.listAttributeItem[i].Status = 99;
      }
    }
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

    this.GetListTypeAttribute();
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

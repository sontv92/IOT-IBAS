import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { Manufacturer } from '../../../data/model';
import { CommonService } from '../../../service/common.service';
import { ToastrService } from 'ngx-toastr';
import { domainImage } from '../../../data/const';
import { Paging, QueryFilter } from '../../../data/dt';



@Component({
  selector: 'app-manufacturer',
  templateUrl: './manufacturer.component.html',
  styleUrls: ['./manufacturer.component.scss']
})
export class ManufacturerComponent implements OnInit {
  @ViewChild('ModalManuFacture') public ModalManuFacture: ModalDirective;
  @ViewChild('file') file: ElementRef;

  public paging: Paging;
  public q: QueryFilter;

  public listManufacturer = [];
  public listCompany = [];

  public ckeConfig: any;

  public Item: Manufacturer;

  public progress: number;

  public domainImage = domainImage;

  public httpOptions: any;

  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
    public common: CommonService
  ) {
    this.Item = new Manufacturer();

    this.paging = new Paging();
    this.paging.page = 1;
    this.paging.page_size = 10;
    this.paging.query = "TypeOriginId=1";
    this.paging.order_by = "ManufacturerId Desc";
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
    this.GetListManufacturer();
  }

  // get ds nhà sản xuất
  GetListManufacturer() {
    this.http.get('/api/manufacturer/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listManufacturer = res["data"];
          this.listManufacturer.forEach(item => {
            item.IsShow = item.Status == 1 ? true : false;
          });
          this.paging.item_count = res["metadata"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  // get ds công ty
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

  //Chuyển trang
  PageChanged(event) {
    this.paging.page = event.page;
    this.GetListManufacturer();
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


  QueryChanged() {
    let query = 'TypeOriginId=1';
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

    this.GetListManufacturer();
  }

  //Mở modal
  OpenModalManuFacturer(item) {
    this.Item = new Manufacturer();
    this.file.nativeElement.value = "";
    this.progress = undefined;
    if (item != undefined) {
      this.Item = Object.assign(this.Item, item);
    }

    this.ModalManuFacture.show();
  }

  ChangeTitle(key) {
    switch (key) {
      case 1:
        this.Item.MetaTitle = this.Item.Name;
        this.Item.MetaKeywords = this.Item.Name;
        this.Item.Url = this.common.ConvertUrl(this.Item.Name);
        break;
      case 2:
        this.Item.MetaDescription = this.Item.Description;
        break;
      default:
        break;
    }
  }

  // cập nhật 
  SaveManuFacture() {
    if (this.Item.Code == undefined || this.Item.Code == '') {
      this.toastWarning("Chưa nhập Mã nhà sản xuất!");
      return;
    } else if (this.Item.Code.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập mã nhà sản xuất!");
      return;
    } else if (this.Item.Name == undefined || this.Item.Name == '') {
      this.toastWarning("Chưa nhập tên nhà sản xuất!");
      return;
    } else if (this.Item.Name.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên nhà sản xuất!");
      return;
    }
    this.Item.UserId = parseInt(localStorage.getItem("userId"));
    this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
    this.Item.TypeOriginId = 1;

    if (this.Item.ManufacturerId) {
      this.http.put('/api/Manufacturer/' + this.Item.ManufacturerId, this.Item, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListManufacturer();
            this.ModalManuFacture.hide();
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
        });
    }
    else {
      this.http.post('/api/Manufacturer', this.Item, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListManufacturer();
            this.ModalManuFacture.hide();
            this.toastSuccess("Thêm thành công!");
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
    this.http.delete('/api/Manufacturer/' + Id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListManufacturer();
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
  upload(files) {
    if (files.length === 0)
      return;

    const formData = new FormData();

    for (let file of files)
      formData.append(file.name, file);
    const uploadReq = new HttpRequest('POST', 'api/upload/uploadImage/5', formData, {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      }),
      reportProgress: true,
    });

    this.http.request(uploadReq).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress) {
        this.progress = Math.round(100 * event.loaded / event.total);
      }
      else if (event.type === HttpEventType.Response) {
        this.Item.Logo = event.body["data"].toString();
      }
    });
  }

  RemoveImage() {
    this.Item.Logo = undefined;
    this.file.nativeElement.value = "";
    this.progress = undefined;
  }

  ShowHide(id, i) {
    let stt = this.listManufacturer[i].IsShow ? 1 : 10;
    this.http.put('/api/Manufacturer/ShowHide/' + id + "/" + stt, undefined, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.toastSuccess("Thay đổi trạng thái thành công!");
        } else if (res["meta"]["error_code"] == 222) {
          this.toastError("Bạn không có quyền thực hiện chức năng này!");
          this.listManufacturer[i].IsShow = !this.listManufacturer[i].IsShow;
        }
        else {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
          this.listManufacturer[i].IsShow = !this.listManufacturer[i].IsShow;
        }
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        this.listManufacturer[i].IsShow = !this.listManufacturer[i].IsShow;
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

    this.GetListManufacturer();
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

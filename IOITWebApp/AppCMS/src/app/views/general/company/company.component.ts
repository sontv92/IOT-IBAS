import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef} from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Company } from '../../../data/model';
import { Paging, QueryFilter } from '../../../data/dt';
import { domainImage } from '../../../data/const';



@Component({
  selector: 'app-company',
  templateUrl: './company.component.html',
  styleUrls: ['./company.component.scss']
})
export class CompanyComponent implements OnInit {
  @ViewChild('CompanyModal') public CompanyModal: ModalDirective;
  @ViewChild('file') file: ElementRef;

  public paging: Paging;
  public q: QueryFilter;
  public regexEmail: '/^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/';
  public listCompany = [];
  public ckeConfig: any;
  public Item: Company;
  public httpOptions: any;
  public progress: number;
  public message: string;
  public domainImage = domainImage;

  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService
    
  ) {
    this.Item = new Company();
    this.paging = new Paging();
    this.paging.page = 1;
    this.paging.page_size = 10;
    this.paging.query = "1=1";
    this.paging.order_by = "CompanyId Desc";
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

    this.GetListCompany();
  }

  //GET
  GetListCompany() {
    this.http.get('/api/company/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listCompany = res["data"];
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
    this.GetListCompany();
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

    this.GetListCompany();
  }

  //Mở modal
  OpenCompanyModal(item) {
    this.Item = new Company();
    this.file.nativeElement.value = "";
    if (item != undefined) {
      this.Item = JSON.parse(JSON.stringify(item));
    }
    this.CompanyModal.show();
  }

  //Thêm mới
  SaveCompany() {
    if (this.Item.Code == undefined || this.Item.Code == '') {
      this.toastWarning("Chưa nhập Mã!");
      return;
    } else if (this.Item.Code.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập mã!");
      return;
    } else if (this.Item.Name == undefined || this.Item.Name == '') {
      this.toastWarning("Chưa nhập Tên!");
      return;
    } else if (this.Item.Name.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên!");
      return;
    } else if (this.Item.Email == undefined || this.Item.Email == '') {
      this.toastWarning("Chưa nhập Email!");
      return;
    } else if (this.Item.Email.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập Email!");
      return;
    } else if (this.Item.Phone == undefined || this.Item.Phone == '') {
      this.toastWarning("Chưa nhập số điện thoại!");
      return;
    } else if (this.Item.Phone.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập số điện thoại!");
      return;
    } else if (this.Item.Code.length < 2) {
      this.toastWarning("Mã ít nhất 2 ký tự!");
      return;
    }

    this.Item.UserId = parseInt(localStorage.getItem("userId"));

    if (this.Item.CompanyId == undefined) {
      this.http.post('/api/Company', this.Item, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListCompany();
            this.CompanyModal.hide();
            this.toastSuccess("Thêm thành công!");
          }
          else if (res["meta"]["error_code"] == 212) {
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
    else {
      this.http.put('/api/Company/' + this.Item.CompanyId, this.Item, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListCompany();
            this.CompanyModal.hide();
            this.toastSuccess("Cập nhật thành công!");
          }
          else if (res["meta"]["error_code"] == 212) {
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
        this.Item.Logo = this.message;
      }
    });
  }

  Delete(Id) {
    this.http.delete('/api/Company/' + Id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListCompany();
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

    this.GetListCompany();
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

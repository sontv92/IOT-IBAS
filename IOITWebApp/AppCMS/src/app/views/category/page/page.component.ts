import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpRequest, HttpEventType, HttpHeaders } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { typeCategoryPage } from '../../../data/const';
import { Category } from '../../../data/model';
import { Paging, QueryFilter } from '../../../data/dt';
import { ToastrService } from 'ngx-toastr';
import { CommonService } from '../../../service/common.service';
import { domainImage } from '../../../data/const';
declare var loadNestable;
import { Subscription } from 'rxjs';
import { CallCategoryFunctionService } from '../../../service/call-category-function.service';
import { Router } from '@angular/router';



@Component({
  selector: 'app-page',
  templateUrl: './page.component.html',
  styleUrls: ['./page.component.scss']
})
export class PageComponent implements OnInit, OnDestroy {
  @ViewChild('CatePageModal') public CatePageModal: ModalDirective;
  @ViewChild('file') file: ElementRef;
  @ViewChild('fileIcon') fileIcon: ElementRef;
  subscription: Subscription;


  public paging: Paging;
  public q: QueryFilter;
  public listLanguage = [];
  public listCatePage = [];
  public listCatePageParent = [];
  public ckeConfig: any;
  public typeCategoryPage = typeCategoryPage;
  public Item: Category;
  public progress: number;
  public progressIcon: number;
  public message: string;
  public messageIcon: string;
  public domainImage = domainImage;
  public httpOptions: any;

  public query = "arr=6&arr=7&arr=8&arr=9&arr=10";
  public total_item: number;

  key: string = 'categorySorts';


  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
    public common: CommonService,
    public callCategoryFunctionService: CallCategoryFunctionService,
    public elm: ElementRef,
    public router: Router
  ) {
    this.Item = new Category();
    this.paging = new Paging();
    this.paging.page = 1;
    this.paging.page_size = 10;
    this.paging.query = "(TypeCategoryId=6 OR TypeCategoryId=7 OR TypeCategoryId=8 OR TypeCategoryId=9 OR TypeCategoryId=10)";
    this.paging.order_by = "CategoryId Desc";
    this.paging.item_count = 0;

    this.q = new QueryFilter();
    this.q.txtSearch = "";

    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      })
    }

    this.subscription = this.callCategoryFunctionService.getAction().subscribe(action => {
      if (action.TypeAction == 1) {
        this.OpenCatePageModal(undefined, action.CategoryId);
      } else if (action.TypeAction == 2) {
        this.OpenCatePageModal(action.CategoryId, undefined);
      } else if (action.TypeAction == 3) {
        this.ShowConfirmDelete(action.CategoryId);
      }
    });
  }

  ngOnInit() {

    this.ckeConfig = {
      allowedContent: false,
      extraPlugins: 'divarea',
      forcePasteAsPlainText: true
    };
    this.GetListCatePage();
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
    this.router.onSameUrlNavigation = 'ignore';
  } sss

  upload(files, Type) {
    if (files.length === 0)
      return;

    const formData = new FormData();

    for (let file of files)
      formData.append(file.name, file);
    console.log(formData);
    const uploadReq = new HttpRequest('POST', 'api/upload/uploadImage/' + Type, formData, {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      }),
      reportProgress: true,
    });

    this.http.request(uploadReq).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress) {
        if (Type == 5) {
          this.progress = Math.round(100 * event.loaded / event.total);
        }
        else {
          this.progressIcon = Math.round(100 * event.loaded / event.total);
        }
      }
      else if (event.type === HttpEventType.Response) {
        if (Type == 5) {
          this.message = event.body["data"].toString();
          this.Item.Image = this.message
        }
        else {
          this.messageIcon = event.body["data"].toString();
          this.Item.Icon = this.messageIcon;
        }
      }
    });
  }


  //Get danh sách danh mục trang
  GetListCatePage() {
    this.listCatePage = [];
    this.http.get('/api/category/GetCategorySort?' + this.query, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listCatePage = res["data"];
          this.total_item = res["metadata"]
          loadNestable();
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  //Get danh sách danh mục cha
  GetListCatePageParent(Id) {
    this.http.get('/api/category/GetByTree?arr=6&arr=7&arr=8&arr=9&arr=10', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listCatePageParent = res["data"];
          this.listCatePageParent.forEach(item => {
            if (item.CategoryId == Id) {
              item.disabled = true;
            }

            item.Space = "";
            for (var i = 0; i < (item.Level - 1) * 7; i++) {
              item.Space += "&nbsp;";
            }
          })
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  // Get danh sách ngôn ngữ
  GetListLanguage() {
    this.http.get('/api/Language/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listLanguage = res["data"];
          if (this.listLanguage.length == 1 && (this.Item.CategoryId == undefined || (this.Item.CategoryId != undefined && this.Item.LanguageId == undefined))) {
            this.Item.LanguageId = this.listLanguage[0].LanguageId;
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
    this.GetListCatePage();
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
    let query = '(TypeCategoryId=6 OR TypeCategoryId=7 OR TypeCategoryId=8 OR TypeCategoryId=9 OR TypeCategoryId=10)';
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

    this.GetListCatePage();
  }

  //Mở modal thêm mới
  OpenCatePageModal(CategoryId, CategoryParentId) {
    this.Item = new Category();
    this.Item.CategoryParentId = CategoryParentId;
    this.file.nativeElement.value = "";
    this.fileIcon.nativeElement.value = "";
    this.message = undefined;
    this.messageIcon = undefined;
    this.progress = undefined;
    this.progressIcon = undefined;
    if (CategoryId != undefined) {
      this.http.get('/api/category/' + CategoryId, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.Item = Object.assign(this.Item, res["data"]);
            if (this.Item.CategoryParentId == 0) this.Item.CategoryParentId = undefined;
            this.GetListCatePageParent(this.Item.CategoryId);
            this.CatePageModal.show();
          }
          else {
            this.toastError("Không tìm thấy danh mục trên hệ thống!");
            return;
          }
        },
        (err) => {
          this.toastError("Không tìm thấy danh mục trên hệ thống!");
          return;
        }
      );
    }
    else {
      this.GetListCatePageParent(undefined);
      this.CatePageModal.show();
    }

    this.GetListLanguage();
  }

  //Thêm mới danh mục trang
  SaveCatePage() {
    if (this.Item.Code == undefined || this.Item.Code == '') {
      this.toastWarning("Chưa nhập Mã danh mục!");
      return;
    } else if (this.Item.Code.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập mã danh mục");
      return;
    } else if (this.Item.Name == undefined || this.Item.Name == '') {
      this.toastWarning("Chưa nhập Tên danh mục!");
      return;
    } else if (this.Item.Name.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên danh mục !");
      return;
    } else if (this.Item.TypeCategoryId == undefined || this.Item.TypeCategoryId == 0) {
      this.toastWarning("Chưa chọn Loại danh mục!");
      return;
    } else if (this.Item.LanguageId == undefined) {
      this.toastWarning("Chưa chọn ngôn ngữ!");
      return;
    }

    this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
    this.Item.UserId = parseInt(localStorage.getItem("userId"));
    this.Item.WebsiteId = parseInt(localStorage.getItem("websiteId"));
    if (!this.Item.LanguageId) {
      this.Item.LanguageId = parseInt(localStorage.getItem("languageId"));
    }

    if (this.Item.CategoryId) {
      this.http.put('/api/Category/' + this.Item.CategoryId, this.Item, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListCatePage();
            this.CatePageModal.hide();
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
    else {
      this.http.post('/api/Category', this.Item, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListCatePage();
            this.CatePageModal.hide();
            this.toastSuccess("Thêm mới thành công!");
          }
          else if (res["meta"]["error_code"] == 213) {
            this.toastWarning("Tên đã tồn tại!");
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
  //Change title
  ChangeTitle(key) {
    switch (key) {
      case 1:
        this.Item.MetaTitle = this.Item.Name;
        this.Item.MetaKeyword = this.Item.Name;
        this.Item.Url = this.common.ConvertUrl(this.Item.Name);
        break;
      case 2:
        this.Item.MetaDescription = this.Item.Description;
        break;
      default:
        break;
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
            this.DeleteCatePage(Id);
          }
        },
        {
          text: 'Đóng',
          buttonClass: 'btn btn-default',

        }
      ],
    });
  }

  DeleteCatePage(Id) {
    this.http.delete('/api/Category/' + Id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListCatePage();
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

  findParent(item) {
    if (item == undefined) {
      return "";
    }
    else {
      return item.Name;
    }
  }

  RemoveImage(Type) {
    if (Type == 5) {
      this.file.nativeElement.value = "";
      this.Item.Image = undefined;
      this.message = undefined;
      this.progress = undefined;
    }
    else {
      this.fileIcon.nativeElement.value = "";
      this.Item.Icon = undefined;
      this.messageIcon = undefined;
      this.progressIcon = undefined;
    }
  }

  ShowHide(id, i) {
    let stt = this.listCatePage[i].IsShow ? 1 : 10;
    this.http.put('/api/Category/ShowHide/' + id + "/" + stt, undefined, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.toastSuccess("Thay đổi trạng thái thành công!");
        } else if (res["meta"]["error_code"] == 222) {
          this.toastError("Bạn không có quyền thực hiện chức năng này!");
          this.listCatePage[i].IsShow = !this.listCatePage[i].IsShow;
        }
        else {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
          this.listCatePage[i].IsShow = !this.listCatePage[i].IsShow;
        }
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        this.listCatePage[i].IsShow = !this.listCatePage[i].IsShow;
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

    this.GetListCatePage();
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

  SaveSortCategory() {
    let attribute = document.getElementById("nestable");
    let Arr = [];
    this.common.ConvertHtmlToJson(Arr, attribute, "#nestable", 0, 1);

    this.http.post('/api/Category/SaveCategorySort', Arr, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.ResetCurrentRouter();
          this.CatePageModal.hide();
          this.toastSuccess("Lưu thông tin sắp xếp thành công!");
        }
        else if (res["meta"]["error_code"] == 222) {
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

  ResetCurrentRouter() {
    this.router.routeReuseStrategy.shouldReuseRoute = function() {
      return false;
    };
    this.router.onSameUrlNavigation = 'reload';
    this.router.navigateByUrl(this.router.url);
  }
}

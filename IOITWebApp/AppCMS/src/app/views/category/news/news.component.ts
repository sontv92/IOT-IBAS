import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { typeCategoryNews, domainImage } from '../../../data/const';
import { ToastrService } from 'ngx-toastr';
import { Category } from '../../../data/model';
import { Paging, QueryFilter } from '../../../data/dt';
import { CommonService } from '../../../service/common.service';
import * as $ from 'jquery';
declare var loadNestable;
import { Subscription } from 'rxjs';
import { CallCategoryFunctionService } from '../../../service/call-category-function.service';
import { Router } from '@angular/router';



@Component({
  selector: 'app-news',
  templateUrl: './news.component.html',
  styleUrls: ['./news.component.scss']
})

export class NewsComponent implements OnInit, OnDestroy {
  @ViewChild('CateNewsModal') public CateNewsModal: ModalDirective;
  @ViewChild('SortNewsModal') public SortNewsModal: ModalDirective;
  @ViewChild('file') file: ElementRef;
  @ViewChild('fileIcon') fileIcon: ElementRef;
  subscription: Subscription;

  public listCateNews = [];
  public listCateParent = [];
  public listLanguage = [];
  public listOrderByCat = [];


  public ckeConfig: any;

  public typeCategoryNews = typeCategoryNews;

  public Item: Category;

  public progress: number;
  public message: string;

  public progressIcon: number;
  public messageIcon: string;

  public domainImage = domainImage;

  public httpOptions: any;

  public total_item: number;
  public txtSearch: string;
  public query = "arr=1&arr=2&arr=3&arr=4&arr=5";

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

    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      })
    }

    this.subscription = this.callCategoryFunctionService.getAction().subscribe(action => {
      if (action.TypeAction == 1) {
        this.OpenCateNewsModal(undefined, action.CategoryId);
      } else if (action.TypeAction == 2) {
        this.OpenCateNewsModal(action.CategoryId, undefined);
      } else if (action.TypeAction == 3) {
        this.ShowConfirmDelete(action.CategoryId);
      }
    });
  }

  ngOnInit() {

    this.GetListCateNews();
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
    this.router.onSameUrlNavigation = 'ignore';
  }

  //Get danh sách tin
  GetListCateNews() {
    this.listCateNews = [];
    this.http.get('/api/category/GetCategorySort?' + this.query, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listCateNews = res["data"];
          this.total_item = res["metadata"]
          loadNestable();
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  QueryChanged() {
    let query = "arr=1&arr=2&arr=3&arr=4&arr=5";
    if (this.txtSearch != undefined && this.txtSearch != "") {
      this.query = query + "&txtSearch=" + this.txtSearch;
    }
    else {
      this.query = query;
    }

    this.GetListCateNews();
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

  GetListOrderByCat() {
    this.http.get('api/category/listNews/' + this.Item.CategoryId, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listOrderByCat = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  //Open sắp xếp tin
  OpenSortNewsModal(item) {
    this.Item = JSON.parse(JSON.stringify(item));
    this.listOrderByCat = [];
    this.GetListOrderByCat();
    this.SortNewsModal.show();
  }

  SaveSortNews() {
    for (let i = this.listOrderByCat.length; i > 0; i--) {
      this.listOrderByCat[i - 1].Location = (this.listOrderByCat.length - i) + 1;
    }

    this.http.put('/api/category/sortCategoryMapping/' + this.Item.CategoryId, this.listOrderByCat, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.SortNewsModal.hide();
          this.toastSuccess("Lưu thành công!");
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

  //Get danh sách danh mục cha
  GetListCateParent(Id) {
    console.log(Id);
    this.http.get('/api/category/GetByTree?arr=1&arr=2&arr=3&arr=4&arr=5', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listCateParent = res["data"];
          this.listCateParent.forEach(item => {
            if (item.CategoryId == Id || item.Genealogy.indexOf(Id) != -1)
              item.disabled = true;
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

  //Mở modal thêm mới
  OpenCateNewsModal(CategoryId, CategoryParentId) {
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
            this.GetListCateParent(this.Item.CategoryId);
            this.CateNewsModal.show();
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
      this.GetListCateParent(undefined);
      this.CateNewsModal.show();
    }

    this.GetListLanguage();

  }

  //Thêm mới danh mục trang
  SaveCateNews() {
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
      this.toastWarning("Chưa nhập tên danh mục");
      return;
    } else if (this.Item.Url == undefined || this.Item.Url == '') {
      this.toastWarning("Chưa nhập Đường dẫn!");
      return;
    } else if (this.Item.Url.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập đường dẫn!");
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
            this.ResetCurrentRouter();
            this.CateNewsModal.hide();
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
            this.ResetCurrentRouter();
            this.CateNewsModal.hide();
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
        text: "Bạn có chắc chắn muốn xóa danh mục này và các danh mục con của nó?"
      },
      actionButtons: [
        {
          text: 'Đồng ý',
          buttonClass: 'btn btn-success',
          onAction: () => {
            console.log('OnAction');
            this.DeleteCateNews(Id);
          }
        },
        {
          text: 'Đóng',
          buttonClass: 'btn btn-default',

        }
      ],
    });
  }

  DeleteCateNews(Id) {
    this.http.delete('/api/Category/' + Id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.ResetCurrentRouter();
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
    let stt = this.listCateNews[i].IsShow ? 1 : 10;
    this.http.put('/api/Category/ShowHide/' + id + "/" + stt, undefined, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.toastSuccess("Thay đổi trạng thái thành công!");
        } else if (res["meta"]["error_code"] == 222) {
          this.toastError("Bạn không có quyền thực hiện chức năng này!");
          this.listCateNews[i].IsShow = !this.listCateNews[i].IsShow;
        }
        else {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
          this.listCateNews[i].IsShow = !this.listCateNews[i].IsShow;
        }
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        this.listCateNews[i].IsShow = !this.listCateNews[i].IsShow;
      }
    );
  }

  SaveSortCategory() {
    let attribute = document.getElementById("nestable");
    let Arr = [];
    this.common.ConvertHtmlToJson(Arr, attribute, "#nestable", 0, 1);

    this.http.post('/api/Category/SaveCategorySort', Arr, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.ResetCurrentRouter();
          this.CateNewsModal.hide();
          this.toastSuccess("Lưu thông tin sắp xếp thành công!");
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

  ResetCurrentRouter() {
    this.router.routeReuseStrategy.shouldReuseRoute = function() {
      return false;
    };
    this.router.onSameUrlNavigation = 'reload';
    this.router.navigateByUrl(this.router.url);
  }
}

import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { typeCategoryNews, domainImage } from '../../../data/const';
import { News, Attactment, Website } from '../../../data/model';
import { ToastrService } from 'ngx-toastr';
import { DatePipe } from '@angular/common';
import { CommonService } from '../../../service/common.service';
import { Paging, QueryFilter } from '../../../data/dt';
import { DateTimeAdapter, OWL_DATE_TIME_FORMATS, OWL_DATE_TIME_LOCALE } from 'ng-pick-datetime';
import { MomentDateTimeAdapter } from 'ng-pick-datetime-moment';
import { TabsetComponent } from 'ngx-bootstrap/tabs';



export const MY_CUSTOM_FORMATS = {
  parseInput: 'DD/MM/YYYY HH:mm',
  fullPickerInput: 'DD/MM/YYYY HH:mm',
  datePickerInput: 'DD/MM/YYYY',
  timePickerInput: ' HH:mm',
  monthYearLabel: 'MMM YYYY',
  dateA11yLabel: 'LL',
  monthYearA11yLabel: 'MMMM YYYY'
};


@Component({
  selector: 'app-news-text',
  templateUrl: './news-text.component.html',
  styleUrls: ['./news-text.component.scss'],
  providers: [
    { provide: DateTimeAdapter, useClass: MomentDateTimeAdapter, deps: [OWL_DATE_TIME_LOCALE] },
    { provide: OWL_DATE_TIME_FORMATS, useValue: MY_CUSTOM_FORMATS }
  ]
})
export class NewsTextComponent implements OnInit {
  @ViewChild('NewsModal') public NewsModal: ModalDirective;
  @ViewChild('HighlightNewsModal') public HighlightNewsModal: ModalDirective;
  @ViewChild('file') file: ElementRef;
  @ViewChild('attachment') attachment: ElementRef;
  @ViewChild('tabset') tabset: TabsetComponent;


  public paging: Paging;
  public q: QueryFilter;
  public IsAll: boolean;
  public listNews = [];
  public listOrderByProduct = [];
  public listNewsT = [];
  public listCateNews = [];
  public listSuggestNews = [];
  public Tag: string;
  public CheckBoxStatus: boolean;

  public ckeConfig: any;
  public objNew: any;

  public listTypeNews = typeCategoryNews;

  public Item: News;

  public progress: number;
  public progressAttachment: number;

  public message: string;
  public domainImage = domainImage;
  public CheckConfirmNews: boolean;
  public website: Website;

  public httpOptions: any;

  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
    public datePipe: DatePipe,
    public common: CommonService
  ) {
    this.Item = new News();
    this.paging = new Paging();
    this.website = new Website();
    this.paging.page = 1;
    this.paging.page_size = 10;
    this.paging.query = "IsService != true";
    this.paging.order_by = "NewsId Desc";
    this.paging.item_count = 0;

    this.q = new QueryFilter();
    this.q.txtSearch = "";
    // this.q = {
    //   cate: -1,
    //   type: -1,
    //   txtSearch: ''
    // }
    this.IsAll = false;

    this.CheckConfirmNews = this.common.CheckAccessKey(localStorage.getItem("access_key"), "DBV");

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

    this.GetListNews();
    this.GetListCateNews();
  }

  //Lấy toàn bộ danh sách tin văn bản
  GetListAllNews() {
    let query = "";
    if (this.Item.NewsId != undefined) {
      query = "IsService != true and TypeNewsId == 1 and NewsId !=" + this.Item.NewsId;
    }
    else {
      query = "IsService != true and TypeNewsId == 1";
    }
    this.http.get('/api/news/GetByPage?page=1&query=' + query + '&order_by=&select=NewsId,Title,Url,Image', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listSuggestNews = res["data"];
          this.listSuggestNews.forEach(item => {
            item.Check = false;
          });

          if (this.Item.NewsId != undefined) {
            for (var i = 0; i < this.listSuggestNews.length; i++) {
              for (var j = 0; j < this.Item.listRelated.length; j++) {
                if (this.listSuggestNews[i].NewsId == this.Item.listRelated[j].TargetRelatedId) {
                  this.listSuggestNews[i].Check = true;
                  break;
                }
              }
            }
          }
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  //Get danh sách danh bài viết
  GetListNews() {
    this.http.get('/api/news/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listNews = res["data"];
          this.listNews.forEach(item => {
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

  GetListCateNews() {
    // this.Item.listCategory = [];
    console.log(this.Item.listCategory);
    let query = 'TypeCategoryId=1 OR TypeCategoryId=2 OR TypeCategoryId=3 OR TypeCategoryId=4 OR TypeCategoryId=5';
    if (!this.IsAll) {
      query = this.Item.TypeNewsId != undefined ? "TypeCategoryId=" + this.Item.TypeNewsId : "TypeCategoryId=-1";
    }

    this.http.get('/api/category/GetByPage?page=1&query=' + query, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listCateNews = [];
          if (res["data"].length > 0) {
            res["data"].forEach(cate => {
              this.listCateNews.push({ CategoryId: cate.CategoryId, Name: cate.Name, Check: false });
            });

            if (this.Item.NewsId != undefined) {
              for (let i = 0; i < this.listCateNews.length; i++) {
                for (let j = 0; j < this.Item.listCategory.length; j++) {
                  if (this.listCateNews[i].CategoryId == this.Item.listCategory[j].CategoryId) {
                    this.listCateNews[i].Check = true;
                    break;
                  }
                }
              }
            }
          }
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  GetListOrderBy() {
    this.http.get('/api/orderby/GetOrderBy/11', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listOrderByProduct = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  OpenChooseHighlightsNews() {
    this.listOrderByProduct = [];
    this.GetListOrderBy();
    this.HighlightNewsModal.show();
  }

  SaveHighlightNews() {
    this.http.post('/api/orderby', this.listOrderByProduct, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.HighlightNewsModal.hide();
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

  DeleteOrderBy(item) {
    for (let i = 0; i < this.listOrderByProduct.length; i++) {
      if (this.listOrderByProduct[i].CategoryMappingId == item.CategoryMappingId) {
        this.listOrderByProduct[i].Status = 99;
        break;
      }
    }
  }
  //Chuyển trang
  PageChanged(event) {
    this.paging.page = event.page;
    this.GetListNews();
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
    let query = 'IsService != true';
    if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
      if (query != '') {
        query += ' and Title.Contains("' + this.q.txtSearch + '")';
      }
      else {
        query += 'Title.Contains("' + this.q.txtSearch + '")';
      }
    }

    if (this.q["Type"] != undefined) {
      if (query != '') {
        query += ' and TypeNewsId=' + this.q["Type"];
      }
      else {
        query += 'TypeNewsId=' + this.q["Type"];
      }
    }

    if (query == '')
      this.paging.query = '1=1';
    else
      this.paging.query = query;

    this.GetListNews();
  }

  //Mở modal thêm mới
  OpenNewsModal(item) {
    //this.tabset.tabs[0].active = true;
    this.Item = new News();
    this.Item.listCategory = [];
    this.Item.listTag = [];
    this.Item.listAttachment = [];
    this.Tag = "";
    this.IsAll = true;
    if (this.file) this.file.nativeElement.value = "";

    this.message = undefined;
    this.progress = undefined;

    this.progressAttachment = undefined;
    this.GetListAllNews();
    this.GetListCateNews();
    this.Item.TypeNewsId = 1;

    this.CheckBoxStatus = true;
    if (item != undefined) {
      // this.Item = Object.assign(this.Item, item);
      this.Item = JSON.parse(JSON.stringify(item));
      this.CheckBoxStatus = this.Item.Status == 1 ? true : false;
    }
    
    this.NewsModal.show();
  }
  //Thêm mới danh mục trang
  SaveNews() {
    if (this.Item.TypeNewsId == undefined) {
      this.toastWarning("Chưa chọn Loại tin!");
      return;
    } else if (this.Item.Title == undefined || this.Item.Title == '') {
      this.toastWarning("Chưa nhập Tiêu đề!");
      return;
    } else if (this.Item.Title.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tiêu đề!");
      return;
    } else if (this.Item.Url == undefined || this.Item.Url == '') {
      this.toastWarning("Chưa nhập Đường dẫn!");
      return;
    } else if (this.Item.Url.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập đường dẫn!");
      return;
    }
    else if (this.Item.TypeNewsId != 3 && this.Item.TypeNewsId != 4 && (this.Item.Contents == undefined || this.Item.Contents == '')) {
      this.toastWarning("Chưa nhập Nội dung!");
      return;
    }

    this.Item.Status = this.CheckBoxStatus ? 1 : 10;
    this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
    this.Item.UserId = parseInt(localStorage.getItem("userId"));

    if (typeof this.Item.DateStartActive === 'object' && this.Item.DateStartActive != undefined) {
      let DateStartActive = this.Item.DateStartActive.add(7, 'hours');
      this.Item.DateStartActive = DateStartActive.toISOString();
    }

    if (typeof this.Item.DateStartOn === 'object' && this.Item.DateStartOn != undefined) {
      let DateStartOn = this.Item.DateStartOn.add(7, 'hours');
      this.Item.DateStartOn = DateStartOn.toISOString();
    }

    if (typeof this.Item.DateEndOn === 'object' && this.Item.DateEndOn != undefined) {
      let DateEndOn = this.Item.DateEndOn.add(7, 'hours');
      this.Item.DateEndOn = DateEndOn.toISOString();
    }
    
    let obj = Object.assign({}, this.Item);
    obj.listRelated = [];
    this.listSuggestNews.forEach(item => {
      if (item.Check == true) {
        let it = { TargetRelatedId: item.NewsId }
        obj.listRelated.push(it);
      }
    });

    if (this.Item.NewsId == undefined) {
      //this.Item.listCategory = [];
      //this.listCateNews.forEach(item => {
      //  if (item.Check) {
      //    this.Item.listCategory.push(item);
      //  }
      //});

      obj.listCategory = this.listNewsT;

      this.http.post('/api/news', obj, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListNews();
            this.NewsModal.hide();
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
      let arr = [];
      obj.listCategory.forEach(item => {
        var flag = false;
        for (var i = 0; i < this.listCateNews.length; i++) {
          if (item.CategoryId == this.listCateNews[i].CategoryId && this.listCateNews[i].Check == true) {
            flag = true;
            break;
          }
        }

        if (!flag) {
          item.Check = false;
          arr.push(item);
        }
      });

      obj.listCategory = arr.concat(this.listCateNews.filter(e => e.Check == true));

      this.http.put('/api/news/' + obj.NewsId, obj, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListNews();
            this.NewsModal.hide();
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

  ToggleCateToList(id) {
    console.log(this.listCateNews);
    this.listNewsT = [];
    //if (this.Item.listCategory.includes(id)) {
    //  let index = this.Item.listCategory.indexOf(id, 0);
    //  this.Item.listCategory.splice(index, 1);
    //}
    //else
    //  this.Item.listCategory.push(id);
    for (var i = 0; i < this.listCateNews.length; i++) {
      if (this.listCateNews[i].Check == true) {
        this.objNew = this.listCateNews[i];
        this.listNewsT.push(this.objNew);
      }
    }
    console.log('---');
    //console.log(this.listNewsT);
  }

  AddTag() {
    if (this.Tag != undefined && this.Tag != '') {
      this.Item.listTag.push({ TagId: null, Name: this.Tag, Check: false });
      this.Tag = '';
    }
  }

  RemoveTag(i) {
    if (this.Item.NewsId == undefined) {
      this.Item.listTag.splice(i, 1);
    }
    else {
      if (this.Item.listTag[i].TagId != null) {
        this.Item.listTag[i].Check = false;
      }
      else {
        this.Item.listTag.splice(i, 1);
      }
    }
  }

  ChangeTitle(key) {
    switch (key) {
      case 1:
        this.Item.MetaTitle = this.Item.Title;
        this.Item.MetaKeyword = this.Item.Title;
        this.Item.Url = this.common.ConvertUrl(this.Item.Title);
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
            this.DeleteNews(Id);
          }
        },
        {
          text: 'Đóng',
          buttonClass: 'btn btn-default',

        }
      ],
    });
  }

  DeleteNews(Id) {
    this.http.delete('/api/news/' + Id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListNews();
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

  upload(files, cs) {
    if (files.length === 0)
      return;

    const formData = new FormData();

    for (let file of files)
      formData.append(file.name, file);

    const uploadReq = new HttpRequest('POST', 'api/upload/uploadImage/1', formData, {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      }),
      reportProgress: true,
    });

    this.http.request(uploadReq).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress)
        switch (cs) {
          case 1:
            this.progress = Math.round(100 * event.loaded / event.total);
            break;
          case 2:
            this.progressAttachment = Math.round(100 * event.loaded / event.total);
            this.attachment.nativeElement.value = "";
            break;
          default:
            break;
        }
      else if (event.type === HttpEventType.Response) {
        switch (cs) {
          case 1:
            this.message = event.body["data"].toString();
            this.Item.Image = this.message;
            break;
          case 2:
            this.attachment.nativeElement.value = "";

            event.body["data"].forEach(item => {
              let attachment = new Attactment();
              attachment.Url = item;
              attachment.IsImageMain = false;
              attachment.Status = 1;
              this.Item.listAttachment.push(attachment);
            });
            break;
          default:
            break;
        }
      }
    });
  }

  findAuthor(item) {
    if (item == undefined) {
      return "";
    }
    else {
      return item.FullName;
    }
  }

  RemoveImage() {
    this.Item.Image = undefined;
    this.file.nativeElement.value = "";
    this.message = undefined;
    this.progress = undefined;
  }

  ShowHide(id, i) {
    let stt = this.listNews[i].IsShow ? 1 : 10;
    this.http.put('/api/news/ShowHide/' + id + "/" + stt, undefined, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.toastSuccess("Thay đổi trạng thái thành công!");
        } else if (res["meta"]["error_code"] == 222) {
          this.toastError("Bạn không có quyền thực hiện chức năng này!");
          this.listNews[i].IsShow = !this.listNews[i].IsShow;
          }
        else {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
          this.listNews[i].IsShow = !this.listNews[i].IsShow;
        }
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        this.listNews[i].IsShow = !this.listNews[i].IsShow;
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

    this.GetListNews();
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

  RemoveAttachment(idx) {
    if (this.Item.listAttachment[idx].AttactmentId == undefined) {
      this.Item.listAttachment.splice(idx, 1);
    }
    else {
      this.Item.listAttachment[idx].Status = 99;
    }
  }

  SetIsMain(idx) {
    for (let i = 0; i < this.Item.listAttachment.length; i++) {
      this.Item.listAttachment[i].IsImageMain = false;
      if (idx == i) {
        this.Item.listAttachment[i].IsImageMain = true;
      }
    }
  }
}

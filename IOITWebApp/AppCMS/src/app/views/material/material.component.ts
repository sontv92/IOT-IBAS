import { Component, OnInit, ViewChild, ElementRef, ViewContainerRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpParams, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { typeCategoryNews, domainImage } from '../../data/const';
import { News, Material } from '../../data/model';
import { ToastrService } from 'ngx-toastr';
import { DatePipe } from '@angular/common';
import { debug } from 'util';
import { CommonService } from '../../service/common.service';



@Component({
  selector: 'app-material',
  templateUrl: './material.component.html',
  styleUrls: ['./material.component.scss']
})
export class MaterialComponent implements OnInit {
  @ViewChild('AddModal') public addModal: ModalDirective;
  @ViewChild('EditModal') public editModal: ModalDirective;
  @ViewChild('file') file: ElementRef;
  @ViewChild('fileE') fileE: ElementRef;

  public paging: any;
  public q: any;
  public IsAll: boolean;
  public showSort: boolean[] = [true, false, false];
  public showRemove: boolean = false;

  public listMaterial = [];
  public listCategorys = [];
  public listProvince = [];
  public listYear = [];
  public listMonth = [];
  public CheckBoxStatus: boolean;


  public listTypeNews = typeCategoryNews;

  public newItem: Material;
  public editItem: Material;

  public progress: number;
  public message: string;
  public domainImage = domainImage;

  public httpOptions: any;

  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
    public common: CommonService
  ) {
    this.newItem = new Material();
    this.editItem = new Material();
    this.paging = {
      page: 1,
      page_size: 10,
      query: '1=1',
      order_by: '',
      item_count: 0
    };

    this.q = {
      cate: -1,
      type: -1,
      txtSearch: ''
    }
    this.IsAll = true;

    this.httpOptions = {
        headers: new HttpHeaders({
          'Authorization': 'bearer ' + localStorage.getItem("access_token")
        })
      }
  }

  ngOnInit() {
    this.GetListMaterial();
    this.GetListProvince();
    this.GetListYear();
    this.GetListMonth();
  }

  //Get danh sách danh mục vật liệu
  GetListMaterial() {
    this.http.get('/api/material/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listMaterial = res["data"];
          this.paging.item_count = res["metadata"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  GetListProvince() {
    this.http.get('/api/province/GetByPage?page=1&page_size=100&query=1=1&order_by=ProvinceId asc' , this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listProvince = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  GetListYear() {
    let dateNow = new Date();
    for (var i = dateNow.getFullYear(); i >= dateNow.getFullYear() - 10; i--) {
      this.listYear.push({ YearId: i, Name: i });
    }
  }

  GetListMonth() {
    let dateNow = new Date();
    for (var i = 1; i <=12; i++) {
      this.listMonth.push({ MonthId: i, Name: 'Thàng ' + i });
    }
  }

  //Chuyển trang
  PageChanged(event) {
    this.paging.page = event.page;
    this.GetListMaterial();
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
        query += ' and Title.Contains("' + this.q.txtSearch + '")';
      }
      else {
        query += 'Title.Contains("' + this.q.txtSearch + '")';
      }
    }

    if (query == '')
      this.paging.query = '1=1';
    else
      this.paging.query = query;

    this.GetListMaterial();
  }

  Filter() {
    let query = "1=1";

    if (query == 'TypeNewsId=null' || query == 'CategoryId=null')
      this.paging.query = '1=1';
    else
      this.paging.query = query;

    this.GetListMaterial();
  }

  //Mở modal thêm mới
  OpenAddModal() {
    this.newItem = new Material();
    this.newItem.listMaterialImportExcelChild = [];
    this.file.nativeElement.value = "";
    //this.message = undefined;
    this.newItem.Note = "";
    this.addModal.show();
  }
  //Thêm mới danh mục vật liệu
  AddNew() {
    if (this.newItem.ProvinceId == undefined) {
      this.toastWarning("Chưa chọn Tỉnh/Thành phố!");
      return;
    }
    if (this.newItem.Year == undefined) {
      this.toastWarning("Chưa chọn năm!");
      return;
    }
    if (this.newItem.Month == undefined) {
      this.toastWarning("Chưa nhập tháng!");
      return;
    }
    if (this.newItem.listMaterialImportExcelChild.length <=0) {
      this.toastWarning("Chưa nhập danh sách danh mục vật liệu xây dựng!");
      return;
    }

    this.newItem.Status = 1;
    this.newItem.UserId = parseInt(localStorage.getItem("userId"));


    this.http.post('/api/material', this.newItem, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListMaterial();
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

  //ToggleCateToList(id) {
  //  //console.log(this.listCateNews);
  //  //return;
  //  //if (this.newItem.listCategory.includes(id)) {
  //  //  let index = this.newItem.listCategory.indexOf(id, 0);
  //  //  this.newItem.listCategory.splice(index, 1);
  //  //}
  //  //else
  //  //  this.newItem.listCategory.push(id);
  //}

  //AddTag(IsNew) {
  //  if (this.Tag != undefined && this.Tag != '') {
  //    if (IsNew) {
  //      this.newItem.listTag.push({ TagId: null, Name: this.Tag, Check: false });
  //    }
  //    else {
  //      this.editItem.listTag.push({ TagId: null, Name: this.Tag, Check: false });
  //    }
  //    this.Tag = '';
  //  }
  //}

  //RemoveTag(i, IsNew) {
  //  //let index = this.newItem.listTag.indexOf(tag, 0);
  //  if (IsNew) {
  //    this.newItem.listTag.splice(i, 1);
  //  }
  //  else {
  //    if (this.editItem.listTag[i].TagId != null) {
  //      this.editItem.listTag[i].Check = false;
  //    }
  //    else {
  //      this.editItem.listTag.splice(i, 1);
  //    }
  //  }
  //}

  //ChangeTitle(IsNew) {
  //  if (IsNew) {
  //    this.newItem.MetaTitle = this.newItem.Title;
  //    this.newItem.MetaKeyword = this.newItem.Title;
  //    this.newItem.MetaDescription = this.newItem.Description;
  //    //let str = this.newItem.Title;
  //    this.newItem.Url = this.common.ConvertUrl(this.newItem.Title);
  //  }
  //  else {
  //    this.editItem.MetaTitle = this.editItem.Title;
  //    this.editItem.MetaKeyword = this.editItem.Title;
  //    this.editItem.MetaDescription = this.editItem.Description;
  //    this.editItem.Url = this.common.ConvertUrl(this.editItem.Title);
  //  }
  //}

  //OpenEditModal(item) {
  //  this.editItem = new News();
  //  this.IsAll = true;
  //  this.Tag = '';
  //  this.editItem = Object.assign(this.editItem, item);
  //  this.editItem.Contents = item.Contents;
  //  this.fileE.nativeElement.value = "";
  //  this.editItem.DateStartActive = this.datePipe.transform(item.DateStartActive, "yyyy-MM-dd");
  //  this.editItem.DateStartOn = this.datePipe.transform(item.DateStartOn, "yyyy-MM-dd");
  //  this.editItem.DateEndOn = this.datePipe.transform(item.DateEndOn, "yyyy-MM-dd");
  //  this.CheckBoxStatus = this.editItem.Status == 1 ? true : false;

  //  this.message = this.editItem.Image;
  //  if (this.editItem.Image != undefined) { this.showRemove = true }
  //  this.GetListCateNews(false);
  //  this.editModal.show();
  //}

  //EditNewsFunc() {
  //  if (this.editItem.Title == undefined || this.editItem.Title == '') {
  //    this.toastWarning("Chưa nhập Tiêu đề!");
  //    return;
  //  } else if (this.editItem.Url == undefined || this.editItem.Url == '') {
  //    this.toastWarning("Chưa nhập Đường dẫn!");
  //    return;
  //  } else if (this.editItem.Contents == undefined || this.editItem.Contents == '') {
  //    this.toastWarning("Chưa nhập Nội dung!");
  //    return;
  //  } else if (this.editItem.TypeNewsId == undefined) {
  //    this.toastWarning("Chưa chọn Loại tin!");
  //    return;
  //  }

  //  this.editItem.UserId = userInfo.userId;
  //  this.editItem.Status = this.CheckBoxStatus ? 1 : 10;

  //  let arr = [];
  //  this.editItem.listCategory.forEach(item => {
  //    var flag = false;
  //    for (var i = 0; i < this.listCateNews.length; i++) {
  //      if (item.CategoryId == this.listCateNews[i].CategoryId && this.listCateNews[i].Check == true) {
  //        flag = true;
  //        break;
  //      }
  //    }

  //    if (!flag) {
  //      item.Check = false;
  //      arr.push(item);
  //    }
  //  });

  //  this.editItem.listCategory = arr.concat(this.listCateNews.filter(e => e.Check == true));

  //  this.http.put('/api/news/' + this.editItem.NewsId, this.editItem, httpOptions).subscribe(
  //    (res) => {
  //      if (res["meta"]["error_code"] == 200) {
  //        this.GetListNews();
  //        this.editModal.hide();
  //        this.toastSuccess("Cập nhật thành công!");
  //      }
  //      else {
  //        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
  //      }
  //    },
  //    (err) => {
  //      this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
  //    }
  //  );
  //}

  ////Popup xác nhận xóa
  //ShowConfirmDelete(Id) {
  //  this.modalDialogService.openDialog(this.viewRef, {
  //    title: 'Xác nhận',
  //    childComponent: SimpleModalComponent,
  //    data: {
  //      text: "Bạn có chắc chắn muốn xóa bản ghi này?"
  //    },
  //    actionButtons: [
  //      {
  //        text: 'Đồng ý',
  //        buttonClass: 'btn btn-success',
  //        onAction: () => {
  //          this.DeleteNews(Id);
  //        }
  //      },
  //      {
  //        text: 'Đóng',
  //        buttonClass: 'btn btn-default',

  //      }
  //    ],
  //  });
  //}

  //DeleteNews(Id) {
  //  this.http.delete('/api/news/' + Id, httpOptions).subscribe(
  //    (res) => {
  //      if (res["meta"]["error_code"] == 200) {
  //        this.GetListNews();
  //        this.viewRef.clear();
  //        this.toastSuccess("Xóa thành công!");
  //      }
  //      else {
  //        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
  //      }
  //    },
  //    (err) => {
  //      this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
  //    }
  //  );
  //}

  upload(files, IsNew) {
    if (files.length === 0)
      return;

    const formData = new FormData();

    for (let file of files)
      formData.append(file.name, file);

    const uploadReq = new HttpRequest('POST', 'api/material/importMaterialExcel', formData, {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      }),
      reportProgress: true,
    });

    this.http.request(uploadReq).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress)
        this.progress = Math.round(100 * event.loaded / event.total);
      else if (event.type === HttpEventType.Response) {
        //this.message = event.body["data"].toString();
        if (IsNew) {
          this.newItem.listMaterialImportExcelChild = event.body["data"];
          //this.showRemove = true;
        }
        //else {
        //  this.editItem.Image = this.message;
        //  this.showRemove = true;
        //}
      }
    });
  }

  findName(item) {
    if (item == undefined) {
      return "";
    }
    else {
      return item.Name;
    }
  }

  //RemoveImage(IsNew) {
  //  if (IsNew) {
  //    this.file.nativeElement.value = "";
  //    this.message = undefined;
  //    this.showRemove = false;
  //  }
  //  else {
  //    this.fileE.nativeElement.value = "";
  //    this.message = undefined;
  //    this.showRemove = false;
  //  }
  //}
  
  Sort(Is, Target, Index) {

  }
}

import { Component, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { typeCategoryPage, typeRank } from '../../../data/const';//có thể bỏ typeRank
import { CategoryRank } from '../../../data/model';
import { ToastrService } from 'ngx-toastr';



@Component({
  selector: 'app-rank',
  templateUrl: './rank.component.html',
  styleUrls: ['./rank.component.scss']
})
export class RankComponent implements OnInit {
  @ViewChild('AddModal') public addModal: ModalDirective;
  @ViewChild('EditModal') public editModal: ModalDirective;

  public paging: any;
  public q: any;
  public show: boolean = false;
  public showSort: boolean[] = [true, false, false, false];
  public listCateRank = [];
  public listTypeRank = [];
  public listWebsite = [];
  public listLanguage = [];

  public ckeConfig: any;

  public typeRank = typeRank;

  public newItem: CategoryRank;
  public editItem: CategoryRank;

  public httpOptions: any;


  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService
  ) {
    this.newItem = new CategoryRank();
    this.editItem = new CategoryRank();
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

    this.GetListCateRank();
  }
  //
  Sort(IsK, s, i) {
    if (IsK) {
      this.paging.order_by = s + " asc";
    }
    else {
      this.paging.order_by = s + " desc";
    }
    this.GetListCateRank();
    //this.showSort = !IsK;
    for (var j = 0; j < this.showSort.length; j++) {
      if (j == i) {
        this.showSort[j] = IsK;
        console.log("if = " + this.showSort[j]);
      }
      else {
        this.showSort[j] = false;
        console.log("else = " + this.showSort[j]);
      }
    }
  }
  //Get danh sách rank
  GetListCateRank() {
    this.http.get('/api/categoryrank/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listCateRank = res["data"];
          this.paging.item_count = res["metadata"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  //Get danh sách type rank
  GetListTypeRank() {
    this.http.get('/api/typeattributeitem/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listTypeRank = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  GetListLanguage() {
    this.http.get('/api/Language/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listLanguage = res["data"];
          if (this.listLanguage.length == 1) {
            this.newItem.LanguageId = parseInt(localStorage.getItem("languageId"));
            this.editItem.LanguageId = parseInt(localStorage.getItem("languageId"));
          }
          else {
            this.show = true;
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
    this.GetListCateRank();
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

    this.GetListCateRank();
  }

  //Mở modal thêm mới
  OpenAddModal() {
    this.newItem = new CategoryRank();
    this.newItem.Description = "";
    this.GetListTypeRank();
    this.GetListLanguage();
    this.addModal.show();
  }

  //Thêm mới danh mục trang
  AddFunc() {
    if (this.newItem.Name == undefined || this.newItem.Name == '') {
      this.toastWarning("Chưa nhập Tên danh mục!");
      return;
    } else if (this.newItem.Name.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên danh mục !");
      return;
    } else if (this.newItem.RankStart == undefined) {
      this.toastWarning("Chưa nhập khoảng bắt đầu!");
      return;
    } else if (this.newItem.RankEnd == undefined) {
      this.toastWarning("Chưa nhập khoảng kết thúc!");
      return;
    } else if (this.newItem.LanguageId == undefined) {
      this.toastWarning("Chưa chọn ngôn ngữ!");
      return;
    }

    if (this.newItem.RankStart > this.newItem.RankEnd) {
      this.toastWarning("Khoảng bắt đầu phải nhỏ hơn khoảng kết thúc!");
      return;
    }

    this.newItem.CompanyId = parseInt(localStorage.getItem("companyId"));
    this.newItem.UserId = parseInt(localStorage.getItem("userId"));
    this.newItem.WebsiteId = parseInt(localStorage.getItem("websiteId"));

    this.http.post('/api/CategoryRank', this.newItem, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListCateRank();
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

  //Mở modal cập nhật
  OpenEditModal(item) {
    this.editItem = new CategoryRank();
    this.editItem = Object.assign(this.editItem, item);
    if (this.editItem.TypeRankId == 0) this.editItem.TypeRankId = undefined;
    this.GetListTypeRank();
    this.GetListLanguage();
    this.editModal.show();
  }

  //Cập nhật danh mục trang
  EditFunc() {
    if (this.editItem.Name == undefined || this.editItem.Name == '') {
      this.toastWarning("Chưa nhập Tên danh mục!");
      return;
    } else if (this.editItem.Name.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên danh mục");
      return;
    } else if (this.editItem.RankStart == undefined || this.editItem.RankStart == 0) {
      this.toastWarning("Chưa nhập khoảng bắt đầu!");
      return;
    } else if (this.editItem.RankEnd == undefined || this.editItem.RankEnd == 0) {
      this.toastWarning("Chưa nhập khoảng kết thúc!");
      return;
    }
    else if (this.editItem.LanguageId == undefined) {
      this.toastWarning("Chưa chọn ngôn ngữ!");
      return;
    }
    if (this.newItem.RankStart > this.newItem.RankEnd) {
      this.toastWarning("Khoảng bắt đầu phải nhỏ hơn khoảng kết thúc!");
      return;
    }
    this.newItem.CompanyId = parseInt(localStorage.getItem("companyId"));
    this.newItem.UserId = parseInt(localStorage.getItem("userId"));
    this.newItem.WebsiteId = parseInt(localStorage.getItem("websiteId"));

    this.http.put('/api/CategoryRank/' + this.editItem.CategoryRankId, this.editItem, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListCateRank();
          this.editModal.hide();
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

  Delete(Id) {
    this.http.delete('/api/CategoryRank/' + Id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListCateRank();
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

  ShowHide(id, i) {
    let stt = this.listCateRank[i].IsShow ? 1 : 10;
    this.http.put('/api/CategoryRank/ShowHide/' + id + "/" + stt, undefined, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.toastSuccess("Thay đổi trạng thái thành công!");
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
  //The End//
}

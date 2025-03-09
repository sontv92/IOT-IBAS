import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpParams, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { Comment } from '../../../data/model';
import { ToastrService } from 'ngx-toastr';
import { debug } from 'util';
import { CommonService } from '../../../service/common.service';
import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import CKFinder from '@ckeditor/ckeditor5-ckfinder/src/ckfinder';



@Component({
  selector: 'app-comment',
  templateUrl: './comment.component.html',
  styleUrls: ['./comment.component.scss']
})
export class CommentComponent implements OnInit {
  @ViewChild('Modal') public modal: ModalDirective;

  public paging: any;
  public q: any;

  public listComment = [];
  public typeComment = [];
  public listProduct = [];
  public listNews = [];
  public item: Comment;
  public showSort: boolean[] = [true, false];
  public httpOptions: any;
  constructor(
  	public http: HttpClient,
  	public modalDialogService: ModalDialogService,
  	public viewRef: ViewContainerRef,
  	public toastr: ToastrService,
  	public common: CommonService
  	) {
  	this.item = new Comment();
  	this.paging = {
  		page: 1,
  		page_size: 10,
  		query: '1=1',
  		order_by: '',
  		item_count: 0
  	};

    this.q = {
      txtSearch: '',
      TargetType: undefined,
      TargetProductId: undefined,
      TargetNewsId: undefined
    }

    this.typeComment = [
    	{ TargetType: 1, Name: "Sản phẩm" },
    	{ TargetType: 2, Name: "Tin tức" }
    ];

    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      })
    }

   }

  ngOnInit() {
  	this.GetListComment();
  }

  Sort(IsK, s, i) {
    if (IsK) {
      this.paging.order_by = s + " asc";
    }
    else {
      this.paging.order_by = s + " desc";
    }
    this.GetListComment();
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

  GetListComment() {
    this.http.get('/api/comment/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listComment = res["data"];
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
    this.GetListComment();
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
  QueryChanged(flag) {
    let query = '';
    if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
      if (query != '') {
        query += ' and Contents.Contains("' + this.q.txtSearch + '")';
      }
      else {
        query += 'Contents.Contains("' + this.q.txtSearch + '")';
      }
    }

    if(this.q.TargetType != undefined && this.q.TargetType != '') {
    	if(this.q.TargetType == 1 && flag) {
    		this.GetListProduct();
    	} else if(this.q.TargetType == 2 && flag) {
    		this.GetListNews();
    	}

    	if (query != '') {
    		query += ' and TargetType = ' + this.q.TargetType;
    	}
    	else {
    		query += 'TargetType = ' + this.q.TargetType;
    	}
    }

    if(this.q.TargetProductId != undefined && this.q.TargetProductId != '' && this.q.TargetType == 1) {
    	if (query != '') {
    		query += ' and TargetId = ' + this.q.TargetProductId;
    	}
    	else {
    		query += 'TargetId = ' + this.q.TargetProductId;
    	}
    }

    if(this.q.TargetNewsId != undefined && this.q.TargetNewsId != '' && this.q.TargetType == 2) {
    	if (query != '') {
    		query += ' and TargetId = ' + this.q.TargetNewsId;
    	}
    	else {
    		query += 'TargetId = ' + this.q.TargetNewsId;
    	}
    }


    if (query == '')
      this.paging.query = '1=1';
    else
      this.paging.query = query;

    this.GetListComment();
  }

  Expand(i) {
  	this.listComment[i].Expand = this.listComment[i].Expand ? false : true;
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
            this.DeleteComment(Id);
          }
        },
        {
          text: 'Đóng',
          buttonClass: 'btn btn-default',

        }
      ],
    });
  }

  DeleteComment(Id) {
    this.http.delete('/api/comment/' + Id, this.httpOptions).subscribe(
        (res) => { 
          if(res["meta"]["error_code"] == 200) {
            this.GetListComment();
            this.viewRef.clear();
            this.toastSuccess("Xóa thành công!");
          } else if (res["meta"]["error_code"] == 222) {
            this.toastError("Bạn không có quyền thực hiện chức năng này!");
            }
          else
          {
            this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
          }
         },
         (err) => {
           this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
          }
      );
  }

  ShowHide(id, i, idx) {
  	let stt = idx == undefined ? (this.listComment[i].IsShow ? 1 : 10) : (this.listComment[i].listChildComment[idx].IsShow ? 1 : 10);
  	this.http.put('/api/comment/ShowHide/' + id + "/" + stt,undefined, this.httpOptions).subscribe(
        (res) => { 
          if(res["meta"]["error_code"] == 200) {
            this.toastSuccess("Thay đổi trạng thái bình luận thành công!");
          } else if (res["meta"]["error_code"] == 222) {
            this.toastError("Bạn không có quyền thực hiện chức năng này!");
            if(idx == undefined) this.listComment[i].IsShow = !this.listComment[i].IsShow
            	else this.listComment[i].listChildComment[idx].IsShow = !this.listComment[i].listChildComment[idx].IsShow;
            }
          else
          {
            this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            if(idx == undefined) this.listComment[i].IsShow = !this.listComment[i].IsShow
            	else this.listComment[i].listChildComment[idx].IsShow = !this.listComment[i].listChildComment[idx].IsShow;

          }
         },
         (err) => {
           this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
           if(idx == undefined) this.listComment[i].IsShow = !this.listComment[i].IsShow
            	else this.listComment[i].listChildComment[idx].IsShow = !this.listComment[i].listChildComment[idx].IsShow;
          }
      );
  }

  GetListProduct() {
  	this.http.get('/api/product/GetByPage?page=1&query=1=1&order_by=&select=ProductId,Name', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listProduct = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  GetListNews() {
  	this.http.get('/api/news/GetByPage?page=1&query=1=1&order_by=&select=NewsId,Title', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listNews = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

}

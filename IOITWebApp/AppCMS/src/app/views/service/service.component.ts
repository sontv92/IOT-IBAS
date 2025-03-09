import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { typeCategoryNews, domainImage } from '../../data/const';
import { News } from '../../data/model';
import { ToastrService } from 'ngx-toastr';
import { DatePipe } from '@angular/common';
import { CommonService } from '../../service/common.service';
import { Paging, QueryFilter } from '../../data/dt';
import { DateTimeAdapter, OWL_DATE_TIME_FORMATS, OWL_DATE_TIME_LOCALE } from 'ng-pick-datetime';
import { MomentDateTimeAdapter } from 'ng-pick-datetime-moment';


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
	selector: 'app-service',
	templateUrl: './service.component.html',
	styleUrls: ['./service.component.scss'],
	providers: [
		{ provide: DateTimeAdapter, useClass: MomentDateTimeAdapter, deps: [OWL_DATE_TIME_LOCALE] },
		{ provide: OWL_DATE_TIME_FORMATS, useValue: MY_CUSTOM_FORMATS }
	]
})
export class ServiceComponent implements OnInit {
	@ViewChild('NewsModal') public NewsModal: ModalDirective;
	@ViewChild('file') file: ElementRef;

	public paging: Paging;
	public q: QueryFilter;
	public listNews = [];

	public Item: News;

	public progress: number;
	public message: string;
	public domainImage = domainImage;

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
		this.paging.page = 1;
		this.paging.page_size = 10;
		this.paging.query = "IsService == true";
		this.paging.order_by = "NewsId Desc";
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
		this.GetListNews();
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
		let query = 'IsService == true';
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

		this.GetListNews();
	}

	//Mở modal thêm mới
	OpenNewsModal(item) {
		this.Item = new News();
		this.file.nativeElement.value = "";
		this.message = undefined;
		this.progress = undefined;
		if (item != undefined) {
			this.Item = JSON.parse(JSON.stringify(item));
		}
		this.NewsModal.show();
	}
	//Thêm mới danh mục trang
	SaveNews() {
		if (this.Item.Title == undefined || this.Item.Title == '') {
			this.toastWarning("Chưa nhập Tiêu đề!");
          return;
        } else if (this.Item.Title.replace(/ /g, '') == '') {
        this.toastWarning("Chưa nhập tiêu đề!");
        return;
    } else if (this.Item.Url == undefined || this.Item.Url == '') {
			this.toastWarning("Chưa nhập Đường dẫn!");
			return;
		}else if (this.Item.Url.replace(/ /g, '') == '') {
        this.toastWarning("Chưa nhập đường dẫn!");
        return;
    }
		else if (this.Item.Contents == undefined || this.Item.Contents == '') {
			this.toastWarning("Chưa nhập Nội dung!");
          return;
        } else if (this.Item.Contents.replace(/ /g, '') == '') {
        this.toastWarning("Chưa nhập nội dung!");
        return;
    }

		this.Item.Status = 1;
		this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
		this.Item.UserId = parseInt(localStorage.getItem("userId"));


		if (this.Item.NewsId == undefined) {
			this.http.post('/api/news', this.Item, this.httpOptions).subscribe(
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
			this.http.put('/api/news/' + this.Item.NewsId, this.Item, this.httpOptions).subscribe(
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

	upload(files) {
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
				this.progress = Math.round(100 * event.loaded / event.total);
			else if (event.type === HttpEventType.Response) {
				this.message = event.body["data"].toString();
				this.Item.Image = this.message;
			}
		});
	}

	RemoveImage() {
		this.Item.Image = undefined;
		this.file.nativeElement.value = "";
		this.message = undefined;
		this.progress = undefined;
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

}

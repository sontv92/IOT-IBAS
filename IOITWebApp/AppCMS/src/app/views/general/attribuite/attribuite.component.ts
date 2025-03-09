import { Component, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { ToastrService } from 'ngx-toastr';
import { Attribuite } from '../../../data/model';
import { Paging, QueryFilter } from '../../../data/dt';
import { TypeContact } from '../../../data/const';


@Component({
	selector: 'app-attribuite',
	templateUrl: './attribuite.component.html',
	styleUrls: ['./attribuite.component.scss']
})
export class AttribuiteComponent implements OnInit {
	@ViewChild('AttribuiteModal') public AttribuiteModal: ModalDirective;
	public paging: Paging;
	public q: QueryFilter;
	public listAttribuite = [];
	public httpOptions: any;
	public Item: Attribuite;

	constructor(
		public http: HttpClient,
		public modalDialogService: ModalDialogService,
		public viewRef: ViewContainerRef,
		public toastr: ToastrService,
	) {
		this.paging = new Paging();
		this.paging.page = 1;
		this.paging.page_size = 10;
		this.paging.query = "1=1";
		this.paging.order_by = "AttribuiteId Desc";
		this.paging.item_count = 0;

		this.q = new QueryFilter();
		this.q.txtSearch = "";

		this.Item = new Attribuite();

		this.httpOptions = {
			headers: new HttpHeaders({
				'Authorization': 'bearer ' + localStorage.getItem("access_token")
			})
		}
	}

	ngOnInit() {
		this.GetListAttribuite();
	}

	//GET
	GetListAttribuite() {
		this.http.get('/api/attribuite/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
			(res) => {
				if (res["meta"]["error_code"] == 200) {
					this.listAttribuite = res["data"];
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
		this.GetListAttribuite();
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
				query += ' and (Name.Contains("' + this.q.txtSearch + '") Or Name.Contains("' + this.q.txtSearch + '"))';
			}
			else {
				query += '(Name.Contains("' + this.q.txtSearch + '") or Name.Contains("' + this.q.txtSearch + '"))';
			}
		}

		if (query == '')
			this.paging.query = '1=1';
		else
			this.paging.query = query;

		this.GetListAttribuite();
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

		this.GetListAttribuite();
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

	//Mở modal thêm mới
	OpenAttribuiteModal(item) {
		this.Item = new Attribuite();
		if (item != undefined) {
			this.Item = JSON.parse(JSON.stringify(item));
		}
		this.AttribuiteModal.show();
	}

	//Thêm mới danh mục trang
	SaveAttribuite() {
		if (this.Item.Name == undefined || this.Item.Name == '') {
			this.toastWarning("Chưa nhập Tên thuộc tính!");
			return;
		} else if (this.Item.Name.replace(/ /g, '') == '') {
			this.toastWarning("Chưa nhập Tên thuộc tính!");
			return;
		} else if (this.Item.Location == undefined) {
			this.toastWarning("Chưa nhập Thứ tự hiển thị!");
			return;
		}

		if (this.Item.AttribuiteId == undefined) {
			this.http.post('/api/attribuite', this.Item, this.httpOptions).subscribe(
				(res) => {
					if (res["meta"]["error_code"] == 200) {
						this.GetListAttribuite();
						this.AttribuiteModal.hide();
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
			this.http.put('/api/attribuite/' + this.Item.AttribuiteId, this.Item, this.httpOptions).subscribe(
				(res) => {
					if (res["meta"]["error_code"] == 200) {
						this.GetListAttribuite();
						this.AttribuiteModal.hide();
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
						this.http.delete('/api/attribuite/' + Id, this.httpOptions).subscribe(
							(res) => {
								if (res["meta"]["error_code"] == 200) {
									this.GetListAttribuite();
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
				},
				{
					text: 'Đóng',
					buttonClass: 'btn btn-default',

				}
			],
		});
	}
}

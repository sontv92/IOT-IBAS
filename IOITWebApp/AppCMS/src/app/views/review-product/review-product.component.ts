import { Component, OnInit } from '@angular/core';
import { Paging, QueryFilter } from '../../data/dt';
import { ProductReviewStatus } from '../../data/const';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';




@Component({
	selector: 'app-review-product',
	templateUrl: './review-product.component.html',
	styleUrls: ['./review-product.component.scss']
})
export class ReviewProductComponent implements OnInit {
	public paging: Paging;
	public q: QueryFilter;

	public listProductReviews = [];
	public ProductReviewStatus = ProductReviewStatus;

	public httpOptions: any;
	public total: any;

	constructor(public http: HttpClient, public toastr: ToastrService) {
		this.paging = new Paging();
		this.paging.page = 1;
		this.paging.page_size = 10;
		this.paging.query = "1=1";
		this.paging.order_by = "";
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
		this.GetListProductReviews();
	}

	GetListProductReviews() {
		this.http.get('/api/product/ProductReview/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
			(res) => {
				if (res["meta"]["error_code"] == 200) {
					this.listProductReviews = res["data"];
					this.paging.item_count = res["metadata"].Sum;
					this.total = res["metadata"];
				}
			},
			(err) => {
				console.log("Error: connect to API");
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

		this.GetListProductReviews();
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

	ChangeStatusProductReview(ProductReviewId, Status) {
		this.http.put('/api/Product/ChangeStatusProductReview/' + ProductReviewId + "/" + Status, undefined, this.httpOptions).subscribe(
			(res) => {
				if (res["meta"]["error_code"] == 200) {
					this.toastSuccess("Thay đổi trạng thái thành công!");
					this.GetListProductReviews();
				} else if (res["meta"]["error_code"] == 222) {
					this.toastError("Bạn không có quyền thực hiện chức năng này!");
					this.GetListProductReviews();
					}
				else {
					this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
					this.GetListProductReviews();
				}
			},
			(err) => {
				this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
				this.GetListProductReviews();
			}
		);
	}

	//Chuyển trang
	PageChanged(event) {
		this.paging.page = event.page;
		this.GetListProductReviews();
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

	QueryChanged() {
		let query = '';

		if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
			if (query != '') {
				query += ' and (Name.Contains("' + this.q.txtSearch + '") OR Email.Contains("' + this.q.txtSearch + '") OR ProductName.Contains("' + this.q.txtSearch + '"))';
			}
			else {
				query += '(Name.Contains("' + this.q.txtSearch + '") OR Email.Contains("' + this.q.txtSearch + '") OR ProductName.Contains("' + this.q.txtSearch + '"))';
			}
		}

		if (this.q["Type"] != undefined) {
			if (query != '') {
				query += ' and Status=' + this.q["Type"];
			}
			else {
				query += 'Status=' + this.q["Type"];
			}
		}

		if (query == '')
			this.paging.query = '1=1';
		else
			this.paging.query = query;

		this.GetListProductReviews();
	}
}

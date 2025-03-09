import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { domainImage, ProductReviewStatus } from '../../data/const';
import { Product, Attribute, ImageProduct } from '../../data/Model';
import { ToastrService } from 'ngx-toastr';
import { CommonService } from '../../service/common.service';
import { DatePipe } from '@angular/common';
import { Paging, QueryFilter } from '../../data/dt';
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
	selector: 'app-product',
	templateUrl: './product.component.html',
	styleUrls: ['./product.component.scss'],
	providers: [
		{ provide: DateTimeAdapter, useClass: MomentDateTimeAdapter, deps: [OWL_DATE_TIME_LOCALE] },
		{ provide: OWL_DATE_TIME_FORMATS, useValue: MY_CUSTOM_FORMATS }
	]
})
export class ProductComponent implements OnInit {
	@ViewChild('ProductModal') public ProductModal: ModalDirective;
	@ViewChild('OrderByModal') public OrderByModal: ModalDirective;
	@ViewChild('ProductReviewModal') public ProductReviewModal: ModalDirective;
	@ViewChild('AttribuiteModal') public AttribuiteModal: ModalDirective;


	@ViewChild('file') file: ElementRef;
	@ViewChild('tabset') tabset: TabsetComponent;

	public paging: Paging;
	public q: QueryFilter;

	public pagingReview: Paging;
	public qReview: QueryFilter;

	// public Tag: string;
	public listProduct = [];
	public listProductReview = [];
	public listTrademark = [];
	public ckeConfig: any;

	public Item: Product;
	public ImageProduct: ImageProduct;
	public listManufacture = [];
	public listCateNews = [];
	public listOrderByProduct = [];

	public domainImage = domainImage;
	public progress: number;

	public httpOptions: any;

	public listSuggestProduct = [];
	public ProductName = "";
	public ProductId: number;
	public ProductReviewStatus = ProductReviewStatus;

	public attribuites = [];
	public ItemAttribuite: Attribute;

	PriceCurrencyMaskConfig = {
		align: "left",
		allowNegative: false,
		decimal: ".",
		precision: 0,
		prefix: "",
		suffix: " Vnđ",
		thousands: ","
	};


	StockQuantityMaskConfig = {
		align: "left",
		allowNegative: false,
		decimal: ".",
		precision: 0,
		prefix: "",
		suffix: "",
		thousands: ","
	};

	constructor(
		public http: HttpClient,
		public modalDialogService: ModalDialogService,
		public viewRef: ViewContainerRef,
		public toastr: ToastrService,
		public common: CommonService,
		public datePipe: DatePipe
	) {
		this.Item = new Product();
		this.paging = new Paging();
		this.paging.page = 1;
		this.paging.page_size = 10;
		this.paging.query = "TypeProduct=1";
		this.paging.order_by = "ProductId Desc";
		this.paging.item_count = 0;

		this.q = new QueryFilter();
		this.q.txtSearch = "";

		this.pagingReview = new Paging();
		this.pagingReview.page = 1;
		this.pagingReview.page_size = 10;
		this.pagingReview.query = "1=1";
		this.pagingReview.order_by = "";
		this.pagingReview.item_count = 0;

		this.qReview = new QueryFilter();
		this.qReview.txtSearch = "";

		this.httpOptions = {
			headers: new HttpHeaders({
				'Authorization': 'bearer ' + localStorage.getItem("access_token")
			})
		}

		this.ItemAttribuite = new Attribute();
	}

	ngOnInit() {
		this.ckeConfig = {
			allowedContent: false,
			extraPlugins: 'divarea',
			forcePasteAsPlainText: true
		};

		this.GetListProduct();
		this.GetListManufacture();
		this.GetListTrademark();
      this.GetAttribuites();


	}

	//Get danh sách sản phẩm
	GetListProduct() {
		this.http.get('/api/product/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
			(res) => {
				if (res["meta"]["error_code"] == 200) {
					this.listProduct = res["data"];
					this.listProduct.forEach(item => {
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

	//Lấy toàn bộ danh sách sản phẩm
	GetListAllProduct() {
		let query = "";
		if (this.Item.ProductId != undefined) {
			query = "TypeProduct=1 and ProductId !=" + this.Item.ProductId
		}
		else {
			query = "TypeProduct=1"
		}
      this.http.get('/api/product/GetByPage?page=1&query=' + query + '&order_by=&select=ProductId,PriceSpecial,Name,Image', this.httpOptions).subscribe(
			(res) => {
				if (res["meta"]["error_code"] == 200) {
					this.listSuggestProduct = res["data"];
					this.listSuggestProduct.forEach(item => {
						item.Check = false;
					});

					if (this.Item.ProductId != undefined) {
						for (var i = 0; i < this.listSuggestProduct.length; i++) {
							for (var j = 0; j < this.Item.listRelated.length; j++) {
								if (this.listSuggestProduct[i].ProductId == this.Item.listRelated[j].TargetRelatedId) {
									this.listSuggestProduct[i].Check = true;
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

	//Danh sách nhà sản xuất
	GetListManufacture() {
		this.http.get('/api/manufacturer/GetByPage?page=1&query=TypeOriginId=1&order_by=', this.httpOptions).subscribe(
			(res) => {
				if (res["meta"]["error_code"] == 200) {
					this.listManufacture = res["data"];
				}
			},
			(err) => {
				console.log("Error: connect to API");
			}
		);
	}

	//Danh sách thương hiệu
	GetListTrademark() {
		this.http.get('/api/manufacturer/GetByPage?page=1&query=TypeOriginId=2&order_by=', this.httpOptions).subscribe(
			(res) => {
				if (res["meta"]["error_code"] == 200) {
					this.listTrademark = res["data"];
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
		this.GetListProduct();
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
		let query = 'TypeProduct=1';
		if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
			if (query != '') {
				query += ' and Name.Contains("' + this.q.txtSearch + '")';
			}
			else {
				query += 'Name.Contains("' + this.q.txtSearch + '")';
			}
		}

		if (this.q["ManufacturerId"] != undefined) {
			if (query != '') {
				query += ' and ManufacturerId=' + this.q["ManufacturerId"];
			}
			else {
				query += 'ManufacturerId=' + this.q["ManufacturerId"];
			}
		}

		if (this.q["TrademarkId"] != undefined) {
			if (query != '') {
				query += ' and TrademarkId=' + this.q["TrademarkId"];
			}
			else {
				query += 'TrademarkId=' + this.q["TrademarkId"];
			}
		}

		if (query == '')
			this.paging.query = '1=1';
		else
			this.paging.query = query;

		this.GetListProduct();
	}

	//Mở modal thêm mới
	OpenProductModal(item) {
		this.tabset.tabs[0].active = true;
		this.Item = new Product();
		this.file.nativeElement.value = "";
		this.progress = undefined;
		this.listCateNews = [];
		this.Item.listImage = [];
		if (item != undefined) {
			this.Item = JSON.parse(JSON.stringify(item));

			let listAttribute = JSON.parse(JSON.stringify(item.listAttribute));
			let attribuites = JSON.parse(JSON.stringify(this.attribuites));
			attribuites.forEach(itemLoop => {
				for (let i = 0; i < listAttribute.length; i++) {
					if (itemLoop.AttribuiteId == listAttribute[i].AttribuiteId) {
						itemLoop.Value = listAttribute[i].Value;
						break;
					}
				}
			});
			this.Item.listAttribute = attribuites;
		} else {
			this.Item.listAttribute = JSON.parse(JSON.stringify(this.attribuites));
		}

		this.GetListAllProduct();
		this.GetListCateNews();
		this.ProductModal.show();
	}

	//Thêm mới danh mục trang
	SaveProduct() {
		if (this.Item.Name == undefined || this.Item.Name == '') {
			this.toastWarning("Chưa nhập Tên sản phẩm!");
			return;
		} else if (this.Item.Name.replace(/ /g, '') == '') {
			this.toastWarning("Chưa nhập tên sản phẩm!");
			return;
		} else if (this.Item.Url == undefined || this.Item.Url == '') {
			this.toastWarning("Chưa nhập Đường dẫn!");
			return;
		} else if (this.Item.Url.replace(/ /g, '') == '') {
			this.toastWarning("Chưa nhập đường dẫn!");
			return;
		} else if (this.Item.StockQuantity == undefined) {
			this.toastWarning("Chưa nhập Số lượng sản phẩm!");
			return;
		}

		this.Item.UserId = parseInt(localStorage.getItem("userId"));
		this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
		this.Item.WebsiteId = parseInt(localStorage.getItem("websiteId"));

		if (typeof this.Item.DateStartActive === 'object' && this.Item.DateStartActive != undefined) {
			let DateStartActive = this.Item.DateStartActive.add(7, 'hours');
			this.Item.DateStartActive = DateStartActive.toISOString();
		}

		this.Item.listRelated = [];
		this.listSuggestProduct.forEach(item => {
			if (item.Check == true) {
				let obj = { TargetRelatedId: item.ProductId }
				this.Item.listRelated.push(obj);
			}
		});

		if (this.Item.ProductId == undefined) {
			this.Item.listCategory = [];
			this.listCateNews.forEach(item => {
				if (item.Check) {
					this.Item.listCategory.push(item);
				}
			});
			this.Item.TypeProduct = 1;

			this.http.post('/api/Product', this.Item, this.httpOptions).subscribe(
				(res) => {
					if (res["meta"]["error_code"] == 200) {
						this.GetListProduct();
						this.ProductModal.hide();
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
			this.Item.listCategory.forEach(item => {
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

			this.Item.listCategory = arr.concat(this.listCateNews.filter(e => e.Check == true));

			this.http.put('/api/product/' + this.Item.ProductId, this.Item, this.httpOptions).subscribe(
				(res) => {
					if (res["meta"]["error_code"] == 200) {
						this.GetListProduct();
						this.ProductModal.hide();
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
		this.http.delete('/api/Product/' + Id, this.httpOptions).subscribe(
			(res) => {
				if (res["meta"]["error_code"] == 200) {
					this.GetListProduct();
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

	// check chữ
	ChangeNameProduct(key) {
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

	GetListCateNews() {
		this.http.get('/api/category/GetByTree?arr=11', this.httpOptions).subscribe(
			(res) => {
				if (res["meta"]["error_code"] == 200) {
					this.listCateNews = res["data"];

					if (this.Item.ProductId != undefined) {
						for (var i = 0; i < this.listCateNews.length; i++) {
							for (var j = 0; j < this.Item.listCategory.length; j++) {
								if (this.listCateNews[i].CategoryId == this.Item.listCategory[j].CategoryId) {
									this.listCateNews[i].Check = true;
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




		// this.http.get('/api/category/GetByPage?page=1&query=TypeCategoryId=11', this.httpOptions).subscribe(
		// 	(res) => {
		// 		if (res["meta"]["error_code"] == 200) {
		// 			this.listCateNews = [];
		// 			if (res["data"].length > 0) {
		// 				res["data"].forEach(cate => {
		// 					this.listCateNews.push({ CategoryId: cate.CategoryId, Name: cate.Name, Check: false });
		// 				});

		// 				if (this.Item.ProductId != undefined) {
		// 					for (var i = 0; i < this.listCateNews.length; i++) {
		// 						for (var j = 0; j < this.Item.listCategory.length; j++) {
		// 							if (this.listCateNews[i].CategoryId == this.Item.listCategory[j].CategoryId) {
		// 								this.listCateNews[i].Check = true;
		// 								break;
		// 							}
		// 						}
		// 					}
		// 				}
		// 			}
		// 		}
		// 	},
		// 	(err) => {
		// 		console.log("Error: connect to API");
		// 	}
		// );
	}

	upload(files) {
		if (files.length === 0)
			return;

		const formData = new FormData();

		for (let file of files)
			formData.append(file.name, file);

		const uploadReq = new HttpRequest('POST', 'api/upload/uploadImage/2', formData, {
			headers: new HttpHeaders({
				'Authorization': 'bearer ' + localStorage.getItem("access_token")
			}),
			reportProgress: true,
		});

		this.http.request(uploadReq).subscribe(event => {
			if (event.type === HttpEventType.UploadProgress)
				this.progress = Math.round(100 * event.loaded / event.total);
			else if (event.type === HttpEventType.Response) {
				event.body["data"].forEach(item => {
					this.ImageProduct = new ImageProduct();
					this.ImageProduct.Image = item;
					this.ImageProduct.IsImageMain = false;
					this.ImageProduct.Status = 1;
					this.Item.listImage.push(this.ImageProduct);
				});
			}
		});
	}

	RemoveImage(idx) {
		if (this.Item.listImage[idx].ProductImageId == undefined) {
			this.Item.listImage.splice(idx, 1);
		}
		else {
			this.Item.listImage[idx].Status = 99;
		}
	}

	findTrademark(item) {
		if (item == undefined) {
			return "";
		}
		else {
			return item.Name;
		}
	}

	ShowHide(id, i) {
		let stt = this.listProduct[i].IsShow ? 1 : 10;
		this.http.put('/api/Product/ShowHide/' + id + "/" + stt, undefined, this.httpOptions).subscribe(
			(res) => {
				if (res["meta"]["error_code"] == 200) {
					this.toastSuccess("Thay đổi trạng thái thành công!");
				} else if (res["meta"]["error_code"] == 222) {
					this.toastError("Bạn không có quyền thực hiện chức năng này!");
					this.listProduct[i].IsShow = !this.listProduct[i].IsShow;
				  }
				else {
					this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
					this.listProduct[i].IsShow = !this.listProduct[i].IsShow;
				}
			},
			(err) => {
				this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
				this.listProduct[i].IsShow = !this.listProduct[i].IsShow;
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

		this.GetListProduct();
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

	SetIsMain(idx) {
		for (let i = 0; i < this.Item.listImage.length; i++) {
			this.Item.listImage[i].IsImageMain = false;
			if (idx == i) {
				this.Item.listImage[i].IsImageMain = true;
			}
		}
	}


	GetListOrderBy() {
		this.http.get('/api/orderby/GetOrderBy/10', this.httpOptions).subscribe(
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

	OpenOrderByModal() {
		this.listOrderByProduct = [];
		this.GetListOrderBy();
		this.OrderByModal.show();
	}

	DeleteOrderBy(item) {
		for (let i = 0; i < this.listOrderByProduct.length; i++) {
			if (this.listOrderByProduct[i].CategoryMappingId == item.CategoryMappingId) {
				this.listOrderByProduct[i].Status = 99;
				break;
			}
		}
	}

	SaveOrderBy() {
		this.http.post('/api/orderby', this.listOrderByProduct, this.httpOptions).subscribe(
			(res) => {
				if (res["meta"]["error_code"] == 200) {
					this.GetListProduct();
					this.OrderByModal.hide();
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

	//Product Review
	GetListProductReviews() {
		this.http.get('/api/product/ProductReview/GetByPage?page=' + this.pagingReview.page + '&page_size=' + this.pagingReview.page_size + '&query=' + this.pagingReview.query + '&order_by=' + this.pagingReview.order_by, this.httpOptions).subscribe(
			(res) => {
				if (res["meta"]["error_code"] == 200) {
					this.listProductReview = res["data"];
					this.pagingReview.item_count = res["metadata"];
				}
			},
			(err) => {
				console.log("Error: connect to API");
			}
		);
	}

	ProductReviewsModal(ProductId, Name) {
		this.ProductName = Name;
		this.ProductId = ProductId;

		this.pagingReview = new Paging();
		this.pagingReview.page = 1;
		this.pagingReview.page_size = 10;
		this.pagingReview.query = "ProductId=" + ProductId;
		this.pagingReview.order_by = "";
		this.pagingReview.item_count = 0;

		this.qReview = new QueryFilter();
		this.qReview.txtSearch = "";
		this.qReview.Type = undefined;

		this.GetListProductReviews();
		this.ProductReviewModal.show();
	}

	PageChangedReview(event) {
		this.pagingReview.page = event.page;
		this.GetListProductReviews();
	}

	QueryReviewChanged() {
		let query = 'ProductId=' + this.ProductId;
		if (this.qReview["Type"] != undefined) {
			if (query != '') {
				query += ' and Status=' + this.qReview["Type"];
			}
			else {
				query += 'Status=' + this.qReview["Type"];
			}
		}

		if (query == '')
			this.pagingReview.query = '1=1';
		else
			this.pagingReview.query = query;

		this.GetListProductReviews();
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

	CheckCategory(CategoryId, curItem) {
		let Check = curItem["Check"];
		let CategoryParentId = curItem["CategoryParentId"];

		let CheckParent = false;

		this.listCateNews.forEach(item => {
			if (Check) {
				if (item.Genealogy.indexOf(CategoryId.toString()) != -1) {
					item.Check = !Check;
				}
			}


			if (Check == false) {
				CheckParent = true;
			}
			else {
				if (item.CategoryParentId == CategoryParentId) {
					if (item.Check == true) {
						CheckParent = true;
					}
				}
			}

		});

		if (CheckParent) {
			this.listCateNews.forEach(item => {
				if (item.CategoryId == CategoryParentId) {
					item.Check = true;
				}
			});
		}
	}

	//Lấy ra danh sách thuộc tính
	GetAttribuites() {
		this.http.get('/api/attribuite/GetByPage?page=1&query=1=1&order_by=Location Asc&select=AttribuiteId,Name,Location', this.httpOptions).subscribe(
			(res) => {
				if (res["meta"]["error_code"] == 200) {
					this.attribuites = res["data"];
				}
			},
			(err) => {
				console.log("Error: connect to API");
			}
		);
	}

	OpenAttribuiteModal() {
		this.ItemAttribuite = new Attribute();
		this.ItemAttribuite.Status = 1;
		this.AttribuiteModal.show();
	}

	SaveAttribuite() {
		if (this.ItemAttribuite.AttribuiteId == undefined) {
			this.toastWarning("Chưa chọn Thuộc tính!");
			return;
		} else if (this.ItemAttribuite.Value == undefined || this.ItemAttribuite.Value == '') {
			this.toastWarning("Chưa nhập Giá trị thuộc tính!");
			return;
		} else if (this.ItemAttribuite.Value.replace(/ /g, '') == '') {
			this.toastWarning("Chưa nhập Giá trị thuộc tính!");
			return;
		} else if (this.ItemAttribuite.Location == undefined) {
			this.toastWarning("Chưa nhập Thứ tự hiển thị!");
			return;
		}

		if (this.Item.listAttribute == undefined) {
			this.Item.listAttribute = [];
		}

		this.Item.listAttribute.push(this.ItemAttribuite);
		this.ItemAttribuite = new Attribute();
		this.AttribuiteModal.hide();
	}

	ShowConfirmDeleteAttribuite(i) {
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
						this.Item.listAttribute[i].Status = 99;
						this.viewRef.clear();
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

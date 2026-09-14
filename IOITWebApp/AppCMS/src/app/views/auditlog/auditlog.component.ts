import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { DatePipe } from '@angular/common';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';
import { CommonService } from '../../service/common.service';

/** Một dòng so sánh giá trị cũ / mới trong modal chi tiết */
export interface AuditDiffRow {
  field: string;
  oldValue: string;
  newValue: string;
  changed: boolean;
}

@Component({
  selector: 'app-auditlog',
  templateUrl: './auditlog.component.html'
})
export class AuditLogComponent implements OnInit {
  @ViewChild('DetailModal') public detailModal: ModalDirective;

  public paging: any;
  public q: any;
  public listLog = [];
  public listUser = [];
  public listAction = [
    { value: 'CREATE', name: 'Thêm mới' },
    { value: 'UPDATE', name: 'Sửa' },
    { value: 'DELETE', name: 'Xóa' }
  ];
  public listEntity = [
    { value: 'MacBeTong', name: 'Mác bê tông' },
    { value: 'HopDong', name: 'Hợp đồng' },
    { value: 'KhachHang', name: 'Khách hàng' },
    { value: 'DuAn', name: 'Dự án' },
    { value: 'NhanVien', name: 'Nhân viên' },
    { value: 'Xe', name: 'Xe' }
  ];
  public listSuccess = [
    { value: -1, name: 'Tất cả' },
    { value: 1, name: 'Thành công' },
    { value: 0, name: 'Thất bại' }
  ];

  public Item: any = {};
  public diffRows: AuditDiffRow[] = [];
  public oldJson: string = '';
  public newJson: string = '';
  public onlyChanged: boolean = true;

  public httpOptions: any;
  access_key: string;
  functionRole: string;
  actionView: boolean;
  actionExport: boolean;

  constructor(
    public http: HttpClient,
    public toastr: ToastrService,
    public commonService: CommonService,
    public datepipe: DatePipe,
    private SpinnerService: Ng4LoadingSpinnerService
  ) {
    this.paging = {
      page: 1,
      page_size: 20,
      order_by: 'OccurredAt Desc',
      item_count: 0
    };

    this.q = {
      tungay: '',
      denngay: '',
      UserName: '',
      Action: '',
      EntityType: '',
      EntityId: '',
      Success: -1,
      search: ''
    };

    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      })
    }
  }

  ngOnInit() {
    // Mặc định xem 7 ngày gần nhất
    let today = new Date();
    let weekAgo = new Date();
    weekAgo.setDate(today.getDate() - 7);
    this.q.tungay = this.datepipe.transform(weekAgo, 'yyyy-MM-dd');
    this.q.denngay = this.datepipe.transform(today, 'yyyy-MM-dd');

    this.access_key = localStorage.getItem('access_key');
    this.checkRoleByCode();

    this.GetListUser();
    this.GetListLog();
  }

  checkRoleByCode() {
    this.functionRole = '000000000';
    if (this.access_key) {
      var functionRole = this.access_key.split('-');
      functionRole.forEach(item => {
        var code = item.split(':')[0];
        if (code == 'NKND') {
          this.functionRole = item.split(':')[1];
        }
      });
    }
    this.actionView = this.functionRole.charAt(0) == "1";
    this.actionExport = this.functionRole.charAt(5) == "1";
  }

  /** Chuỗi query string từ bộ lọc hiện tại */
  BuildQuery(): string {
    let s = '&tungay=' + (this.q.tungay || '')
      + '&denngay=' + (this.q.denngay || '')
      + '&UserName=' + encodeURIComponent(this.q.UserName || '')
      + '&Action=' + (this.q.Action || '')
      + '&EntityType=' + (this.q.EntityType || '')
      + '&EntityId=' + encodeURIComponent(this.q.EntityId || '')
      + '&Success=' + (this.q.Success == null ? -1 : this.q.Success)
      + '&search=' + encodeURIComponent(this.q.search || '');
    return s;
  }

  GetListUser() {
    this.http.get('/api/auditlog/GetUsers', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listUser = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  GetListLog() {
    if (this.q.tungay && this.q.denngay && this.q.tungay > this.q.denngay) {
      this.toastWarning('Từ ngày phải nhỏ hơn hoặc bằng Đến ngày!');
      return;
    }
    this.SpinnerService.show();
    this.http.get('/api/auditlog/GetByPage?page=' + this.paging.page
      + '&page_size=' + this.paging.page_size
      + '&order_by=' + this.paging.order_by
      + this.BuildQuery(), this.httpOptions).subscribe(
      (res) => {
        this.SpinnerService.hide();
        if (res["meta"]["error_code"] == 200) {
          this.listLog = res["data"];
          this.paging.item_count = res["metadata"];
        }
        else if (res["meta"]["error_code"] == 222) {
          this.toastWarning('Bạn không có quyền xem nhật ký người dùng!');
        }
        else {
          this.toastError(res["meta"]["error_message"]);
        }
      },
      (err) => {
        this.SpinnerService.hide();
        console.log("Error: connect to API");
      }
    );
  }

  QueryChanged() {
    this.paging.page = 1;
    this.GetListLog();
  }

  ResetFilter() {
    let today = new Date();
    let weekAgo = new Date();
    weekAgo.setDate(today.getDate() - 7);
    this.q.tungay = this.datepipe.transform(weekAgo, 'yyyy-MM-dd');
    this.q.denngay = this.datepipe.transform(today, 'yyyy-MM-dd');
    this.q.UserName = '';
    this.q.Action = '';
    this.q.EntityType = '';
    this.q.EntityId = '';
    this.q.Success = -1;
    this.q.search = '';
    this.QueryChanged();
  }

  PageChanged(event) {
    this.paging.page = event.page;
    this.GetListLog();
  }

  SortTable(str) {
    let First = str;
    let Sort = '';
    let Last = this.paging.order_by.split(' ')[1];
    if (First == this.paging.order_by.split(' ')[0]) {
      Sort = Last == 'Desc' ? 'Asc' : 'Desc';
    } else {
      Sort = 'Desc';
    }
    this.paging.order_by = First + ' ' + Sort;
    this.GetListLog();
  }

  ExportExcel() {
    this.SpinnerService.show();
    fetch('/api/auditlog/Export?page=1&page_size=0' + this.BuildQuery(), {
      method: 'GET',
      headers: new Headers({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      }),
    })
      .then(response => response.blob())
      .then(blob => {
        var DateObj = new Date();
        var date = ('0' + DateObj.getDate()).slice(-2) + '_' + ('0' + (DateObj.getMonth() + 1)).slice(-2) + '_' + DateObj.getFullYear();
        this.DownloadFile(blob, "nhat_ky_nguoi_dung_" + date + ".xlsx", 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
        this.SpinnerService.hide();
      })
      .catch(() => {
        this.SpinnerService.hide();
        this.toastError('Xuất Excel không thành công!');
      });
  }

  DownloadFile(data: Blob, filename: string, mime: string): void {
    var blob = new Blob([data], { type: mime || 'application/octet-stream' });
    if (typeof window.navigator.msSaveBlob !== 'undefined') {
      window.navigator.msSaveBlob(blob, filename);
    }
    else {
      var blobURL = window.URL.createObjectURL(blob);
      var tempLink = document.createElement('a');
      tempLink.href = blobURL;
      tempLink.setAttribute('download', filename);
      tempLink.setAttribute('target', '_blank');
      document.body.appendChild(tempLink);
      tempLink.click();
      document.body.removeChild(tempLink);
      setTimeout(function () {
        window.URL.revokeObjectURL(blobURL);
      }, 100);
    }
  }

  // ===== Chi tiết 1 bản ghi =====
  OpenDetail(item) {
    this.SpinnerService.show();
    this.http.get('/api/auditlog/GetById/' + item.Id, this.httpOptions).subscribe(
      (res) => {
        this.SpinnerService.hide();
        if (res["meta"]["error_code"] == 200) {
          this.Item = res["data"];
          this.BuildDiff();
          this.detailModal.show();
        }
        else {
          this.toastError(res["meta"]["error_message"]);
        }
      },
      (err) => {
        this.SpinnerService.hide();
        console.log("Error: connect to API");
      }
    );
  }

  /** Parse JSON an toàn, trả về null nếu không phải JSON */
  private ParseJson(str: string): any {
    if (!str) return null;
    try {
      return JSON.parse(str);
    } catch (e) {
      return null;
    }
  }

  /** Làm phẳng object lồng nhau thành map "a.b[0].c" -> chuỗi giá trị */
  private Flatten(obj: any, prefix: string, out: { [key: string]: string }) {
    if (obj === null || obj === undefined) {
      out[prefix] = '';
      return;
    }
    if (Array.isArray(obj)) {
      if (obj.length == 0) { out[prefix] = '[]'; return; }
      obj.forEach((v, i) => this.Flatten(v, prefix + '[' + i + ']', out));
      return;
    }
    if (typeof obj === 'object') {
      Object.keys(obj).forEach(k => this.Flatten(obj[k], prefix ? prefix + '.' + k : k, out));
      return;
    }
    out[prefix] = String(obj);
  }

  BuildDiff() {
    let oldObj = this.ParseJson(this.Item.OldValues);
    let newObj = this.ParseJson(this.Item.NewValues);

    this.oldJson = oldObj ? JSON.stringify(oldObj, null, 2) : (this.Item.OldValues || '');
    this.newJson = newObj ? JSON.stringify(newObj, null, 2) : (this.Item.NewValues || '');

    let oldFlat: { [key: string]: string } = {};
    let newFlat: { [key: string]: string } = {};
    if (oldObj) this.Flatten(oldObj, '', oldFlat);
    if (newObj) this.Flatten(newObj, '', newFlat);

    let keys: string[] = [];
    Object.keys(oldFlat).forEach(k => { if (keys.indexOf(k) == -1) keys.push(k); });
    Object.keys(newFlat).forEach(k => { if (keys.indexOf(k) == -1) keys.push(k); });

    this.diffRows = keys.map(k => {
      let o = oldFlat.hasOwnProperty(k) ? oldFlat[k] : '';
      let n = newFlat.hasOwnProperty(k) ? newFlat[k] : '';
      return { field: k, oldValue: o, newValue: n, changed: o !== n };
    });
  }

  get visibleDiffRows(): AuditDiffRow[] {
    // Với thao tác thêm/xóa thì mọi dòng đều "thay đổi", nên chỉ lọc khi là UPDATE
    if (this.onlyChanged && this.Item && this.Item.Action == 'UPDATE') {
      return this.diffRows.filter(r => r.changed);
    }
    return this.diffRows;
  }

  get changedCount(): number {
    return this.diffRows.filter(r => r.changed).length;
  }

  ActionName(code: string): string {
    let f = this.listAction.find(x => x.value == code);
    return f ? f.name : code;
  }

  EntityName(code: string): string {
    let f = this.listEntity.find(x => x.value == code);
    return f ? f.name : code;
  }

  ActionClass(code: string): string {
    switch (code) {
      case 'CREATE': return 'badge badge-success';
      case 'UPDATE': return 'badge badge-warning';
      case 'DELETE': return 'badge badge-danger';
      default: return 'badge badge-secondary';
    }
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
}

import { Component, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Config } from '../../../data/model';
import { ToastrService } from 'ngx-toastr';
import { TypeUpload } from '../../../data/const';



@Component({
  selector: 'app-config-general',
  templateUrl: './config-general.component.html',
  styleUrls: ['./config-general.component.scss']
})
export class ConfigGeneralComponent implements OnInit {

  public listConfig = [];
  public CompanyId = parseInt(localStorage.getItem("companyId"));

  public ckeConfig: any;

  public typeUpload = TypeUpload;

  public updateItem: Config;

  public httpOptions: any;

  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
  ) {
    this.updateItem = new Config();
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
    this.GetListConfig(this.CompanyId);
  }

  GetListConfig(CompanyId) {
    this.http.get('/api/Config/' + CompanyId, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listConfig = res["data"];
          this.updateItem = Object.assign(this.updateItem, this.listConfig);
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    )
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


  Update() {
    if (this.updateItem.EmailHost == undefined) {
      this.toastWarning("Chưa nhập Email Host!");
      return;
    } else if (this.updateItem.EmailHost.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập Email host!");
      return;
    } else if (this.updateItem.EmailSender == undefined) {
      this.toastWarning("Chưa nhập Email Sender!");
      return;
    } else if (this.updateItem.EmailSender.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập Email Sender!");
      return;
    } else if (this.updateItem.EmailEnableSsl == undefined) {
      this.toastWarning("Chưa nhập Email Enable SSl");
      return;
    }else if (this.updateItem.EmailUserName == undefined) {
      this.toastWarning("Chưa nhập Email User Name!");
      return;
    } else if (this.updateItem.EmailUserName.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập Email UserName!");
      return;
    } else if (this.updateItem.EmailPasswordHash == undefined) {
      this.toastWarning("Chưa nhập Email Password Hash");
      return;
    } else if (this.updateItem.EmailPasswordHash.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập EmailPasswordHash!");
      return;
    } else if (this.updateItem.EmailPort == undefined) {
      this.toastWarning("Chưa nhập Email Port!");
      return;
    }

    this.http.put('/api/Config/' + this.updateItem.ConfigId, this.updateItem, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {

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

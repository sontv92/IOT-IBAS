import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../service/auth.service';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Md5 } from 'ts-md5/dist/md5';
import { CookieService } from 'ngx-cookie-service';
import { ToastrService } from 'ngx-toastr';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { AuthGuard } from '../../auth.guard';
import { Title } from "@angular/platform-browser";

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json',
    'Access-Control-Allow-Credentials': 'true',
'Access-Control-Allow-Origin': '*',
'Access-Control-Allow-Methods': 'OPTIONS, GET, POST',
'Access-Control-Allow-Headers': 'Origin, Content-Type, Accept, Access-Control-Allow-Origin'
  }),
  withCredentials: true
}

const md5 = new Md5();

const formData = new FormData();

@Component({
  selector: 'app-dashboard',
  templateUrl: 'login.component.html',
  styleUrls: ['login.component.css']
})
export class LoginComponent implements OnInit {

  loginForm: FormGroup;
  message: string;
  returnUrl: string;
  submitted: Boolean;

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    public authService: AuthService,
    public http: HttpClient,
    private cookieService: CookieService,
    private toastr: ToastrService,
    private modalDialogService: ModalDialogService,
    private viewRef: ViewContainerRef,
    private authGuard: AuthGuard,
    private title: Title
  ) { }

  ngOnInit() {
    //this.cookieService.set('Expire', '');

    this.title.setTitle("Đăng nhập");
    this.submitted = false;
    this.loginForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
    this.returnUrl = '/dashboard';
    // if(this.authGuard.canActivate) {
    //   this.router.navigate([this.returnUrl]);
    // }
    //this.authService.logout();
  }

  get f() { return this.loginForm.controls };

  login() {
    this.submitted = false;
    if (this.loginForm.invalid) {
      this.submitted = true;
      return;
    }
    else {

      let email = this.f.username.value;
      let password = Md5.hashStr(this.f.password.value);

      this.http.post('/api/user/login', JSON.stringify({ email, password }), httpOptions).subscribe(
        (res) => {
          let data = JSON.stringify(res);
          if (res["meta"]["error_code"] == 200) {
            localStorage.setItem('isLoggedIn', "true");
            localStorage.setItem('data', res.toString());
            localStorage.setItem('access_token', res["data"]["access_token"].toString());
            localStorage.setItem('access_key', res["data"]["access_key"].toString());
            localStorage.setItem('userId', res["data"]["userId"].toString());
            localStorage.setItem('userName', res["data"]["userName"].toString());
            localStorage.setItem('avata', res["data"]["avata"] != undefined ? res["data"]["avata"].toString() : undefined);
            localStorage.setItem('fullName', res["data"]["fullName"] != undefined ? res["data"]["fullName"].toString() : undefined);
            localStorage.setItem('companyId', res["data"]["companyId"] != undefined ? res["data"]["companyId"].toString() : undefined);
            localStorage.setItem('languageId', res["data"]["languageId"] ? res["data"]["languageId"].toString() : undefined);
            localStorage.setItem('websiteId', res["data"]["websiteId"] != undefined ? res["data"]["websiteId"].toString() : undefined);
            localStorage.setItem('BranchId', res["data"]["BranchId"] != undefined ? res["data"]["BranchId"].toString() : undefined);
            localStorage.setItem('PMQLXe', res["data"]["PMQLXe"] != undefined ? res["data"]["PMQLXe"].toString() : undefined);

            localStorage.setItem('menu', JSON.stringify(res["data"]["listMenus"]));
            localStorage.setItem('roles', JSON.stringify(res["data"]["listRoles"]));
            this.cookieService.set('Expire', Date.now().toLocaleString(), 0.1);
            this.router.navigate([this.returnUrl]);
          }
          else {
            this.submitted = true;
            this.message = "Tài khoản hoặc mật khẩu không đúng";
            this.router.navigate(['/login']);
          }
        },
        (err) => {
          this.showConfirm("Đăng nhập không thành công. Xin vui lòng thử lại sau!");
        }
      );
    }
  }


  toat(): void {
    this.toastr.info('Phần này tạm thời chưa có nhé', 'Thông báo');
  }

  showConfirm(message) {
    this.modalDialogService.openDialog(this.viewRef, {
      title: 'Thông báo',
      childComponent: SimpleModalComponent,
      data: {
        text: message
      },
      actionButtons: [
        {
          text: 'Xác nhận',
          buttonClass: 'btn btn-success'
        }
      ],
    });
  }

}

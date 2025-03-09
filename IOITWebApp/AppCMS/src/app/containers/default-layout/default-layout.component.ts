import { Component, Input, ViewChild, ElementRef } from '@angular/core';
import { navItems, domainImage } from './../../data/const';
import { AuthService } from '../../service/auth.service';
import { CookieService } from 'ngx-cookie-service';
import { UserChangePass } from './../../data/Model';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { Md5 } from 'ts-md5/dist/md5';
import { User } from '../../data/model';




@Component({
  selector: 'app-dashboard',
  templateUrl: './default-layout.component.html',
  styleUrls: ['./default-layout.component.scss']
})
export class DefaultLayoutComponent {
  public navItem = [];
  public sidebarMinimized = true;
  private changes: MutationObserver;
  public element: HTMLElement = document.body;
  public userChangePass: UserChangePass = new UserChangePass();
  public domainImage = domainImage;
  public Item: User;
  public role: number;
  companyId: number;
  BranchId: string;
  UserId: number;
  public httpOptions: any;

  @ViewChild('ChangePasswordModal') public changePasswordModal: ModalDirective;
  @ViewChild('modalMyInfo') public modalMyInfo: ModalDirective;
  @ViewChild('file') file: ElementRef;
  constructor(public auth: AuthService, public cookie: CookieService, public toastr: ToastrService, public http: HttpClient) {
    setTimeout(() => {
      var json = JSON.parse(localStorage.getItem('menu'));
      console.log(json);
      this.Item = new User();

      this.navItem.push({
        icon: "fas fa-tachometer-alt",
        name: "Dashboard",
        url: "/dashboard"
      });

      for (var i = 0; i < json.length; i++) {
        this.navItem.push(this.createMenu(json[i], undefined));
      }
      console.log( this.navItem);
    }, 1000);

    this.changes = new MutationObserver((mutations) => {
      this.sidebarMinimized = document.body.classList.contains('sidebar-minimized');
    });

    this.changes.observe(<Element>this.element, {
      attributes: true
    });

    this.myFuntion();

    this.userChangePass.UserId = parseInt(localStorage.getItem("userId"));
    this.userChangePass.UserName = localStorage.getItem("userName");
    this.userChangePass.Avatar = localStorage.getItem("avata");
    this.userChangePass.Logo = localStorage.getItem("logo");
    this.userChangePass.FullName = localStorage.getItem("fullName");

    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      })
    }
  }
  ngOnInit(): void {
    var json = JSON.parse(localStorage.getItem('roles'));
    this.companyId = parseInt(localStorage.getItem('companyId'));
    this.UserId = parseInt(localStorage.getItem('userId'));
    this.BranchId = localStorage.getItem('BranchId');
    this.userChangePass.Logo = localStorage.getItem("logo");
    console.log(this.userChangePass.Logo);
    if (json.length > 0) {
      for (var i = 0; i < json.length; i++) {
        this.role = json[i].RoleId
      }
    }
  }
  createMenu(item, urlParent: string) {
    item["name"] = item["Name"];
    item["url"] = urlParent == undefined ? "/" + item["Url"] : urlParent + "/" + item["Url"];
    item["icon"] = item["Icon"];

    delete item["MenuId"];
    delete item["Code"];
    delete item["Name"];
    delete item["MenuParent"];
    delete item["Url"];
    delete item["Icon"];
    delete item["ActiveKey"];
    delete item["Status"];

    if (item["listMenus"].length > 0) {
      item["children"] = [];
      for (var i = 0; i < item["listMenus"].length; i++) {
        item["children"].push(item["listMenus"][i]);
        this.createMenu(item["children"][i], item["url"]);
      }
    }

    delete item["listMenus"];

    return item;
  }

  logout() {
    this.auth.logout();
  }

  myFuntion() {
    //
    setInterval(() => {
      if (this.cookie.get("Expire") == '' || this.cookie.get("Expire") == undefined || localStorage.getItem('isLoggedIn') != "true") {
        this.auth.logout();
      }
    }, 10000);
  }

  OpenChangePasswordModal() {
    this.userChangePass.PasswordOldE = undefined;
    this.userChangePass.PasswordNewE = undefined;
    this.userChangePass.ConfirmPassword = undefined;
    this.changePasswordModal.show();
  }

  ChangePassword() {
    if (this.userChangePass.PasswordOldE == undefined || this.userChangePass.PasswordOldE == '') {
      this.toastWarning("Chưa nhập Mật khẩu hiện tại!");
      return;
    } else if (this.userChangePass.PasswordNewE == undefined || this.userChangePass.PasswordNewE == '') {
      this.toastWarning("Chưa nhập Mật khẩu mới!");
      return;
    } else if (this.userChangePass.ConfirmPassword == undefined || this.userChangePass.ConfirmPassword == '') {
      this.toastWarning("Chưa nhập Mật khẩu xác nhận!");
      return;
    } else if (this.userChangePass.ConfirmPassword != this.userChangePass.PasswordNewE) {
      this.toastWarning("Mật khẩu xác nhận không đúng!");
      return;
    }

    this.userChangePass.PasswordOld = Md5.hashStr(this.userChangePass.PasswordOldE).toString();
    this.userChangePass.PasswordNew = Md5.hashStr(this.userChangePass.PasswordNewE).toString();

    this.http.put('/api/user/changePass/' + this.userChangePass.UserId, this.userChangePass, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.changePasswordModal.hide();
          this.toastSuccess("Đổi mật khẩu tài khoản thành công!");
        } else if (res["meta"]["error_code"] == 213) {
          this.toastError("Mật khẩu hiện tại không đúng. Vui lòng thử lại!");
        }
        else {
          this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
        }
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
      }
    );
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

  OpenModalInfo() {
    this.Item = new User();
    this.file.nativeElement.value = "";

    this.http.get('/api/user/infoUser/' + this.userChangePass.UserId, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.Item = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      });

    this.modalMyInfo.show();
  }

  upload(files) {
    if (files.length === 0)
      return;

    const formData = new FormData();

    for (let file of files)
      formData.append(file.name, file);

    const uploadReq = new HttpRequest('POST', 'api/upload/uploadImage/6', formData, {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      }),
      reportProgress: true,
    });

    this.http.request(uploadReq).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress) {
      }
      else if (event.type === HttpEventType.Response) {
        this.Item.Avata = event.body["data"].toString();
      }
    });
  }

  SaveInfo() {
    if (this.Item.FullName == undefined || this.Item.FullName == '') {
      this.toastWarning("Chưa nhập Tên người dùng!");
      return;
    } else if (this.Item.UserName == undefined || this.Item.UserName == '') {
      this.toastWarning("Chưa nhập Tên tài khoản!");
      return;
    }

    this.http.put('/api/user/changeInfoUser', this.Item, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.modalMyInfo.hide();
          this.toastSuccess("Lưu thông tin thành công!");
        }
        else {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        }
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
      });
  }
}

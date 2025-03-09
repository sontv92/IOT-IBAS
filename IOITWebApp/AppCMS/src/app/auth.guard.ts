import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private router: Router, private cookie: CookieService) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
  	let url: string = state.url;
  	return this.verifyLogin(url);
  }

  verifyLogin(url) : boolean {

    console.log(url);
    console.log(this.isLoggedIn());
  	if(!this.isLoggedIn()) {
      if(url != '/login') {
        this.router.navigate(['/login']);
        return false;
      }
      return true;
  	}
  	else
  	{
  		if(this.isLoggedIn()) {
        if(url == '/login') {
          this.router.navigate(['/dashboard']);
          return false;
        }

        let arr = new Array();
        var json = JSON.parse(localStorage.getItem('menu'));

        json.push({
          ActiveKey: "111111111",
          Code: "",
          Icon: "fa fa-tachometer",
          MenuId: undefined,
          MenuParent: 0,
          Name: "Dashboard",
          Status: undefined,
          Url: "dashboard",
          listMenus: []
        });

        for(var i = 0; i < json.length; i++) {
          this.checkUrlPermission(arr, undefined, json[i]);
        }

        if(arr.indexOf(url) == -1) {
          this.router.navigate(['/404']);
        }
        else
        {
          return true;
        }

        return true;
  		}
  	}
  }

  public isLoggedIn(): boolean {
  	let status = false;
  	if (localStorage.getItem('isLoggedIn') == "true" && this.cookie.get("Expire") != '') {
  		status = true;
  	}
  	else
  	{
  		status = false;
  	}
  	return status;
  }

  checkUrlPermission(arr, urlParent, item): void {
    let url = urlParent == undefined ? "/" + item["Url"] : urlParent + "/" + item["Url"];
    arr.push(url);
    if(item["listMenus"].length > 0) {
      for(var i = 0; i < item["listMenus"].length; i ++) {
        this.checkUrlPermission(arr, url, item["listMenus"][i]);
      }
    }
  }

}

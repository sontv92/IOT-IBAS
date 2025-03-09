import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';


@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private router: Router, private cookieService: CookieService) { }

  logout(): void {
    localStorage.setItem('isLoggedIn', "false");
    localStorage.removeItem('data');
    localStorage.removeItem('access_token');
    localStorage.removeItem('access_key');
    localStorage.removeItem('userId');
    localStorage.removeItem('userName');
    localStorage.removeItem('avata');
    localStorage.removeItem('fullName');
    localStorage.removeItem('companyId');
    localStorage.removeItem('languageId');
    localStorage.removeItem('menu');
    this.cookieService.delete("Expire");
    this.router.navigate(['/login']);
  }
}

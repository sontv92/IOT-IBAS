import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd, ActivatedRoute } from '@angular/router';
import { filter, map, mergeMap } from 'rxjs/operators';
import { Title } from '@angular/platform-browser';
import { AuthService } from './service/auth.service';
import { CookieService } from 'ngx-cookie-service';

@Component({
  // tslint:disable-next-line
  selector: 'body',
  template: '<router-outlet></router-outlet><ngx-loading-bar></ngx-loading-bar>',
  //selector: 'app-root',
  //templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  constructor(private router: Router, private titleService: Title,
    private activatedRoute: ActivatedRoute, private cookie: CookieService, private auth: AuthService) { 
      this.titleService.setTitle('');
      //this.myFuntion();
    }

  ngOnInit() {
    // this.router.events.subscribe((evt) => {
    //   if (!(evt instanceof NavigationEnd)) {
    //     return;
    //   }
    //   window.scrollTo(0, 0);
    // });

    this.router.events.pipe(
      filter((event) => event instanceof NavigationEnd),
      map(() => this.activatedRoute),
      map((route) => {
        while (route.firstChild) route = route.firstChild;
          return route;
      }),
      filter((route) => route.outlet === 'primary'),
      mergeMap((route) => route.data)).
      subscribe((event) => this.titleService.setTitle(event['title'])
    );
  }

  myFuntion() {
    setInterval(() => {
      if(this.cookie.get("Expire") == '') {
        this.auth.logout();
      }
    }, 10000);
  }
}

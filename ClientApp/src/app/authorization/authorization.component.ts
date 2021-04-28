import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { XeroAuth } from '../app.model';
import { AppService } from '../app.service';

@Component({
  selector: 'app-authorization',
  templateUrl: './authorization.component.html'
})
export class AuthorizationComponent implements OnInit {
  xeroAuth: XeroAuth;
  XeroOAuth2Token;
  Timesheets;
  Tenant;

  constructor(private router: Router, private routerPath: ActivatedRoute, private appService: AppService) { }

  ngOnInit() {
    this.xeroAuth = null
    this.routerPath.queryParams.subscribe(params => {
      this.xeroAuth = {
        authorizationCode: params['code'],
        scope: params['scope'],
        sessionState: params['session_state'],
        state: params['state'],
        error: params['error']
      }
    });

    //console.log('xeroAuth', this.xeroAuth);
    if (this.xeroAuth.error) {
      alert(this.xeroAuth.error);
      this.router.navigate(['/'])
    }
    else {
      this.appService.callback(this.xeroAuth)
        .subscribe((response) => {
          //console.log('callback', response)
          this.XeroOAuth2Token = response;
          this.Tenant = response.tenants[0]
          this.appService.setTokenData(response);
        },
          error => {
            alert("Please reconnect.");
            this.router.navigate(['/'])
          });

      //this.appService.xeroTokenAPI(this.xeroAuth)
      //  .subscribe((response) => {
      //    console.log('xeroTokenAPI', response)
      //    this.XeroOAuth2Token = response;
      //  });
    }
  }

  getTimesheets() {
    this.Timesheets = [];
    this.appService.timesheetsAPI()
      .subscribe((response) => {
        //console.log('timesheetsAPI', response)
        this.Timesheets = response;
      },
        error => {
          alert("Please reconnect.");
          this.router.navigate(['/'])
        });
  }

  disconnectXero() {
    this.Timesheets = null;
    this.appService.disconnectXero()
      .subscribe((response) => {
        alert('Disconnected.')
        this.appService.removeTokenData();
        this.router.navigate(['/'])
      });
  }
}

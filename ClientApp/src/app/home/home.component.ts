import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { AppService } from '../app.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit  {
  connectURL = "https://login.xero.com/identity/connect/authorize?response_type=code"

  constructor(http: HttpClient, private router: Router, private routerPath: ActivatedRoute,
    private appService: AppService) { }

  ngOnInit() {

    this.appService.removeTokenData();

    this.connectURL += '&client_id=' + environment.clientId
      + '&redirect_uri=' + environment.callbackUri
      + '&scope=' + environment.scope
      + '&state=' + environment.state

    //console.log(this.connectURL)

  }
}

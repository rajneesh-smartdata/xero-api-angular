import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { XeroAuth } from './app.model';


@Injectable()
export class AppService {
  tokenData = new BehaviorSubject<any>({});
  private loadingStateSubject = new BehaviorSubject<boolean>(false);
  public loadingState = this.loadingStateSubject.asObservable();

  constructor(private http: HttpClient) { }

  setTokenData(data: any) {
    localStorage.setItem('TenantId', data.tenants[0].tenantId)
    localStorage.setItem('Tenant', JSON.stringify(data.tenants[0]))
    //localStorage.setItem('AccessToken', data.accessToken) //setItem('access_token', JSON.stringify(data.accessToken)
    //localStorage.setItem('RefreshToken', data.refreshToken)
    //localStorage.setItem('ExpiresAtUtc', data.expiresAtUtc)

    this.tokenData.next(data);
  }

  getTokenData(): Observable<any> {
    return this.tokenData.asObservable();
  }

  removeTokenData() {
    localStorage.removeItem('TenantId');
    localStorage.removeItem('Tenant')
    //localStorage.removeItem('AccessToken')
    //localStorage.removeItem('RefreshToken')
    //localStorage.removeItem('ExpiresAtUtc')

    // Set current token to an empty object
    this.tokenData.next({});
  }

  callback(data : XeroAuth): Observable<any> {
    this.loadingStateSubject.next(true)
    const params = new HttpParams()
      .set('code', data.authorizationCode)
      .set('state', data.state);

    //let headers = new HttpHeaders();
    //headers = headers.append('responseType', 'json');

    return this.http.get<any>(`${environment.apiURL}/Authorization/Callback`, { params })
      .pipe(map(res => { this.loadingStateSubject.next(false); return res }));

  }

  disconnectXero(): Observable<any> {
    const params = new HttpParams()
      .set('TenantId', localStorage.getItem('TenantId'));

    return this.http.get<any>(`${environment.apiURL}/Authorization/Disconnect`, { params })
      .pipe(map(res => { this.loadingStateSubject.next(false); return res }));
  }

  timesheetsAPI(): Observable<any> {
    this.loadingStateSubject.next(true)
    const params = new HttpParams()
      .set('TenantId', localStorage.getItem('TenantId'));

    //let headers = new HttpHeaders();
    //headers = headers.append('responseType', 'json');

    return this.http.get<any>(`${environment.apiURL}/Payroll/Timesheets`, { params })
      .pipe(map(res => { this.loadingStateSubject.next(false); return res }));

  }

  xeroTokenAPI(data: XeroAuth): Observable<any> {
    this.loadingStateSubject.next(true)
    const params = new HttpParams()
      .set('grant_type', 'authorization_code')
      .set('code', data.authorizationCode)
      .set('redirect_uri', environment.callbackUri);

    let headers = new HttpHeaders(); //base64encode
    headers = headers.append('authorization', "Basic " + btoa(environment.clientId + ":" + environment.clientSecret));
    headers = headers.append('Content-Type', 'application/x-www-form-urlencoded');

    return this.http.post<any>(`${environment.xeroTokenURL}`, { params }, { headers })
      .pipe(map(res => { this.loadingStateSubject.next(false); return res }));;

  }

}

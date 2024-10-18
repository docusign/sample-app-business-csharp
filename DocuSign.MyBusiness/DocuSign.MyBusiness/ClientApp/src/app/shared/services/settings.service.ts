import { Observable } from 'rxjs'
import { Injectable, Inject } from '@angular/core'
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http'

@Injectable({ providedIn: 'root' })
export class SettingsService {
    constructor(
        private http: HttpClient,
        @Inject('BASE_URL') private baseUrl: string
    ) {}

    getSettings(): Observable<any> {
        return this.http.get<any>(this.baseUrl + 'api/settings', {
            headers: new HttpHeaders({
                'Content-Type': 'application/json'
            })
        })
    }

    getDatasource(): Observable<any> {
        return this.http.get<any>(this.baseUrl + 'api/settings/datasource', {
            headers: new HttpHeaders({
                'Content-Type': 'application/json'
            })
        })
    }

    getConnectionStatus(): Observable<any> {
        return this.http.get<any>(this.baseUrl + 'api/account/status', {
            headers: new HttpHeaders({
                'Content-Type': 'application/json'
            })
        })
    }

    getAccounts(basePath: string, userId: string): Observable<any> {
        const params = new HttpParams().set('basePath', basePath).set('userId', userId)
        return this.http.get<any>(this.baseUrl + 'api/accounts?', {
            headers: new HttpHeaders({
                'Content-Type': 'application/json'
            }),
            params
        })
    }

    logout(): Observable<any> {
        return this.http.get<any>(this.baseUrl + 'api/account/logout', {
            headers: new HttpHeaders({
                'Content-Type': 'application/json'
            })
        })
    }
}

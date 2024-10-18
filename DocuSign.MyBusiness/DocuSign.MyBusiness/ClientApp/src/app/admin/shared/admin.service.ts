import { Observable } from 'rxjs'
import { Injectable, Inject } from '@angular/core'
import { HttpClient, HttpHeaders } from '@angular/common/http'
import { Settings, Authorize, AccountConnect } from './admin.model'

@Injectable({ providedIn: 'root' })
export class AdminService {
    constructor(
        private http: HttpClient,
        @Inject('BASE_URL') private baseUrl: string
    ) {}

    saveSettings(settings: Settings): Observable<any> {
        return this.http.put<any>(this.baseUrl + 'api/settings', JSON.stringify(settings), {
            headers: new HttpHeaders({
                'Content-Type': 'application/json'
            })
        })
    }

    authorize(authorize: Authorize): Observable<any> {
        return this.http.post<any>(this.baseUrl + 'api/account/consent/obtain', JSON.stringify(authorize), {
            headers: new HttpHeaders({
                'Content-Type': 'application/json'
            })
        })
    }

    unauthorize(): Observable<any> {
        return this.http.get<any>(this.baseUrl + 'api/account/consent/remove', {
            headers: new HttpHeaders({
                'Content-Type': 'application/json'
            })
        })
    }

    connect(accontConnect: AccountConnect): Observable<any> {
        return this.http.post<any>(this.baseUrl + 'api/account/connect', JSON.stringify(accontConnect), {
            headers: new HttpHeaders({
                'Content-Type': 'application/json'
            })
        })
    }

    disconnect(): Observable<any> {
        return this.http.get<any>(this.baseUrl + 'api/account/disconnect', {
            headers: new HttpHeaders({
                'Content-Type': 'application/json'
            })
        })
    }
}

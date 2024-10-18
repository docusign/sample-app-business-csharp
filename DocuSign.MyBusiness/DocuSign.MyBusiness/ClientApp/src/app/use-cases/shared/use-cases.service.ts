import { Observable } from 'rxjs'
import { Injectable, Inject } from '@angular/core'
import { HttpClient, HttpHeaders } from '@angular/common/http'
import {
    EmploymentContractEnvelopeModel,
    TermsAndConditionsWithConractEnvelopModel,
    AuthorizeProfileInfo,
    UpdateCustomerProfileModel,
    CustomQuoteInfo
} from './use-cases.model'

@Injectable({ providedIn: 'root' })
export class UseCasesService {
    constructor(
        private http: HttpClient,
        @Inject('BASE_URL') private baseUrl: string
    ) {}

    createEmploymentContract(data: EmploymentContractEnvelopeModel): Observable<any> {
        return this.http.post<any>(this.baseUrl + 'api/envelopes/employment-contract', JSON.stringify(data), {
            headers: new HttpHeaders({
                'Content-Type': 'application/json'
            })
        })
    }

    sendTermsAndConditions(termsAndConditionsData: TermsAndConditionsWithConractEnvelopModel): Observable<any> {
        return this.http.post<any>(
            this.baseUrl + 'api/envelopes/terms-and-conditions-with-contract',
            JSON.stringify(termsAndConditionsData),
            {
                headers: new HttpHeaders({
                    'Content-Type': 'application/json'
                })
            }
        )
    }

    authorizeProfile(profileInfo: AuthorizeProfileInfo): Observable<any> {
        return this.http.post<any>(this.baseUrl + 'api/profile/authorize', JSON.stringify(profileInfo), {
            headers: new HttpHeaders({
                'Content-Type': 'application/json'
            })
        })
    }

    getProfile(): Observable<any> {
        return this.http.get<any>(this.baseUrl + 'api/profile', {
            headers: new HttpHeaders({
                'Content-Type': 'application/json'
            })
        })
    }

    editProfile(profileInfo: UpdateCustomerProfileModel): Observable<any> {
        return this.http.post<any>(this.baseUrl + 'api/envelopes/update-profile', JSON.stringify(profileInfo), {
            headers: new HttpHeaders({
                'Content-Type': 'application/json'
            })
        })
    }

    sendCustomQuoteInfo(customQuoteInfo: CustomQuoteInfo): Observable<any> {
        return this.http.post<any>(this.baseUrl + 'api/envelopes/custom-quote', JSON.stringify(customQuoteInfo), {
            headers: new HttpHeaders({
                'Content-Type': 'application/json'
            })
        })
    }
}

import { Injectable } from '@angular/core'
import { Observable, BehaviorSubject, map } from 'rxjs'

@Injectable({
    providedIn: 'root'
})
export class UpdateStatusService {
    private connectionStatusSubject = new BehaviorSubject<{ isConnected: boolean; isConsentGranted: boolean }>({
        isConnected: false,
        isConsentGranted: false
    })

    getConnectionStatus(): { isConnected$: Observable<boolean>; isConsentGranted$: Observable<boolean> } {
        const isConnected$ = this.connectionStatusSubject.asObservable().pipe(map((payload) => payload.isConnected))
        const isConsentGranted$ = this.connectionStatusSubject.asObservable().pipe(map((payload) => payload.isConsentGranted))

        return { isConnected$, isConsentGranted$ }
    }

    updateConnectionStatus(payload: { isConnected: boolean; isConsentGranted: boolean }) {
        this.connectionStatusSubject.next(payload)
    }
}

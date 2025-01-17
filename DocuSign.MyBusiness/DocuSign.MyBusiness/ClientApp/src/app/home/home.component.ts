import { Component } from '@angular/core'
import { SettingsService } from '../shared/services/settings.service'
import { SignalrService } from '../shared/services/signalr.service'
import { UpdateStatusService } from '../shared/services/updateStatus.service'
import { Observable } from 'rxjs'

@Component({
    standalone: false,
    selector: 'app-home',
    templateUrl: './home.component.html'
})
export class HomeComponent {
    connectionStatus: { isConnected$: Observable<boolean>; isConsentGranted$: Observable<boolean> }
    connectedUser = {
        name: '',
        email: '',
        accountName: ''
    }

    constructor(
        private settingsService: SettingsService,
        private updateStatusService: UpdateStatusService,
        private signalrService: SignalrService
    ) {
        this.connectionStatus = this.updateStatusService.getConnectionStatus()
    }

    ngOnInit(): void {
        this.getConnectionStatus()
    }

    getConnectionStatus(): void {
        this.settingsService.getConnectionStatus().subscribe((payload) => {
            this.connectedUser = payload.connectedUser
            this.updateStatusService.updateConnectionStatus({
                isConnected: payload.isConnected,
                isConsentGranted: payload.isConsentGranted
            })
            if (payload.isConnected && this.signalrService.connection.state !== 'Connected') {
                this.signalrService.connection.start().catch((error: any) => {
                    console.log(`Signalr error: ${error}`)
                })
            }
        })
    }
}

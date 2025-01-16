import { Component, OnInit, Input } from '@angular/core'
import { Router, NavigationEnd } from '@angular/router'
import { filter, map, startWith } from 'rxjs/operators'
import { Observable, Subscription } from 'rxjs'
import { ToastrService } from 'ngx-toastr'
import { SettingsService } from '../services/settings.service'
import { UpdateStatusService } from '../services/updateStatus.service'

@Component({
    selector: 'app-header',
    standalone: false,
    templateUrl: './header.component.html'
})
export class HeaderComponent implements OnInit {
    @Input() style = 'white'
    isHomePage$: Observable<boolean>
    isAuthenticated = false
    connectionStatus: { isConnected$: Observable<boolean>; isConsentGranted$: Observable<boolean> }
    isConnectedSubscription: Subscription
    constructor(
        private router: Router,
        private toastr: ToastrService,
        private updateStatusService: UpdateStatusService,
        private settingsService: SettingsService
    ) {
        this.connectionStatus = this.updateStatusService.getConnectionStatus()
    }

    ngOnInit(): void {
        this.isHomePage$ = this.router.events.pipe(
            filter((event) => event instanceof NavigationEnd),
            map((event: NavigationEnd) => event.url === '/'),
            startWith(true)
        )
    }

    logout() {
        this.settingsService.logout().subscribe({
            next: () => {
                window.location.href = '/'
            },
            error: (error: any) => {
                this.toastr.error(`Please try again: ${error.error?.Message}`, 'Something went wrong', {
                    toastClass: 'error-message ngx-toastr custom-toastr',
                    tapToDismiss: true
                })
            }
        })
    }
}

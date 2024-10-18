import { Component, OnInit, Input } from '@angular/core'
import { SettingsService } from '../../shared/services/settings.service'

@Component({
    selector: 'app-user-info',
    templateUrl: './user-info.component.html'
})
export class UserInfoComponent implements OnInit {
    @Input() connectedUser = {
        name: '',
        email: '',
        accountName: ''
    }

    constructor(private settingsService: SettingsService) {}

    ngOnInit(): void {}
}

import { Component, OnInit, Input } from '@angular/core'
import { SettingsService } from '../services/settings.service'

@Component({
    standalone: false,
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

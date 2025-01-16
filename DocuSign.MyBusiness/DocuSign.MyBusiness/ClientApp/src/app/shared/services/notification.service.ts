import { Injectable } from '@angular/core'
import { ToastrService } from 'ngx-toastr'
import { NotificationInfo, UseCaseNames } from '../models/shared.model'

@Injectable({
    providedIn: 'root'
})
export class NotificationService {
    notificationsMap = {
        'envelope-sent': {
            title: 'Envelope sent',
            class: 'info-message',
            showTabs: false
        },
        'envelope-completed': {
            title: 'Envelope completed',
            class: 'success-message',
            showTabs: true
        },
        'envelope-declined': {
            title: 'Envelope declined',
            class: 'error-message',
            showTabs: false
        },
        'envelope-voided': {
            title: 'Envelope voided',
            class: 'error-message',
            showTabs: false
        }
    }

    constructor(private toastr: ToastrService) {}

    ngOnInit(): void {}

    showMessage(event: NotificationInfo) {
        const notification = this.notificationsMap[event.event]
        const notificationTime = new Intl.DateTimeFormat('en-US', {
            dateStyle: 'short',
            timeStyle: 'medium'
        }).format(new Date(event.date))
        this.toastr.success(
            `<div class="toast-title my-2">${notification.title}</div>
             <div>${event.signer.name}</div>
             <div>${event.signer.email}</div>
             <div>${notificationTime}</div><br/>` +
                (notification.showTabs && event.signer.tabs
                    ? `<table>${event.signer.tabs
                          .map((tab) => {
                              if (tab.value) {
                                  return `<tr><td>${tab.key.replace(/\./g, ' ')}</td><td>${tab.value}</td></tr>`
                              }
                              return null
                          })
                          .join('')}</table>`
                    : ''),
            UseCaseNames[event.useCase],
            {
                toastClass: `${notification.class} ${notification.showTabs ? 'tabs' : ''} ngx-toastr info-toastr custom-toastr`,
                enableHtml: true,
                timeOut: 0,
                extendedTimeOut: 0,
                tapToDismiss: true
            }
        )
    }
}

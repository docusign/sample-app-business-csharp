import { Injectable } from '@angular/core'
import { BehaviorSubject } from 'rxjs'
import * as signalR from '@microsoft/signalr'
import { NotificationService } from './notification.service'

@Injectable({
    providedIn: 'root'
})
export class SignalrService {
    hubUrl: string
    connection: any
    hubHelloMessage: BehaviorSubject<string>
    constructor(private notificationService: NotificationService) {
        this.hubUrl = '/events-hub'
    }

    public async initiateSignalrConnection(): Promise<void> {
        try {
            this.connection = new signalR.HubConnectionBuilder().withUrl(this.hubUrl).withAutomaticReconnect().build()

            this.setSignalrClientMethods()

            console.log(`SignalR connection success! connectionId: ${this.connection.connectionId}`)
        } catch (error) {
            console.log(`SignalR connection error: ${error}`)
        }
    }

    private setSignalrClientMethods(): void {
        this.connection.on('ReceivedEvent', (event: any) => {
            this.notificationService.showMessage(event)
            this.hubHelloMessage.next(event)
        })
    }
}

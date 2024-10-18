import { Component, OnInit, Input } from '@angular/core'

@Component({
    selector: 'app-action',
    templateUrl: './action.component.html'
})
export class ActionComponent implements OnInit {
    @Input() action: any
    ngOnInit(): void {}
}

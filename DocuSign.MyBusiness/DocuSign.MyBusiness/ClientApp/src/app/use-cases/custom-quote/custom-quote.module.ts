import { NgModule } from '@angular/core'
import { CommonModule } from '@angular/common'
import { TranslateModule } from '@ngx-translate/core'
import { CustomQuoteComponent } from './custom-quote.component'
import { SharedModule } from './../../shared/shared.module'
import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { NgSelectModule } from '@ng-select/ng-select'
import { CustomQuoteRoutingModule } from './custom-quote-routing.module'

@NgModule({
    declarations: [CustomQuoteComponent],
    imports: [
        CustomQuoteRoutingModule,
        FormsModule,
        ReactiveFormsModule,
        CommonModule,
        SharedModule,
        NgSelectModule,
        TranslateModule.forChild()
    ]
})
export class CustomQuoteModule {}

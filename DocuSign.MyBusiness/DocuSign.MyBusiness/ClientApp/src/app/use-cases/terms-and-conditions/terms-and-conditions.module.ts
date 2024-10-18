import { NgModule } from '@angular/core'
import { CommonModule } from '@angular/common'
import { TranslateModule } from '@ngx-translate/core'
import { TermsAndConditionsComponent } from './terms-and-conditions.component'
import { SharedModule } from './../../shared/shared.module'
import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { TermsAndConditionsRoutingModule } from './terms-and-conditions-routing.module'

@NgModule({
    declarations: [TermsAndConditionsComponent],
    imports: [TermsAndConditionsRoutingModule, FormsModule, ReactiveFormsModule, CommonModule, SharedModule, TranslateModule.forChild()]
})
export class TermsAndConditionsModule {}

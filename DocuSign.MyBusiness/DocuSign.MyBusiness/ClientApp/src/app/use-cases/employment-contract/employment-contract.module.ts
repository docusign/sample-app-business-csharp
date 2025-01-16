import { NgModule } from '@angular/core'
import { CommonModule } from '@angular/common'
import { TranslateModule } from '@ngx-translate/core'
import { EmploymentContractComponent } from './employment-contract.component'
import { SharedModule } from '../../shared/shared.module'
import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { EmploymentContractRoutingModule } from './employment-contract-routing.module'

@NgModule({
    declarations: [EmploymentContractComponent],
    imports: [EmploymentContractRoutingModule, FormsModule, ReactiveFormsModule, CommonModule, SharedModule, TranslateModule.forChild()]
})
export class EmploymentContractModule {}

import { SharedModule } from '../shared/shared.module'
import { NgModule } from '@angular/core'
import { AdminRoutingModule } from './admin-routing.module'
import { AdminComponent } from './admin.component'
import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { NgSelectModule } from '@ng-select/ng-select'

@NgModule({
    declarations: [AdminComponent],
    imports: [AdminRoutingModule, FormsModule, ReactiveFormsModule, SharedModule, NgSelectModule]
})
export class AdminModule {}

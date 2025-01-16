import { NgModule } from '@angular/core'
import { CommonModule } from '@angular/common'
import { TranslateModule } from '@ngx-translate/core'
import { UpdateProfileComponent } from './update-profile.component'
import { SharedModule } from '../../shared/shared.module'
import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { UpdateProfileRoutingModule } from './update-profile-routing.module'

@NgModule({
    declarations: [UpdateProfileComponent],
    imports: [UpdateProfileRoutingModule, FormsModule, ReactiveFormsModule, CommonModule, SharedModule, TranslateModule.forChild()]
})
export class UpdateProfileModule {}

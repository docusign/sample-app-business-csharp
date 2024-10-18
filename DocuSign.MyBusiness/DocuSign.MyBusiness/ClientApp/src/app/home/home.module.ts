import { NgModule } from '@angular/core'
import { CommonModule } from '@angular/common'
import { TranslateModule } from '@ngx-translate/core'
import { HomeComponent } from './home.component'
import { ActionsComponent } from './actions/actions.component'
import { ActionComponent } from './actions/action/action.component'
import { SharedModule } from './../shared/shared.module'
import { HomeRoutingModule } from './home-routing.module'

@NgModule({
    declarations: [HomeComponent, ActionsComponent, ActionComponent],
    imports: [CommonModule, HomeRoutingModule, SharedModule, TranslateModule.forChild()]
})
export class HomeModule {}

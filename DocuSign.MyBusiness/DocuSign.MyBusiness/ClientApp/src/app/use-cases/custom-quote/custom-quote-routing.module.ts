import { NgModule } from '@angular/core'
import { Routes, RouterModule } from '@angular/router'
import { CustomQuoteComponent } from './custom-quote.component'

const routes: Routes = [{ path: '', component: CustomQuoteComponent }]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class CustomQuoteRoutingModule {}

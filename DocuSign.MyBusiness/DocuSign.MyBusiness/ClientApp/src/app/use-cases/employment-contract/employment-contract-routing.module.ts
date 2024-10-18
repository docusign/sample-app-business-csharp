import { NgModule } from '@angular/core'
import { Routes, RouterModule } from '@angular/router'
import { EmploymentContractComponent } from './employment-contract.component'

const routes: Routes = [{ path: '', component: EmploymentContractComponent }]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class EmploymentContractRoutingModule {}

import { NgModule } from '@angular/core'
import { Routes, RouterModule } from '@angular/router'

const routes: Routes = [
    {
        path: '',
        loadChildren: () => import('./home/home.module').then((m) => m.HomeModule)
    },
    {
        path: 'admin',
        loadChildren: () => import('./admin/admin.module').then((m) => m.AdminModule)
    },
    {
        path: 'employment-contract',
        loadChildren: () => import('./use-cases/employment-contract/employment-contract.module').then((m) => m.EmploymentContractModule)
    },
    {
        path: 'terms-and-conditions',
        loadChildren: () => import('./use-cases/terms-and-conditions/terms-and-conditions.module').then((m) => m.TermsAndConditionsModule)
    },
    {
        path: 'update-profile',
        loadChildren: () => import('./use-cases/update-profile/update-profile.module').then((m) => m.UpdateProfileModule)
    },
    {
        path: 'custom-quote',
        loadChildren: () => import('./use-cases/custom-quote/custom-quote.module').then((m) => m.CustomQuoteModule)
    },
    {
        path: '**',
        redirectTo: ''
    }
]

@NgModule({
    imports: [
        RouterModule.forRoot(routes, {
            scrollPositionRestoration: 'top'
        })
    ],
    exports: [RouterModule]
})
export class AppRoutingModule {}

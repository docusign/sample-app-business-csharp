import { Component, OnInit } from '@angular/core'
import { Router } from '@angular/router'

@Component({
    selector: 'app-actions',
    templateUrl: './actions.component.html'
})
export class ActionsComponent implements OnInit {
    actionsMap = [
        {
            title: `Actions.EmploymentContractTitle`,
            description: `Actions.EmploymentContractDescription`,
            actionTitle: `Actions.InitiateButton`,
            action: () => this.router.navigate(['/employment-contract']),
            featuresDescription: [
                `Actions.EmploymentContractFeature1`,
                `Actions.EmploymentContractFeature2`,
                `Actions.EmploymentContractFeature3`,
                `Actions.EmploymentContractFeature4`,
                `Actions.EmploymentContractFeature5`
            ],
            iconSrc: 'assets/img/ic-custom-one.png'
        },
        {
            title: `Actions.TermsAndConditionsTitle`,
            description: `Actions.TermsAndConditionsDescription`,
            actionTitle: `Actions.InitiateButton`,
            action: () => this.router.navigate(['/terms-and-conditions']),
            featuresDescription: [
                `Actions.TermsAndConditionsFeature1`,
                `Actions.TermsAndConditionsFeature2`,
                `Actions.TermsAndConditionsFeature3`,
                `Actions.TermsAndConditionsFeature4`
            ],
            iconSrc: 'assets/img/ic-custom-two.png'
        },
        {
            title: `Actions.UpdateProfileTitle`,
            description: `Actions.UpdateProfileDescription`,
            actionTitle: `Actions.InitiateButton`,
            action: () => this.router.navigate(['/update-profile']),
            featuresDescription: [
                `Actions.UpdateProfileFeature1`,
                `Actions.UpdateProfileFeature2`,
                `Actions.UpdateProfileFeature3`,
                `Actions.UpdateProfileFeature4`
            ],
            iconSrc: 'assets/img/ic-custom-three.png'
        },
        {
            title: `Actions.CustomQuoteTitle`,
            description: `Actions.CustomQuoteDescription`,
            actionTitle: `Actions.InitiateButton`,
            action: () => this.router.navigate(['/custom-quote']),
            featuresDescription: [`Actions.CustomQuoteFeature1`, `Actions.CustomQuoteFeature2`, `Actions.CustomQuoteFeature3`],
            iconSrc: 'assets/img/ic-custom-four.png'
        }
    ]

    constructor(private router: Router) {}

    ngOnInit(): void {}

    goToSettings() {
        this.router.navigate(['/admin'])
    }
}

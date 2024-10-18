import { Component, OnInit } from '@angular/core'
import { FormControl, Validators, FormGroup, AbstractControl } from '@angular/forms'
import { SettingsService } from '../../shared/services/settings.service'
import { forkJoin } from 'rxjs'
import { Router } from '@angular/router'
import { ToastrService } from 'ngx-toastr'
import { FormService } from './../../shared/services/form.service'
import { UseCasesService } from './../shared/use-cases.service'
import { EnvelopeAction, EmploymentContractEnvelopeModel, SignatureType } from '../shared/use-cases.model'
import { HelperService } from './../../shared/services/helper.service'
import { EMAIL_REGEX } from './../../shared/constants/constants'
import { UseCaseNames } from './../../shared/models/shared.model'

@Component({
    selector: 'app-employment-contract',
    templateUrl: './employment-contract.component.html'
})
export class EmploymentContractComponent implements OnInit {
    templates = []
    signatureTypes = []
    selectedTemplate: string
    selectedSignatureType: string
    executingAction = false
    showAccessCode = false

    employmentContractForm: FormGroup = new FormGroup({
        firstName: new FormControl('', Validators.required),
        lastName: new FormControl('', Validators.required),
        email: new FormControl('', [Validators.required, Validators.pattern(EMAIL_REGEX)])
    })

    constructor(
        private settingsService: SettingsService,
        private router: Router,
        private formService: FormService,
        private toastr: ToastrService,
        private useCasesService: UseCasesService,
        private helperService: HelperService
    ) {}

    ngOnInit(): void {
        this.executingAction = true
        forkJoin({
            settings: this.settingsService.getSettings(),
            datasource: this.settingsService.getDatasource()
        }).subscribe({
            next: (response) => {
                this.templates = response.datasource.templates
                this.signatureTypes = response.datasource.signatureTypes
                this.selectedTemplate = response.settings.template
                this.selectSignatureType(response.settings.signatureType)
                this.executingAction = false
            },
            error: () => {
                this.toastr.error('Please try again', 'Something went wrong', {
                    toastClass: 'error-message ngx-toastr custom-toastr',
                    tapToDismiss: true
                })
                this.executingAction = false
            }
        })
    }

    selectTemplate(value: string) {
        this.selectedTemplate = value
    }

    selectSignatureType(value: string) {
        this.selectedSignatureType = value
        if (this.selectedSignatureType === SignatureType.DsEuAdvancedProviderName) {
            this.employmentContractForm.addControl('accessCode', new FormControl('', Validators.required))
            this.employmentContractForm.patchValue({
                accessCode: this.helperService.generateRandomNumber()
            })
            this.showAccessCode = true
        } else {
            this.employmentContractForm.removeControl('accessCode')
            this.showAccessCode = false
        }
    }

    isInvalid(control: AbstractControl): boolean {
        return this.formService.isInvalid(control)
    }

    sendNow() {
        this.executingAction = true
        const envelopeData: EmploymentContractEnvelopeModel = {
            envelopeAction: EnvelopeAction.Send,
            redirectUrl: '/',
            template: this.selectedTemplate,
            signerInfo: {
                firstName: this.employmentContractForm.value.firstName,
                lastName: this.employmentContractForm.value.lastName,
                email: this.employmentContractForm.value.email
            },
            signatureInfo: {
                signatureType: this.selectedSignatureType
            }
        }
        if (this.selectedSignatureType === SignatureType.DsEuAdvancedProviderName) {
            envelopeData.signatureInfo.accessCode = this.employmentContractForm.value.accessCode
        }
        this.useCasesService.createEmploymentContract(envelopeData).subscribe({
            next: () => {
                this.toastr.success(
                    '<div class="toast-title my-2">Envelope is created</div><div>Contract was sent for signing</div>',
                    UseCaseNames['employment-contract'],
                    {
                        toastClass: 'info-message ngx-toastr custom-toastr',
                        enableHtml: true,
                        timeOut: 0,
                        extendedTimeOut: 0,
                        tapToDismiss: true
                    }
                )
                this.executingAction = false
                this.router.navigate(['/'])
            },
            error: (error: any) => {
                if (error.error) {
                    this.toastr.error(`Please try again: ${error.error.Message}`, 'Something went wrong', {
                        toastClass: 'error-message ngx-toastr custom-toastr',
                        tapToDismiss: true
                    })
                }
                this.executingAction = false
            }
        })
    }

    reviewAndSend() {
        this.executingAction = true

        const envelopeData: EmploymentContractEnvelopeModel = {
            envelopeAction: EnvelopeAction.ReviewAndSend,
            redirectUrl: '',
            template: this.selectedTemplate,
            signerInfo: {
                firstName: this.employmentContractForm.value.firstName,
                lastName: this.employmentContractForm.value.lastName,
                email: this.employmentContractForm.value.email
            },
            signatureInfo: {
                signatureType: this.selectedSignatureType
            }
        }
        if (this.selectedSignatureType === SignatureType.DsEuAdvancedProviderName) {
            envelopeData.signatureInfo.accessCode = this.employmentContractForm.value.accessCode
        }
        this.useCasesService.createEmploymentContract(envelopeData).subscribe({
            next: (payload) => {
                this.executingAction = false
                window.location.href = payload.redirectUrl
            },
            error: (error: any) => {
                if (error.error) {
                    this.toastr.error(`Please try again: ${error.error.Message}`, 'Something went wrong', {
                        toastClass: 'error-message ngx-toastr custom-toastr',
                        tapToDismiss: true
                    })
                }
                this.executingAction = false
            }
        })
    }

    navigateHome() {
        this.router.navigate(['/'])
    }

    copyToClipboard(accessCode) {
        if (!accessCode.value) {
            return
        }
        navigator.clipboard.writeText(accessCode.value)
    }
}

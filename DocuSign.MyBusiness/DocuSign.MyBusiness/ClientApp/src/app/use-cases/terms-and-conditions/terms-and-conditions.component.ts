import { Component, OnInit } from '@angular/core'
import { FormControl, Validators, FormGroup, AbstractControl } from '@angular/forms'
import { SettingsService } from '../../shared/services/settings.service'
import { forkJoin } from 'rxjs'
import { Router } from '@angular/router'
import { ToastrService } from 'ngx-toastr'
import { FormService } from './../../shared/services/form.service'
import { HelperService } from './../../shared/services/helper.service'
import { UseCasesService } from './../shared/use-cases.service'
import { TermsAndConditionsWithConractEnvelopModel } from './../shared/use-cases.model'
import { EMAIL_REGEX } from './../../shared/constants/constants'
import { UseCaseNames } from './../../shared/models/shared.model'

@Component({
    selector: 'app-terms-and-conditions',
    templateUrl: './terms-and-conditions.component.html'
})
export class TermsAndConditionsComponent implements OnInit {
    templates = []
    signatureTypes = []
    selectedTemplate: string
    selectedSignatureType: string
    executingAction = false
    termsAndConditionsForm: FormGroup = new FormGroup({
        firstName: new FormControl('', Validators.required),
        lastName: new FormControl('', Validators.required),
        email: new FormControl('', [Validators.required, Validators.pattern(EMAIL_REGEX)]),
        fullNameCc: new FormControl('', Validators.required),
        emailCc: new FormControl('', Validators.required),
        accessCode: new FormControl('', Validators.required)
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
            datasource: this.settingsService.getDatasource(),
            connectionStatus: this.settingsService.getConnectionStatus()
        }).subscribe({
            next: (response) => {
                this.templates = response.datasource.templates
                this.signatureTypes = response.datasource.signatureTypes
                this.selectedTemplate = response.settings.template
                this.selectedSignatureType = response.settings.signatureType
                this.termsAndConditionsForm.patchValue({
                    emailCc: response.connectionStatus?.connectedUser?.email,
                    fullNameCc: response.connectionStatus?.connectedUser?.name
                })
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

        this.termsAndConditionsForm.patchValue({
            accessCode: this.helperService.generateRandomNumber()
        })
    }

    selectTemplate(value: string) {
        this.selectedTemplate = value
    }

    selectSignatureType(value: string) {
        this.selectedSignatureType = value
    }

    isInvalid(control: AbstractControl): boolean {
        return this.formService.isInvalid(control)
    }

    sendNow() {
        this.executingAction = true
        const termsAndConditionsData: TermsAndConditionsWithConractEnvelopModel = {
            signerInfo: {
                firstName: this.termsAndConditionsForm.value.firstName,
                lastName: this.termsAndConditionsForm.value.lastName,
                email: this.termsAndConditionsForm.value.email,
                accessCode: this.termsAndConditionsForm.value.accessCode
            },
            carbonCopyInfo: {
                fullName: this.termsAndConditionsForm.value.fullNameCc,
                email: this.termsAndConditionsForm.value.emailCc
            }
        }

        this.useCasesService.sendTermsAndConditions(termsAndConditionsData).subscribe({
            next: () => {
                this.toastr.success(
                    '<div class="toast-title my-2">Envelope is created</div><div>Contract was sent for signing</div>',
                    UseCaseNames['tearms-and-conditions'],
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
            error: (error) => {
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

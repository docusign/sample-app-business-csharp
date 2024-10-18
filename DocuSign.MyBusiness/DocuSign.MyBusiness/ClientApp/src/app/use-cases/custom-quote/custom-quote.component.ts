import { Component, OnInit } from '@angular/core'
import { FormControl, Validators, FormGroup, AbstractControl } from '@angular/forms'
import { Router } from '@angular/router'
import { ToastrService } from 'ngx-toastr'
import { FormService } from './../../shared/services/form.service'
import { UseCasesService } from './../shared/use-cases.service'
import { HelperService } from './../../shared/services/helper.service'
import { CustomQuoteInfo } from './../shared/use-cases.model'
import { UseCaseNames } from './../../shared/models/shared.model'
import { PHONE_REGEX } from './../../shared/constants/constants'

@Component({
    selector: 'app-custom-quote',
    templateUrl: './custom-quote.component.html'
})
export class CustomQuoteComponent implements OnInit {
    executingAction = false
    isConnected = false
    customQuoteForm: FormGroup = new FormGroup({
        firstName: new FormControl('', Validators.required),
        lastName: new FormControl('', Validators.required),
        countryCode: new FormControl(null, Validators.required),
        phoneNumber: new FormControl('', [Validators.required, Validators.pattern(PHONE_REGEX)])
    })

    countryList = this.helperService.getCountriesList()

    constructor(
        private router: Router,
        private formService: FormService,
        private toastr: ToastrService,
        private useCasesService: UseCasesService,
        private helperService: HelperService
    ) {}

    ngOnInit(): void {}

    isInvalid(control: AbstractControl): boolean {
        return this.formService.isInvalid(control)
    }

    sendNow() {
        this.executingAction = true
        const customQuoteInfo: CustomQuoteInfo = {
            firstName: this.customQuoteForm.value.firstName,
            lastName: this.customQuoteForm.value.lastName,
            countryCode: this.customQuoteForm.value.countryCode,
            phoneNumber: this.customQuoteForm.value.phoneNumber
        }
        this.useCasesService.sendCustomQuoteInfo(customQuoteInfo).subscribe({
            next: () => {
                this.toastr.success(
                    '<div class="toast-title my-2">Envelope is created</div><div>Document was sent for signing</div>',
                    UseCaseNames['custom-quote'],
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

    navigateHome() {
        this.router.navigate(['/'])
    }
}

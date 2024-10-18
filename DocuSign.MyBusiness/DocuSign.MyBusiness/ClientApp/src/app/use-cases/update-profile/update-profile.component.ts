import { Component, OnInit } from '@angular/core'
import { FormControl, Validators, FormGroup, AbstractControl } from '@angular/forms'
import { Router } from '@angular/router'
import { ToastrService } from 'ngx-toastr'
import { FormService } from './../../shared/services/form.service'
import { UseCasesService } from './../shared/use-cases.service'
import { TranslateService } from '@ngx-translate/core'
import { AuthorizeProfileInfo, UpdateCustomerProfileModel } from './../shared/use-cases.model'

@Component({
    selector: 'app-update-profile',
    templateUrl: './update-profile.component.html'
})
export class UpdateProfileComponent implements OnInit {
    executingAction = false
    isConnected = false
    profileData = null
    updateProfileForm: FormGroup = new FormGroup({
        login: new FormControl('', Validators.required),
        password: new FormControl('', Validators.required)
    })

    constructor(
        private router: Router,
        private formService: FormService,
        private toastr: ToastrService,
        private translate: TranslateService,
        private useCasesService: UseCasesService
    ) {}

    ngOnInit(): void {}

    isInvalid(control: AbstractControl): boolean {
        return this.formService.isInvalid(control)
    }

    authorize() {
        this.executingAction = true
        const profileInfo: AuthorizeProfileInfo = {
            login: this.updateProfileForm.value.login,
            password: this.updateProfileForm.value.password
        }
        this.useCasesService.authorizeProfile(profileInfo).subscribe({
            next: () => {
                this.isConnected = true
                this.executingAction = false
                this.getProfileData()
            },
            error: (error: any) => {
                Object.keys(error.error.errors).forEach((key) => {
                    this.updateProfileForm.controls[key].setErrors({ incorrect: true })
                })
                this.toastr.error('', this.translate.instant(`ErrorMessages.GeneralErrors.${error.error.generalErrorMessage}`), {
                    toastClass: 'error-message ngx-toastr custom-toastr no-description',
                    tapToDismiss: true
                })
                this.executingAction = false
            }
        })
    }

    getProfileData() {
        this.executingAction = true
        this.useCasesService.getProfile().subscribe({
            next: (data) => {
                this.profileData = data
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

    navigateHome() {
        this.router.navigate(['/'])
    }

    editProfile() {
        this.executingAction = true
        const profileInfo: UpdateCustomerProfileModel = {
            redirectUrl: ''
        }
        this.useCasesService.editProfile(profileInfo).subscribe({
            next: (payload) => {
                if (payload.redirectUrl != null && payload.redirectUrl !== '') {
                    window.location.href = payload.redirectUrl
                }
                this.executingAction = false
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
}

import { Component, OnInit } from '@angular/core'
import { FormControl, Validators, FormGroup, AbstractControl } from '@angular/forms'
import { Router, ActivatedRoute } from '@angular/router'
import { AdminService } from './shared/admin.service'
import { SettingsService } from './../shared/services/settings.service'
import { Settings, Authorize, ConsentType, AccountConnect, AuthenticationType, Account } from './shared/admin.model'
import { ToastrService } from 'ngx-toastr'
import { TranslateService } from '@ngx-translate/core'
import { FormService } from './../shared/services/form.service'
import { SignalrService } from './../shared/services/signalr.service'
import { HelperService } from './../shared/services/helper.service'
import { UpdateStatusService } from './../shared/services/updateStatus.service'
import { URL_REGEX, GUID_REGEX, PHONE_REGEX, EMAIL_REGEX } from './../shared/constants/constants'
import { debounceTime } from 'rxjs'

@Component({
    selector: 'app-admin',
    templateUrl: './admin.component.html'
})
export class AdminComponent implements OnInit {
    executingAction: boolean
    selectedAccountId = null
    connectedUser = {
        name: '',
        email: '',
        accountName: ''
    }

    accountsList = []

    connectionStatus = this.updateStatusService.getConnectionStatus()
    adminForm: FormGroup = new FormGroup({
        basePath: new FormControl('', [Validators.required, Validators.pattern(URL_REGEX)]),
        baseUri: new FormControl('', [Validators.required, Validators.pattern(URL_REGEX)]),
        accountId: new FormControl('', [Validators.required, Validators.pattern(GUID_REGEX)]),
        userId: new FormControl('', [Validators.required, Validators.pattern(GUID_REGEX)])
    })

    userProfileForm: FormGroup = new FormGroup({
        fullName: new FormControl('', Validators.required),
        email: new FormControl('', [Validators.required, Validators.pattern(EMAIL_REGEX)]),
        countryCode: new FormControl(null, Validators.required),
        phoneNumber: new FormControl('', [Validators.required, Validators.pattern(PHONE_REGEX)])
    })

    countryList = this.helperService.getCountriesList()
    authenticationType = AuthenticationType.UserAccount

    environmentTypes = [
        { name: 'demo', title: 'Demo', url: 'https://account-d.docusign.com' },
        { name: 'Production', title: 'Production', url: 'https://account.docusign.com' }
    ]

    initialUserId = null
    isFirstLoad = true

    constructor(
        private settingsService: SettingsService,
        private adminService: AdminService,
        private toastr: ToastrService,
        private router: Router,
        private route: ActivatedRoute,
        private translate: TranslateService,
        private formService: FormService,
        private signalrService: SignalrService,
        private updateStatusService: UpdateStatusService,
        private helperService: HelperService
    ) {}

    ngOnInit(): void {
        this.route.queryParams.subscribe((params) => {
            this.authenticationType = AuthenticationType[params.type]
            if (this.authenticationType === AuthenticationType.TestAccount) {
                this.connectTestAccount()
            }
        })
        this.getConnectionStatus()
        this.executingAction = true
        this.setFormValues()

        this.adminForm
            .get('userId')
            .valueChanges.pipe(debounceTime(500))
            .subscribe((val) => {
                if (val && (val !== this.initialUserId || this.isFirstLoad)) {
                    this.isFirstLoad = false
                    this.getAccounts(this.adminForm.value.basePath, val)
                }
            })
    }

    setFormValues() {
        this.settingsService.getSettings().subscribe({
            next: (response) => {
                this.adminForm.patchValue({
                    basePath: response.basePath,
                    baseUri: response.baseUri,
                    accountId: response.accountId,
                    userId: response.userId
                })
                this.userProfileForm.patchValue({
                    fullName: response.userProfile.fullName,
                    email: response.userProfile.email,
                    countryCode: response.userProfile.countryCode,
                    phoneNumber: response.userProfile.phoneNumber
                })
                this.initialUserId = this.adminForm.value.userId
                this.executingAction = false
            },
            error: () => {
                this.toastr.error('Please try again', 'Something went wrong', {
                    toastClass: 'error-message ngx-toastr custom-toastr',
                    timeOut: 0,
                    extendedTimeOut: 0,
                    tapToDismiss: true
                })
                this.executingAction = false
            }
        })
    }

    setBasePath(url: string) {
        this.adminForm.get('basePath').setValue(url)
    }

    saveSettings(): void {
        this.executingAction = true
        const settings: Settings = {
            basePath: this.adminForm.value.basePath,
            baseUri: this.adminForm.value.baseUri,
            accountId: this.adminForm.value.accountId,
            userId: this.adminForm.value.userId,
            userProfile: {
                fullName: `${this.userProfileForm.value.fullName}`,
                email: this.userProfileForm.value.email,
                countryCode: this.userProfileForm.value.countryCode,
                phoneNumber: this.userProfileForm.value.phoneNumber
            }
        }
        this.adminService.saveSettings(settings).subscribe({
            next: () => {
                this.executingAction = false
                this.router.navigate(['/'])
            },
            error: (error: any) => {
                this.toastr.error(`Please try again: ${error.error.Message}`, 'Something went wrong', {
                    toastClass: 'error-message ngx-toastr custom-toastr',
                    tapToDismiss: true
                })
                this.executingAction = false
            }
        })
    }

    authorizeAsAdmin(): void {
        this.executingAction = true
        const authorize: Authorize = {
            basePath: this.adminForm.value.basePath,
            consentType: ConsentType.Admin,
            redirectUrl: 'admin'
        }
        this.adminService.authorize(authorize).subscribe((payload) => {
            if (payload.redirectUrl != null && payload.redirectUrl !== '') {
                window.location.href = payload.redirectUrl
            }
            this.executingAction = false
        })
    }

    authorizeAsIndividual(): void {
        this.executingAction = true
        const authorize: Authorize = {
            basePath: this.adminForm.value.basePath,
            consentType: ConsentType.Individual,
            redirectUrl: 'admin'
        }
        this.adminService.authorize(authorize).subscribe((payload) => {
            if (payload.redirectUrl != null && payload.redirectUrl !== '') {
                window.location.href = payload.redirectUrl
            }
            this.executingAction = false
        })
    }

    unauthorize(): void {
        this.executingAction = true
        this.adminService.unauthorize().subscribe({
            next: () => {
                this.executingAction = false
                this.getConnectionStatus()
            },
            error: (error: any) => {
                this.toastr.error(`Please try again: ${error.error.Message}`, 'Something went wrong', {
                    toastClass: 'error-message ngx-toastr custom-toastr',
                    tapToDismiss: true
                })
                this.executingAction = false
            }
        })
    }

    connectTestAccount(): void {
        this.adminService.connect({ authenticationType: AuthenticationType.TestAccount }).subscribe({
            next: () => {
                this.signalrService.connection.start().catch((error: any) => {
                    console.log(`Signalr error: ${error}`)
                })

                this.setFormValues()
                this.getConnectionStatus()
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

    connect(): void {
        this.executingAction = true
        const accontConnect: AccountConnect = {
            authenticationType: AuthenticationType.UserAccount,
            basePath: this.adminForm.value.basePath,
            baseUri: this.adminForm.value.baseUri,
            accountId: this.adminForm.value.accountId,
            userId: this.adminForm.value.userId
        }
        this.adminService.connect(accontConnect).subscribe({
            next: () => {
                this.signalrService.connection.start().catch((error: any) => {
                    console.log(`Signalr error: ${error}`)
                })

                this.setFormValues()
                this.getConnectionStatus()
                this.executingAction = false
            },
            error: (error: any) => {
                if (error.error?.errors) {
                    Object.keys(error.error.errors).forEach((key) => {
                        this.adminForm.controls[key].setErrors({ incorrect: true })
                    })
                    this.toastr.error('', this.translate.instant(`ErrorMessages.GeneralErrors.${error.error.generalErrorMessage}`), {
                        toastClass: 'error-message ngx-toastr custom-toastr no-description',
                        tapToDismiss: true
                    })
                } else {
                    this.toastr.error(`Please try again: ${error.error?.Message}`, 'Something went wrong', {
                        toastClass: 'error-message ngx-toastr custom-toastr',
                        tapToDismiss: true
                    })
                }
                this.executingAction = false
            }
        })
    }

    disconnect(): void {
        this.executingAction = true
        this.adminService.disconnect().subscribe({
            next: () => {
                if (this.signalrService.connection.isConnected) {
                    this.signalrService.connection.stop()
                }
                this.getConnectionStatus()
                this.setFormValues()
                this.executingAction = false
            },
            error: (error: any) => {
                this.toastr.error(`Please try again: ${error.error?.Message}`, 'Something went wrong', {
                    toastClass: 'error-message ngx-toastr custom-toastr',
                    tapToDismiss: true
                })
                this.executingAction = false
            }
        })
    }

    isInvalid(control: AbstractControl): boolean {
        return this.formService.isInvalid(control)
    }

    getConnectionStatus(): void {
        this.settingsService.getConnectionStatus().subscribe((payload) => {
            this.connectedUser = payload.connectedUser
            this.updateStatusService.updateConnectionStatus({
                isConnected: payload.isConnected,
                isConsentGranted: payload.isConsentGranted
            })
        })
    }

    selectAccount(value: string) {
        const selectedAccount = this.accountsList.find((account: Account) => account.accountId === value)
        this.adminForm.patchValue({
            baseUri: selectedAccount.baseUri,
            accountId: selectedAccount.accountId
        })
        if (!this.adminForm.value.userId) {
            this.adminForm.patchValue({
                userId: this.initialUserId
            })
        }
        this.selectedAccountId = value
    }

    getAccounts(basePath: string, userId: string) {
        this.executingAction = true
        this.settingsService.getAccounts(basePath, userId).subscribe({
            next: (payload: Account[]) => {
                this.accountsList = payload
                const defaultAccount = payload.find((account: Account) => account.isDefault)
                if (defaultAccount) {
                    this.adminForm.patchValue({
                        baseUri: defaultAccount.baseUri,
                        accountId: defaultAccount.accountId
                    })
                    this.selectedAccountId = defaultAccount.accountId
                }
                this.initialUserId = userId
                this.executingAction = false
            },
            error: (error: any) => {
                this.toastr.error(`Please try again: ${error.error}`, 'Something went wrong', {
                    toastClass: 'error-message ngx-toastr custom-toastr',
                    tapToDismiss: true
                })
                this.adminForm.patchValue({
                    userId: this.initialUserId
                })
                this.executingAction = false
            }
        })
    }
}

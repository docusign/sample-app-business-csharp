export enum ConsentType {
    Individual = 'Individual',
    Admin = 'Admin'
}

export enum AuthenticationType {
    UserAccount = 'UserAccount',
    TestAccount = 'TestAccount'
}

export interface AccountConnect {
    authenticationType: AuthenticationType
    basePath?: string
    baseUri?: string
    accountId?: number
    userId?: number
}

export interface UserProfile {
    fullName: string
    email: string
    countryCode: string
    phoneNumber: string
}

export interface Settings {
    basePath: string
    baseUri: string
    accountId: number
    userId: number
    template?: string
    signatureType?: string
    userProfile?: UserProfile
}

export interface Authorize {
    basePath: string
    redirectUrl: string
    consentType: ConsentType
}

export interface Account {
    baseUri: string
    accountId: string
    accountName: string
    isDefault: boolean
}

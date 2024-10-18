export enum EnvelopeAction {
    Send = 'Send',
    ReviewAndSend = 'ReviewAndSend'
}

export enum SignatureType {
    DsEmail = 'ds_email',
    DsElectronicProviderName = 'universalsignaturepen_imageonly',
    DsEuAdvancedProviderName = 'universalsignaturepen_opentrust_hash_tsp'
}

export interface SignerInfo {
    firstName: string
    lastName: string
    email: string
    accessCode?: string
}

export interface CarbonCopyInfo {
    fullName: string
    email: string
}

export interface SignatureInfo {
    signatureType: string
    accessCode?: string
}

export interface EmploymentContractEnvelopeModel {
    envelopeAction: EnvelopeAction
    template: string
    redirectUrl: string
    signerInfo: SignerInfo
    signatureInfo: SignatureInfo
}

export interface TermsAndConditionsWithConractEnvelopModel {
    signerInfo: SignerInfo
    carbonCopyInfo: CarbonCopyInfo
}

export interface AuthorizeProfileInfo {
    login: string
    password: string
}

export interface CustomQuoteInfo {
    firstName: string
    lastName: string
    countryCode: string
    phoneNumber: string
}

export interface UpdateCustomerProfileModel {
    redirectUrl: string
}

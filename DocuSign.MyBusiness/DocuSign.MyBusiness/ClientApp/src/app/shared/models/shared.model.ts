export interface SignerInfo {
    email: string
    name: string
    tabs: [{ key: string; value: string }]
}

export interface NotificationInfo {
    date: string
    event: string
    signer: SignerInfo
    useCase: string
}

export enum UseCaseNames {
    'employment-contract' = 'Send an Employment Contract',
    'tearms-and-conditions' = 'T&Cs and Contract',
    'update-profile' = 'Update Customer Profile Self-service',
    'custom-quote' = 'Custom Quote'
}

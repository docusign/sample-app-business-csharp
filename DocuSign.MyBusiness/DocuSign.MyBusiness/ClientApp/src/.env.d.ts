declare interface Env {
    readonly NODE_ENV: string
    readonly NG_APP_PARTNER_IK: string
}

declare interface ImportMeta {
    readonly env: Env
}

window.DSFreeTrial = {
    startFreeTrialCreation: ({ partnerIK, loginRedirectUri }) => {
        console.log({ partnerIK, loginRedirectUri })
        const url = `https://account-d.docusign.com/oauth/auth?client_id=${partnerIK}&redirect_uri=${loginRedirectUri}&prompt=login`
        const w = window.open(url, '_blank', 'width=600,height=400,left=500,top=300')
        const stateInterval = setInterval(() => {
            try {
                if (w.closed) {
                    clearInterval(stateInterval)
                    return
                }
                if (w.location.hostname === window.location.hostname) {
                    clearInterval(stateInterval)
                    w.close()
                    window.location = loginRedirectUri
                }
            } catch {
                /* empty */
            }
        }, 100)
    }
}


export const environment = {
    production: true,
    testManagerApiBaseUrl: "https://dev.test-manager-api.testmaster.io",
    accountManagerApiBaseUrl: "https://dev.account-manager.testmaster.io",
    testRunnerBaseUrl: "https://dev.test-runner.testmaster.io",
    editorApiKey: "x0yf00jatpue54s2pib29qju4ql049rbbv602narz7nfx4p2",
    uploadPubicKey: "8a991600a5b7c5405c5a",
    auth: {
        domain: "dev-kz3mgkb4xl50tobe.us.auth0.com",
        clientId: "SaMKFPQSdPoAyooI3zF6oiz6zdZ6hU18",
        audience: "http://test-manager",
        scope: "read:current_user",
        intercepters: [
            "https://dev.test-manager-api.testmaster.io/*",
            "https://dev.account-manager.testmaster.io/*"
        ]
    }
}
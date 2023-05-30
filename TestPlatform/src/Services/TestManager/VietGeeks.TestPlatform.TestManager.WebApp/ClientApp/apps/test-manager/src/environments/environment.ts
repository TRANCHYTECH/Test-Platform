
export const environment = {
    production: false,
    testManagerApiBaseUrl: "https://localhost:6200",
    accountManagerApiBaseUrl: "https://localhost:6500",
    testRunnerBaseUrl: "http://localhost:4300",
    editorApiKey: "x0yf00jatpue54s2pib29qju4ql049rbbv602narz7nfx4p2",
    uploadPubicKey: "8a991600a5b7c5405c5a",
    auth: {
        domain: 'dev-kz3mgkb4xl50tobe.us.auth0.com',
        clientId: 'SaMKFPQSdPoAyooI3zF6oiz6zdZ6hU18',
        audience:  'http://test-manager',
        scope: 'read:current_user',
        intercepters: ['https://localhost:6200/*','https://localhost:6500/*','https://dev.account-manager.testmaster.io/*']
    }
}
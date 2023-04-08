
export const environment = {
    production: true,
    testManagerApiBaseUrl: "https://dev.api.testmaster.io",
    testRunnerBaseUrl: "",
    auth: {
        domain: 'dev-kz3mgkb4xl50tobe.us.auth0.com',
        clientId: 'SaMKFPQSdPoAyooI3zF6oiz6zdZ6hU18',
        audience:  'http://test-manager',
        scope: 'read:current_user',
        intercepters: ['https://dev.api.testmaster.io/*']
    }
}
import NextAuth, { NextAuthOptions } from "next-auth"
import Auth0Provider from "next-auth/providers/auth0"

// For more information on each option (and a full list of options) go to
// https://next-auth.js.org/configuration/options
export const authOptions: NextAuthOptions = {
  // https://next-auth.js.org/configuration/providers/oauth
  providers: [
    Auth0Provider({
      clientId: process.env.AUTH0_ID ?? '',
      clientSecret: process.env.AUTH0_SECRET ?? '',
      issuer: process.env.AUTH0_ISSUER,
      idToken: true,
      authorization: {
        params: {
          audience: encodeURI('http://test-manager')
        }
      },
    })
  ],
  callbacks: {
    async jwt({ token, account, profile }) {
      if (account) {
        token.idToken = account.id_token;
        token.accessToken = account?.access_token;
      }
      console.log('token', token);
      return token;
    },
    async session({ session, token, user }) {
      // Send properties to the client, like an access_token and user id from a provider.
      session.accessToken = token.accessToken;
      console.debug('session', session);
      return session;
    }
  },
}

const handler = NextAuth(authOptions);

export { handler as GET, handler as POST }

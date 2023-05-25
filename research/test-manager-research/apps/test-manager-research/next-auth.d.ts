import "next-auth/jwt"
import { DefaultSession } from "next-auth"

// Read more at: https://next-auth.js.org/getting-started/typescript#module-augmentation

declare module "next-auth/jwt" {
  interface JWT {
    /** The user's role. */
    userRole?: "admin",
    idToken?: string,
    accessToken?: string
  }
}

declare module "next-auth" {
  interface Session {
    user: object & DefaultSession['user'],
    accessToken?: string
  }
}
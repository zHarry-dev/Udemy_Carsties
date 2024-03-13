import { NextAuthOptions } from "next-auth"
import DuendeIdentityServer6 from "next-auth/providers/duende-identity-server6"

export const authOptions: NextAuthOptions = {
    session: {
        strategy: 'jwt'
    },
    providers: [
        DuendeIdentityServer6({
            id: 'id-server',
            clientId: 'nextApp',
            clientSecret: 'secret',
            issuer: process.env.ID_URL,
            authorization: { params: { scope: 'openid profile auctionApp' } },
            idToken: true
        })
    ],
    callbacks: {
        async jwt({ token, profile, account, user }) {
            if (profile) {
                token.username = profile.username
            }
            if (account) {
                token.accessToken = account.access_token
            }
            return token
        },
        async session({ session, token }) {
            if (token) {
                session.user.username = token.username
            }
            return session;
        }
    }
}
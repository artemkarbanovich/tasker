export interface Account {
    email: string;
    username: string;
    accessToken: string;
    refreshToken: string;
    role: string;
    accessTokenExpiryTime: Date;
}

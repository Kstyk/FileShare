import { authStateType } from './auth.reducer';

export const selectAuth = (state: { auth: authStateType }) => state.auth;

import { ActionReducerMap } from '@ngrx/store';
import { authReducer, authStateType } from '../auth/store/auth.reducer';

export type AppStateType = { auth: authStateType };

export const appReducer: ActionReducerMap<AppStateType> = {
  auth: authReducer,
};

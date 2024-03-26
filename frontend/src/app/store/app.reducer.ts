import { ActionReducerMap } from '@ngrx/store';
import { authReducer, authStateType } from '../auth/store/auth.reducer';
import { filesReducer, filesStateType } from '../files/store/files.reducer';

export type AppStateType = { auth: authStateType; files: filesStateType };

export const appReducer: ActionReducerMap<AppStateType> = {
  auth: authReducer,
  files: filesReducer,
};

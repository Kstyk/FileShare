import { createAction, props } from '@ngrx/store';

export const LOGIN_START = '[Auth] Login Start';
export const AUTHENTICATE_SUCCESS = '[Auth] Login';
export const AUTHENTICATE_FAIL = '[Auth] Login Fail';
export const CLEAR_ERROR = '[Auth] Clear Error';
export const LOGOUT = '[Auth] Logout';
export const AUTO_LOGIN = '[Auth] Auto Login';

export const loginStart = createAction(
  LOGIN_START,
  props<{ payload: { email: string; password: string } }>()
);

export const authenticatesuccess = createAction(
  AUTHENTICATE_SUCCESS,
  props<{
    payload: {
      firstName: string;
      lastName: string;
      email: string;
      userId: string;
      token: string;
      tokenExpirationDate: Date;
      redirect: boolean;
    };
  }>()
);

export const authenticateFail = createAction(
  AUTHENTICATE_FAIL,
  props<{ payload: string }>()
);
export const clearError = createAction(CLEAR_ERROR);
export const logout = createAction(LOGOUT);
export const autoLogin = createAction(AUTO_LOGIN);

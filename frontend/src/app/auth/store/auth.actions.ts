import { createAction, props } from '@ngrx/store';
import { RegisterValidationError } from '../../../models/register-validation-errors.model';

export const LOGIN_START = '[Auth] Login Start';
export const AUTHENTICATE_SUCCESS = '[Auth] Login';
export const AUTHENTICATE_FAIL = '[Auth] Login Fail';
export const CLEAR_ERROR = '[Auth] Clear Error';
export const LOGOUT = '[Auth] Logout';
export const AUTO_LOGIN = '[Auth] Auto Login';
export const SIGNUP_START = '[Auth] Signup Start';
export const REFRESH_TOKEN = '[Auth] Refresh Token';

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
      refreshToken: string;
    };
  }>()
);

export const authenticateFail = createAction(
  AUTHENTICATE_FAIL,
  props<{
    payload: {
      registerValidationError: RegisterValidationError;
      authError: string;
    };
  }>()
);
export const clearError = createAction(CLEAR_ERROR);
export const logout = createAction(LOGOUT);
export const autoLogin = createAction(AUTO_LOGIN);
export const signupStart = createAction(
  SIGNUP_START,
  props<{
    payload: {
      firstName: string;
      lastName: string;
      email: string;
      password: string;
      confirmPassword: string;
    };
  }>()
);
export const refreshToken = createAction(
  REFRESH_TOKEN,
  props<{ payload: { refreshToken: string } }>()
);

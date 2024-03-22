import { createReducer, on } from '@ngrx/store';
import { User } from '../../../models/user.model';
import {
  authenticateFail,
  authenticatesuccess,
  clearError,
  loginStart,
  logout,
  signupStart,
} from './auth.actions';
import { RegisterValidationError } from '../../../models/register-validation-errors.model';

export type authStateType = {
  user: User; // user can be null
  authError: string;
  loading: boolean;
  registerValidationError: RegisterValidationError;
};

const initialState: authStateType = {
  user: null,
  authError: null,
  loading: false,
  registerValidationError: null,
};

export const authReducer = createReducer(
  initialState,
  on(authenticatesuccess, (state, action) => {
    const user = new User(
      action.payload.firstName,
      action.payload.lastName,
      action.payload.email,
      action.payload.userId,
      action.payload.token,
      action.payload.tokenExpirationDate,
      action.payload.refreshToken
    );

    return {
      ...state,
      authError: null,
      user: user,
      loading: false,
    };
  }),
  on(authenticateFail, (state, action) => {
    return {
      ...state,
      authError: action.payload.authError,
      user: null,
      loading: false,
      registerValidationError: action.payload.registerValidationError,
    };
  }),
  on(loginStart, (state) => {
    return { ...state, authError: null, loading: true };
  }),
  on(clearError, (state) => {
    return { ...state, authError: null };
  }),
  on(logout, (state) => {
    return { ...state, user: null };
  }),
  on(signupStart, (state) => {
    return { ...state, authError: null, loading: true };
  })
);

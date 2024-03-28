import { catchError, map, of, switchMap, tap } from 'rxjs';
import { User } from '../models/user.model';
import {
  authenticateFail,
  authenticatesuccess,
  autoLogin,
  loginStart,
  logout,
  refreshToken,
  signupStart,
} from './auth.actions';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { RefreshTokenService } from '../refresh-token.service';

export type AuthResponseData = {
  accessToken: string;
  refreshToken: string;
};

export type TokenDecodedType = {
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier': string;
  Id: string;
  Email: string;
  FirstName: string;
  LastName: string;
  exp: number;
  iss: string;
  aud: string;
};

const handleAuthentication = (token: string, refreshToken: string) => {
  // decoding token
  const decodedToken: TokenDecodedType = jwtDecode(token);

  const expirationDate = new Date(+decodedToken.exp * 1000);

  const user = new User(
    decodedToken.FirstName,
    decodedToken.LastName,
    decodedToken.Email,
    decodedToken.Id,
    token,
    expirationDate,
    refreshToken
  );
  localStorage.setItem('userData', JSON.stringify(user));
  return authenticatesuccess({
    payload: {
      firstName: decodedToken.FirstName,
      lastName: decodedToken.LastName,
      email: decodedToken.Email,
      userId: decodedToken.Id,
      token: token,
      tokenExpirationDate: expirationDate,
      redirect: true,
      refreshToken: refreshToken,
    },
  });
};

const handleError = (errorRes: any) => {
  let errorMessage = 'An unknown error occurred!';

  if (!errorRes.error && !errorRes.error.error) {
    return of(
      authenticateFail({
        payload: { authError: errorMessage, registerValidationError: null },
      })
    );
  }
  switch (errorRes.status) {
    case 401:
      errorMessage = errorRes.error;
      break;
    case 400:
      if (errorRes.error.errors) {
        return of(
          authenticateFail({
            payload: {
              registerValidationError: errorRes.error.errors,
              authError: null,
            },
          })
        );
      }
      break;
    default:
      break;
  }

  return of(
    authenticateFail({
      payload: { authError: errorMessage, registerValidationError: null },
    })
  );
};

@Injectable()
export class AuthEffects {
  constructor(
    private http: HttpClient,
    private actions$: Actions,
    private router: Router,
    private refreshTokenService: RefreshTokenService
  ) {}

  authLogin = createEffect(() =>
    this.actions$.pipe(
      ofType(loginStart),

      switchMap((authData) => {
        return this.http
          .post<AuthResponseData>('https://localhost:7041/api/account/login', {
            email: authData.payload.email,
            password: authData.payload.password,
          })
          .pipe(
            tap((resData) => {
              const expiresIn = this.calculateExpiresIn(resData.accessToken);
              this.refreshTokenService.setRefreshTokenTimer(
                +expiresIn * 1000,
                resData.refreshToken
              );
            }),
            map((resData) => {
              console.log(resData);
              return handleAuthentication(
                resData.accessToken,
                resData.refreshToken
              );
            }),
            catchError((errorRes) => {
              return handleError(errorRes);
            })
          );
      })
    )
  );

  authSignup = createEffect(() =>
    this.actions$.pipe(
      ofType(signupStart),
      switchMap((authData) => {
        return this.http
          .post('https://localhost:7041/api/account/register', {
            email: authData.payload.email,
            password: authData.payload.password,
            confirmPassword: authData.payload.confirmPassword,
            firstName: authData.payload.firstName,
            lastName: authData.payload.lastName,
          })
          .pipe(
            map((resData) => {
              this.router.navigate(['/auth']);

              return { type: '[Auth] Signup Success' }; // Add this line to return an action
            }),
            catchError((errorRes) => {
              return handleError(errorRes);
            })
          );
      })
    )
  );

  refreshToken = createEffect(() => {
    return this.actions$.pipe(
      ofType(refreshToken),
      switchMap((authData) => {
        console.log('here');
        return this.http
          .post<{ accessToken: string }>(
            'https://localhost:7041/api/account/refresh-token',
            {
              refreshToken: authData.payload.refreshToken,
            }
          )
          .pipe(
            tap((resData) => {
              if (resData) {
                const expiresIn = this.calculateExpiresIn(resData.accessToken);
                this.refreshTokenService.setRefreshTokenTimer(
                  +expiresIn * 1000,
                  authData.payload.refreshToken
                );
              }
            }),
            map((resData) => {
              return handleAuthentication(
                resData.accessToken,
                authData.payload.refreshToken
              );
            }),
            catchError((errorRes) => {
              console.log(errorRes);
              return handleError(errorRes);
            })
          );
      })
    );
  });

  authRedirect = createEffect(
    () =>
      this.actions$.pipe(
        ofType(authenticatesuccess, logout),
        ofType(authenticatesuccess),
        tap((authSuccessAction) => {
          if (authSuccessAction.payload.redirect) {
            this.router.navigate(['/']);
          }
        })
      ),
    { dispatch: false }
  );

  autoLogin = createEffect(() =>
    this.actions$.pipe(
      ofType(autoLogin),
      map(() => {
        const userData: {
          firstName: string;
          lastName: string;
          email: string;
          id: string;
          _token: string;
          _tokenExpirationDate: string;
          _refreshToken: string;
        } = JSON.parse(localStorage.getItem('userData'));
        if (!userData) {
          return { type: 'DUMMY' };
        }

        const loadedUser = new User(
          userData.firstName,
          userData.lastName,
          userData.email,
          userData.id,
          userData._token,
          new Date(userData._tokenExpirationDate),
          userData._refreshToken
        );

        if (loadedUser && loadedUser.token) {
          // this.user.next(loadedUser);
          const expirationDuration =
            new Date(userData._tokenExpirationDate).getTime() -
            new Date().getTime();
          this.refreshTokenService.setRefreshTokenTimer(
            expirationDuration,
            userData._refreshToken
          );
          return authenticatesuccess({
            payload: {
              firstName: loadedUser.firstName,
              lastName: loadedUser.lastName,
              email: loadedUser.email,
              userId: loadedUser.id,
              token: loadedUser.token,
              tokenExpirationDate: new Date(userData._tokenExpirationDate),
              redirect: false,
              refreshToken: loadedUser.refreshToken,
            },
          });
        }

        return { type: 'DUMMY' };
      })
    )
  );

  authLogout = createEffect(
    () =>
      this.actions$.pipe(
        ofType(logout),
        tap(() => {
          this.refreshTokenService.clearRefreshTokenTimer();
          localStorage.removeItem('userData');
          this.router.navigate(['/auth']);
        })
      ),
    { dispatch: false }
  );

  private calculateExpiresIn = (accessToken: string) => {
    const decodedToken: TokenDecodedType = jwtDecode(accessToken);
    const expirationTimeInSeconds = decodedToken.exp;

    // Calculate the remaining time until the token expires
    const currentTimeInSeconds = Math.floor(Date.now() / 1000);
    const expiresIn = expirationTimeInSeconds - currentTimeInSeconds;

    return expiresIn;
  };
}

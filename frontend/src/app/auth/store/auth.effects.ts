import { catchError, map, of, switchMap, tap } from 'rxjs';
import { User } from '../../../models/user.model';
import {
  authenticateFail,
  authenticatesuccess,
  autoLogin,
  loginStart,
  logout,
} from './auth.actions';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { AutoLogoutService } from '../auto-logout.service';

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
  console.log('Treść błędu:');
  console.log(errorRes);

  if (!errorRes.error && !errorRes.error.error) {
    return of(authenticateFail({ payload: errorMessage }));
  }
  switch (errorRes.status) {
    case 401:
      errorMessage = errorRes.error;
      break;
  }

  return of(authenticateFail({ payload: errorMessage }));
};

@Injectable()
export class AuthEffects {
  constructor(
    private http: HttpClient,
    private actions$: Actions,
    private router: Router,
    private autoLogoutService: AutoLogoutService
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
              this.autoLogoutService.setLogoutTimer(+expiresIn * 1000);
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

        console.log('here');
        console.log(loadedUser);

        if (loadedUser.token) {
          // this.user.next(loadedUser);
          const expirationDuration =
            new Date(userData._tokenExpirationDate).getTime() -
            new Date().getTime();
          this.autoLogoutService.setLogoutTimer(expirationDuration);
          return authenticatesuccess({
            payload: {
              firstName: loadedUser.firstName,
              lastName: loadedUser.lastName,
              email: loadedUser.email,
              userId: loadedUser.id,
              token: loadedUser.token,
              tokenExpirationDate: new Date(userData._tokenExpirationDate),
              redirect: false,
              refreshToken: userData._refreshToken,
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
          this.autoLogoutService.clearLogoutTimer();
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

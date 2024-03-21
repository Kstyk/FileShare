import { catchError, map, of, switchMap, tap } from 'rxjs';
import { User } from '../../../models/user.model';
import {
  authenticateFail,
  authenticatesuccess,
  loginStart,
} from './auth.actions';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';

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

const handleAuthentication = (token: string) => {
  // decoding token
  const decodedToken: TokenDecodedType = jwtDecode(token);

  const expirationDate = new Date(
    new Date().getTime() + +decodedToken.exp * 1000
  );
  const user = new User(
    decodedToken.FirstName,
    decodedToken.LastName,
    decodedToken.Email,
    decodedToken.Id,
    token,
    expirationDate
  );
  localStorage.setItem('userData', JSON.stringify(user));
  return authenticatesuccess({
    payload: {
      firstName: decodedToken.FirstName,
      lastName: decodedToken.LastName,
      email: decodedToken.Email,
      userId: decodedToken.Id,
      token: token,
      expirationDate: expirationDate,
      redirect: true,
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
    private action$: Actions,
    private router: Router
  ) {}

  authLogin = createEffect(() =>
    this.action$.pipe(
      ofType(loginStart),
      switchMap((authData) => {
        return this.http
          .post<AuthResponseData>('https://localhost:7041/api/account/login', {
            email: authData.payload.email,
            password: authData.payload.password,
          })
          .pipe(
            tap((resData) => {
              // this.authService.setLogoutTimer(+resData.expiresIn); // to see if it is working
              // this.authService.setLogoutTimer(+resData.expiresIn * 1000);
            }),
            map((resData) => {
              console.log(resData);
              return handleAuthentication(resData.accessToken);
            }),
            catchError((errorRes) => {
              return handleError(errorRes);
            })
          );
      })
    )
  );
}

import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpParams,
  HttpRequest,
} from '@angular/common/http';
import { Store } from '@ngrx/store';
import { AppStateType } from '../store/app.reducer';
import { Observable, exhaustMap, map, take } from 'rxjs';
import { selectAuth } from './store/auth.selector';
import { Injectable } from '@angular/core';

@Injectable()
export class AuthInterceptorService implements HttpInterceptor {
  constructor(private store: Store<AppStateType>) {}

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    return this.store.select(selectAuth).pipe(
      take(1),
      map((authState) => {
        return authState.user;
      }),
      exhaustMap((user) => {
        if (!user) {
          return next.handle(req);
        }
        const modifiedReq = req.clone({
          setHeaders: {
            Authorization: `Bearer ${user.token ? user.token : ''}`,
          },
        });
        return next.handle(modifiedReq);
      })
    );
  }
}

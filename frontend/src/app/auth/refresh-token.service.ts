import { Store } from '@ngrx/store';
import { AppStateType } from '../store/app.reducer';
import { Injectable } from '@angular/core';
import { refreshToken } from './store/auth.actions';

@Injectable({ providedIn: 'root' })
export class RefreshTokenService {
  private tokenExpirationTimer: any;

  constructor(private store: Store<AppStateType>) {}

  setRefreshTokenTimer(expirationDuration: number, token: string) {
    this.tokenExpirationTimer = setTimeout(() => {
      this.store.dispatch(refreshToken({ payload: { refreshToken: token } }));
    }, expirationDuration);
  }

  clearRefreshTokenTimer() {
    if (this.tokenExpirationTimer) {
      clearTimeout(this.tokenExpirationTimer);
      this.tokenExpirationTimer = null;
    }
  }
}

import { Store } from '@ngrx/store';
import { AppStateType } from '../store/app.reducer';
import { Injectable } from '@angular/core';
import { logout } from './store/auth.actions';

@Injectable({ providedIn: 'root' })
export class AutoLogoutService {
  private tokenExpirationTimer: any;

  constructor(private store: Store<AppStateType>) {}

  setLogoutTimer(expirationDuration: number) {
    this.tokenExpirationTimer = setTimeout(() => {
      this.store.dispatch(logout());
    }, expirationDuration);
  }

  clearLogoutTimer() {
    if (this.tokenExpirationTimer) {
      clearTimeout(this.tokenExpirationTimer);
      this.tokenExpirationTimer = null;
    }
  }
}

import { Component, OnDestroy, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Subscription, map } from 'rxjs';
import { AppStateType } from '../store/app.reducer';
import { selectAuth } from '../auth/store/auth.selector';
import { logout } from '../auth/store/auth.actions';
import { User } from '../auth/models/user.model';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent implements OnInit, OnDestroy {
  isAuthenticated = false;
  user: User = null;
  private userSub: Subscription;

  constructor(private store: Store<AppStateType>) {}

  ngOnInit(): void {
    this.userSub = this.store
      .select(selectAuth)
      .pipe(map((authState) => authState.user))
      .subscribe((user) => {
        this.isAuthenticated = !!user;

        if (!!user) {
          this.user = user;
        }
      });
  }

  onLogout() {
    this.user = null;
    this.store.dispatch(logout());
  }

  ngOnDestroy(): void {
    this.userSub.unsubscribe();
  }
}

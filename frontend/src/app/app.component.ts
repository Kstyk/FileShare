import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { AppStateType } from './store/app.reducer';
import { autoLogin } from './auth/store/auth.actions';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit {
  title = 'frontend';

  constructor(private store: Store<AppStateType>) {}

  ngOnInit(): void {
    this.store.dispatch(autoLogin());
  }
}

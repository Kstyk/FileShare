import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AppStateType } from '../store/app.reducer';
import { Store } from '@ngrx/store';
import { Subscription, map } from 'rxjs';
import { selectAuth } from './store/auth.selector';
import { clearError, loginStart } from './store/auth.actions';
import { RegisterValidationError } from '../../models/register-validation-errors.model';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.scss',
})
export class AuthComponent implements OnDestroy, OnInit {
  error: string = null;

  loginForm: FormGroup;

  private storeSub: Subscription;

  constructor(private store: Store<AppStateType>) {}

  ngOnInit(): void {
    this.initForm();
    // How can i change that? I cannot login or register in the app when authstate is undefineed

    this.storeSub = this.store.select(selectAuth).subscribe((authState) => {
      console.log(authState);
      this.error = authState ? authState.authError : null;
    });
  }

  onSubmit() {
    console.log(this.loginForm.value);
    this.store.dispatch(
      loginStart({
        payload: {
          email: this.loginForm.value.email,
          password: this.loginForm.value.password,
        },
      })
    );

    this.loginForm.reset();
  }

  hideError() {
    this.store.dispatch(clearError());
  }

  private initForm() {
    let email = '';
    let password = '';

    this.loginForm = new FormGroup({
      email: new FormControl(email, [Validators.required, Validators.email]),
      password: new FormControl(password, [Validators.required]),
    });
  }

  ngOnDestroy(): void {
    this.storeSub.unsubscribe();
  }
}

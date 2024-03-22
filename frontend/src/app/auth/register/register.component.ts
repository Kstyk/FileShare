import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { Subscription } from 'rxjs';
import { AppStateType } from '../../store/app.reducer';
import { signupStart } from '../store/auth.actions';
import { RegisterValidationError } from '../../../models/register-validation-errors.model';
import { selectAuth } from '../store/auth.selector';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  errorValidation: RegisterValidationError;
  private storeSub: Subscription;

  ngOnInit() {
    this.storeSub = this.store.select(selectAuth).subscribe((authState) => {
      this.errorValidation = authState.registerValidationError;

      if (this.errorValidation) {
        console.log(this.errorValidation);
      }
    });
    this.initForm();
  }

  constructor(private store: Store<AppStateType>) {}

  hideError() {}

  onSubmit() {
    console.log(this.registerForm.value);
    this.store.dispatch(signupStart({ payload: this.registerForm.value }));
  }

  // Initialize registerForm based on register.component.html
  // Add two more fields firstName and lastName
  private initForm() {
    this.registerForm = new FormGroup({
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [
        Validators.required,
        // Validators.minLength(6),
      ]),
      confirmPassword: new FormControl('', [Validators.required]),
      firstName: new FormControl('', [Validators.required]),
      lastName: new FormControl('', [Validators.required]),
    });
  }
}

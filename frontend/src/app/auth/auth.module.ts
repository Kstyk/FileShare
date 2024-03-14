import { NgModule } from '@angular/core';
import { AuthComponent } from './auth.component';
import { RouterModule } from '@angular/router';
import { AuthRoutingModule } from './auth-routing.module';
import { RegisterComponent } from './register/register.component';

@NgModule({
  declarations: [AuthComponent, RegisterComponent],
  providers: [],
  imports: [RouterModule, AuthRoutingModule],
})
export class AuthModule {}

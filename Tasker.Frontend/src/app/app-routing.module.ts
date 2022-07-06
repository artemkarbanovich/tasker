import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './core/components/account/login/login.component';
import { RegistrationComponent } from './core/components/account/registration/registration.component';
import { HomeComponent } from './core/components/home/home.component';
import { NoAuthGuard } from './core/guards/no-auth.guard';

const routes: Routes = [
  { path: '', component: HomeComponent },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [NoAuthGuard],
    children: [
      { path: 'register', component: RegistrationComponent },
      { path: 'login', component: LoginComponent },
    ]
  },
  { path: '**', component: HomeComponent, pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './core/components/account/login/login.component';
import { RegistrationComponent } from './core/components/account/registration/registration.component';
import { HomeComponent } from './core/components/home/home.component';
import { ObjectiveCreateComponent } from './core/components/objective/objective-create/objective-create.component';
import { ObjectiveEditComponent } from './core/components/objective/objective-edit/objective-edit.component';
import { ObjectiveListComponent } from './core/components/objective/objective-list/objective-list.component';
import { NoAuthGuard } from './core/guards/no-auth.guard';
import { RoleGuard } from './core/guards/role.guard';

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
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [RoleGuard],
    data: { role: 'User' },
    children: [
      { path: 'objectives', component: ObjectiveListComponent },
      { path: 'objectives/create', component: ObjectiveCreateComponent },
      { path: 'objectives/:id', component: ObjectiveEditComponent }
    ]
  },
  { path: '**', component: HomeComponent, pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

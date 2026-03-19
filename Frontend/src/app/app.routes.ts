import { Routes } from '@angular/router';

import { DashboardComponent } from './pages/dashboard.component';
import { OrganizationDetailComponent } from './pages/organization-detail.component';
import { OrganizationsComponent } from './pages/organizations.component';
import { ProjectDetailComponent } from './pages/project-detail.component';
import { ProjectsComponent } from './pages/projects.component';

export const appRoutes: Routes = [
  { path: '', component: DashboardComponent },
  { path: 'organizations', component: OrganizationsComponent },
  { path: 'organizations/:id', component: OrganizationDetailComponent },
  { path: 'projects', component: ProjectsComponent },
  { path: 'projects/:id', component: ProjectDetailComponent },
  { path: '**', redirectTo: '' },
];

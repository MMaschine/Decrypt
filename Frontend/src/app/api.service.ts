import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { environment } from 'src/environment/environment';

export interface DashboardSummary {
  totalOrganizations: number;
  totalUsers: number;
  totalProjects: number;
  activeProjects: number;
  totalTimeEntries: number;
  totalInvoiced: number;
}

export interface Organization {
  id: string;
  name: string;
  industry: string;
  tier: 'enterprise' | 'professional' | 'starter';
  contactEmail: string;
}

export interface OrganizationSummary {
  organization: Organization;
  projectCount: number;
  userCount: number;
  totalInvoiced: number;
  currency: string;
}

export interface ProjectOrganization {
  id: string;
  name: string;
}

export interface Project {
  id: string;
  name: string;
  status: 'active' | 'completed' | 'draft';
  budgetHours: number;
  startDate: string | null;
  endDate: string | null;
  totalHoursLogged?: number;
  organization?: ProjectOrganization;
}

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;;

  getDashboard(): Observable<DashboardSummary> {
    return this.get<DashboardSummary>('/dashboard');
  }

  getOrganizations(params?: Record<string, string | number | boolean>): Observable<Organization[]> {
    return this.get<Organization[]>('/organizations', params);
  }

  getOrganization(id: string): Observable<Organization> {
    return this.get<Organization>(`/organizations/${id}`);
  }

  getOrganizationSummary(id: string): Observable<OrganizationSummary> {
    return this.get<OrganizationSummary>(`/organizations/${id}/summary`);
  }

  getUsers<T>(params?: Record<string, string | number | boolean>): Observable<T> {
    return this.get<T>('/users', params);
  }

  getUser<T>(id: string): Observable<T> {
    return this.get<T>(`/users/${id}`);
  }

  getProjects(params?: Record<string, string | number | boolean>): Observable<Project[]> {
    return this.get<Project[]>('/projects', params);
  }

  getProject(id: string): Observable<Project> {
    return this.get<Project>(`/projects/${id}`);
  }

  getTimeEntries<T>(params?: Record<string, string | number | boolean>): Observable<T> {
    return this.get<T>('/time-entries', params);
  }

  getInvoices<T>(params?: Record<string, string | number | boolean>): Observable<T> {
    return this.get<T>('/invoices', params);
  }

  private get<T>(path: string, params?: Record<string, string | number | boolean>): Observable<T> {
    let httpParams = new HttpParams();

    if (params) {
      Object.entries(params).forEach(([key, value]) => {
        httpParams = httpParams.set(key, String(value));
      });
    }

    return this.http.get<T>(`${this.baseUrl}${path}`, { params: httpParams }).pipe(
      catchError((error: HttpErrorResponse) => {
        const message =
          typeof error.error === 'string'
            ? error.error
            : error.error?.message || error.message || error.statusText || 'Request failed';

        return throwError(() => new Error(message));
      }),
    );
  }
}

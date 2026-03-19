import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';

import { ApiService, DashboardSummary } from '../api.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div *ngIf="error" class="card error">Error: {{ error }}</div>

    <div *ngIf="!error && !data" class="card loading">Loading dashboard...</div>

    <ng-container *ngIf="data as dashboard">
      <h1 style="margin-bottom: 1rem;">Dashboard</h1>
      <div class="grid grid-3">
        <div class="card">
          <h2>Organizations</h2>
          <p>{{ dashboard.totalOrganizations }}</p>
        </div>
        <div class="card">
          <h2>Users</h2>
          <p>{{ dashboard.totalUsers }}</p>
        </div>
        <div class="card">
          <h2>Projects</h2>
          <p>{{ dashboard.totalProjects }} ({{ dashboard.activeProjects }} active)</p>
        </div>
        <div class="card">
          <h2>Time entries</h2>
          <p>{{ dashboard.totalTimeEntries }}</p>
        </div>
        <div class="card">
          <h2>Total invoiced</h2>
          <p>\${{ dashboard.totalInvoiced | number }}</p>
        </div>
      </div>
    </ng-container>
  `,
})
export class DashboardComponent implements OnInit {
  private readonly api = inject(ApiService);

  data: DashboardSummary | null = null;
  error: string | null = null;

  ngOnInit(): void {
    this.api.getDashboard().subscribe({
      next: (data) => {
        this.data = data;
      },
      error: (error: Error) => {
        this.error = error.message;
      },
    });
  }
}

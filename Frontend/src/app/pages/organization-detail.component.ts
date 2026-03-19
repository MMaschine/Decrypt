import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { ApiService, OrganizationSummary } from '../api.service';

@Component({
  selector: 'app-organization-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div *ngIf="error" class="card error">Error: {{ error }}</div>

    <div *ngIf="!error && !summary" class="card loading">Loading...</div>

    <ng-container *ngIf="summary as details">
      <p><a routerLink="/organizations">&larr; Organizations</a></p>
      <h1 style="margin-bottom: 1rem;">{{ details.organization.name }}</h1>
      <div class="card">
        <p><strong>Industry:</strong> {{ details.organization.industry }}</p>
        <p>
          <strong>Tier:</strong>
          <span class="badge" [ngClass]="'badge--' + details.organization.tier">{{ details.organization.tier }}</span>
        </p>
        <p><strong>Contact:</strong> {{ details.organization.contactEmail }}</p>
      </div>
      <div class="grid grid-3">
        <div class="card">
          <h2>Projects</h2>
          <p>{{ details.projectCount }}</p>
        </div>
        <div class="card">
          <h2>Users</h2>
          <p>{{ details.userCount }}</p>
        </div>
        <div class="card">
          <h2>Total invoiced</h2>
          <p>{{ details.currency }} {{ details.totalInvoiced | number }}</p>
        </div>
      </div>
    </ng-container>
  `,
})
export class OrganizationDetailComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly route = inject(ActivatedRoute);

  summary: OrganizationSummary | null = null;
  error: string | null = null;

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      this.error = 'Organization id is missing';
      return;
    }

    this.api.getOrganizationSummary(id).subscribe({
      next: (summary) => {
        this.summary = summary;
      },
      error: (error: Error) => {
        this.error = error.message;
      },
    });
  }
}


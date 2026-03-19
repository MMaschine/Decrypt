import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';

import { ApiService, Organization } from '../api.service';

@Component({
  selector: 'app-organizations',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div *ngIf="error" class="card error">Error: {{ error }}</div>

    <div *ngIf="!error && organizations === null" class="card loading">Loading...</div>

    <ng-container *ngIf="organizations as list">
      <h1 style="margin-bottom: 1rem;">Organizations</h1>
      <div class="grid grid-2">
        <a
          *ngFor="let organization of list"
          [routerLink]="['/organizations', organization.id]"
          style="color: inherit;"
        >
          <div class="card">
            <h2>{{ organization.name }}</h2>
            <p>
              {{ organization.industry }} &middot;
              <span class="badge" [ngClass]="'badge--' + organization.tier">{{ organization.tier }}</span>
            </p>
            <p>{{ organization.contactEmail }}</p>
          </div>
        </a>
      </div>
    </ng-container>
  `,
})
export class OrganizationsComponent implements OnInit {
  private readonly api = inject(ApiService);

  organizations: Organization[] | null = null;
  error: string | null = null;

  ngOnInit(): void {
    this.api.getOrganizations().subscribe({
      next: (organizations) => {
        this.organizations = organizations;
      },
      error: (error: Error) => {
        this.error = error.message;
      },
    });
  }
}


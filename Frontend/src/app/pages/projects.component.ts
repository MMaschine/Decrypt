import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';

import { ApiService, Project } from '../api.service';

@Component({
  selector: 'app-projects',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div *ngIf="error" class="card error">Error: {{ error }}</div>

    <div *ngIf="!error && projects === null" class="card loading">Loading...</div>

    <ng-container *ngIf="projects as list">
      <h1 style="margin-bottom: 1rem;">Projects</h1>
      <div class="grid grid-2">
        <a *ngFor="let project of list" [routerLink]="['/projects', project.id]" style="color: inherit;">
          <div class="card">
            <h2>{{ project.name }}</h2>
            <p>
              <span class="badge" [ngClass]="'badge--' + project.status">{{ project.status }}</span>
            </p>
            <p>
              Budget: {{ project.budgetHours }}h &middot;
              {{ project.startDate || '-' }} to {{ project.endDate || '-' }}
            </p>
          </div>
        </a>
      </div>
    </ng-container>
  `,
})
export class ProjectsComponent implements OnInit {
  private readonly api = inject(ApiService);

  projects: Project[] | null = null;
  error: string | null = null;

  ngOnInit(): void {
    this.api.getProjects().subscribe({
      next: (projects) => {
        this.projects = projects;
      },
      error: (error: Error) => {
        this.error = error.message;
      },
    });
  }
}


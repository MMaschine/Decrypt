import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { ApiService, Project } from '../api.service';

@Component({
  selector: 'app-project-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div *ngIf="error" class="card error">Error: {{ error }}</div>

    <div *ngIf="!error && !project" class="card loading">Loading...</div>

    <ng-container *ngIf="project as details">
      <p><a routerLink="/projects">&larr; Projects</a></p>
      <h1 style="margin-bottom: 1rem;">{{ details.name }}</h1>
      <div class="card">
        <p><strong>Status:</strong> {{ details.status }}</p>
        <p><strong>Budget:</strong> {{ details.budgetHours }} hours</p>
        <p><strong>Hours logged:</strong> {{ details.totalHoursLogged ?? 0 }}</p>
        <p><strong>Organization:</strong> {{ details.organization?.name }}</p>
        <p><strong>Dates:</strong> {{ details.startDate || '-' }} to {{ details.endDate || '-' }}</p>
      </div>
    </ng-container>
  `,
})
export class ProjectDetailComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly route = inject(ActivatedRoute);

  project: Project | null = null;
  error: string | null = null;

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      this.error = 'Project id is missing';
      return;
    }

    this.api.getProject(id).subscribe({
      next: (project) => {
        this.project = project;
      },
      error: (error: Error) => {
        this.error = error.message;
      },
    });
  }
}


import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet],
  template: `
    <nav>
      <a routerLink="/" [routerLinkActiveOptions]="{ exact: true }" routerLinkActive="active-link">Dashboard</a>
      <a routerLink="/organizations" routerLinkActive="active-link">Organizations</a>
      <a routerLink="/projects" routerLinkActive="active-link">Projects</a>
    </nav>

    <main class="container">
      <router-outlet></router-outlet>
    </main>
  `,
  styles: [
    `
      .active-link {
        text-decoration: underline;
      }
    `,
  ],
})
export class AppComponent {}

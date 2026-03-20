import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './core/layout/header.component';
import { FooterComponent } from './core/layout/footer.component';
import { SidebarComponent } from './core/layout/sidebar.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, FooterComponent, SidebarComponent],
  template: `
    <div class="app-layout">
      <app-header (toggleSidebar)="sidebarOpen = !sidebarOpen"></app-header>
      <div class="app-body">
        <app-sidebar [isOpen]="sidebarOpen" (closeSidebar)="sidebarOpen = false"></app-sidebar>
        <main class="app-content" [class.sidebar-open]="sidebarOpen">
          <router-outlet></router-outlet>
        </main>
      </div>
      <app-footer></app-footer>
    </div>
  `,
  styles: [`
    .app-layout {
      display: flex;
      flex-direction: column;
      min-height: 100vh;
    }
    .app-body {
      display: flex;
      flex: 1;
      margin-top: 64px;
    }
    .app-content {
      flex: 1;
      padding: 24px;
      transition: margin-left 0.3s ease;
    }
    .app-content.sidebar-open {
      margin-left: 260px;
    }
    @media (max-width: 768px) {
      .app-content.sidebar-open {
        margin-left: 0;
      }
    }
  `]
})
export class AppComponent {
  sidebarOpen = false;
}

import { Component } from '@angular/core';

@Component({
  selector: 'app-footer',
  standalone: true,
  template: `
    <footer class="footer">
      <span>&copy; {{ year }} College LMS</span>
    </footer>
  `,
  styles: [`
    .footer {
      padding: 16px 24px;
      text-align: center;
      background: #212121;
      color: #9e9e9e;
      font-size: 14px;
    }
  `]
})
export class FooterComponent {
  year = new Date().getFullYear();
}

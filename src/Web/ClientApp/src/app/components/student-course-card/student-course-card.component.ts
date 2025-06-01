import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { TuiIcon } from '@taiga-ui/core';
import { DatePipe } from '@angular/common';
import { GradedCourseResponse } from '../../web-api-client';

@Component({
  selector: 'app-student-course-card',
  imports: [TuiIcon, DatePipe],
  templateUrl: './student-course-card.component.html',
  styles: ``,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class StudentCourseCardComponent {
  course = input.required<GradedCourseResponse>();

  getPlaceholderImageUrl(): string {
    const name = this.course().name;
    const firstLetter = name.charAt(0).toUpperCase();

    const stringToHslColor = (str: string, s: number, l: number) => {
      let hash = 0;
      for (let i = 0; i < str.length; i++) {
        hash = str.charCodeAt(i) + ((hash << 5) - hash);
      }
      const h = hash % 360;
      return `hsl(${h}, ${s}%, ${l}%)`;
    };

    const color1 = stringToHslColor(name, 70, 50);
    const color2 = stringToHslColor(name + 'gradient', 80, 60);

    const hexToRgb = (hex: string) => {
      const hslToRgb = (h: number, s: number, l: number) => {
        s /= 100;
        l /= 100;
        const c = (1 - Math.abs(2 * l - 1)) * s;
        const x = c * (1 - Math.abs(((h / 60) % 2) - 1));
        const m = l - c / 2;
        let r = 0,
          g = 0,
          b = 0;

        if (0 <= h && h < 60) {
          r = c;
          g = x;
          b = 0;
        } else if (60 <= h && h < 120) {
          r = x;
          g = c;
          b = 0;
        } else if (120 <= h && h < 180) {
          r = 0;
          g = c;
          b = x;
        } else if (180 <= h && h < 240) {
          r = 0;
          g = x;
          b = c;
        } else if (240 <= h && h < 300) {
          r = x;
          g = 0;
          b = c;
        }
        r = Math.round((r + m) * 255);
        g = Math.round((g + m) * 255);
        b = Math.round((b + m) * 255);

        return { r, g, b };
      };

      const hslMatch = color1.match(/hsl\((\d+),\s*(\d+)%,\s*(\d+)%\)/);
      if (hslMatch) {
        const h = parseInt(hslMatch[1]);
        const s = parseInt(hslMatch[2]);
        const l = parseInt(hslMatch[3]);
        return hslToRgb(h, s, l);
      }
      return { r: 0, g: 0, b: 0 };
    };

    const rgb = hexToRgb(color1);
    const luminance = (0.299 * rgb.r + 0.587 * rgb.g + 0.114 * rgb.b) / 255;
    const textColor = luminance > 0.5 ? '#000000' : '#FFFFFF';

    const svgString = `
      <svg width="400" height="200" xmlns="http://www.w3.org/2000/svg">
        <defs>
          <linearGradient id="courseGradient" x1="0%" y1="0%" x2="100%" y2="100%">
            <stop offset="0%" style="stop-color:${color1};stop-opacity:1" />
            <stop offset="100%" style="stop-color:${color2};stop-opacity:1" />
          </linearGradient>
        </defs>
        <rect width="100%" height="100%" fill="url(#courseGradient)"/>
        <text x="50%" y="50%" dominant-baseline="middle" text-anchor="middle" font-family="Inter, sans-serif" font-size="100" fill="${textColor}">
          ${firstLetter}
        </text>
      </svg>
    `;

    const utf8EncodedSvg = encodeURIComponent(svgString);
    const latin1String = utf8EncodedSvg.replace(
      /%([0-9A-F]{2})/g,
      function toSolidBytes(match, p1) {
        return String.fromCharCode(parseInt(p1, 16));
      },
    );
    const encodedSvg = btoa(latin1String);
    return `data:image/svg+xml;base64,${encodedSvg}`;
  }
}

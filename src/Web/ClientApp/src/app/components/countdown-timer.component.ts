import {
  Component,
  signal,
  computed,
  effect,
  OnDestroy,
  ChangeDetectionStrategy,
  input,
  output,
} from '@angular/core';

@Component({
  selector: 'app-countdown-timer',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div
      class="timer-container inline-flex flex-col items-center text-gray-600"
    >
      <div
        class="timer-display select-none font-mono text-5xl tracking-wider"
        [class.text-red-400]="isExpired() && timeToDisplay() === '00:00:00'"
        [class.animate-pulse-strong]="
          isExpired() && timeToDisplay() === '00:00:00'
        "
        aria-live="polite"
        role="timer"
      >
        @for (part of animatedTimeParts(); track $index) {
          <span
            class="digit-wrapper"
            [class.animate-digit-change]="part.changed"
            [class.colon]="part.char === ':'"
          >
            {{ part.char }}
          </span>
        }
      </div>
      @if (isExpired() && timeToDisplay() === '00:00:00') {
        <div class="mt-3 text-sm text-red-300">Время истекло</div>
      }
    </div>
  `,
  styles: [
    `
      .timer-display {
        display: flex;
        transition: color 0.3s ease-in-out;
      }
      .digit-wrapper {
        display: inline-block;
        position: relative;
        min-width: 0.6em;
        text-align: center;
        padding: 0 0.02em;
      }
      .colon {
        transform: translateY(-0.05em);
        min-width: 0.3em;
        padding: 0 0.05em;
      }
      .animate-digit-change {
        animation: digitChangeAnimation 0.5s
          cubic-bezier(0.68, -0.55, 0.27, 1.55) forwards;
      }
      .animate-pulse-strong {
        animation: pulseStrong 1.5s cubic-bezier(0.4, 0, 0.6, 1) infinite;
      }

      @keyframes digitChangeAnimation {
        0% {
          transform: translateY(0.6em) scaleY(0.4) rotateX(-90deg);
          opacity: 0;
          color: #facc15; /* Tailwind yellow-400 */
        }
        60% {
          transform: translateY(0) scaleY(1) rotateX(0deg);
          opacity: 1;
          color: #60a5fa; /* Tailwind blue-400 */
        }
        100% {
          transform: translateY(0) scaleY(1) rotateX(0deg);
          opacity: 1;
          color: inherit;
        }
      }

      @keyframes pulseStrong {
        0%,
        100% {
          opacity: 1;
          transform: scale(1);
        }
        50% {
          opacity: 0.7;
          transform: scale(1.03);
        }
      }
    `,
  ],
})
export class CountdownTimerComponent implements OnDestroy {
  targetDateTime = input.required<Date>();

  private remainingMs = signal<number>(0);
  isExpired = signal<boolean>(false);
  expired = output();

  timeToDisplay = computed<string>(() => {
    if (this.isExpired() || this.remainingMs() <= 0) {
      return '00:00:00';
    }
    const totalSeconds = Math.floor(this.remainingMs() / 1000);
    const h = Math.floor(totalSeconds / 3600);
    const m = Math.floor((totalSeconds % 3600) / 60);
    const s = totalSeconds % 60;
    return `${String(h).padStart(2, '0')}:${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')}`;
  });

  animatedTimeParts = signal<Array<{ char: string; changed: boolean }>>(
    this.getInitialParts('00:00:00'),
  );
  private lastDisplayedString = 'XX:XX:XX';

  private intervalId: ReturnType<typeof setInterval> | null = null;

  constructor() {
    effect(() => {
      this.targetDateTime();
      this.stopTimer();

      this.lastDisplayedString = 'XX:XX:XX';
      this.updateRemainingTimeAndState();

      if (!this.isExpired()) {
        this.startTimer();
      }
    });

    effect(() => {
      const currentString = this.timeToDisplay();

      if (
        currentString === '00:00:00' &&
        this.isExpired() &&
        this.lastDisplayedString === '00:00:00' &&
        this.animatedTimeParts().every((p) => !p.changed)
      ) {
        return;
      }

      const newParts =
        currentString === this.lastDisplayedString &&
        currentString === '00:00:00' &&
        this.isExpired()
          ? this.getInitialParts(currentString, false)
          : currentString.split('').map((char, index) => ({
              char,
              changed: this.lastDisplayedString[index] !== char,
            }));
      this.animatedTimeParts.set(newParts);
      this.lastDisplayedString = currentString;
    });
  }

  ngOnDestroy(): void {
    this.stopTimer();
  }

  private getInitialParts(
    str: string,
    changed: boolean = false,
  ): Array<{ char: string; changed: boolean }> {
    return str.split('').map((char) => ({ char, changed }));
  }

  private startTimer(): void {
    if (this.intervalId) clearInterval(this.intervalId);
    this.intervalId = setInterval(() => {
      this.updateRemainingTimeAndState();
    }, 1000);
  }

  private stopTimer(): void {
    if (this.intervalId) {
      clearInterval(this.intervalId);
      this.intervalId = null;
    }
  }

  private updateRemainingTimeAndState(): void {
    const difference = this.targetDateTime().getTime() - Date.now();

    if (difference <= 0) {
      if (this.remainingMs() !== 0) this.remainingMs.set(0);
      if (!this.isExpired()) {
        this.isExpired.set(true);
        this.expired.emit();
      }
      this.stopTimer();
    } else {
      this.remainingMs.set(difference);
      if (this.isExpired()) this.isExpired.set(false);
    }
  }
}

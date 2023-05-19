import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TimeSpanV1 } from '../../models/timespan.model';
import { FormatTimespanPipe } from '../../pipes/format-timespan.pipe';

@Component({
  selector: 'viet-geeks-exam-result-timer',
  standalone: true,
  imports: [CommonModule, FormatTimespanPipe],
  templateUrl: './exam-result-timer.component.html',
  styleUrls: ['./exam-result-timer.component.scss']
})
export class ExamResultTimerComponent {
  @Input()
  containerClass = '';

  @Input()
  totalTime: TimeSpanV1 | string = {};

  @Input()
  maxTime: TimeSpanV1 | string = {};

  @Input()
  examTime!: { startTime?: Date | string, endTime?: Date | string };
}

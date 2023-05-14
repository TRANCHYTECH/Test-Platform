import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TimeSpanV1 } from '../../models/timespan.model';
import { FormatTimespanPipe } from '../../pipes/format-timespan.pipe';
import { FormatLocalDateTimePipe } from '../../pipes/format-local-datetime.pipe';

@Component({
  selector: 'viet-geeks-exam-result-timer',
  standalone: true,
  imports: [CommonModule, FormatTimespanPipe, FormatLocalDateTimePipe],
  templateUrl: './exam-result-timer.component.html',
  styleUrls: ['./exam-result-timer.component.scss']
})
export class ExamResultTimerComponent {
  @Input()
  totalTime: TimeSpanV1 = {};

  @Input()
  maxTime: TimeSpanV1 = {};

  @Input()
  examTime!: { startTime?: Date, endTime?: Date };
}

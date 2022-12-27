import { Component } from '@angular/core';
import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';

@Component({
  selector: 'viet-geeks-grading-and-summary',
  templateUrl: './grading-and-summary.component.html',
  styleUrls: ['./grading-and-summary.component.scss'],
})
export class GradingAndSummaryComponent {
  public Editor = ClassicEditor;
}

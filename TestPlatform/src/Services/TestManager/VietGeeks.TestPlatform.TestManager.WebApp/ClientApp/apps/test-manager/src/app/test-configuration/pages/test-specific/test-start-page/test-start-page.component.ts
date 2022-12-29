import { Component } from '@angular/core';
import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';

@Component({
  selector: 'viet-geeks-test-start-page',
  templateUrl: './test-start-page.component.html',
  styleUrls: ['./test-start-page.component.scss'],
})
export class TestStartPageComponent {
  Editor = ClassicEditor;
}

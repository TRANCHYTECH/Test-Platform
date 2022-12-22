import { Component } from '@angular/core';
import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';

@Component({
  selector: 'viet-geeks-basic-settings',
  templateUrl: './basic-settings.component.html',
  styleUrls: ['./basic-settings.component.scss']
})
export class BasicSettingsComponent {
  public Editor = ClassicEditor;

   selectValue = ['Choice 1', 'Choice 2', 'Choice 3'];
}

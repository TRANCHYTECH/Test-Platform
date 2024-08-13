import { Directive, ElementRef, Renderer2 } from '@angular/core';
import { EditorComponent } from '@tinymce/tinymce-angular';
import { take } from 'rxjs';

@Directive({
  selector: '[vietGeeksEditorLoadingIndicator]',
  standalone: true
})
export class EditorLoadingIndicatorDirective {
  constructor(private editor: EditorComponent, private el: ElementRef, private renderer: Renderer2) {
    this.editor.onPostRender.pipe(take(1)).subscribe((v) => {
      this.renderer.addClass(this.el.nativeElement, 'rendered');
    });
  }
}

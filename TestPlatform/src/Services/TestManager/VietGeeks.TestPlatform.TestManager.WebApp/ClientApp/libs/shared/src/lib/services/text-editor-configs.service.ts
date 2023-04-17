import { Injectable, InjectionToken, inject } from '@angular/core';
import { AsyncValidatorFn, ValidationErrors } from '@angular/forms';
import { EditorComponent } from '@tinymce/tinymce-angular';
import { EventObj } from '@tinymce/tinymce-angular/editor/Events';
import { UploadClient } from '@uploadcare/upload-client'
import { AsyncSubject, Observable, map } from 'rxjs';

type TinymceTextEditor = EventObj<null>["editor"];

const EditorPlugins = {
  simple: ['lists', 'table', 'code', 'wordcount']
};

const Toolbars = {
  simple: 'undo redo | bold italic | alignleft aligncenter alignright | bullist numlist outdent indent',
  image: '| image'
}

export const EDITOR_API_KEY = new InjectionToken<string>("tiny-editor-api-key");

@Injectable({
  providedIn: 'root'
})
export class TextEditorConfigsService {
  private _apiKey = inject(EDITOR_API_KEY);
  private _client = new UploadClient({ publicKey: '8a991600a5b7c5405c5a' });
  private _editorSubjects: { [key: string]: AsyncSubject<TinymceTextEditor> } = {};

  apiKey = this._apiKey;

  simpleEditorConfig: EditorComponent['init'] = {
    menubar: false,
    plugins: EditorPlugins.simple,
    toolbar: Toolbars.simple,
    height: 300
  };

  simpleEditorWithImage: EditorComponent['init'] = {
    menubar: false,
    height: 300,
    plugins: [
      ...EditorPlugins.simple,
      'image'
    ],
    toolbar: Toolbars.simple + Toolbars.image,
    file_picker_types: 'image',
    image_advtab: false,
    image_description: false,
    image_dimensions: true,
    block_unsupported_drop: true,
    placeholder: 'Content here...',
    images_reuse_filename: true,
    paste_data_images: true,
    images_upload_handler: (blobInfo) => {
      const uploadTask = this._client.uploadFile(blobInfo.blob(), { fileName: blobInfo.filename() });
      return new Promise<string>((resolve, reject) => {
        uploadTask.then(rs => {
          resolve(rs.cdnUrl || '');
        }, err => reject(err))
      });
    },
  }

  handleEditorInit(controlUniqueName: string, e: EventObj<null>) {
    if (this._editorSubjects[controlUniqueName] === undefined) {
      return;
    }

    this._editorSubjects[controlUniqueName].next(e.editor);
    this._editorSubjects[controlUniqueName].complete();
  }

  editorMaxLength = (controlUniqueName: string, max: number): AsyncValidatorFn => {
    const editorSubject = new AsyncSubject<TinymceTextEditor>();
    this._editorSubjects[controlUniqueName] = editorSubject;

    return (): Observable<ValidationErrors | null> => {
      return editorSubject.pipe(
        map((editor) => {
          const characterCount = editor.plugins['wordcount']['body'].getCharacterCount();
          return characterCount <= max
            ? null
            : {
              maxlength: {
                requiredLength: max,
                actualLength: characterCount
              },
            };
        })
      );
    };
  };
}

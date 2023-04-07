import { Injectable, InjectionToken } from '@angular/core';
import { EditorComponent } from '@tinymce/tinymce-angular';
import { UploadClient } from '@uploadcare/upload-client'

const EditorPlugins = {
  simple: ['lists', 'table', 'code', 'wordcount']
};

const Toolbars = {
  simple: 'undo redo | bold italic | alignleft aligncenter alignright | bullist numlist outdent indent',
  image: '| image'
}

export const EDITOR_API_KEY = new InjectionToken("tiny-editor-api-key");

@Injectable({
  providedIn: 'root'
})
export class TextEditorConfigsService {
  private readonly _client = new UploadClient({ publicKey: '8a991600a5b7c5405c5a' });

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
}

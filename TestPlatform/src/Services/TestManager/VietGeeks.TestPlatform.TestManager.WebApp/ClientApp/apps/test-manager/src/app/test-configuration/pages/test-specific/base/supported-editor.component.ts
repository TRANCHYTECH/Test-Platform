import { inject } from "@angular/core";
import { AsyncValidatorFn, ValidationErrors } from "@angular/forms";
import { EventObj } from "@tinymce/tinymce-angular/editor/Events";
import { EDITOR_API_KEY, TextEditorConfigsService } from "@viet-geeks/shared";
import { AsyncSubject, Observable, map } from "rxjs";

type TextEditor = EventObj<null>["editor"];

export abstract class SupportedEditorComponent {
    textEditorConfigs = inject(TextEditorConfigsService);
    editorApiKey = inject<string>(EDITOR_API_KEY);
    
    private editorSubjects: { [key: string]: AsyncSubject<TextEditor> } = {};

    handleEditorInit(controlUniqueName: string, e: EventObj<null>) {
        if (this.editorSubjects[controlUniqueName] === undefined) {
            return;
        }

        this.editorSubjects[controlUniqueName].next(e.editor);
        this.editorSubjects[controlUniqueName].complete();
    }

    EditorMaxLengthValidator = (controlUniqueName: string, max: number): AsyncValidatorFn => {
        const editorSubject = new AsyncSubject<TextEditor>();
        this.editorSubjects[controlUniqueName] = editorSubject;

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

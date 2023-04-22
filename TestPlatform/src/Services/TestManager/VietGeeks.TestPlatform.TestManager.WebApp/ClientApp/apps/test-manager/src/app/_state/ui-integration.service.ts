import { Injectable, inject } from "@angular/core";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { NewTestCategoryComponent } from "../test-categories/new-test-category/new-test-category.component";

@Injectable({ providedIn: 'root' })
export class UiIntegrationService {
    private _modal = inject(NgbModal);

    openNewTestCategoryModal() {
        this._modal.open(NewTestCategoryComponent, { size: 'md', centered: true });
    }
}
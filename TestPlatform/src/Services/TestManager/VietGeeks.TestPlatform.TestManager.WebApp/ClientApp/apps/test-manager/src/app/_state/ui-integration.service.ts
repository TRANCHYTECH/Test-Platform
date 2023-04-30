import { Injectable, inject } from "@angular/core";
import { NgbModal, NgbModalRef } from "@ng-bootstrap/ng-bootstrap";
import { NewTestCategoryComponent } from "../test-categories/new-test-category/new-test-category.component";
import { forIn } from "lodash-es";

@Injectable({ providedIn: 'root' })
export class UiIntegrationService {
    private _modal = inject(NgbModal);
    private _registers: { [key: string]: () => NgbModalRef } = {};

    openNewTestCategoryModal() {
        this._modal.open(NewTestCategoryComponent, { size: 'md', centered: true });
    }

    openModal(modalCode: string, inputs: { [inputProp: string]: unknown }) {
        const ref = this._registers[modalCode]();
        forIn(inputs, (inputPropData, inputProp) => ref.componentInstance[inputProp] = inputPropData);
    }

    registerModal(comp: object, modalCode: string) {
        this._registers[modalCode] = () => this._modal.open(comp, { size: 'md', centered: true });
    }
}
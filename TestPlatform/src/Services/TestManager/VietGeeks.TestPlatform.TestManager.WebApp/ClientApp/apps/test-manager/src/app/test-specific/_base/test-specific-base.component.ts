import { ChangeDetectorRef, Component, inject, OnDestroy, OnInit } from "@angular/core";
import { FormBuilder, FormGroup } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { untilDestroyed } from "@ngneat/until-destroy";
import { EntitySpecificBaseComponent, getTestId, TextEditorConfigsService, ToastService } from "@viet-geeks/shared";
import { firstValueFrom } from "rxjs";
import { createTest, Test, TestStatus } from "../_state/test.model";
import { TestsService } from "../_state/tests.service";
import { TestsQuery } from "../_state/tests.query";

@Component({
    selector: 'viet-geeks-test-specific-base',
    template: ''
})
export abstract class TestSpecificBaseComponent extends EntitySpecificBaseComponent implements OnInit, OnDestroy {
    testId!: string;
    test: Test = createTest({});

    route = inject(ActivatedRoute);
    fb = inject(FormBuilder);
    changeRef = inject(ChangeDetectorRef);

    testsService = inject(TestsService);
    testsQuery = inject(TestsQuery);
    notifyService = inject(ToastService);
    textEditorConfigs = inject(TextEditorConfigsService);

    get isNewTest() {
        return this.testId === 'new';
    }

    ngOnDestroy(): void {
        this.testsService.removeCurrentActive();
        this.onDestroy();
    }

    async loadEntity() {
        this.testId = getTestId(this.route);
        if (!this.isNewTest) {
            await firstValueFrom(this.testsService.getById(this.testId), { defaultValue: null });
            const testDef = this.testsQuery.getEntity(this.testId);
            if (testDef === undefined) {
                await this.router.navigate(['tests']);
                return;
            }

            this.test = testDef;
            this.testsService.setActive(testDef.id);
        }
    }

    onDestroy() {
        // Default do nothing.
    }

    get isReadonly() {
        return this.test.status !== undefined && this.test.status !== TestStatus.Draft;
    }

    async invokeLongAction(action: () => Promise<void>) {
        this.maskBusyForUI();
        await action();
        this.maskReadyForUI();
        this.changeRef.markForCheck();
    }

    //todo(tau): how to generalize it?
    setupControlValidityTrigger(parent: FormGroup, sourcePath: string[], targetPaths: string[][]) {
        //todo: improve the destroying subscription.
        parent.get(sourcePath)?.valueChanges.pipe(untilDestroyed(this)).subscribe(() => setTimeout(() => {
            targetPaths.forEach(p => {
                const control = parent.get(p);
                control?.updateValueAndValidity();
                control?.markAsTouched();
            });
        }));
    }

    listenTypeChange(formGroup: FormGroup, instance: object, controlIds: number[]) {
        formGroup.get(['type'])?.valueChanges.pipe(untilDestroyed(instance)).subscribe(v => {
            controlIds.forEach(id => {
                const ctrl = formGroup.get([id.toString()]);
                v === id ? ctrl?.enable() : ctrl?.disable();
            })
        });
    }
}
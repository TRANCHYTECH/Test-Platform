import { ChangeDetectorRef, Component, inject, OnDestroy, OnInit } from "@angular/core";
import { FormBuilder, FormGroup } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { untilDestroyed } from "@ngneat/until-destroy";
import { EntitySpecificBaseComponent, getTestId, TextEditorConfigsService, ToastService } from "@viet-geeks/shared";
import { TestStatus } from '../../_state/test-support.model';
import { firstValueFrom } from "rxjs";
import { createTest, Test } from "../_state/tests/test.model";
import { TestsQuery } from "../_state/tests/tests.query";
import { TestsService } from "../_state/tests/tests.service";

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
                await this.router.navigate(['/test/list']);
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
        this.maskBusyForMainFlow();
        await action();
        this.maskReadyForMainFlow();
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

    //todo: rename it
    listenToToggleControlState<T>(instance: T, formGroup: FormGroup, sourcePath: string, targetPath: string) {
        formGroup.controls[sourcePath].valueChanges.pipe(untilDestroyed(instance)).subscribe((v: boolean) => {
          setTimeout(() => {
            const method = this.getChangeControlStateMethod(v);
            formGroup.controls[targetPath][method]();
          }, 100);
        });
      }

    getChangeControlStateMethod(v: boolean) {
        return v ? 'enable' : 'disable';
      }
}
import { ChangeDetectorRef, Component, inject, OnDestroy, OnInit } from "@angular/core";
import { FormBuilder, FormGroup } from "@angular/forms";
import { ActivatedRoute, NavigationEnd, Router } from "@angular/router";
import { untilDestroyed } from "@ngneat/until-destroy";
import { ToastService } from "@viet-geeks/shared";
import { NgxSpinnerService } from "ngx-spinner";
import { BehaviorSubject, filter, firstValueFrom } from "rxjs";
import { SupportedEditorComponent } from "./supported-editor.component";
import { createTest, Test, TestStatus } from "../_state/test.model";
import { TestsService } from "../_state/tests.service";
import { TestsQuery } from "../_state/tests.query";
import { getTestId } from "../../../../../../libs/shared/src/lib/functions/router-param-functions";

@Component({
    selector: 'viet-geeks-test-specific-base',
    template: ''
})
export abstract class TestSpecificBaseComponent extends SupportedEditorComponent implements OnInit, OnDestroy {
    testId!: string;
    test: Test = createTest({});

    router = inject(Router);
    route = inject(ActivatedRoute);
    fb = inject(FormBuilder);
    changeRef = inject(ChangeDetectorRef);

    testsService = inject(TestsService);
    testsQuery = inject(TestsQuery);
    notifyService = inject(ToastService);

    get readyForUI$() {
        return this._readyForUI.asObservable();
    }

    get isNewTest() {
        return this.testId === 'new';
    }

    private _spinner = inject(NgxSpinnerService);

    private _readyForUI = new BehaviorSubject(false);

    ngOnInit(): void {
        // Listen to process the first time.
        this.route.params.pipe(untilDestroyed(this)).subscribe(() => {
            this.processParams();
        });

        // Listen to reload the page.
        this.router.events.pipe(filter(event => event instanceof NavigationEnd), untilDestroyed(this)).subscribe(() => {
            console.log('reload page');
            this.processParams();
        });

        this.configureLoadingIndicator();

        this.onInit();
    }

    ngOnDestroy(): void {
        this.testsService.removeCurrentActive();
        this.onDestroy();
    }

    private async processParams() {
        this._readyForUI.next(false);

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

        this.afterGetTest();
    }

    private configureLoadingIndicator() {
        this._readyForUI.pipe(untilDestroyed(this)).subscribe(v => {
            if (v === false) {
                this._spinner.show(undefined, {
                    type: 'ball-fussion',
                    size: 'medium',
                    bdColor: 'rgba(100,149,237, .2)',
                    color: 'white',
                    fullScreen: false
                });
            } else {
                this._spinner.hide();
            }
        });
    }

    onInit() {
        // Place holder
    }

    onDestroy() {
        // Default do nothing.
    }

    abstract afterGetTest(): void;

    abstract submit(): Promise<void>;

    abstract get canSubmit(): boolean;

    get isReadonly() {
        return this.test.status !== undefined && this.test.status !== TestStatus.Draft;
    }

    maskBusyForUI() {
        this._readyForUI.next(false);
    }

    maskReadyForUI() {
        this._readyForUI.next(true);
    }

    submitFunc = async () => {
        if (!this.canSubmit) {
            return;
        }

        await this.submit();

        // Refresh the page to bind latest info.
        this.router.navigate([this.router.url], { onSameUrlNavigation: 'reload' });
    };

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
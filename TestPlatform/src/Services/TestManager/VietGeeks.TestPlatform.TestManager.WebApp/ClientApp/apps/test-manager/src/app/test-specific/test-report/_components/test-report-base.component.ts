import { Component, inject } from "@angular/core";
import { TestSpecificBaseComponent } from "../../_base/test-specific-base.component";
import { TestRunSummary } from "../_state/exam-summary.model";
import { firstValueFrom } from "rxjs";
import { ExamSummaryService } from "../_state/exam-summary.service";
import { UserProfileService } from "@viet-geeks/core";

@Component({ template: '' })
export class TestReportBaseComponent extends TestSpecificBaseComponent {
    testRuns: TestRunSummary[] = [];

    protected _examSummaryService = inject(ExamSummaryService);
    protected _userProfileService = inject(UserProfileService);

    override async postLoadEntity(): Promise<void> {
        this.testRuns = await firstValueFrom(this._examSummaryService.getTestRuns(this.testId));
    }

    override submit(): Promise<void> {
        throw new Error("Method not implemented.");
    }

    override get canSubmit(): boolean {
        throw new Error("Method not implemented.");
    }
}

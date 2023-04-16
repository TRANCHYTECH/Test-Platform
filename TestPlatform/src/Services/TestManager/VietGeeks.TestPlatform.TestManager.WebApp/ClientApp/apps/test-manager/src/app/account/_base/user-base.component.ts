import { Component, OnInit, inject } from "@angular/core";
import { UntilDestroy, untilDestroyed } from "@ngneat/until-destroy";
import { BehaviorSubject, combineLatest, filter, firstValueFrom } from "rxjs";
import { UserQuery } from "../_state/user.query";
import { UserService } from "../_state/user.service";
import { NavigationEnd, Router } from "@angular/router";
import { User } from "../_state/user.model";
import { FormBuilder } from "@angular/forms";
import { ToastService } from "@viet-geeks/shared";

@UntilDestroy()
@Component({
    template: '',
    styleUrls: []
})
export abstract class UserBaseComponent implements OnInit {
    userProfile!: User;
    timeZones: string[] = [];
    languages: string[] = ['Vietnamese', 'English'];

    userService = inject(UserService);
    userQuery = inject(UserQuery);
    router = inject(Router);
    fb = inject(FormBuilder);
    notifyService = inject(ToastService);

    private _readyForUI = new BehaviorSubject(false);

    get readyForUI$() {
        return this._readyForUI.asObservable();
    }

    ngOnInit(): void {
        this.getUserData();
        
        // Listen to reload the page.
        this.router.events.pipe(filter(event => event instanceof NavigationEnd), untilDestroyed(this)).subscribe(() => {
            console.log('reload page');
            this.getUserData();
        });
    }

    async getUserData() {
        this._readyForUI.next(false);

        const data = await firstValueFrom(combineLatest([this.userService.getTimeZones(), this.userService.get()]));
        this.timeZones = data[0];
        this.userProfile = data[1];

        this.AfterGetUserData();
    }

    abstract submit(): Promise<void>;

    abstract get canSubmit(): boolean;

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

    abstract AfterGetUserData(): void;
}

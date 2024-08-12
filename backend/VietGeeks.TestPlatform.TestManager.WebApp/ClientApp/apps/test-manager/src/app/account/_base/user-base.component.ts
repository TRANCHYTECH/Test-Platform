import { Component, OnInit, inject } from "@angular/core";
import { FormBuilder } from "@angular/forms";
import { EntitySpecificBaseComponent, ToastService } from "@viet-geeks/shared";
import { combineLatest, firstValueFrom } from "rxjs";
import { User } from "../_state/user.model";
import { UserQuery } from "../_state/user.query";
import { UserService } from "../_state/user.service";

@Component({ template: '' })
export abstract class UserBaseComponent extends EntitySpecificBaseComponent implements OnInit {
    userProfile!: User;
    timeZones: string[] = [];
    languages: string[] = ['Vietnamese', 'English'];

    userService = inject(UserService);
    userQuery = inject(UserQuery);
    fb = inject(FormBuilder);
    notifyService = inject(ToastService);

    override async loadEntity(): Promise<void> {
        const data = await firstValueFrom(combineLatest([this.userService.getTimeZones(), this.userService.get()]));
        this.timeZones = data[0];
        this.userProfile = data[1];
    }
}

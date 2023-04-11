import { Injectable } from "@angular/core";
import { format, utcToZonedTime, zonedTimeToUtc } from "date-fns-tz";

@Injectable({
    providedIn: 'root'
})
export class UserProfileService {
    currentTimeZone = 'Asia/Ho_Chi_Minh';
    currentUserTime = format(new Date(), 'yyyy-MM-dd HH:mm', { timeZone: this.currentTimeZone });

    convertUtcToLocalDateString(input: Date) {
        return format(utcToZonedTime(input, this.currentTimeZone), 'yyyy-MM-dd HH:mm');
    }

    zonedTimeToUtc(input: Date) {
        return zonedTimeToUtc(input, this.currentTimeZone);
    }
}
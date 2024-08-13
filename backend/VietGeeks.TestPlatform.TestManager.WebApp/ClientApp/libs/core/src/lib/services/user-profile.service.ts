import { Injectable } from "@angular/core";
import { format, utcToZonedTime, zonedTimeToUtc } from "date-fns-tz";

@Injectable({
    providedIn: 'root'
})
export class UserProfileService {
    // TZ identififer. 
    // UTC offset standard of it. get IANA database.
    currentTimeZone = 'Asia/Ho_Chi_Minh';
    currentUtcOffset = 'UTC+7';
    currentUserTime = format(new Date(), 'yyyy-MM-dd HH:mm', { timeZone: this.currentTimeZone });

    convertUtcToLocalDateString(input: Date) {
        return format(utcToZonedTime(input, this.currentTimeZone), 'yyyy-MM-dd HH:mm');
    }

    zonedTimeToUtc(input: Date) {
        return zonedTimeToUtc(input, this.currentTimeZone);
    }
}
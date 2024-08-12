import { Injectable } from '@angular/core';
import { TestDuration, TimeSpan } from '../../api/models';
import { TestDurationMethod, TimeSettings } from '../../state/test-session.model';

@Injectable({ providedIn: 'root' })
export class TestDurationService {
  private _milliSecondsInASecond = 1000;
  private _hoursInADay = 24;
  private _minutesInAnHour = 60;
  private _secondsInAMinute = 60;

  mapToTimeSettings(testDuration?: TestDuration, totalDuration?: TimeSpan): TimeSettings {
    if (testDuration == null) {
      return { duration: {}, totalDuration: {}, method: 0 };
    }

    return {
      duration: this.parse(testDuration.duration?.toString()),
      totalDuration: this.parse(totalDuration?.toString()),
      method: testDuration.method as number
    };
  }

  getDuration(startTime?: Date, endTime?: Date): TimeSpan {
    const timeDifference = this.getTimeDifferenceInMiliseconds(startTime, endTime);
    return this.getDurationFromTimeDifference(timeDifference);
  }

  getDurationFromTimeDifference(timeDifference: number): TimeSpan {
    const duration: TimeSpan = {};
    duration.seconds = Math.max(Math.floor((timeDifference) / (this._milliSecondsInASecond) % this._secondsInAMinute), 0);
    duration.minutes = Math.max(Math.floor((timeDifference) / (this._milliSecondsInASecond * this._minutesInAnHour) % this._secondsInAMinute), 0);
    duration.hours = Math.max(Math.floor((timeDifference) / (this._milliSecondsInASecond * this._minutesInAnHour * this._secondsInAMinute) % this._hoursInADay), 0);
    duration.days = Math.max(Math.floor((timeDifference) / (this._milliSecondsInASecond * this._minutesInAnHour * this._secondsInAMinute * this._hoursInADay)), 0);

    return duration;
  }

  getTimeDifferenceInMiliseconds(startTime?: Date, endTime?: Date) {
    if (!startTime || !endTime) {
      return 0;
    }

    return endTime.getTime() - startTime.getTime();
  }

  parse(durationString?: string): TimeSpan {
    if (!durationString) {
      return {};
    }

    const duration = durationString.split(':').reduce((r, v, i) => {
      const value = parseInt(v);

      switch (i) {
        case 0:
          r.hours = value;
          break;
        case 1:
          r.minutes = value;
          break;
        case 2:
          r.seconds = value;
          break;
      }

      return r;
    }, {} as TimeSpan);

    return duration;
  }

  getMaximumTime(timeSettings?: TimeSettings, totalQuestions?: number): TimeSpan {
    const duration = timeSettings?.duration;

    if (!duration) {
      return {};
    }

    if (timeSettings.method == TestDurationMethod.CompleteTestTime) {
      return timeSettings.duration ?? {};
    }

    const getValueOrZero = (numberValue?: number) => numberValue ?? 0;
    const durationInSeconds = getValueOrZero(duration.hours) * 3600 + getValueOrZero(duration.minutes) * 60 + getValueOrZero(duration.seconds);
    const maximumDurationInSeconds = durationInSeconds * getValueOrZero(totalQuestions);
    const maximumTime = this.getDurationFromTimeDifference(maximumDurationInSeconds * 1000);

    return maximumTime;
  }
}

import { Injectable } from '@angular/core';
import { TimeSpan } from '../../api/models';
import { TestDurationMethod, TimeSettings } from '../../state/test-session.model';

@Injectable({ providedIn: 'root' })
export class TestDurationService {
  private _milliSecondsInASecond = 1000;
  private _hoursInADay = 24;
  private _minutesInAnHour = 60;
  private _secondsInAMinute = 60;

  getDuration(startTime?: Date, endTime?: Date): TimeSpan {
    const timeDifference = this.getTimeDifference(startTime, endTime);
    return this.getDurationFromTimeDifference(timeDifference);
  }

  getDurationFromTimeDifference(timeDifference: number): TimeSpan {
    const duration: TimeSpan = {};
    duration.seconds = Math.floor((timeDifference) / (this._milliSecondsInASecond) % this._secondsInAMinute);
    duration.minutes = Math.floor((timeDifference) / (this._milliSecondsInASecond * this._minutesInAnHour) % this._secondsInAMinute);
    duration.hours = Math.floor((timeDifference) / (this._milliSecondsInASecond * this._minutesInAnHour * this._secondsInAMinute) % this._hoursInADay);
    duration.days = Math.floor((timeDifference) / (this._milliSecondsInASecond * this._minutesInAnHour * this._secondsInAMinute * this._hoursInADay));

    return duration;
  }

  getTimeDifference(startTime?: Date, endTime?: Date) {
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

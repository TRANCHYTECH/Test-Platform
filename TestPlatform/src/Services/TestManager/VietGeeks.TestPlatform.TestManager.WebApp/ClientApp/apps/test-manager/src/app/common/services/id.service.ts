import { Injectable } from '@angular/core';
import ObjectID from 'bson-objectid';

@Injectable({ providedIn: 'root' })
export class IdService {
  generateId() {
    return ObjectID().id;
  }
}

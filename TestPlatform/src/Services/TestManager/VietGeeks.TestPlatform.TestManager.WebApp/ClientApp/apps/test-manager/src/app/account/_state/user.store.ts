import { Injectable } from '@angular/core';
import { EntityState, EntityStore, StoreConfig } from '@datorama/akita';
import { User } from './user.model';

export type UserState = EntityState<User>;

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'User' })
export class UserStore extends EntityStore<UserState> {

  constructor() {
    super();
  }
}

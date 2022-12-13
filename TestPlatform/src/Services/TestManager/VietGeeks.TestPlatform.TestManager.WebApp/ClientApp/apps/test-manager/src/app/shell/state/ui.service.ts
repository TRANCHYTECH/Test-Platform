import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ID } from '@datorama/akita';
import { tap } from 'rxjs/operators';
import { Ui } from './ui.model';
import { UiStore } from './ui.store';

@Injectable({ providedIn: 'root' })
export class UiService {

  constructor(private uiStore: UiStore, private http: HttpClient) {
    
  }


  get() {
    return this.http.get<Ui[]>('https://api.com').pipe(tap(entities => {
      this.uiStore.set(entities);
    }));
  }

  add(ui: Ui) {
    this.uiStore.add(ui);
  }

  update(id : string, ui: Partial<Ui>) {
    this.uiStore.update(id, ui);
  }

  remove(id: ID) {
    this.uiStore.remove(id);
  }

}

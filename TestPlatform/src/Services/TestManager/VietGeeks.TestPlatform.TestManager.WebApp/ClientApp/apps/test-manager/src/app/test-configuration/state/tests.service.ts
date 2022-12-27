import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Test, TestBasicSetting } from './test.model';
import { TestsStore } from './tests.store';

@Injectable({ providedIn: 'root' })
export class TestsService {

  constructor(private testsStore: TestsStore, private http: HttpClient) {
  }

  get() {
    return this.http.get<Test[]>('https://api.com').pipe(tap(entities => {
      this.testsStore.set(entities);
    }));
  }

  add(test: Test) {
    this.testsStore.add(test);
  }

  update(id: string, test: Partial<Test>) {
    this.testsStore.update(id, test);
  }

  remove(id: string) {
    this.testsStore.remove(id);
  }

  getBasicSettings(id: string) {
    const mock: TestBasicSetting = { name: 'Test 1', category: '124', description: 'desc' };
    this.testsStore.update(id, { basicSetting: mock });
    return of(true);
  }
}

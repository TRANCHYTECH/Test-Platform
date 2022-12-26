import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ID } from '@datorama/akita';
import { tap } from 'rxjs/operators';
import { TestCategory } from './test-category.model';
import { TestCategoriesStore } from './test-categories.store';

@Injectable({ providedIn: 'root' })
export class TestCategoriesService {

  constructor(private testCategoriesStore: TestCategoriesStore, private http: HttpClient) {
  }

  get() {
    return this.http.get<TestCategory[]>('https://api.com').pipe(tap(entities => {
      this.testCategoriesStore.set(entities);
    }));
  }

  add(testCategory: TestCategory) {
    this.testCategoriesStore.add(testCategory);
  }

  update(id: string, testCategory: Partial<TestCategory>) {
    this.testCategoriesStore.update(id, testCategory);
  }

  remove(id: ID) {
    this.testCategoriesStore.remove(id);
  }

}

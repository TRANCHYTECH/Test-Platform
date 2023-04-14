import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegionalSettingsComponent } from './regional-settings.component';

describe('RegionalSettingsComponent', () => {
  let component: RegionalSettingsComponent;
  let fixture: ComponentFixture<RegionalSettingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RegionalSettingsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RegionalSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NotServerFoundComponent } from './not-server-found.component';

describe('NotServerFoundComponent', () => {
  let component: NotServerFoundComponent;
  let fixture: ComponentFixture<NotServerFoundComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [NotServerFoundComponent]
    });
    fixture = TestBed.createComponent(NotServerFoundComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

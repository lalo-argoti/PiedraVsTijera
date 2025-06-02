import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DepositosComponent } from './depositos.component';

describe('DepositosComponent', () => {
  let component: DepositosComponent;
  let fixture: ComponentFixture<DepositosComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DepositosComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DepositosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

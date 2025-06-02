import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FondoMonetarioComponent } from './fondo-monetario.component';

describe('FondoMonetarioComponent', () => {
  let component: FondoMonetarioComponent;
  let fixture: ComponentFixture<FondoMonetarioComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FondoMonetarioComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FondoMonetarioComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

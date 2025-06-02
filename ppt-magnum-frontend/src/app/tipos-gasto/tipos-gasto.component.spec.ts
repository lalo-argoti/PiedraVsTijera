import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TiposGastoComponent } from './tipos-gasto.component';

describe('TiposGastoComponent', () => {
  let component: TiposGastoComponent;
  let fixture: ComponentFixture<TiposGastoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TiposGastoComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TiposGastoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

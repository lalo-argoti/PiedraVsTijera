import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CodigoFormComponent } from './codigo-form.component';

describe('CodigoFormComponent', () => {
  let component: CodigoFormComponent;
  let fixture: ComponentFixture<CodigoFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CodigoFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CodigoFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

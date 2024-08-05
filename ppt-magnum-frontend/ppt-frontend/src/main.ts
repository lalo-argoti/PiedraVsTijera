import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { AppComponent } from './app/app.component';
import { appConfig } from './app/app.config';

// Proporciona HttpClient globalmente
bootstrapApplication(AppComponent, {
  providers: [
    provideHttpClient(), // Proporciona HttpClient
    ...appConfig.providers
  ],
})
.catch((err) => console.error(err));

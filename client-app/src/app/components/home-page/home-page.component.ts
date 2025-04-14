import {Component} from '@angular/core';
import {MatInputModule} from '@angular/material/input';
import {MatCardModule} from '@angular/material/card';
import {MatButtonModule} from '@angular/material/button';
import {MatIconModule} from '@angular/material/icon';
import {FormsModule} from '@angular/forms';
import {HttpClient} from '@angular/common/http';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.css',
  imports: [MatInputModule, MatCardModule, MatButtonModule, MatIconModule, FormsModule, ]
})
export class HomePageComponent {
  value= 'Shiba Inu'

  constructor(private httpClient: HttpClient) {
    this.httpClient.get('/api/words')
      .subscribe({
        next: next => console.log(next),
        error: error => console.log(error)
    })
  }

}

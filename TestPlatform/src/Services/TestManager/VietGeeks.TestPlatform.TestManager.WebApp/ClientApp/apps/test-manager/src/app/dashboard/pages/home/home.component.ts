import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AuthService } from '@auth0/auth0-angular';
import { Observable } from 'rxjs';

@Component({
  selector: 'viet-geeks-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit{
  constructor(public auth: AuthService, private http: HttpClient) {
   }

   ngOnInit() {
    this.http.get('http://localhost:8087/WeatherForecast').subscribe(c => console.log(c));
   }
}
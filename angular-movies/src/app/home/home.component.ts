import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  ngOnInit(): void {
    this.moviesinTheaters= [{
      title: 'Spider-Man',
      releaseDate: new Date(),
      price: 1400.99,
      poster:'https://m.media-amazon.com/images/M/MV5BMzY2ODk4NmUtOTVmNi00ZTdkLTlmOWYtMmE2OWVhNTU2OTVkXkEyXkFqcGdeQXVyMTQxNzMzNDI@._V1_.jpg'
    },{
      title: 'Moana',
      releaseDate: new Date('2016-11-14'),
      price: 300.99,
      poster:"https://m.media-amazon.com/images/M/MV5BMjI4MzU5NTExNF5BMl5BanBnXkFtZTgwNzY1MTEwMDI@._V1_UY1200_CR90,0,630,1200_AL_.jpg"
    }];

    this.moviesinFutureReleases = [];
  }
  
   
  moviesinTheaters
  moviesinFutureReleases;

}

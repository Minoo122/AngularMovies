import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { movieTheatersCreationDTO, movieTheatersDTO } from '../movie-theaters.models';

@Component({
  selector: 'app-edit-movie-theaters',
  templateUrl: './edit-movie-theaters.component.html',
  styleUrls: ['./edit-movie-theaters.component.css']
})
export class EditMovieTheatersComponent implements OnInit {

  constructor(private activatedRoute:ActivatedRoute) { }

  model: movieTheatersDTO = {name: 'Agora', latitude: 52.40660895131466 , longitude:22.241144031286243};

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params =>{
      
    })
  }

  saveChanges(movieTheater: movieTheatersCreationDTO){

  }
}

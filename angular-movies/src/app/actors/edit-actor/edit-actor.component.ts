import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { actorCreationDTO, actorDTO } from '../actors.model';

@Component({
  selector: 'app-edit-actor',
  templateUrl: './edit-actor.component.html',
  styleUrls: ['./edit-actor.component.css']
})
export class EditActorComponent implements OnInit {

  constructor(private activatedRoute: ActivatedRoute) { }

  model: actorDTO={name:'Tom Holland',
   dateOfBirth:new Date(),
   biography: 'default value',
   picture: 'https://upload.wikimedia.org/wikipedia/commons/thumb/0/05/Arnold_Schwarzenegger_1974.jpg/200px-Arnold_Schwarzenegger_1974.jpg'
  } 

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params=>{
      // alert(params.id);
    })
  }
  saveChanges(actorCreationDTO: actorCreationDTO){
    console.log(actorCreationDTO);
  }
}

import { AlertifyService } from './../_services/alertify.service';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() CancelRegister = new EventEmitter();
  model: any = {};
  constructor( private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  register() {
   // this.alertify.message('registration started');

    this.authService.register(this.model).subscribe( () => {
     this.alertify.success('registration successfull');

    }, error => {
     this.alertify.error(error);
    });
  }
  cancel() {
    this.CancelRegister.emit(false);
  }

}

import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { User } from 'src/app/_models/user';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { NgForm } from '@angular/forms';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm', {static: true}) editForm: NgForm;
  user: User;
  // variable to refresh instantly nav photo
  photoUrl: string;

  // an paw na kleisw teleiw to parathuro, gia na me rwtisei an thelw na vgw an exw kanei allages
  // tha xreiastw to host listener
  // se kathe alli periptwsi entos tis efarmogis, me kaluptei to canDeactivate tou guard
  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }


  constructor(private route: ActivatedRoute,
              private alertify: AlertifyService,
              private userService: UserService,
              private authService: AuthService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data['user'];
    });
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
    // meta allazw kai stin html tin eikona
  }

  updateUser() {
    this.userService.updateUser(this.authService.decodedToken.nameid, this.user).subscribe(next => {
      this.alertify.success('Profile updated successfully!');
      this.editForm.reset(this.user);
    }, error => {
      this.alertify.error(error);
    });
  }

  updateMainPhoto(photoUrl){
    this.user.photoUrl = photoUrl;
  }

}

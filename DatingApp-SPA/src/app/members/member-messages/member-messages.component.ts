import { Component, OnInit, Input } from '@angular/core';
import { Message } from 'src/app/_models/message';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() recipientId: number;
  messages: Message[];
  newMessage: any = {};

  constructor(private usersService: UserService,
              private authService: AuthService,
              private alertify: AlertifyService) { }

  ngOnInit() {
    this.loadMessages();
  }

  loadMessages(){
    // i tap voithaei na kanoume kati prwtou na kanoume subscribe
    const currentUserId = +this.authService.decodedToken.nameid; // var for markasread
    this.usersService.getMessageThread(this.authService.decodedToken.nameid, this.recipientId)
      .pipe(
        tap(
          messages => {
            // tslint:disable-next-line: prefer-for-of
            for (let i = 0; i < messages.length; i++) {
              if (messages[i].isRead === false && messages[i].recipientId === currentUserId) {
                this.usersService.markAsRead(currentUserId, messages[i].id);
              }
            }
          }
        )
      )
      .subscribe(messages => {
        this.messages = messages;
      }, error => {
        this.alertify.error(error);
      });
  }

  sendMessage(){
    this.newMessage.recipientId = this.recipientId;
    this.usersService.sendMessage(this.authService.decodedToken.nameid, this.newMessage).subscribe(
      (message: Message) => {
        console.log(this.newMessage);
        this.messages.unshift(message); //  to vazei stin arxi tou array kai oxi sto telos
        this.newMessage.content = '';
      }, error => {
        this.alertify.error(error);
      }
    );
  }

}

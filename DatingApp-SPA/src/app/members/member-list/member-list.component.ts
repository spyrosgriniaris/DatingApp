import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/user';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { Pagination, PaginatedResult } from 'src/app/_models/pagination';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  users: User[];

  // additional filtering area
  user: User = JSON.parse(localStorage.getItem('user'));
  genderList = [{value: 'male', display: 'Males'}, {value: 'female', display: 'Female'}];
  userParams: any = {};
  // end of additional filtering area

  pagination: Pagination;

  constructor(private userService: UserService,
              private alertify: AlertifyService,
              private route: ActivatedRoute) { }

  // before pagination
  // ngOnInit() {
  //   // this.loadUsers();
  //   this.route.data.subscribe(data => {
  //   this.users = data['users'];
  //   });
  //   }

  // after pagination
  ngOnInit() {
    // this.loadUsers();
    this.route.data.subscribe(data => {
    this.users = data['users'].result;
    this.pagination = data['users'].pagination;
    });

    // additional filtering area
    this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female';
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
    // end of additional filtering area

    // sorting area
    this.userParams.orderBy = 'lastActive';
    // end ofsorting area
    }



    resetFilters() {
      this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female';
      this.userParams.minAge = 18;
      this.userParams.maxAge = 99;
      this.loadUsers();
    }

    pageChanged(event: any): void{
      this.pagination.currentPage = event.page;
      this.loadUsers();
    }

    loadUsers() {
      // without additional filtering
      // this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage)
      //   .subscribe((res: PaginatedResult<User[]>) => {
      //     this.users = res.result;
      //     this.pagination = res.pagination;
      //   }, error => {
      //     this.alertify.error(error);
      //   });

      // additional filtering area
      this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, this.userParams)
      .subscribe((res: PaginatedResult<User[]>) => {
        this.users = res.result;
        this.pagination = res.pagination;
      }, error => {
        this.alertify.error(error);
      });
      // end of additional filtering area
    }

  // loadUsers() {
  //   this.userService.getUsers().subscribe((users: User[]) => {
  //     this.users = users;
  //   }, error => {
  //     this.alertify.error(error);
  //   });
  // }

}

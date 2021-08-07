import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/Pagination';
import { User } from 'src/app/_models/user';
import { UserParams } from 'src/app/_models/userParams';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  members:member[]=[];
  pagination:Pagination;
  userParams:UserParams;
  user:User;
  genderList=[{value:'male',display:'Males'},{value:'female',display:'Females'}];

  members$: Observable<member[]> = new Observable<member[]>();
  constructor(private memberService: MembersService) {
    this.userParams=this.memberService.getUserParams();
   }

  ngOnInit(): void {
    this.loadMembers();
  }

  pageChanged(event:any){
    this.userParams.pageNumber=event.page;
    this.memberService.setUserParams(this.userParams)
    this.loadMembers();
  }

  loadMembers(){
    this.memberService.setUserParams(this.userParams);
    this.memberService.getmembers(this.userParams).subscribe(resp=>{
      this.members=resp.result;
      this.pagination=resp.pagination;
    })
  }

  resetFilters(){
    this.userParams=this.memberService.resetUserParams();
    this.loadMembers();
  }
}

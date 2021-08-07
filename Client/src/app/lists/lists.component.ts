import { Component, OnInit } from '@angular/core';
import { member } from '../_models/member';
import { Pagination } from '../_models/Pagination';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  members:Partial<member[]>;
  predicate:string='liked';
  pageNumber=1;
  pageSize=5;
  pagination:Pagination;

  constructor(private memberService:MembersService) { }

  ngOnInit(): void {
    this.loadLikes()
  }

  loadLikes(){
    this.memberService.getLikes(this.pageNumber,this.pageSize,this.predicate).subscribe(resp=>{
      this.members=resp.result;
      this.pagination=resp.pagination;
    })
  }

  pageChanged(event:any){
    this.pageNumber=event.page;
    this.loadLikes();
  }

}

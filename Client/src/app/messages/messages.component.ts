import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';
import { Pagination } from '../_models/Pagination';
import { MessageService } from '../_services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages:Message[]=[];
  pagination:Pagination;
  container='unread';
  pageNumber=1;
  pageSize=5;
  loading=false;

  constructor(private messageService:MessageService) { }

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages(){
    this.loading=true;
    this.messageService.getMessages(this.pageNumber,this.pageSize,this.container).subscribe(resp=>{
      this.messages=resp.result;
      this.pagination=resp.pagination;
      this.loading=false;
    })
  }

  deleteMessage(id:number){
    this.messageService.deleteMessage(id).subscribe(()=>{
      this.loadMessages();
    })
  }

  pageChanged(event:any){
    this.pageNumber=event.page;
    this.loadMessages();
  }

}

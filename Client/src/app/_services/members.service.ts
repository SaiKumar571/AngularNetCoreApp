import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { member } from '../_models/member';


@Injectable({
  providedIn: 'root'
})
export class MembersService {
  members:member[]=[];

  baseurl=environment.apiUrl;

  constructor(private http:HttpClient) { }

  getmembers(){
    if(this.members.length>0)
    return of(this.members)
    return this.http.get<member[]>(this.baseurl+'users').pipe(
      map(members=>{this.members=members
      return members;
      })
    );
  }

  getmember(username:string){
    const member=this.members.find(x=>x.username===username);
    if(member)
    return of(member);
    return this.http.get<member>(this.baseurl+'users/'+username);
  }

  updateMember(member:any){
    return this.http.put<member>(this.baseurl+'users',member).pipe(
      map(()=>{
        const index=this.members.indexOf(member);
        this.members[index]=member;
      })
    );
  }
}

import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { member } from '../_models/member';
import { PaginatedResult } from '../_models/Pagination';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { getPaginatedResult, getPaginationHeader } from './paginationHelper';


@Injectable({
  providedIn: 'root'
})
export class MembersService {
  members:member[]=[];

  baseurl=environment.apiUrl;
  memberCache=new Map();
  user:User;
  userParams:UserParams;

  constructor(private http:HttpClient,private accountService:AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user=>{
      this.user=user;
      this.userParams=new UserParams(user);
    })
   }

   getUserParams(){
     return this.userParams;
   }

   setUserParams(params:UserParams)
   {
     this.userParams=params;
   }

   resetUserParams(){
     this.userParams=new UserParams(this.user);
     return this.userParams;
   }

  getmembers(userParams:UserParams){
    var response=this.memberCache.get(Object.values(userParams).join('-'));
    if(response){
      return of(response);
    }

    let params=getPaginationHeader(userParams.pageNumber,userParams.pageSize);
    params=params.append('minAge',userParams.minAge.toString());
    params=params.append('maxAge',userParams.maxAge.toString());
    params=params.append('gender',userParams.gender);
    params=params.append('orderBy',userParams.orderBy);

    return getPaginatedResult<member[]>(this.baseurl+ 'users',params,this.http).pipe(
      map(resp=>{
        this.memberCache.set(Object.values(userParams).join('-'),resp);
        console.log(this.memberCache)
        return resp;
      })
    );
  }

  

  getmember(username:string){
    const member=[...this.memberCache.values()].reduce((arr,element)=>arr.concat(element.result),[]).find(x=>x.username==username);
    if(member){return of(member)}
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

  SetMainPhoto(photoId:number){
    return this.http.put(this.baseurl+'users/set-main-photo/'+photoId,{});
  }

  DeletePhoto(photoId:number){
    return this.http.delete(this.baseurl+'users/delete-photo/'+photoId);
  }

  addLike(username:string){
    return this.http.post(this.baseurl+'likes/'+username,{});
  }

  getLikes(pageNumber:number, pageSize:number,predicate:string){
    let params=getPaginationHeader(pageNumber,pageSize);
    params=params.append('predicate',predicate);
    return getPaginatedResult<Partial<member[]>>(this.baseurl+'likes',params,this.http);
  }
}

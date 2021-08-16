import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import {map} from 'rxjs/operators'
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseurl=environment.apiUrl;
  private CurrentUserSource=new ReplaySubject<any>(1);
  currentUser$=this.CurrentUserSource.asObservable();
  constructor(private http:HttpClient) { }

  login(model:any){
    return this.http.post(this.baseurl+'account/login',model).pipe(
      map((response:any)=>{
        const user=response;
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  setCurrentUser(user:User){
    user.roles=[];
    const roles=this.getDecodedToken(user.token).role;
    Array.isArray(roles)?user.roles=roles:user.roles.push(roles);
    localStorage.setItem('user',JSON.stringify(user));
    this.CurrentUserSource.next(user);
  }
  register(model:any){
    return this.http.post(this.baseurl+'account/register',model).pipe(
      map((user:User)=>{
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }
  logout(){
    localStorage.removeItem('user');
    this.CurrentUserSource.next(undefined);
  }

  getDecodedToken(token){
    return JSON.parse(atob(token.split('.')[1]));
  }
}

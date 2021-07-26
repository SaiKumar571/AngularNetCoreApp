import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { member } from '../_models/member';


@Injectable({
  providedIn: 'root'
})
export class MembersService {
  

  baseurl=environment.apiUrl;

  constructor(private http:HttpClient) { }

  getmembers(){
    return this.http.get<member[]>(this.baseurl+'users');
  }

  getmember(username:string){
    return this.http.get<member>(this.baseurl+'users/'+username);
  }
}

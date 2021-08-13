import { Injectable } from '@angular/core';
import {
  Router, Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, of } from 'rxjs';
import { member } from '../_models/member';
import { MembersService } from '../_services/members.service';

@Injectable({
  providedIn: 'root'
})
export class MemberDetailedResolver implements Resolve<member> {

  constructor(private memberService:MembersService){}

  resolve(route: ActivatedRouteSnapshot): Observable<member> {
    return this.memberService.getmember(route.paramMap.get('username'));
  }
}

import { HttpClient, HttpParams } from "@angular/common/http";
import { map } from "rxjs/operators";
import { PaginatedResult } from "../_models/Pagination";

export function getPaginatedResult<T>(url,params: HttpParams,http:HttpClient) {
    const paginatedResult:PaginatedResult<T>=new PaginatedResult<T>();
  
      return http.get<T>(url, { observe: 'response', params }).pipe(
        map(resp => {
          paginatedResult.result = resp.body;
          if (resp.headers.get('Pagination') != null) {
            paginatedResult.pagination = JSON.parse(resp.headers.get('Pagination'));
          }
          return paginatedResult;
        })
      );
    }
  
    export function getPaginationHeader(pageNumber:number,pageSize:number){
      let params=new HttpParams();
        params=params.append('pageNumber',pageNumber.toString());
        params=params.append('pageSize',pageSize.toString());
        return params;
    }
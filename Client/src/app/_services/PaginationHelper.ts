import { HttpClient, HttpParams } from "@angular/common/http";
import { map } from "rxjs";
import { PaginationResulat } from "../_modules/Pagination";

export function GetPaginationResult<T>(url: string, params: HttpParams,http:HttpClient) {
    const paginationResulat: PaginationResulat<T> = new PaginationResulat<T>;
    return http.get<T>(url, { observe: 'response', params }).pipe(
      map((response: any) => {
        paginationResulat.result = response.body;
        if (response?.body) {
          paginationResulat.result = response?.body;
        }
        const pagination = response.headers?.get('Pagination');
        if (pagination) {
          paginationResulat.pagination = JSON.parse(pagination);
        }
        return paginationResulat;
      })
    );
  }
  export function GetPaginationHeader(pageNumber: number, pageSize: number) {
    let params = new HttpParams();
    params = params.append('PageNumber', pageNumber);
    params = params.append('PageSize', pageSize);
    return params;
  }
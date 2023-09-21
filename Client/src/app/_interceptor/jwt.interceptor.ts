import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, take } from 'rxjs';
import { AccountService } from '../services/account.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private account: AccountService) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.account.CurrentUser$.pipe(take(1)).subscribe((ele: any) => {
      if (ele) {
        request = request.clone({
          setHeaders: {
            'Authorization': `Bearer ${ele.token}`
          },
        })
      }
    })
    return next.handle(request);
  }
}

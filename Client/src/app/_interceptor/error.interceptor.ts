import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, catchError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private router: Router, private toastr: ToastrService) { }
  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((ele: HttpErrorResponse) => {
        if (ele.status) {
          switch (ele.status) {
            case (400):
              this.toastr.error('Bad Request', `Please check your input data`);
              const ModelState = [];
              if (ele.error.errors) {
                for (const key in ele.error.errors) {
                  ModelState.push(ele.error.errors[key])
                }
              }
              throw ModelState.flat()
            case (404):
              this.toastr.warning(`Page not found`);
              // this.router.navigate(['notfound']);
              break;
            case (401):
              this.toastr.info(`${ele.message}`, `Not Authincations`);
              this.router.navigate(['/'])
              break;
            case (500):
              const native: NavigationExtras = {
                state: { err: ele.error.errors }
              }
              this.router.navigate(['/NoServerFound', native])
              break;
            default:
              this.toastr.error("Unexcepeted Error");
          }
        }
        throw ele;
      })
    )
  }
}

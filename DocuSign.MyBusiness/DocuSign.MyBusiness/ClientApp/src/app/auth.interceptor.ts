import { Injectable } from '@angular/core'
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpErrorResponse } from '@angular/common/http'
import { Observable, throwError } from 'rxjs'
import { catchError } from 'rxjs/operators'
import { ToastrService } from 'ngx-toastr'
import { Router } from '@angular/router'

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    constructor(
        private toastr: ToastrService,
        private router: Router
    ) {}

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(
            catchError((error: any) => {
                if (error instanceof HttpErrorResponse) {
                    if (error.status === 401) {
                        this.router.navigate(['/'])
                        this.toastr.error('The connection session is expired', 'Please Login again', {
                            toastClass: 'error-message ngx-toastr custom-toastr',
                            timeOut: 0,
                            extendedTimeOut: 0,
                            tapToDismiss: true
                        })
                    } else {
                        return throwError(error)
                    }
                }
            })
        )
    }
}

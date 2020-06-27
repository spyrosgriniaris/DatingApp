import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
  })

  export class ErrorInterceptor implements HttpInterceptor {
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(req).pipe(
            catchError(error => {
                if (error.status === 401){
                    return throwError(error.statusText);
                }
                if (error instanceof HttpErrorResponse){ // this will take care of 500 internal server error
                    const applicationError = error.headers.get('Application-Error');
                    if(applicationError){
                        return throwError(applicationError);
                    }
                    // take care of model state errors
                    // to prwto error paei gia to error response pou vrisketao sto catch error
                    // to deutero paei gia to deutero error molis anoiksoume to velaki tou error ston browser
                    const serverError = error.error;
                    // to modalStateErrors einai gia error tupou password too short klp
                    let modalStateErrors ='';
                    // exoun ti morfi error.error.errors kai mesa periexoun arrays. px an valw kwdiko mikro pairnw tetoio error
                    if (serverError.errors && typeof serverError.errors === 'object'){
                        for (const key in serverError.errors){
                            if(serverError.errors[key]){
                                modalStateErrors += serverError.errors[key] + '\n';
                            }
                        }
                    }
                    // an den exw modal state eroors rixnw to server error
                    return throwError(modalStateErrors || serverError || 'Server Error');
                }
            })
        );
    }
  }

// dilwnw ton interceptor sto appmodule
export const ErrorInterceptorProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass: ErrorInterceptor,
    multi: true
};



import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { uploadComplete, uploadFail, uploadStart } from './files.actions';
import { catchError, map, of, pipe, switchMap, tap } from 'rxjs';

const uploadFileHandleError = (errorRes: any) => {
  let errorMessage = 'An unknown error occurred!';

  if (!errorRes.error && !errorRes.error.error) {
    return of(
      uploadFail({
        payload: { error: errorMessage },
      })
    );
  }
  switch (errorRes.status) {
    case 401:
      errorMessage = errorRes.error;
      break;
    case 400:
      break;
    default:
      break;
  }

  return of(
    uploadFail({
      payload: { error: errorMessage },
    })
  );
};

@Injectable()
export class FilesEffects {
  constructor(
    private http: HttpClient,
    private actions$: Actions,
    private router: Router
  ) {}

  uploadStart = createEffect(() =>
    this.actions$.pipe(
      ofType(uploadStart),
      switchMap((uploadData) => {
        var fd = new FormData();
        fd.append('file', uploadData.payload.file);
        return this.http
          .post('https://localhost:7041/api/file/upload', fd, {
            responseType: 'text',
          })
          .pipe(
            map((resData) => {
              return uploadComplete();
            }),
            catchError((errorRes) => {
              return of(uploadFail({ payload: { error: errorRes } }));
            })
          );
      })
    )
  );

  successRedirect = createEffect(
    () =>
      this.actions$.pipe(
        ofType(uploadComplete),
        tap(() => {
          console.log('here');
          this.router.navigate(['/files']);
        })
      ),
    { dispatch: false }
  );
}

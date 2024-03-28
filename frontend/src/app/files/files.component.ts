import { Component, OnDestroy, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { AppStateType } from '../store/app.reducer';
import { FileModel } from './models/file.model';
import { getFilesStart } from './store/files.actions';
import { ActivatedRoute } from '@angular/router';
import { selectFiles } from './store/files.selectors';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-files',
  templateUrl: './files.component.html',
  styleUrl: './files.component.scss',
})
export class FilesComponent implements OnInit, OnDestroy {
  userFiles: FileModel[] = [];
  private sub: Subscription;

  constructor(private store: Store<AppStateType>) {}

  ngOnInit(): void {
    this.store.dispatch(getFilesStart());

    this.sub = this.store.select(selectFiles).subscribe((filesState) => {
      this.userFiles = filesState.files;
    });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}

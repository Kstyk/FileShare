import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Store } from '@ngrx/store';
import { AppStateType } from '../../store/app.reducer';
import { FileModel } from '../models/file.model';

@Component({
  selector: 'app-file-detail',
  templateUrl: './file-detail.component.html',
  styleUrl: './file-detail.component.scss',
})
export class FileDetailComponent implements OnInit {
  id: number;
  file: FileModel;

  constructor(
    private route: ActivatedRoute,
    private store: Store<AppStateType>
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      console.log(params['id']);
      this.id = +params['id'];
    });

    this.store.select('files').subscribe((filesState) => {
      this.file = filesState.files.find((file) => file.id === this.id);
    });
  }
}

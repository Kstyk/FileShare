import { NgModule } from '@angular/core';
import { FilesComponent } from './files.component';
import { RouterModule } from '@angular/router';
import { FilesRoutingModule } from './files-routing.module';
import { UploadFileComponent } from './upload-file/upload-file.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@NgModule({
  declarations: [FilesComponent, UploadFileComponent],
  providers: [],
  imports: [
    RouterModule,
    FilesRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
  ],
})
export class FilesModule {}

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FilesComponent } from './files.component';
import { UploadFileComponent } from './upload-file/upload-file.component';
import { filesResolver } from './files-resolver.service';
import { FileDetailComponent } from './file-detail/file-detail.component';

const routes: Routes = [
  {
    path: '',
    component: FilesComponent,
  },
  {
    path: 'upload',
    component: UploadFileComponent,
  },
  {
    path: ':id',
    component: FileDetailComponent,
    resolve: { files: filesResolver },
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class FilesRoutingModule {}

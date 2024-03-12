import { NgModule } from '@angular/core';
import { FilesComponent } from './files.component';
import { RouterModule } from '@angular/router';
import { FilesRoutingModule } from './files-routing.module';

@NgModule({
  declarations: [FilesComponent],
  providers: [],
  imports: [RouterModule, FilesRoutingModule],
})
export class FilesModule {}

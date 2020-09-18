import { Component, Injector, AfterViewInit, Optional, Inject } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { API_BASE_URL, FileServiceProxy } from '@shared/service-proxies/service-proxies';
import { Observable, throwError as _observableThrow, of as _observableOf } from 'rxjs';

import {
  FileOutput,
  FileDto
} from '@shared/service-proxies/service-proxies';
@Component({
  selector: 'app-admin-tasks',
  templateUrl: './admin-tasks.component.html',
  styleUrls: ['./admin-tasks.component.css']
})
export class AdminTasksComponent extends AppComponentBase implements AfterViewInit {
  _file: FileDto = new FileDto();
  baseUrl: string;
  constructor(
    private _fileappService: FileServiceProxy,
    injector: Injector,
    @Optional() @Inject(API_BASE_URL) baseUrl?: string
  ) {
      super(injector);
      this.baseUrl = baseUrl ? baseUrl : "";
      this._file.init({
        filename: "filename",
        token: 'token'
      });
  }

  ngAfterViewInit(): void {

  }

  generateDb(): Promise<string> {
    return new Promise<string>((resolve, reject) => {
      this._fileappService.generateDb().subscribe((result: FileOutput) => {
        this._file = result.file;
        resolve(result.file.token);
      });
    });
  }
}

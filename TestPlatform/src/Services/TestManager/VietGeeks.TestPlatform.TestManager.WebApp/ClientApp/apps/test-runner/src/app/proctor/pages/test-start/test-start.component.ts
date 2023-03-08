import { Component, inject, OnInit } from '@angular/core';
import { ProctorService } from '../../proctor.service';

@Component({
  selector: 'viet-geeks-test-start',
  templateUrl: './test-start.component.html',
  styleUrls: ['./test-start.component.scss'],
})
export class TestStartComponent implements OnInit {

  proctorService = inject(ProctorService);

  ngOnInit(): void {
    console.log('backend url', this.proctorService.testRunnerApiBaseUrl);
  }
}

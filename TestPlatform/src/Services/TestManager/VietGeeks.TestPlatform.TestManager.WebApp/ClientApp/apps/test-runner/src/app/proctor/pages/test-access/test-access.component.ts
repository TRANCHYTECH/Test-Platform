import { Component, inject, OnInit } from '@angular/core';
import { ProctorService } from '../../proctor.service';

@Component({
  selector: 'viet-geeks-test-access',
  templateUrl: './test-access.component.html',
  styleUrls: ['./test-access.component.scss'],
})
export class TestAccessComponent implements OnInit {

  proctorService = inject(ProctorService);

  ngOnInit(): void {
    console.log('backend url', this.proctorService.testRunnerApiBaseUrl);
  }
}

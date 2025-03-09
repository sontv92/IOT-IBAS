import { Component, OnInit, Input } from '@angular/core';

@Component({
	selector: 'check-box',
	templateUrl: './check-box.component.html',
	styleUrls: ['./check-box.component.scss']
})
export class CheckBoxComponent implements OnInit {
	@Input('data') items: Array<Object>;

	constructor() { }

	ngOnInit() {
	}

}

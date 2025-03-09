import { Component, OnInit, Input, OnDestroy, ChangeDetectionStrategy } from '@angular/core';
import { CallCategoryFunctionService } from '../../service/call-category-function.service';
import { Subscription } from 'rxjs';
import { domainImage } from '../../data/const';


@Component({
	selector: 'ol',
	templateUrl: './ol.component.html',
	styleUrls: ['./ol.component.css'],
	changeDetection: ChangeDetectionStrategy.OnPush
})
export class OlComponent implements OnInit, OnDestroy {
	@Input('data') items: Array<Object>;
	@Input('key') key: string;
	@Input('hasAction') hasAction: boolean;
	public domainImage = domainImage;

	subscription: Subscription;


	constructor(private callCategoryFunctionService: CallCategoryFunctionService) {
		// this.subscription = this.callCategoryFunctionService.getAction().subscribe(action => {
		// 	if (action.TypeAction == 4) {
		// 		this.SaveCategorySort();
		// 	}
		// });
	}

	ngOnInit() {
	}

	ngOnDestroy() {
		// this.subscription.unsubscribe();
	}

	AddCate(CategoryId) {
		this.callCategoryFunctionService.sendAction(CategoryId, 1);
	}

	UpdateCate(CategoryId) {
		this.callCategoryFunctionService.sendAction(CategoryId, 2);
	}

	DeleteCate(CategoryId) {
		this.callCategoryFunctionService.sendAction(CategoryId, 3);
	}

	SaveCategorySort() {
		console.log(this.items);
	}

}

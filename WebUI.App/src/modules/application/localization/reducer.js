// @flow

import * as t from './actionTypes';
import type {State} from './model';

const initialState: State = {
    shortName: 'en'
};

export default (state: State = initialState, action: any): State => {
    switch (action.type) {
        default:
            let locale = localStorage.getItem('LOCALE');
            
            if (locale == 'undefined' || !locale) {
                locale = navigator.language || navigator.userLanguage;
                
                locale = locale.split("-")[0];
                
                if (locale == 'undefined' || !locale) {
                    locale = state.shortName;
                }
            }
            
            return {
                shortName: locale
            };
        case t.CHANGE:
            localStorage.setItem('LOCALE', action.payload);
            
            return {
                shortName: action.payload
            };
    }
};
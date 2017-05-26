import * as t from './actionTypes';

const initialState = {};

export default (state = initialState, action) => {
    switch (action.type) {
        case t.LIST:
            state = action.payload;
            
            return state;
        default:
            return state;
    }
}
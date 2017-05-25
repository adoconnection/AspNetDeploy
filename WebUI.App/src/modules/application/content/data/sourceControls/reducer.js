import * as t from './actionTypes';

const initialState = {
    sourceControls: [],
    sourceControlTypes: []
};

export default (state = initialState, action) => {
    switch (action.type) {
        case t.LOAD:
            return action.payload;
        default:
            return state;
    }
}
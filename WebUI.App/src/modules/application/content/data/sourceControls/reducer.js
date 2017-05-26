import * as t from './actionTypes';

const initialState = {
    isLoading: false,
    data: []
};

export default (state = initialState, action) => {
    switch (action.type) {
        case t.PREPARE_LOADING:
            state.isLoading = true;
            
            return state;
        case t.LOADED:
            state.isLoading = false;
            state.data = action.payload;
            
            return state;
        default:
            return state;
    }
}
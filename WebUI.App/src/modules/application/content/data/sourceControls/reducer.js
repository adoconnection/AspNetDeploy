import * as t from './actionTypes';

const initialState = {
    isLoading: false,
    data: []
};

export default (state = initialState, action) => {
    let indexOf = -1;
    
    switch (action.type) {
        case t.PREPARE_LOADING:
            state.isLoading = true;
            
            return state;
        case t.PREPARE_LOADING_VERSIONS:
            
            indexOf = state.data.findIndex((sc) => sc.id == action.payload);
            if(indexOf != -1)
            {
                state.data[indexOf].isVersionsLoading = true;
            }
            
            return state;
        case t.LOADED:
            state.isLoading = false;
            state.data = action.payload;
            
            return state;
        case t.LOADED_VERSIONS:
            indexOf = state.data.findIndex((sc) => sc.id == action.payload.id);
            if(indexOf != -1)
            {
                state.data[indexOf].isVersionsLoading = false;
                state.data[indexOf].versions = action.payload.versions;
            }
            
            return state;
        default:
            return state;
    }
}
import {combineReducers} from "redux";
import {routerReducer} from 'react-router-redux';
import localization from './localization';

export default combineReducers({
    routing: routerReducer,
    [localization.constants.NAME]: localization.reducer
});
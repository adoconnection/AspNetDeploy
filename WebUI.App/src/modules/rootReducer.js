import { combineReducers } from "redux";
import { routerReducer } from 'react-router-redux';
import application from './application';

export default combineReducers({
    routing: routerReducer,
    [application.localization.constants.NAME]: application.localization.reducer,
    [application.content.data.sourceControls.constants.NAME]: application.content.data.sourceControls.reducer
});
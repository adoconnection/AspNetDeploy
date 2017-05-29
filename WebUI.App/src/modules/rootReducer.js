import { combineReducers } from "redux";
import { routerReducer } from 'react-router-redux';
import { reducer as formReducer } from 'redux-form';

import application from './application';

export default combineReducers({
    routing: routerReducer,
    [application.localization.constants.NAME]: application.localization.reducer,
    [application.content.data.sourceControls.constants.NAME]: application.content.data.sourceControls.reducer,
    [application.content.data.sourceControlsDetails.constants.NAME]: application.content.data.sourceControlsDetails.reducer,
    [application.content.data.sourceControlVersions.constants.NAME]: application.content.data.sourceControlVersions.reducer,
    form: formReducer
});
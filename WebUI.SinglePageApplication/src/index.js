/*supporting modules*/
import 'core-js';
import 'babel-polyfill';

/*react libraries importing*/
import React from 'react';
import {render} from 'react-dom';
import {Route, Router, browserHistory} from 'react-router';
import {Provider} from 'react-redux';
import {createStore, applyMiddleware, compose} from 'redux';
import {syncHistoryWithStore} from 'react-router-redux'
import {routerMiddleware} from 'react-router-redux'
import injectTapEventPlugin from 'react-tap-event-plugin';

/*tapping supporting for material ui*/
injectTapEventPlugin();

/*signalr global hub*/
window._hub = null;

import signalr from './modules/signalr';

/*react redux store initialization*/
import combineReducers from './modules/rootReducer';
const middleware = routerMiddleware(browserHistory);
const store = createStore(
    combineReducers,
    compose(
        applyMiddleware(middleware),
        applyMiddleware(signalr.middleware),
        window.devToolsExtension ? window.devToolsExtension() : f => f
    )
);
const history = syncHistoryWithStore(browserHistory, store);

/*styles importing*/
import './resources/css/vendor/bootstrap/bootstrap.css'
import './resources/css/vendor/bootstrap/bootstrap-theme.css'
import './resources/css/default-theme/theme.css'
import './resources/css/vendor/animate.css';

/*components for routing importing*/
import application from './modules/application';

/*debug supporting*/
if (typeof window !== 'undefined') {
    window.React = React
}

/*components initialization callbacks*/
let onApplicationInit = (dispatch) => {
    // TODO: put initialization logic here
};

/*provide store to react-redux-router-signalr configuration*/
signalr.start(
    store, () => {
        render(
            <Provider store={store}>
                <Router
                    history={history}>
                    <Route
                        path="app"
                        component={application.components.AppLayout}
                        onEnter={onApplicationInit(store.dispatch)}>
                    </Route>
                </Router>
            </Provider>,
            document.getElementById('content')
        );
    }
);
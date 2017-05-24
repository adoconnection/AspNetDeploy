/*supporting modules*/
import 'core-js';
import 'babel-polyfill';

/*react libraries importing*/
import React from 'react';
import {render} from 'react-dom';
import {Provider} from 'react-redux';
import {createStore, applyMiddleware, compose} from 'redux';
import injectTapEventPlugin from 'react-tap-event-plugin';
import createHistory from 'history/createBrowserHistory';
import { ConnectedRouter, routerMiddleware } from 'react-router-redux';

/*tapping supporting for material ui*/
injectTapEventPlugin();

/*signalr global hub*/
window._hub = null;

import signalr from './modules/signalr';

/*react redux store initialization*/
import combineReducers from './modules/rootReducer';

const history = createHistory();
const middleware = routerMiddleware(history);
const store = createStore(
    combineReducers,
    compose(
        applyMiddleware(middleware),
        applyMiddleware(signalr.middleware),
        window.devToolsExtension ? window.devToolsExtension() : f => f
    )
);

/*styles importing*/
import './resources/css/vendor/bootstrap/bootstrap.css'
import './resources/css/vendor/bootstrap/bootstrap-theme.css'
import './resources/css/default-theme/theme.css'
import './resources/css/vendor/animate.css';

/*components for routing importing*/

/*debug supporting*/
if (typeof window !== 'undefined') {
    window.React = React
}

/*components initialization callbacks*/
let onApplicationInit = (dispatch) => {
    // TODO: put initialization logic here
};

import App from './modules/application/components/App';

/*provide store to react-redux-router-signalr configuration*/
signalr.start(
    store, () => {
        render(
            <Provider store={store}>
                <ConnectedRouter history={history}>
                    <App>
                    </App>
                </ConnectedRouter>
            </Provider>,
            document.getElementById('content')
        );
    }
);